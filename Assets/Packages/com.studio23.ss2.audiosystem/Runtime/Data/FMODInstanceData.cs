using System;
using FMOD.Studio;
using FMODUnity;
using Studio23.SS2.AudioSystem.Data;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Studio23.SS2.AudioSystem.Data
{
    [System.Serializable]

    public class FMODInstanceData : FMODData
    {
        public EventInstance Instance;

        public FMODInstanceData(string eventName, GameObject referenceGameObject,
            STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT) : base(eventName, referenceGameObject, stopModeType)
        {
            Initialize();
        }

        public override void Initialize()
        {
            Instance = RuntimeManager.CreateInstance(EventName);
        }

        public override void Play()
        {
            Instance.start();
            EventState = FMODEventState.Playing;
        }

        public override void Pause()
        {
            Instance.setPaused(true);
            EventState = FMODEventState.Paused;
        }

        public override void Stop()
        {
            Instance.stop(StopModeType);
            EventState = FMODEventState.Stopped;
        }

        public override void Release()
        {
            Instance.release();
        }

        public override void SetParameter(string parameterName, float parameterValue)
        {
            Instance.setParameterByName(parameterName, parameterValue);
        }

        public override void SwitchLocalization()
        {
            throw new NotImplementedException();
        }
    }
}
