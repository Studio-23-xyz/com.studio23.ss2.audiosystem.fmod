using Studio23.SS2.AudioSystem.fmod.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Studio23.SS2.AudioSystem.fmod
{
    public class FMODBusUtility : MonoBehaviour
    {
        public List<string> Buses;

        [SerializeField] private float _volume;

        /// <summary>
        /// Sets the volume for buses.
        /// </summary>
        public void SetBusVolume()
        {
            foreach (var bus in Buses)
            {
                FMODManager.Instance.MixerManager.SetBusVolume(bus, _volume);
            }
        }

        /// <summary>
        /// Pauses buses.
        /// </summary>
		[ContextMenu("Pause")]
        public void Pause()
        {
            foreach (var bus in Buses)
            {
                FMODManager.Instance.MixerManager.PauseBus(bus, true);
            }
        }

        /// <summary>
        /// Unpauses buses.
        /// </summary>
		[ContextMenu("Unpause")]
        public void Unpause()
        {
            foreach (var bus in Buses)
            {
                FMODManager.Instance.MixerManager.PauseBus(bus, false);
            }
        }

        /// <summary>
        /// Mutes buses.
        /// </summary>
		[ContextMenu("Mute")]
        public void Mute()
        {
            foreach (var bus in Buses)
            {
                FMODManager.Instance.MixerManager.MuteBus(bus, true);
            }
        }

        /// <summary>
        /// Unmutes buses.
        /// </summary>
		[ContextMenu("Unmute")]
        public void Unmute()
        {
            foreach (var bus in Buses)
            {
                FMODManager.Instance.MixerManager.MuteBus(bus, false);
            }
        }

        /// <summary>
        /// Stops all active Events under the buses.
        /// </summary>
		[ContextMenu("StopAllBusEvents")]
        public async void StopAllBusEvents()
        {
            foreach (var bus in Buses)
            {
                await FMODManager.Instance.MixerManager.StopAllBusEvents(bus);
            }
        }
    }
}
