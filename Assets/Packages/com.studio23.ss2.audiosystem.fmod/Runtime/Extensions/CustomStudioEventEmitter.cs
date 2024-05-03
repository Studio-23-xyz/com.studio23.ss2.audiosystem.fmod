using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Studio23.SS2.AudioSystem.fmod.Extensions
{
    [AddComponentMenu("Studio 23/Custom FMOD Studio Event Emitter")]
    public class CustomStudioEventEmitter : FMODUnity.EventHandler
    {
        public EventReference EventReference;

        [Obsolete("Use the EventReference field instead")]
        public string Event = "";

        public EmitterGameEvent PlayEvent = EmitterGameEvent.None;
        public EmitterGameEvent StopEvent = EmitterGameEvent.None;
        public bool AllowFadeout = true;
        public bool TriggerOnce = false;
        public bool Preload = false;
        public bool AllowNonRigidbodyDoppler = false;
        public ParamRef[] Params = new ParamRef[0];
        public bool OverrideAttenuation = false;
        public float OverrideMinDistance = -1.0f;
        public float OverrideMaxDistance = -1.0f;

        protected EventDescription eventDescription;

        protected EventInstance instance;

        private bool hasTriggered = false;
        private bool isQuitting = false;
        private bool isOneshot = false;
        private List<ParamRef> cachedParams = new List<ParamRef>();

        private static List<CustomStudioEventEmitter> activeEmitters = new List<CustomStudioEventEmitter>();

        private const string SnapshotString = "snapshot";

        public EventDescription EventDescription { get { return eventDescription; } }

        public EventInstance EventInstance { get { return instance; } }

        public bool IsActive { get; private set; }

        private float MaxDistance
        {
            get
            {
                if (OverrideAttenuation)
                {
                    return OverrideMaxDistance;
                }

                if (!eventDescription.isValid())
                {
                    Lookup();
                }

                float minDistance, maxDistance;
                eventDescription.getMinMaxDistance(out minDistance, out maxDistance);
                return maxDistance;
            }
        }

        public static void UpdateActiveEmitters()
        {
            foreach (CustomStudioEventEmitter emitter in activeEmitters)
            {
                emitter.UpdatePlayingStatus();
            }
        }

        private static void RegisterActiveEmitter(CustomStudioEventEmitter emitter)
        {
            if (!activeEmitters.Contains(emitter))
            {
                activeEmitters.Add(emitter);
            }
        }

        private static void DeregisterActiveEmitter(CustomStudioEventEmitter emitter)
        {
            activeEmitters.Remove(emitter);
        }

        private void UpdatePlayingStatus(bool force = false)
        {
            // If at least one listener is within the max distance, ensure an event instance is playing
            bool playInstance = StudioListener.DistanceSquaredToNearestListener(transform.position) <= (MaxDistance * MaxDistance);

            if (force || playInstance != IsPlaying())
            {
                if (playInstance)
                {
                    PlayInstance();
                }
                else
                {
                    StopInstance();
                }
            }
        }

        protected override void Start()
        {
            RuntimeUtils.EnforceLibraryOrder();
            if (Preload)
            {
                Lookup();
                eventDescription.loadSampleData();
            }

            HandleGameEvent(EmitterGameEvent.ObjectStart);

            // If a Rigidbody is added, turn off "allowNonRigidbodyDoppler" option
#if UNITY_PHYSICS_EXIST
            if (AllowNonRigidbodyDoppler && GetComponent<Rigidbody>())
            {
                AllowNonRigidbodyDoppler = false;
            }
#endif
        }

        private void OnApplicationQuit()
        {
            isQuitting = true;
        }

        protected override void OnDestroy()
        {
            if (!isQuitting)
            {
                HandleGameEvent(EmitterGameEvent.ObjectDestroy);

                if (instance.isValid())
                {
                    RuntimeManager.DetachInstanceFromGameObject(instance);
                    if (eventDescription.isValid() && isOneshot)
                    {
                        instance.release();
                        instance.clearHandle();
                    }
                }

                DeregisterActiveEmitter(this);

                if (Preload)
                {
                    eventDescription.unloadSampleData();
                }
            }
        }

        protected override void HandleGameEvent(EmitterGameEvent gameEvent)
        {
            if (PlayEvent == gameEvent)
            {
                Play();
            }
            if (StopEvent == gameEvent)
            {
                Stop();
            }
        }

        private void Lookup()
        {
            eventDescription = RuntimeManager.GetEventDescription(EventReference);

            if (eventDescription.isValid())
            {
                for (int i = 0; i < Params.Length; i++)
                {
                    PARAMETER_DESCRIPTION param;
                    eventDescription.getParameterDescriptionByName(Params[i].Name, out param);
                    Params[i].ID = param.id;
                }
            }
        }

        /// <summary>
        /// Creates the Event Instance separately from Play().
        /// </summary>
        public void CustomInitialize()
        {
            if (TriggerOnce && hasTriggered)
            {
                return;
            }

            if (EventReference.IsNull)
            {
                return;
            }

            cachedParams.Clear();

            if (!eventDescription.isValid())
            {
                Lookup();
            }

            bool isSnapshot;
            eventDescription.isSnapshot(out isSnapshot);

            if (!isSnapshot)
            {
                eventDescription.isOneshot(out isOneshot);
            }

            bool is3D;
            eventDescription.is3D(out is3D);

            IsActive = true;

            if (is3D && !isOneshot && Settings.Instance.StopEventsOutsideMaxDistance)
            {
                RegisterActiveEmitter(this);
                UpdatePlayingStatus(true);
            }
            else
            {
                if (!instance.isValid())
                {
                    instance.clearHandle();
                }

                // Let previous oneshot instances play out
                if (isOneshot && instance.isValid())
                {
                    instance.release();
                    instance.clearHandle();
                }

                if (!instance.isValid())
                {
                    eventDescription.createInstance(out instance);

                    // Only want to update if we need to set 3D attributes
                    if (is3D)
                    {
                        var transform = GetComponent<Transform>();
#if UNITY_PHYSICS_EXIST
                    if (GetComponent<Rigidbody>())
                    {
                        Rigidbody rigidBody = GetComponent<Rigidbody>();
                        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject, rigidBody));
                        RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody);
                    }
                    else
#endif
#if UNITY_PHYSICS2D_EXIST
                    if (GetComponent<Rigidbody2D>())
                    {
                        var rigidBody2D = GetComponent<Rigidbody2D>();
                        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject, rigidBody2D));
                        RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody2D);
                    }
                    else
