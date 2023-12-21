using Cysharp.Threading.Tasks;
using Studio23.SS2.AudioSystem.fmod.Data;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("com.studio23.ss2.audiosystem.fmod.playmode.tests")]
namespace Studio23.SS2.AudioSystem.fmod.Core
{
    public class MixerManager
    {
        internal List<FMODBusData> _busDataList;
        internal List<FMODVCAData> _VCADataList;

        internal void Initialize()
        {
            _busDataList = new List<FMODBusData>();
            _VCADataList = new List<FMODVCAData>();
        }

        private FMODBusData GetBus(string busName, float defaultVolume)
        {
            var newBus = new FMODBusData(busName, defaultVolume);
            _busDataList.Add(newBus);
            return newBus;
        }

        /// <summary>
        /// Sets the volume for a Bus.
        /// </summary>
        /// <param name="busName"></param>
        /// <param name="volume"></param>
        public void SetBusVolume(string busName, float volume)
        {
            var busData = _busDataList.FirstOrDefault(x => x.BusName.Equals(busName));
            if (busData == null) busData = GetBus(busName, volume);
            busData.SetVolume(volume);
        }

        /// <summary>
        /// Pauses a Bus.
        /// </summary>
        /// <param name="busName"></param>
        public void PauseBus(string busName)
        {
            var busData = _busDataList.FirstOrDefault(x => x.BusName.Equals(busName));
            if (busData != null)
            {
                busData.Pause();
            }
        }

        /// <summary>
        /// UnPause a Bus.
        /// </summary>
        /// <param name="busName"></param>
        public void UnPauseBus(string busName)
        {
            var busData = _busDataList.FirstOrDefault(x => x.BusName.Equals(busName));
            if (busData != null)
            {
                busData.UnPause();
            }
        }

        /// <summary>
        /// Mute a Bus.
        /// </summary>
        /// <param name="busName"></param>
        public void MuteBus(string busName)
        {
            var busData = _busDataList.FirstOrDefault(x => x.BusName.Equals(busName));
            if (busData != null)
            {
                busData.Mute();
            }
        }

        /// <summary>
        /// UnMute a Bus.
        /// </summary>
        /// <param name="busName"></param>
        public void UnMuteBus(string busName)
        {
            var busData = _busDataList.FirstOrDefault(x => x.BusName.Equals(busName));
            if (busData != null)
            {
                busData.UnMute();
            }
        }

        /// <summary>
        /// Stops all active Events under a Bus.
        /// </summary>
        /// <param name="busName"></param>
        /// <returns></returns>
        public async UniTask StopAllBusEvents(string busName)
        {
            var busData = _busDataList.FirstOrDefault(x => x.BusName.Equals(busName));
            if (busData != null)
            {
                await busData.StopAllEventsAsync();
            }
        }

        private FMODVCAData GetVCA(string VCAName, float defaultVolume)
        {
            var newVCA = new FMODVCAData(VCAName, defaultVolume);
            _VCADataList.Add(newVCA);
            return newVCA;
        }

        /// <summary>
        /// Sets the volume for a VCA.
        /// </summary>
        /// <param name="VCAName"></param>
        /// <param name="volume"></param>
        public void SetVCAVolume(string VCAName, float volume)
        {
            var VCAData = _VCADataList.FirstOrDefault(x => x.VCAName.Equals(VCAName));
            if (VCAData == null) VCAData = GetVCA(VCAName, volume);
            VCAData.SetVolume(volume);
        }
    }
}
