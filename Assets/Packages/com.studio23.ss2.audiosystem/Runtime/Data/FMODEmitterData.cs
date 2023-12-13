using System;
using UnityEngine;
using Object = UnityEngine.Object;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using Cysharp.Threading.Tasks;
using Studio23.SS2.AudioSystem.Extensions;

namespace Studio23.SS2.AudioSystem.Data
{
    [System.Serializable]
    public class FMODEmitterData
    {
        public string BankName;
        public string EventName;
        public GameObject ReferenceGameObject;
        public CustomStudioEventEmitter Emitter;
        public FMODEventState EventState = FMODEventState.Stopped;
        public STOP_MODE StopModeType;

        public FMODEmitterData(FMODEventData eventData, GameObject referenceGameObject, CustomStudioEventEmitter emitter = null, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
        {
            BankName = eventData.BankName;
            EventName = eventData.EventName;
            ReferenceGameObject = referenceGameObject;
            Emitter = emitter;
            StopModeType = stopModeType;

            Initialize();
        }

        public void Initialize()
        {
            if (Emitter == null) Emitter = ReferenceGameObject.AddComponent<CustomStudioEventEmitter>();
            Emitter.EventReference = EventReference.Find(EventName);
            Emitter.CustomInitialize();
            FMODCallBackHandler.InitializeCallback(this);
        }

        public void Play()
        {
            Emitter.CustomPlay();
            EventState = FMODEventState.Playing;
        }

        public void Pause()
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

        public void Stop()
        {
            Emitter.Stop();
            EventState = FMODEventState.Stopped;
        }

        public async UniTask ReleaseAsync()
        {
            await UniTask.RunOnThreadPool(() =>
            {
                Stop();
                Emitter.EventInstance.release();
            });
            Object.Destroy(Emitter);
        }

        public void SetParameter(string parameterName, float parameterValue)
        {
            Emitter.EventInstance.setParameterByName(parameterName, parameterValue);
        }

        public void SwitchLocalization()
        {
            throw new NotImplementedException();
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
