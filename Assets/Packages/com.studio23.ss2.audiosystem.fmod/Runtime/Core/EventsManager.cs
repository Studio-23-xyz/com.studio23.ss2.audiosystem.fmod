using Cysharp.Threading.Tasks;
using FMODUnity;
using Studio23.SS2.AudioSystem.fmod.Data;
using Studio23.SS2.AudioSystem.fmod.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

[assembly: InternalsVisibleTo("com.studio23.ss2.audiosystem.fmod.playmode.tests")]
namespace Studio23.SS2.AudioSystem.fmod.Core
{
    public class EventsManager
    {
        internal List<FMODEmitterData> _emitterDataList;

        internal void Initialize()
        {
            _emitterDataList = new List<FMODEmitterData>();
        }

        /// <summary>
        /// Creates a Custom FMOD Studio Event Emitter for an Event on a GameObject.
        /// If the GameObject already has an Emitter attached, then pass the Emitter to initialize it.
        /// By default it will create an Emitter and the Event Instance's STOP_MODE is set to ALLOWFADEOUT.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        /// <param name="emitter"></param>
        /// <param name="stopModeType"></param>
        public void CreateEmitter(FMODEventData eventData, GameObject referenceGameObject, CustomStudioEventEmitter emitter = null, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData != null) return;
            var newEmitter = new FMODEmitterData(eventData, referenceGameObject, emitter, stopModeType);
            _emitterDataList.Add(newEmitter);
            FMODCallBackHandler.InitializeCallBack(newEmitter);
        }

        /// <summary>
        /// Use to play FMOD Programmer Sounds. Localized audio tables for dialogues can be played.
        /// Any external audio not within FMOD can be played.
        /// If the GameObject already has an Emitter attached, then pass the Emitter to initialize it.
        /// By default it will create an Emitter and the Event Instance's STOP_MODE is set to ALLOWFADEOUT.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        /// <param name="emitter"></param>
        /// <param name="stopModeType"></param>
        public void PlayProgrammerSound(string key, FMODEventData eventData, GameObject referenceGameObject, CustomStudioEventEmitter emitter = null, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData != null)
            {
                FMODProgrammerSoundCallBackHandler.InitializeProgrammerCallback(fetchData, key);
                return;
            }
            var newEmitter = new FMODEmitterData(eventData, referenceGameObject, emitter, stopModeType);
            _emitterDataList.Add(newEmitter);
            FMODProgrammerSoundCallBackHandler.InitializeProgrammerCallback(newEmitter, key, true);
        }

        /// <summary>
        /// Play the Emitter.
        /// Make sure to call CreateEmitter() if there is no Emitter on the GameObject.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        public async void Play(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            if (fetchData.EventState == FMODEventState.Playing) await fetchData.StopAsync(STOP_MODE.IMMEDIATE);
            fetchData.Play();
        }

        /// <summary>
        /// Pause the Emitter.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        public void Pause(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            fetchData.Pause();
        }

        /// <summary>
        /// UnPause the Emitter
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        public void UnPause(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            fetchData.UnPause();
        }

        /// <summary>
        /// Pause all Emitters.
        /// </summary>
        /// <param name="isGamePaused"></param>
        public void TogglePauseAll(bool isGamePaused)
        {
            foreach (var emitter in _emitterDataList)
            {
                emitter.TogglePause(isGamePaused);
            }
        }

        /// <summary>
        /// Stop the Emitter with default STOP_MODE.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        /// <returns></returns>
        public async UniTask Stop(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            await fetchData.StopAsync();
        }

        /// <summary>
        /// Stop the Emitter with a different STOP_MODE.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        /// <param name="stopMode"></param>
        /// <returns></returns>
        public async UniTask Stop(FMODEventData eventData, GameObject referenceGameObject, STOP_MODE stopMode)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            await fetchData.StopAsync(stopMode);
        }

        /// <summary>
        /// Release the Event Instance and destroy the Emitter.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        /// <returns></returns>
        public async UniTask Release(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            await fetchData.ReleaseAsync();
            _emitterDataList.Remove(fetchData);
        }

        /// <summary>
        /// Sets a Local parameter value by name.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public void SetLocalParameter(FMODEventData eventData, GameObject referenceGameObject, string parameterName,
            float parameterValue)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            fetchData.SetParameter(parameterName, parameterValue);
        }

        /// <summary>
        /// Sets a Global parameter value by name.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public void SetGlobalParameter(string parameterName, float parameterValue)
        {
            RuntimeManager.StudioSystem.setParameterByName(parameterName, parameterValue);
        }

        private FMODEmitterData EventEmitterExists(FMODEventData eventData, GameObject referenceGameObject)
        {
            return _emitterDataList.FirstOrDefault(x =>
                x.BankName.Equals(eventData.BankName) && x.EventName.Equals(eventData.EventName) &&
                x.ReferenceGameObject == referenceGameObject);
        }

        internal async UniTask ClearEmitter(string bankPath)
        {
            for (int i = 0; i < FMODManager.Instance.EventsManager._emitterDataList.Count; i++)
            {
                FMODEmitterData emitter = FMODManager.Instance.EventsManager._emitterDataList[i];
                if (emitter.BankName.Equals(bankPath))
                {
                    await emitter.ReleaseAsync();
                    FMODManager.Instance.EventsManager._emitterDataList.Remove(emitter);
                }
            }
        }
    }
}