#endif
                        {
                            instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
                            RuntimeManager.AttachInstanceToGameObject(instance, transform, AllowNonRigidbodyDoppler);
                        }
                    }
                }

                foreach (var param in Params)
                {
                    instance.setParameterByID(param.ID, param.Value);
                }

                foreach (var cachedParam in cachedParams)
                {
                    instance.setParameterByID(cachedParam.ID, cachedParam.Value);
                }

                if (is3D && OverrideAttenuation)
                {
                    instance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, OverrideMinDistance);
                    instance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, OverrideMaxDistance);
                }
            }
        }

        /// <summary>
        /// Plays the created Event Instance.
        /// </summary>
        public void CustomPlay()
        {
            instance.start();

            hasTriggered = true;
        }

        public void Play()
        {
            if (TriggerOnce && hasTriggered)
            {
                return;
            }

            if (EventReference.IsNull)
            {
                return;
            }

            cachedParams.Clear();

            if (!eventDescription.isValid())
            {
                Lookup();
            }

            bool isSnapshot;
            eventDescription.isSnapshot(out isSnapshot);

            if (!isSnapshot)
            {
                eventDescription.isOneshot(out isOneshot);
            }

            bool is3D;
            eventDescription.is3D(out is3D);

            IsActive = true;

            if (is3D && !isOneshot && Settings.Instance.StopEventsOutsideMaxDistance)
            {
                RegisterActiveEmitter(this);
                UpdatePlayingStatus(true);
            }
            else
            {
                PlayInstance();
            }
        }

        private void PlayInstance()
        {
            if (!instance.isValid())
            {
                instance.clearHandle();
            }

            // Let previous oneshot instances play out
            if (isOneshot && instance.isValid())
            {
                instance.release();
                instance.clearHandle();
            }

            bool is3D;
            eventDescription.is3D(out is3D);

            if (!instance.isValid())
            {
                eventDescription.createInstance(out instance);

                // Only want to update if we need to set 3D attributes
                if (is3D)
                {
                    var transform = GetComponent<Transform>();
#if UNITY_PHYSICS_EXIST
                    if (GetComponent<Rigidbody>())
                    {
                        Rigidbody rigidBody = GetComponent<Rigidbody>();
                        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject, rigidBody));
                        RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody);
                    }
                    else
