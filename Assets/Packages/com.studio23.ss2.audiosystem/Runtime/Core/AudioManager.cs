using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using Studio23.SS2.AudioSystem.Data;
using System.ComponentModel;
using Cysharp.Threading.Tasks;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public List<FMODEmitterData> EmitterDataList;
    public Dictionary<string, Bank> BankList;

    public delegate UniTask BankHandler(string bankName);
    public event BankHandler OnBankLoaded;
    public event BankHandler OnBankUnloaded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            DestroyImmediate(this);
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
        BankList = new Dictionary<string, Bank>();
    }

    public void LoadBank(string bankName, LOAD_BANK_FLAGS flag = LOAD_BANK_FLAGS.NORMAL)
    {
        var result = RuntimeManager.StudioSystem.loadBankFile(bankName, flag, out Bank bank);
        if (result == FMOD.RESULT.OK && !BankList.ContainsKey(bankName))
        {
            OnBankLoaded?.Invoke(bankName);
            BankList.Add(bankName, bank);
        }
    }

    public void UnloadBank(string bankName)
    {
        for (int i = 0; i < BankList.Count; i++)
        {
            if (BankList.ElementAt(i).Key.Equals(bankName))
            {
                RemoveBank(i); 
            }
        }
    }

    public void UnloadAllBanks()
    {
        for (int i = 0; i < BankList.Count; i++)
        {
            RemoveBank(i);
        }
    }

    private void RemoveBank(int index)
    {
        var bank = BankList.ElementAt(index).Value;
        bank.getPath(out string bankPath);
        OnBankUnloaded?.Invoke(bankPath);
        bank.unloadSampleData();
        bank.unload();
        BankList.Remove(BankList.ElementAt(index).Key);
    }

    private async UniTask ClearEmitter(string bankPath)
    {
        for (int i = 0; i < EmitterDataList.Count; i++)
        {
            FMODEmitterData emitter = EmitterDataList[i];
            if (emitter.BankName.Equals(bankPath))
            {
                await emitter.ReleaseAsync();
                EmitterDataList.Remove(emitter);
            }
        }
    }

    public void LoadBankSampleData(string bankName)
    {
        for (int i = 0; i < BankList.Count; i++)
        {
            if (BankList.ElementAt(i).Key.Equals(bankName))
            {
                BankList.ElementAt(i).Value.loadSampleData();
                break;
            }
        }
    }

    public void CreateEmitter(FMODEventData eventData, GameObject referenceGameObject, CustomStudioEventEmitter emitter = null, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
    {
        var fetchData = EventEmitterExists(eventData.EventName, referenceGameObject);
        if (fetchData != null) return;
        var newEmitter = new FMODEmitterData(eventData, referenceGameObject, emitter, stopModeType);
        EmitterDataList.Add(newEmitter);
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
        foreach (var emitter in EmitterDataList)
        {
            emitter.TogglePause(isGamePaused);
        }
    }

    public async void Release(FMODEventData eventData, GameObject referenceGameObject)
    {
        var fetchData = EventEmitterExists(eventData.EventName, referenceGameObject);
        if (fetchData == null) return;
        await fetchData.ReleaseAsync();
        EmitterDataList.Remove(fetchData);
    }

    private FMODEmitterData EventEmitterExists(string eventName, GameObject referenceGameObject)
    {
        return EmitterDataList.FirstOrDefault(x =>
            x.EventName.Equals(eventName) && x.ReferenceGameObject == referenceGameObject);
    }
}
