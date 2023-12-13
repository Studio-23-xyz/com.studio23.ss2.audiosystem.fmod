using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using Cysharp.Threading.Tasks;
using Studio23.SS2.AudioSystem.Data;
using Studio23.SS2.AudioSystem.Extensions;

namespace Studio23.SS2.AudioSystem.Core
{
    public class AudioManager : MonoBehaviour
    {
        private List<FMODEmitterData> _emitterDataList;
        private Dictionary<string, Bank> _bankList;

        public delegate UniTask BankHandler(string bankName);
        public event BankHandler OnBankLoaded;
        public event BankHandler OnBankUnloaded;

        private static AudioManager _instance;
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AudioManager>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("AudioManager");
                        _instance = obj.AddComponent<AudioManager>();
                        obj.name = "AudioManager";
                    }
                }

                return _instance;
            }
        }

        private void OnEnable()
        {
            OnBankUnloaded += ClearEmitter;
        }

        private void OnDisable()
        {
            OnBankUnloaded -= ClearEmitter;
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            _emitterDataList = new List<FMODEmitterData>();
            _bankList = new Dictionary<string, Bank>();
        }

        public void LoadBank(string bankName, LOAD_BANK_FLAGS flag = LOAD_BANK_FLAGS.NORMAL)
        {
            var result = RuntimeManager.StudioSystem.loadBankFile(bankName, flag, out Bank bank);
            if (result == FMOD.RESULT.OK && !_bankList.ContainsKey(bankName))
            {
                OnBankLoaded?.Invoke(bankName);
                _bankList.Add(bankName, bank);
            }
        }

        public void UnloadBank(string bankName)
        {
            for (int i = 0; i < _bankList.Count; i++)
            {
                if (_bankList.ElementAt(i).Key.Equals(bankName))
                {
                    RemoveBank(i);
                }
            }
        }

        public void UnloadAllBanks()
        {
            for (int i = 0; i < _bankList.Count; i++)
            {
                RemoveBank(i);
            }
        }

        private void RemoveBank(int index)
        {
            var bank = _bankList.ElementAt(index).Value;
            bank.getPath(out string bankPath);
            OnBankUnloaded?.Invoke(bankPath);
            bank.unloadSampleData();
            bank.unload();
            _bankList.Remove(_bankList.ElementAt(index).Key);
        }

        private async UniTask ClearEmitter(string bankPath)
        {
            for (int i = 0; i < _emitterDataList.Count; i++)
            {
                FMODEmitterData emitter = _emitterDataList[i];
                if (emitter.BankName.Equals(bankPath))
                {
                    await emitter.ReleaseAsync();
                    _emitterDataList.Remove(emitter);
                }
            }
        }

        public void LoadBankSampleData(string bankName)
        {
            for (int i = 0; i < _bankList.Count; i++)
            {
                if (_bankList.ElementAt(i).Key.Equals(bankName))
                {
                    _bankList.ElementAt(i).Value.loadSampleData();
                    break;
                }
            }
        }

        public void CreateEmitter(FMODEventData eventData, GameObject referenceGameObject, CustomStudioEventEmitter emitter = null, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
        {
            var fetchData = EventEmitterExists(eventData.EventName, referenceGameObject);
            if (fetchData != null) return;
            var newEmitter = new FMODEmitterData(eventData, referenceGameObject, emitter, stopModeType);
            _emitterDataList.Add(newEmitter);
        }

        public void Play(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData.EventName, referenceGameObject);
            if (fetchData == null) return;
            fetchData.Play();
        }

        public void Pause(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData.EventName, referenceGameObject);
            if (fetchData == null) return;
            fetchData.Pause();
        }

        public void UnPause(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData.EventName, referenceGameObject);
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

        public async void Release(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData.EventName, referenceGameObject);
            if (fetchData == null) return;
            await fetchData.ReleaseAsync();
            _emitterDataList.Remove(fetchData);
        }

        private FMODEmitterData EventEmitterExists(string eventName, GameObject referenceGameObject)
        {
            return _emitterDataList.FirstOrDefault(x =>
                x.EventName.Equals(eventName) && x.ReferenceGameObject == referenceGameObject);
        }

        private void OnDestroy()
        {
            UnloadAllBanks();
        }
    }
}
