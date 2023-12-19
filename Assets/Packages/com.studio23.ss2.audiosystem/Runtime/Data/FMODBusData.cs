using System;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Studio23.SS2.AudioSystem.Data
{
    public class FMODBusData
    {
        public Bus Bus;
        public string BusName;
        public float DefaultVolume;
        public float CurrentVolume;
        public bool IsPaused;
        public bool IsMuted;

        public FMODBusData(string busName, float defaultVolume)
        {
            Bus = RuntimeManager.GetBus(busName);
            BusName = busName;
            DefaultVolume = defaultVolume;
            SetVolume(defaultVolume);
        }

        public void SetVolume(float volume)
        {
            Bus.setVolume(volume);
            CurrentVolume = volume;
        }

        public void Pause()
        {
            Bus.setPaused(true);
            IsPaused = true;
        }

        public void UnPause()
        {
            Bus.setPaused(false);
            IsPaused = false;
        }

        public void Mute()
        {
            Bus.setMute(true);
            IsMuted = true;
        }

        public void UnMute()
        {
            Bus.setMute(false);
            IsMuted = false;
        }

        public async UniTask StopAllEventsAsync(STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
        {
            await UniTask.RunOnThreadPool(() =>
            {
                Bus.stopAllEvents(stopModeType);
            });
        }
    }

}