#endif
#if UNITY_PHYSICS2D_EXIST
                    if (GetComponent<Rigidbody2D>())
                    {
                        var rigidBody2D = GetComponent<Rigidbody2D>();
                        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject, rigidBody2D));
                        RuntimeManager.AttachInstanceToGameObject(instance, transform, rigidBody2D);
                    }
                    else
#endif
                    {
                        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
                        RuntimeManager.AttachInstanceToGameObject(instance, transform, AllowNonRigidbodyDoppler);
                    }
                }
            }

            foreach (var param in Params)
            {
                instance.setParameterByID(param.ID, param.Value);
            }

            foreach (var cachedParam in cachedParams)
            {
                instance.setParameterByID(cachedParam.ID, cachedParam.Value);
            }

            if (is3D && OverrideAttenuation)
            {
                instance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, OverrideMinDistance);
                instance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, OverrideMaxDistance);
            }

            instance.start();

            hasTriggered = true;
        }

        public void Stop()
        {
            DeregisterActiveEmitter(this);
            IsActive = false;
            cachedParams.Clear();
            StopInstance();
        }

        private void StopInstance()
        {
            if (TriggerOnce && hasTriggered)
            {
                DeregisterActiveEmitter(this);
            }

            if (instance.isValid())
            {
                instance.stop(AllowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
                instance.release();
                if (!AllowFadeout)
                {
                    instance.clearHandle();
                }
            }
        }

        public void SetParameter(string name, float value, bool ignoreseekspeed = false)
        {
            if (Settings.Instance.StopEventsOutsideMaxDistance && IsActive)
            {
                string findName = name;
                ParamRef cachedParam = cachedParams.Find(x => x.Name == findName);

                if (cachedParam == null)
                {
                    PARAMETER_DESCRIPTION paramDesc;
                    eventDescription.getParameterDescriptionByName(name, out paramDesc);

                    cachedParam = new ParamRef();
                    cachedParam.ID = paramDesc.id;
                    cachedParam.Name = paramDesc.name;
                    cachedParams.Add(cachedParam);
                }

                cachedParam.Value = value;
            }

            if (instance.isValid())
            {
                instance.setParameterByName(name, value, ignoreseekspeed);
            }
        }

        public void SetParameter(PARAMETER_ID id, float value, bool ignoreseekspeed = false)
        {
            if (Settings.Instance.StopEventsOutsideMaxDistance && IsActive)
            {
                PARAMETER_ID findId = id;
                ParamRef cachedParam = cachedParams.Find(x => x.ID.Equals(findId));

                if (cachedParam == null)
                {
                    PARAMETER_DESCRIPTION paramDesc;
                    eventDescription.getParameterDescriptionByID(id, out paramDesc);

                    cachedParam = new ParamRef();
                    cachedParam.ID = paramDesc.id;
                    cachedParam.Name = paramDesc.name;
                    cachedParams.Add(cachedParam);
                }

                cachedParam.Value = value;
            }

            if (instance.isValid())
            {
                instance.setParameterByID(id, value, ignoreseekspeed);
            }
        }

        public bool IsPlaying()
        {
            if (instance.isValid())
            {
                PLAYBACK_STATE playbackState;
                instance.getPlaybackState(out playbackState);
                return (playbackState != PLAYBACK_STATE.STOPPED);
            }
            return false;
        }

        void OnDrawGizmos()
        {
            if (this.enabled)
            {
                instance.getPaused(out bool isPaused);

                float minRadius;
                float maxRadius;
                eventDescription.is3D(out bool is3D);
                if (is3D)
                {
                    instance.getMinMaxDistance(out float minDistance, out float maxDistance);
                    minRadius = minDistance;
                    maxRadius = maxDistance;
                }
                else minRadius = maxRadius = 1f;

                if (IsPlaying())
                {
                    Gizmos.color = Color.green;
                    if (is3D) Gizmos.DrawSphere(transform.position, maxRadius);
                    Gizmos.DrawSphere(transform.position, minRadius);
                }
                if (isPaused)
                {
                    Gizmos.color = Color.yellow;
                    if (is3D) Gizmos.DrawSphere(transform.position, maxRadius);
                    Gizmos.DrawSphere(transform.position, minRadius);
                }
                else if (!IsPlaying())
                {
                    Gizmos.color = Color.red;
                    if (is3D) Gizmos.DrawSphere(transform.position, maxRadius);
                    Gizmos.DrawSphere(transform.position, minRadius);
                }
            }
        }
    }
}
