using Studio23.SS2.AudioSystem.fmod.Core;
using Studio23.SS2.AudioSystem.fmod.Data;
using UnityEngine;

public class Sample : MonoBehaviour
{
    public bool isPaused;
    public Language currentLocale;

    #region Basic Audio

    [ContextMenu("Play")]
    public void Play()
    {
        //FMODManager.Instance.EventsManager.CreateEmitter(FMODBank_Sample.Test, gameObject);
        FMODManager.Instance.EventsManager.Play(FMODBank_Sample.Test, gameObject);
    }

    [ContextMenu("Pause")]
    public void Pause()
    {
        FMODManager.Instance.EventsManager.Pause(FMODBank_Sample.Test, gameObject);
    }

    [ContextMenu("UnPause")]
    public void UnPause()
    {
        FMODManager.Instance.EventsManager.UnPause(FMODBank_Sample.Test, gameObject);
    }

    [ContextMenu("Toggle")]
    public void Toggle()
    {
        isPaused = !isPaused;
        FMODManager.Instance.EventsManager.TogglePauseAll(isPaused);
    }

    [ContextMenu("Change Parameter")]
    public void ChangeParameter()
    {
        FMODManager.Instance.EventsManager.SetLocalParameter(FMODBank_Sample.Test, gameObject, FMODParameterList.Test.TestParameter, Random.Range(0.0f, 1.0f));
    }

    [ContextMenu("Stop")]
    public async void Stop()
    {
        await FMODManager.Instance.EventsManager.Stop(FMODBank_Sample.Test, gameObject);
    }

    [ContextMenu("Release")]
    public async void Release()
    {
        await FMODManager.Instance.EventsManager.Release(FMODBank_Sample.Test, gameObject);
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
    public async void StopSound()
    {
        await FMODManager.Instance.EventsManager.Stop(FMODBank_Test.Test_3, gameObject);
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

    #endregion

    #region Dialogue

    [ContextMenu("Play EN")]
    public void PlayEN()
    {
        FMODManager.Instance.BanksManager.SwitchLocalization(FMODLocaleList.LanguageList[currentLocale], FMODLocaleList.LanguageList[Language.EN]);
        currentLocale = Language.EN;
        FMODManager.Instance.EventsManager.PlayProgrammerSound("welcome", FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
    }

    [ContextMenu("Play JP")]
    public void PlayJP()
    {
        FMODManager.Instance.BanksManager.SwitchLocalization(FMODLocaleList.LanguageList[currentLocale], FMODLocaleList.LanguageList[Language.JP]);
        currentLocale = Language.JP;
        FMODManager.Instance.EventsManager.PlayProgrammerSound("welcome", FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
    }

    [ContextMenu("Switch to CN")]
    public void SwitchToCN()
    {
        FMODManager.Instance.BanksManager.SwitchLocalization(FMODLocaleList.LanguageList[currentLocale], FMODLocaleList.LanguageList[Language.CN]);
        currentLocale = Language.CN;
    }

    [ContextMenu("Play CN")]
    public void PlayCN()
    {
        FMODManager.Instance.EventsManager.PlayProgrammerSound("welcome", FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
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
        FMODManager.Instance.MixerManager.PauseBus(FMODBusList.Sample);
    }

    [ContextMenu("UnPause Bus")]
    public void UnPauseBus()
    {
        FMODManager.Instance.MixerManager.UnPauseBus(FMODBusList.Sample);
    }

    [ContextMenu("Mute Bus")]
    public void MuteBus()
    {
        FMODManager.Instance.MixerManager.MuteBus(FMODBusList.Sample);
    }

    [ContextMenu("UnMute Bus")]
    public void UnMuteBus()
    {
        FMODManager.Instance.MixerManager.UnMuteBus(FMODBusList.Sample);
    }

    [ContextMenu("Stop All Bus Events")]
    public async void StopAllBusEvents()
    {
        await FMODManager.Instance.MixerManager.StopAllBusEvents(FMODBusList.Sample);
    }

    #endregion
}
