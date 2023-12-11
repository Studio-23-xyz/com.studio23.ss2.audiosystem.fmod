using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using Studio23.SS2.AudioSystem.Data;
using System.ComponentModel;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public List<FMODEmitterData> EmitterDataList;
    public Dictionary<string, Bank> BankList;

    void Awake()
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

    void Start()
    {
        BankList = new Dictionary<string, Bank>();
    }

    public void LoadBank(string bankName, LOAD_BANK_FLAGS flag = LOAD_BANK_FLAGS.NORMAL)
    {
        var result = RuntimeManager.StudioSystem.loadBankFile(bankName, flag, out Bank bank);
        if (result == FMOD.RESULT.OK && !BankList.ContainsKey(bankName)) BankList.Add(bankName, bank);
    }

    public void UnloadBank(string bankName)
    {
        for (int i = 0; i < BankList.Count; i++)
        {
            if (BankList.ElementAt(i).Key.Equals(bankName))
            {
                var bank = BankList.ElementAt(i).Value;

                

                bank.unload();
                BankList.Remove(BankList.ElementAt(i).Key);
                break;
            }
        }
    }

    public void UnloadAllBanks(string bankName)
    {
        RuntimeManager.StudioSystem.unloadAll();
        BankList.Clear();
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

    public void UnloadBankSampleData(string bankName)
    {
        for (int i = 0; i < BankList.Count; i++)
        {
            if (BankList.ElementAt(i).Key.Equals(bankName))
            {
                BankList.ElementAt(i).Value.unloadSampleData();
                break;
            }
        }
    }

    public void CreateEmitter(string eventName, GameObject referenceGameObject, StudioEventEmitter emitter = null, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
    {
        var fetchData = EventEmitterExists(eventName, referenceGameObject);
        if (fetchData != null) return;
        var newEmitter = new FMODEmitterData(eventName, referenceGameObject, emitter, stopModeType);
        EmitterDataList.Add(newEmitter);
    }

    public void Play(string eventName, GameObject referenceGameObject)
    {
        var fetchData = EventEmitterExists(eventName, referenceGameObject);
        if (fetchData == null) return;
        fetchData.Play();
    }

    public void Pause(string eventName, GameObject referenceGameObject)
    {
        var fetchData = EventEmitterExists(eventName, referenceGameObject);
        if (fetchData == null) return;
        fetchData.Pause();
    }

    public void UnPause(string eventName, GameObject referenceGameObject)
    {
        var fetchData = EventEmitterExists(eventName, referenceGameObject);
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

    public void Release(string eventName, GameObject referenceGameObject)
    {
        var fetchData = EventEmitterExists(eventName, referenceGameObject);
        if (fetchData == null) return;
        fetchData.Release();
        EmitterDataList.Remove(fetchData);
    }

    private FMODEmitterData EventEmitterExists(string eventName, GameObject referenceGameObject)
    {
        return EmitterDataList.FirstOrDefault(x =>
            x.EventName.Equals(eventName) && x.ReferenceGameObject == referenceGameObject);
    }
}
