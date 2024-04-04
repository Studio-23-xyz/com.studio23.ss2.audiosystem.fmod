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
        internal string BankName;
        internal string EventName;
        internal string EventGUID;
        internal GameObject ReferenceGameObject;
        internal CustomStudioEventEmitter Emitter;
        internal FMODEventState EventState = FMODEventState.Stopped;
        internal STOP_MODE StopModeType;
        internal EVENT_CALLBACK_TYPE CurrentCallbackType;

        public FMODEmitterData(FMODEventData eventData, GameObject referenceGameObject, CustomStudioEventEmitter emitter = null, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
        {
            BankName = eventData.BankName;
            EventName = eventData.EventName;
            EventGUID = eventData.EventGUID;
            ReferenceGameObject = referenceGameObject;
            Emitter = emitter;
            StopModeType = stopModeType;

            Initialize();
        }

        public (string, string, int) GetKey()
        {
            return (BankName, EventName, ReferenceGameObject.GetInstanceID());
        }

        /// <summary>
        /// Creates an Emitter if there is none and creates an Event Instance within the Emitter.
        /// </summary>
        public void Initialize()
        {
            if (Emitter == null) Emitter = ReferenceGameObject.AddComponent<CustomStudioEventEmitter>();
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
        }

        /// <summary>
        /// Suspends the Emitter. Is not affected by TogglePause
        /// </summary>
        public void Pause()
        {
            Emitter.EventInstance.setPaused(true);
            EventState = FMODEventState.Suspended;
        }

        /// <summary>
        /// UnSuspends the Emitter. Is not affected by TogglePause
        /// </summary>
        public void UnPause()
        {
            Emitter.EventInstance.setPaused(false);
            EventState = FMODEventState.Playing;
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
            }
            else if (!isGamePaused && (EventState == FMODEventState.Paused)) UnPause();
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
