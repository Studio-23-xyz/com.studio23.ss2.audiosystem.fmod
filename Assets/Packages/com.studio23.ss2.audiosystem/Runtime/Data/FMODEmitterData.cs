using System;
using System.Runtime.InteropServices;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Studio23.SS2.AudioSystem.Data
{
    [System.Serializable]
    public class FMODEmitterData : FMODData
    {
        public StudioEventEmitter Emitter;
        public GCHandle EventGCHandle;
        public FMOD.Studio.EVENT_CALLBACK EventStateCallback;

        public FMODEmitterData(string eventName, GameObject referenceGameObject, StudioEventEmitter emitter = null, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT) : base(eventName, referenceGameObject, stopModeType)
        {
            Emitter = emitter;
            Initialize();
        }

        public override void Initialize()
        {
            if (Emitter == null) Emitter = ReferenceGameObject.AddComponent<StudioEventEmitter>();
            Emitter.EventReference = EventReference.Find(EventName);
            EventStateCallback = new FMOD.Studio.EVENT_CALLBACK(EventStateCallbackHandler);
        }

        public override void Play()
        {
            Emitter.Play();
            EventState = FMODEventState.Playing;

            EventGCHandle = GCHandle.Alloc(this);
            Emitter.EventInstance.setUserData(GCHandle.ToIntPtr(EventGCHandle));

            var result = Emitter.EventInstance.setCallback(EventStateCallback, EVENT_CALLBACK_TYPE.SOUND_STOPPED);
            if (result != FMOD.RESULT.OK) Debug.Log("Error setting callback.");
        }

        public override void Pause()
        {
            Emitter.EventInstance.setPaused(true);
            EventState = FMODEventState.Suspended;
        }

        public void UnPause()
        {
            Emitter.EventInstance.setPaused(false);
            EventState = FMODEventState.Playing;
        }

        public void TogglePause(bool isGamePaused)
        { 
            if (isGamePaused && EventState == FMODEventState.Playing)
            {
                Emitter.EventInstance.setPaused(true);
                EventState = FMODEventState.Paused;
            }
            else if (!isGamePaused && (EventState == FMODEventState.Paused)) UnPause();
        }

        public override void Stop()
        {
            Emitter.Stop();
            EventState = FMODEventState.Stopped;
        }

        public override void Release()
        {
            Stop();
            Emitter.EventInstance.release();
            Object.Destroy(Emitter);
        }

        public override void SetParameter(string parameterName, float parameterValue)
        {
            Emitter.EventInstance.setParameterByName(parameterName, parameterValue);
        }

        public override void SwitchLocalization()
        {
            throw new NotImplementedException();
        }

        [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
        public static FMOD.RESULT EventStateCallbackHandler(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
        {
            FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

            IntPtr timelineInfoPtr;
            FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);

            if (result != FMOD.RESULT.OK)
            {
                Debug.LogError("Timeline Callback error: " + result);
            }
            else if (timelineInfoPtr != IntPtr.Zero)
            {
                // Get the object to store beat and marker details
                GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
                FMODEmitterData timelineInfo = (FMODEmitterData)timelineHandle.Target;

                switch (type)
                {
                    case EVENT_CALLBACK_TYPE.SOUND_STOPPED:
                    {
                        timelineInfo.EventState = FMODEventState.Stopped;
                    }
                    break;
                }
            }
            return FMOD.RESULT.OK;
        }
    }
}
