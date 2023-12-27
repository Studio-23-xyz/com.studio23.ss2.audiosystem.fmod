using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Studio23.SS2.AudioSystem.fmod.Data
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

        /// <summary>
        /// Sets the volume for a Bus.
        /// </summary>
        /// <param name="volume"></param>
        public void SetVolume(float volume)
        {
            Bus.setVolume(volume);
            CurrentVolume = volume;
        }

        /// <summary>
        /// Pauses a Bus.
        /// </summary>
        public void Pause()
        {
            Bus.setPaused(true);
            IsPaused = true;
        }

        /// <summary>
        /// UnPauses a Bus.
        /// </summary>
        public void UnPause()
        {
            Bus.setPaused(false);
            IsPaused = false;
        }

        /// <summary>
        /// Mutes a Bus.
        /// </summary>
        public void Mute()
        {
            Bus.setMute(true);
            IsMuted = true;
        }

        /// <summary>
        /// UnMutes a Bus.
        /// </summary>
        public void UnMute()
        {
            Bus.setMute(false);
            IsMuted = false;
        }

        /// <summary>
        /// Stops all active Events under a Bus.
        /// </summary>
        /// <param name="stopModeType"></param>
        /// <returns></returns>
        public async UniTask StopAllEventsAsync(STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
        {
            await UniTask.RunOnThreadPool(async() =>
            {
                Bus.stopAllEvents(stopModeType);
                await UniTask.WaitForFixedUpdate();
            });
        }
    }
}
