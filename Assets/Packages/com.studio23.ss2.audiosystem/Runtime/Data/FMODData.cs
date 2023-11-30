using FMOD.Studio;
using UnityEngine;

namespace Studio23.SS2.AudioSystem.Data
{
    [System.Serializable]
    public abstract class FMODData
    {
        public string EventName;
        public GameObject ReferenceGameObject;
        public FMODEventState EventState;
        public STOP_MODE StopModeType;

        public FMODData(string eventName, GameObject referenceGameObject, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
        {
            EventName = eventName;
            ReferenceGameObject = referenceGameObject;
            StopModeType = stopModeType;
        }

        public abstract void Initialize();
        public abstract void Play();
        public abstract void Pause();
        public abstract void Stop();
        public abstract void Release();
        public abstract void SetParameter(string parameterName, float parameterValue);
        public abstract void SwitchLocalization();
    }

    public enum FMODEventState
    {
        Play,
        Pause,
        Stop
    }
}
