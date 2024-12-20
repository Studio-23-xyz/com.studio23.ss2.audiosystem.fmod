using Studio23.SS2.AudioSystem.fmod;
using Studio23.SS2.AudioSystem.fmod.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Sample : MonoBehaviour
{
    public bool isPaused;

    private string _currentLocale;

    public AssetReferenceT<TextAsset> MasterBank;
    public AssetReferenceT<TextAsset> MasterStringBank;
    public AssetReferenceT<TextAsset> TestBank;
    public List<AssetReferenceT<TextAsset>> Banks = new List<AssetReferenceT<TextAsset>>();
    
    #region Basic Audio
    [ContextMenu("Create Emitter")]
    public void CreateEmitter()
    {
        FMODManager.Instance.EventsManager.CreateEmitter(FMODBank_Sample.Test, gameObject);
        FMODManager.Instance.MixerManager.SetBusVolume(FMODBusList.Sample, 1.0f);
    }

    [ContextMenu("Play")]
    public void Play()
    {
        FMODManager.Instance.EventsManager.Play(FMODBank_Sample.Test, gameObject);
        FMODManager.Instance.MixerManager.SetBusVolume(FMODBusList.Sample, 1.0f);
    }

    [ContextMenu("Pause")]
    public void Pause()
    {
        FMODManager.Instance.EventsManager.Pause(FMODBank_Sample.Test, gameObject);
    }

    [ContextMenu("Unpause")]
    public void UnPause()
    {
        FMODManager.Instance.EventsManager.Unpause(FMODBank_Sample.Test, gameObject);
    }

    [ContextMenu("Toggle")]
    public void Toggle()
    {
        isPaused = !isPaused;
        FMODManager.Instance.EventsManager.TogglePause(isPaused);
    }

    [ContextMenu("Change Parameter")]
    public void ChangeParameter()
    {
        FMODManager.Instance.EventsManager.SetLocalParameterByName(FMODBank_Sample.Test, gameObject, FMODParameterList.Test.TestParameter, Random.Range(0.0f, 1.0f));
    }

    [ContextMenu("Stop")]
    public void Stop()
    {
        FMODManager.Instance.EventsManager.Stop(FMODBank_Sample.Test, gameObject);
    }

    [ContextMenu("Release")]
    public void Release()
    {
        FMODManager.Instance.EventsManager.Release(FMODBank_Sample.Test, gameObject);
    }

    #endregion

    #region Banks

    [ContextMenu("Play Sound")]
    public void PlaySound()
    {
        FMODManager.Instance.EventsManager.CreateEmitter(FMODBank_Test.Test_3, gameObject);
        FMODManager.Instance.EventsManager.Play(FMODBank_Test.Test_3, gameObject);
    }

    [ContextMenu("Stop Sound")]
    public void StopSound()
    {
        FMODManager.Instance.EventsManager.Stop(FMODBank_Test.Test_3, gameObject);
    }

    [ContextMenu("Load Bank")]
    public void LoadBank()
    {
        FMODManager.Instance.BanksManager.LoadBank(FMODBankList.Test);
    }

    [ContextMenu("Unload Bank")]
    public void UnloadBank()
    {

        FMODManager.Instance.BanksManager.UnloadBank(FMODBankList.Test);
    }

    [ContextMenu("Unload All Banks")]
    public void UnloadAllBanks()
    {
        FMODManager.Instance.BanksManager.UnloadAllBanks();
    }

    [ContextMenu("Load Addressable Bank")]
    public void LoadBankAdd()
    {
        FMODManager.Instance.BanksManager.LoadBank(MasterBank);
        FMODManager.Instance.BanksManager.LoadBank(MasterStringBank);
        FMODManager.Instance.BanksManager.LoadBank(TestBank);
    }

    [ContextMenu("Unload Addressable Bank")]
    public void UnloadBankAdd()
    {
        FMODManager.Instance.BanksManager.UnloadBank(TestBank);
    }

    #endregion

    #region Dialogue

    [ContextMenu("Play EN")]
    public void PlayEN()
    {
        _currentLocale = FMODLocaleList.LanguageList["English (en)"];
        FMODManager.Instance.BanksManager.LoadBank(FMODLocaleList.LanguageList["English (en)"]);
        while (!FMODManager.Instance.BanksManager.HasBankLoaded(FMODLocaleList.LanguageList["English (en)"]))
        {
            Debug.Log("Bank is loading");
            break;
        }

        FMODManager.Instance.BanksManager.SwitchLocalization(_currentLocale, FMODLocaleList.LanguageList["English (en)"]);
        _currentLocale = FMODLocaleList.LanguageList["English (en)"];
        FMODManager.Instance.EventsManager.PlayProgrammerSound("welcome", FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
    }

    [ContextMenu("Play JP")]
    public void PlayJP()
    {
        FMODManager.Instance.BanksManager.SwitchLocalization(_currentLocale, FMODLocaleList.LanguageList["Japanese (jp)"]);
        _currentLocale = FMODLocaleList.LanguageList["Japanese (jp)"];
        FMODManager.Instance.EventsManager.PlayProgrammerSound("welcome", FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
    }

    [ContextMenu("Switch to CN")]
    public void SwitchToCN()
    {
        FMODManager.Instance.BanksManager.SwitchLocalization(_currentLocale, FMODLocaleList.LanguageList["Chinese (cn)"]);
        _currentLocale = FMODLocaleList.LanguageList["Chinese (cn)"];
    }

    [ContextMenu("Play CN")]
    public void PlayCN()
    {
        FMODManager.Instance.EventsManager.PlayProgrammerSound("welcome", FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
    }

    [ContextMenu("Stop Dialogue")]
    public void StopDialogue()
    {
        FMODManager.Instance.EventsManager.Release(FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
    }

    #endregion

    #region Mixer

    [ContextMenu("Set Bus Volume")]
    public void SetBusVolume()
    {
        FMODManager.Instance.MixerManager.SetBusVolume(FMODBusList.Sample, Random.Range(0.0f, 1.0f));
    }

    [ContextMenu("Set VCA Volume")]
    public void SetVCAVolume()
    {
        FMODManager.Instance.MixerManager.SetVCAVolume(FMODVCAList.Sample, Random.Range(0.0f, 1.0f));
    }

    [ContextMenu("Pause Bus")]
    public void PauseBus()
    {
        FMODManager.Instance.MixerManager.PauseBus(FMODBusList.Sample, true);
    }

    [ContextMenu("Unpause Bus")]
    public void UnPauseBus()
    {
        FMODManager.Instance.MixerManager.PauseBus(FMODBusList.Sample, false);
    }

    [ContextMenu("Mute Bus")]
    public void MuteBus()
    {
        FMODManager.Instance.MixerManager.MuteBus(FMODBusList.Sample, true);
    }

    [ContextMenu("UnMute Bus")]
    public void UnMuteBus()
    {
        FMODManager.Instance.MixerManager.MuteBus(FMODBusList.Sample, false);
    }

    [ContextMenu("Stop All Bus Events")]
    public async void StopAllBusEvents()
    {
        await FMODManager.Instance.MixerManager.StopAllBusEvents(FMODBusList.Sample);
    }

    #endregion
}
