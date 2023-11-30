using System;
using FMODUnity;
using Studio23.SS2.AudioSystem.Data;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Studio23.SS2.AudioSystem.Data
{
    [System.Serializable]
    public class FMODEmitterData : FMODData
    {
        public StudioEventEmitter Emitter;

        public FMODEmitterData(string eventName, GameObject referenceGameObject, StudioEventEmitter emitter, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT) : base(eventName, referenceGameObject, stopModeType)
        {
            Emitter = emitter;
            Initialize();
        }

        public override void Initialize()
        {
            Emitter.EventReference = EventReference.Find(EventName);
        }

        public override void Play()
        {
            Emitter.Play();
            EventState = FMODEventState.Play;
        }

        public override void Pause()
        {
            Emitter.EventInstance.setPaused(true);
            EventState = FMODEventState.Pause;
        }

        public override void Stop()
        {
            Emitter.Stop();
            EventState = FMODEventState.Stop;
        }

        public override void Release()
        {
            Emitter.EventInstance.release();
        }

        public override void SetParameter(string parameterName, float parameterValue)
        {
            Emitter.EventInstance.setParameterByName(parameterName, parameterValue);
        }

        public override void SwitchLocalization()
        {
            throw new NotImplementedException();
        }
    }
}
