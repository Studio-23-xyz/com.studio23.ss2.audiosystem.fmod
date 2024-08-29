using Cysharp.Threading.Tasks;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Studio23.SS2.AudioSystem.fmod.Extensions;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
using STOP_MODE = FMOD.Studio.STOP_MODE;

[assembly: InternalsVisibleTo("com.studio23.ss2.audiosystem.fmod.playmode.tests")]
namespace Studio23.SS2.AudioSystem.fmod.Data
{
    [System.Serializable]
    public class FMODEmitterData
    {
        internal string EventGUID;
        internal GameObject ReferencedGameObject;
        internal string ReferencedGameObjectName;
        internal string ReferenceGameObjectSceneName;
        internal CustomStudioEventEmitter Emitter;
        internal bool AllowFadeout;
        internal FMODEventState EventState = FMODEventState.Stopped;
        internal EVENT_CALLBACK_TYPE CurrentCallbackType;

        public UnityEvent OnEventPlayed = new UnityEvent();
        public UnityEvent OnEventSuspended = new UnityEvent();
        public UnityEvent OnEventUnsuspended = new UnityEvent();
        public UnityEvent OnEventPaused = new UnityEvent();
        public UnityEvent OnEventUnPaused = new UnityEvent();
        public UnityEvent OnEventStopped = new UnityEvent();
        public UnityEvent OnEventCompleted = new UnityEvent();

        public FMODEmitterData(string eventGUID, GameObject referencedGameObject, CustomStudioEventEmitter emitter = null, bool allowFadeout = true)
        {
            EventGUID = eventGUID;
            ReferencedGameObject = referencedGameObject;
            ReferencedGameObjectName = referencedGameObject.name;
            ReferenceGameObjectSceneName = referencedGameObject.scene.name;
            Emitter = emitter;
            AllowFadeout = allowFadeout;
            Initialize();
        }

        /// <summary>
        /// Get the unique Key of an emitter
        /// </summary>
        /// <returns></returns>
        public (string, int) GetKey()
        {
            return (EventGUID, ReferencedGameObject.GetInstanceID());
        }

        /// <summary>
        /// Creates an Emitter if there is none and creates an Event Instance within the Emitter.
        /// </summary>
        public void Initialize()
        {

            if (Emitter == null) Emitter = ReferencedGameObject.AddComponent<CustomStudioEventEmitter>();
            var eventReference = new EventReference
            {
                Guid = GUID.Parse(EventGUID)
            };
            Emitter.EventReference = eventReference;
            Emitter.AllowFadeout = AllowFadeout;
            Emitter.CustomInitialize();
        }

        /// <summary>
        /// Plays the Emitter.
        /// </summary>
        public void Play()
        {
            Emitter.CustomPlay();
            EventState = FMODEventState.Playing;
            OnEventPlayed?.Invoke();
        }

        /// <summary>
        /// Suspends the Emitter. Is not affected by TogglePause
        /// </summary>
        public void Pause()
        {
            Emitter.EventInstance.setPaused(true);
            EventState = FMODEventState.Suspended;
            OnEventSuspended?.Invoke();
        }

        /// <summary>
        /// UnSuspends the Emitter. Is not affected by TogglePause
        /// </summary>
        public void Unpause()
        {
            Emitter.EventInstance.setPaused(false);
            EventState = FMODEventState.Playing;
            OnEventUnsuspended?.Invoke();
        }

        /// <summary>
        /// Pauses the Emitter.
        /// </summary>
        /// <param name="isGamePaused"></param>
        public void TogglePause(bool isGamePaused)
        {
            if (isGamePaused && EventState == FMODEventState.Playing)
            {
                Emitter.EventInstance.setPaused(true);
                EventState = FMODEventState.Paused;
                OnEventPaused?.Invoke();
            }
            else if (!isGamePaused && (EventState == FMODEventState.Paused))
            {
                Unpause();
                OnEventUnPaused?.Invoke();
            }
        }

