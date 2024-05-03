using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using System.Runtime.CompilerServices;
using STOP_MODE = FMOD.Studio.STOP_MODE;

[assembly: InternalsVisibleTo("com.studio23.ss2.audiosystem.fmod.playmode.tests")]
namespace Studio23.SS2.AudioSystem.fmod.Data
{
    public class FMODBusData
    {
        internal Bus Bus;
        internal string BusName;
        internal float DefaultVolume;
        internal float CurrentVolume;
        internal bool IsPaused;
        internal bool IsMuted;

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
        /// Pause or unpause a bus
        /// </summary>
        /// <param name="state"></param>
        public void Pause(bool state)
        {
            Bus.setPaused(state);
            IsPaused = state;
        }

        /// <summary>
        /// Mute or unmute a bus
        /// </summary>
        /// <param name="state"></param>
        public void Mute(bool state)
        {
            Bus.setMute(state);
            IsMuted = state;
        }

        /// <summary>
        /// Stops all active Events under a Bus.
        /// </summary>
        /// <param name="stopModeType"></param>
        /// <returns></returns>
        public async UniTask StopAllEventsAsync(STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
        {
            await UniTask.RunOnThreadPool(async () =>
            {
                Bus.stopAllEvents(stopModeType);
                await UniTask.WaitForFixedUpdate();
            });
        }
    }
}
