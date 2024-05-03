using Cysharp.Threading.Tasks;
using Studio23.SS2.AudioSystem.fmod.Data;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("com.studio23.ss2.audiosystem.fmod.playmode.tests")]
namespace Studio23.SS2.AudioSystem.fmod.Core
{
    public class MixerManager
    {
        internal Dictionary<string, FMODBusData> _busDataList;
        internal Dictionary<string, FMODVCAData> _VCADataList;

        internal void Initialize()
        {
            _busDataList = new Dictionary<string, FMODBusData>();
            _VCADataList = new Dictionary<string, FMODVCAData>();
        }

        private FMODBusData GetBus(string busName, float defaultVolume)
        {
            var newBus = new FMODBusData(busName, defaultVolume);
            _busDataList.Add(newBus.BusName, newBus);
            return newBus;
        }

        /// <summary>
        /// Sets the volume for a Bus.
        /// </summary>
        /// <param name="busName"></param>
        /// <param name="volume"></param>
        public void SetBusVolume(string busName, float volume)
        {
            var busData = BusExists(busName);
            if (busData == null) busData = GetBus(busName, volume);
            busData.SetVolume(volume);
        }

        /// <summary>
        /// Pause or unpause a bus
        /// </summary>
        /// <param name="busName"></param>
        /// <param name="state"></param>
        public void PauseBus(string busName, bool state)
        {
            var busData = BusExists(busName);
            busData?.Pause(state);
        }

        /// <summary>
        /// Mute or unmute a bus
        /// </summary>
        /// <param name="busName"></param>
        /// <param name="state"></param>
        public void MuteBus(string busName, bool state)
        {
            var busData = BusExists(busName);
            busData?.Mute(state);
        }

        /// <summary>
        /// Stops all active Events under a Bus.
        /// </summary>
        /// <param name="busName"></param>
        /// <returns></returns>
        public async UniTask StopAllBusEvents(string busName)
        {
            var busData = BusExists(busName);
            if (busData != null)
            {
                await busData.StopAllEventsAsync();
            }
        }

        private FMODVCAData GetVCA(string VCAName, float defaultVolume)
        {
            var newVCA = new FMODVCAData(VCAName, defaultVolume);
            _VCADataList.Add(newVCA.VCAName, newVCA);
            return newVCA;
        }

        /// <summary>
        /// Sets the volume for a VCA.
        /// </summary>
        /// <param name="VCAName"></param>
        /// <param name="volume"></param>
        public void SetVCAVolume(string VCAName, float volume)
        {
            var VCAData = VCAExists(VCAName);
            if (VCAData == null) VCAData = GetVCA(VCAName, volume);
            VCAData.SetVolume(volume);
        }

        private FMODBusData BusExists(string busName)
        {
            var key = busName;
            _busDataList.TryGetValue(key, out var busData);
            return busData;
        }

        private FMODVCAData VCAExists(string VCAName)
        {
            var key = VCAName;
            _VCADataList.TryGetValue(key, out var VCAData);
            return VCAData;
        }
    }
}
