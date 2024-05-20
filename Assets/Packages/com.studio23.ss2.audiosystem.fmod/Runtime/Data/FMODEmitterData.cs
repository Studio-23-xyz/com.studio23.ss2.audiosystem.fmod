using Cysharp.Threading.Tasks;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Studio23.SS2.AudioSystem.fmod.Extensions;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;
using STOP_MODE = FMOD.Studio.STOP_MODE;

[assembly: InternalsVisibleTo("com.studio23.ss2.audiosystem.fmod.playmode.tests")]
namespace Studio23.SS2.AudioSystem.fmod.Data
{
    [System.Serializable]
    public class FMODEmitterData
    {
        internal string EventName;
        internal string EventGUID;
        internal GameObject ReferencedGameObject;
        internal CustomStudioEventEmitter Emitter;
        internal FMODEventState EventState = FMODEventState.Stopped;
        internal STOP_MODE StopModeType;
        internal EVENT_CALLBACK_TYPE CurrentCallbackType;

        public delegate void PlaybackEvent();
        public PlaybackEvent OnEventPlayed;
        public PlaybackEvent OnEventSuspended;
        public PlaybackEvent OnEventUnsuspended;
        public PlaybackEvent OnEventPaused;
        public PlaybackEvent OnEventUnPaused;
        public PlaybackEvent OnEventStopped;

        public FMODEmitterData(FMODEventData eventData, GameObject referencedGameObject, CustomStudioEventEmitter emitter = null, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
        {
            EventName = eventData.EventName;
            EventGUID = eventData.EventGUID;
            ReferencedGameObject = referencedGameObject;
            Emitter = emitter;
            StopModeType = stopModeType;

            Initialize();
        }

        public (string, string, int) GetKey()
        {
            return (EventName, EventGUID, ReferencedGameObject.GetInstanceID());
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
        public void UnPause()
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
                UnPause();
                OnEventUnPaused?.Invoke();
            }
        }

        /// <summary>
        /// Stops the Emitter.
        /// </summary>
        /// <param name="stopModeType"></param>
        /// <returns></returns>
        public async UniTask StopAsync(STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
        {
            Emitter.EventInstance.stop(stopModeType);
            EventState = FMODEventState.Stopped;
            OnEventStopped?.Invoke();
            await UniTask.WaitUntil(() => (CurrentCallbackType == EVENT_CALLBACK_TYPE.STOPPED) || (CurrentCallbackType == EVENT_CALLBACK_TYPE.SOUND_STOPPED) || (CurrentCallbackType == EVENT_CALLBACK_TYPE.DESTROYED));
        }

        /// <summary>
        /// Releases the Event Instance and destroys the Emitter.
        /// </summary>
        /// <returns></returns>
        public async UniTask ReleaseAsync()
        {
            await StopAsync();
            Emitter.EventInstance.release();
            await UniTask.WaitUntil(() => CurrentCallbackType == EVENT_CALLBACK_TYPE.DESTROYED);
            UnloadSampleData();
            Object.Destroy(Emitter);
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
        public void SetParameter(string parameterName, float parameterValue)
        {
            Emitter.EventInstance.setParameterByName(parameterName, parameterValue);
        }

        /// <summary>
        /// Returns the Event of the Emitter.
        /// </summary>
        /// <returns></returns>
        public FMODEventData GetEventData()
        {
            return new FMODEventData(EventName, EventGUID);
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
    }

    [System.Flags]
    public enum FMODEventState
    {
        Playing = 1,
        Suspended = 2,
        Paused = 4,
        Stopped = 8,
    }
}
