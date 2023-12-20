using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Cysharp.Threading.Tasks;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using Studio23.SS2.AudioSystem.Data;
using Studio23.SS2.AudioSystem.Extensions;

[assembly: InternalsVisibleTo("com.studio23.ss2.audiosystem.Tests")]
namespace Studio23.SS2.AudioSystem.Core
{
    public class EventsHandler
    {
        internal List<FMODEmitterData> _emitterDataList;

        internal void Initialize()
        {
            _emitterDataList = new List<FMODEmitterData>();
        }

        public void CreateEmitter(FMODEventData eventData, GameObject referenceGameObject,
            CustomStudioEventEmitter emitter = null, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData != null) return;
            var newEmitter = new FMODEmitterData(eventData, referenceGameObject, emitter, stopModeType);
            _emitterDataList.Add(newEmitter);
            FMODCallBackHandler.InitializeCallback(newEmitter);
        }

        public void PlayProgrammerSound(string key, FMODEventData eventData, GameObject referenceGameObject,
            CustomStudioEventEmitter emitter = null, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData != null)
            {
                FMODProgrammerSoundCallBackHandler.InitializeDialogueCallback(fetchData, key);
                return;
            }

            var newEmitter = new FMODEmitterData(eventData, referenceGameObject, emitter, stopModeType);
            _emitterDataList.Add(newEmitter);
            FMODProgrammerSoundCallBackHandler.InitializeDialogueCallback(newEmitter, key, true);
        }

        public async void Play(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            if (fetchData.EventState == FMODEventState.Playing) await fetchData.StopAsync(STOP_MODE.IMMEDIATE);
            fetchData.Play();
        }

        public void Pause(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            fetchData.Pause();
        }

        public void UnPause(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            fetchData.UnPause();
        }

        public void TogglePauseAll(bool isGamePaused)
        {
            foreach (var emitter in _emitterDataList)
            {
                emitter.TogglePause(isGamePaused);
            }
        }

        public async UniTask Stop(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            await fetchData.StopAsync();
        }

        public async UniTask Stop(FMODEventData eventData, GameObject referenceGameObject, STOP_MODE stopMode)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            await fetchData.StopAsync(stopMode);
        }

        public async UniTask Release(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            await fetchData.ReleaseAsync();
            _emitterDataList.Remove(fetchData);
        }

        public void SetLocalParameter(FMODEventData eventData, GameObject referenceGameObject, string parameterName,
            float parameterValue)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            fetchData.SetParameter(parameterName, parameterValue);
        }

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
            for (int i = 0; i < FMODManager.Instance.EventsHandler._emitterDataList.Count; i++)
            {
                FMODEmitterData emitter = FMODManager.Instance.EventsHandler._emitterDataList[i];
                if (emitter.BankName.Equals(bankPath))
                {
                    await emitter.ReleaseAsync();
                    FMODManager.Instance.EventsHandler._emitterDataList.Remove(emitter);
                }
            }
        }
    }
}