        /// <summary>
        /// Stops the Emitter.
        /// </summary>
        /// <param name="allowFadeout"></param>
        /// <returns></returns>
        public void Stop(bool allowFadeout = true)
        {
            Emitter.AllowFadeout = allowFadeout;
            Emitter.EventInstance.stop(AllowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
            EventState = FMODEventState.Stopped;
            OnEventStopped?.Invoke();
            //await UniTask.WaitUntil(() => CurrentCallbackType == EVENT_CALLBACK_TYPE.STOPPED || CurrentCallbackType == EVENT_CALLBACK_TYPE.SOUND_STOPPED);
        }

        /// <summary>
        /// Releases the Event Instance and destroys the Emitter.
        /// </summary>
        /// <returns></returns>
        public void Release()
        {
            Stop();
            Emitter.EventInstance.release();
            //await UniTask.WaitUntil(() => CurrentCallbackType == EVENT_CALLBACK_TYPE.DESTROYED);
            //UnloadSampleData();
            Object.Destroy(Emitter);
        }

        internal async void CompleteEvent()
        {
            await UniTask.SwitchToMainThread();
            OnEventCompleted?.Invoke();
        }

        public void LoadSampleData()
        {
            Emitter.EventDescription.loadSampleData();
        }

        public void UnloadSampleData()
        {
            Emitter.EventDescription.unloadSampleData();
        }

        /// <summary>
        /// Sets a Local parameter value by name.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public void SetParameterByName(string parameterName, float parameterValue)
        {
            Emitter.EventInstance.setParameterByName(parameterName, parameterValue);
        }

        /// <summary>
        /// Gets a Local parameter value by name.
        /// </summary>
        /// <param name="parameterName"></param>
        public float GetParameterValueByName(string parameterName)
        {
            Emitter.EventInstance.getParameterByName(parameterName, out float value);
            return value;
        }

        /// <summary>
        /// Gets a Local parameter final value by name.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public float GetParameterFinalValueByName(string parameterName)
        {
            Emitter.EventInstance.getParameterByName(parameterName, out float value, out float finalValue);
            return finalValue;
        }

        /// <summary>
        /// Returns the Event of the Emitter.
        /// </summary>
        /// <returns></returns>
        public string GetGUID()
        {
            return EventGUID;
        }

        /// <summary>
        /// Returns the GameObject the Emitter is attached too.
        /// </summary>
        /// <returns></returns>
        public GameObject GetReferencedGameObject()
        {
            return ReferencedGameObject;
        }

        /// <summary>
        /// Returns the name of the GameObject the Emitter is attached too.
        /// </summary>
        /// <returns></returns>
        public string GetReferencedGameObjectName()
        {
            return ReferencedGameObjectName;
        }

        /// <summary>
        /// Returns the name of the scene the GameObject is a part of.
        /// </summary>
        /// <returns></returns>
        public string GetReferencedGameObjectSceneName()
        {
            return ReferenceGameObjectSceneName;
        }

        /// <summary>
        /// Returns the Emitter.
        /// </summary>
        /// <returns></returns>
        public CustomStudioEventEmitter GetEmitter()
        {
            return Emitter;
        }

        /// <summary>
        /// Returns the EventState of the Emitter.
        /// </summary>
        /// <returns></returns>
        public FMODEventState GetEventState()
        {
            return EventState;
        }

        /// <summary>
        /// Returns length of event in milliseconds.
        /// </summary>
        /// <returns></returns>
        public int GetLength()
        {
            Emitter.EventDescription.getLength(out int length);
            return length;
        }

        /// <summary>
        /// Returns the current timeline cursor position.
        /// </summary>
        /// <returns></returns>
        public int GetTimelinePosition()
        {
            Emitter.EventInstance.getTimelinePosition(out int position);
            return position;
        }

        ~FMODEmitterData()
        {
            OnEventPlayed.RemoveAllListeners();
            OnEventSuspended.RemoveAllListeners();
            OnEventUnsuspended.RemoveAllListeners();
            OnEventPaused.RemoveAllListeners();
            OnEventUnPaused.RemoveAllListeners();
            OnEventStopped.RemoveAllListeners();
            OnEventCompleted.RemoveAllListeners();
        }
    }

    /// <summary>
    /// Suspended and Paused have some small differences.
    /// Suspended sounds cannot be paused. It is not intended to be paused with the game,
    /// but when you might need to suspend a sound for some time irrespective of whether
    /// the game is paused or not.
    /// </summary>
    [System.Flags]
    public enum FMODEventState
    {
        Playing = 1,
        Suspended = 2,
        Paused = 4,
        Stopped = 8,
    }
}
