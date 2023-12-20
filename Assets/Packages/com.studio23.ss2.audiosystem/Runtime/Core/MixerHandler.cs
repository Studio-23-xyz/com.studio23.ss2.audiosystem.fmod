using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Studio23.SS2.AudioSystem.Data;

[assembly: InternalsVisibleTo("com.studio23.ss2.audiosystem.Tests")]
namespace Studio23.SS2.AudioSystem.Core
{
    public class MixerHandler
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

        public void SetBusVolume(string busName, float volume)
        {
            var busData = _busDataList.FirstOrDefault(x => x.BusName.Equals(busName));
            if (busData == null) busData = GetBus(busName, volume);
            busData.SetVolume(volume);
        }

        public void PauseBus(string busName)
        {
            var busData = _busDataList.FirstOrDefault(x => x.BusName.Equals(busName));
            if (busData != null)
            {
                busData.Pause();
            }
        }

        public void UnPauseBus(string busName)
        {
            var busData = _busDataList.FirstOrDefault(x => x.BusName.Equals(busName));
            if (busData != null)
            {
                busData.UnPause();
            }
        }

        public void MuteBus(string busName)
        {
            var busData = _busDataList.FirstOrDefault(x => x.BusName.Equals(busName));
            if (busData != null)
            {
                busData.Mute();
            }
        }

        public void UnMuteBus(string busName)
        {
            var busData = _busDataList.FirstOrDefault(x => x.BusName.Equals(busName));
            if (busData != null)
            {
                busData.UnMute();
            }
        }

        public async UniTask StopAllBusEvents(string busName)
        {
            var busData = _busDataList.FirstOrDefault(x => x.BusName.Equals(busName));
            if (busData != null)
            {
                await busData.StopAllEventsAsync();
            }
        }

        public FMODVCAData GetVCA(string VCAName, float defaultVolume)
        {
            var newVCA = new FMODVCAData(VCAName, defaultVolume);
            _VCADataList.Add(newVCA);
            return newVCA;
        }

        public void SetVCAVolume(string VCAName, float volume)
        {
            var VCAData = _VCADataList.FirstOrDefault(x => x.VCAName.Equals(VCAName));
            if (VCAData == null) VCAData = GetVCA(VCAName, volume);
            VCAData.SetVolume(volume);
        }
    }
}
