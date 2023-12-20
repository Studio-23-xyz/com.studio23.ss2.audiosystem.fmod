using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using Studio23.SS2.AudioSystem.Core;
using Studio23.SS2.AudioSystem.Data;

public class Sample : MonoBehaviour
{
    public bool isPaused;
    public Language currentLocale;

    #region Basic Audio

    [ContextMenu("Play")]
    public void Play()
    {
        FMODManager.Instance.CreateEmitter(FMODBank_Sample.Test, gameObject);
        FMODManager.Instance.Play(FMODBank_Sample.Test, gameObject);
    }

    [ContextMenu("Pause")]
    public void Pause()
    {
        FMODManager.Instance.Pause(FMODBank_Sample.Test, gameObject);
    }

    [ContextMenu("UnPause")]
    public void UnPause()
    {
        FMODManager.Instance.UnPause(FMODBank_Sample.Test, gameObject);
    }

    [ContextMenu("Toggle")]
    public void Toggle()
    {
        isPaused = !isPaused;
        FMODManager.Instance.TogglePauseAll(isPaused);
    }

    [ContextMenu("Change Parameter")]
    public void ChangeParameter()
    {
        FMODManager.Instance.SetLocalParameter(FMODBank_Sample.Test, gameObject, FMODParameterList.Test.TestParameter, Random.Range(0.0f, 1.0f));
    }

    [ContextMenu("Stop")]
    public async void Stop()
    {
        await FMODManager.Instance.Stop(FMODBank_Sample.Test, gameObject);
    }

    [ContextMenu("Release")]
    public async void Release()
    {
        await FMODManager.Instance.Release(FMODBank_Sample.Test, gameObject);
    }

    #endregion

    #region Banks

    [ContextMenu("Play Sound")]
    public void PlaySound()
    {
        FMODManager.Instance.CreateEmitter(FMODBank_Test.Test_3, gameObject);
        FMODManager.Instance.Play(FMODBank_Test.Test_3, gameObject);
    }

    [ContextMenu("Stop Sound")]
    public async void StopSound()
    {
        await FMODManager.Instance.Stop(FMODBank_Test.Test_3, gameObject);
    }

    [ContextMenu("Load Bank")]
    public void LoadBank()
    {
        FMODManager.Instance.LoadBank(FMODBankList.Test);
    }

    [ContextMenu("Unload Bank")]
    public void UnloadBank()
    {
        FMODManager.Instance.UnloadBank(FMODBankList.Test);
    }

    [ContextMenu("Unload All Banks")]
    public void UnloadAllBanks()
    {
        FMODManager.Instance.UnloadAllBanks();
    }

    #endregion

    #region Dialogue

    [ContextMenu("Play EN")]
    public void PlayEN()
    {
        FMODManager.Instance.SwitchLocalization(FMODLocaleList.LanguageList[currentLocale], FMODLocaleList.LanguageList[Language.EN]);
        currentLocale = Language.EN;
        FMODManager.Instance.PlayProgrammerSound("welcome", FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
    }

    [ContextMenu("Play JP")]
    public void PlayJP()
    {
        FMODManager.Instance.SwitchLocalization(FMODLocaleList.LanguageList[currentLocale], FMODLocaleList.LanguageList[Language.JP]);
        currentLocale = Language.JP;
        FMODManager.Instance.PlayProgrammerSound("welcome", FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
    }

    [ContextMenu("Switch to CN")]
    public void SwitchToCN()
    {
        FMODManager.Instance.SwitchLocalization(FMODLocaleList.LanguageList[currentLocale], FMODLocaleList.LanguageList[Language.CN]);
        currentLocale = Language.CN;
    }

    [ContextMenu("Play CN")]
    public void PlayCN()
    {
        FMODManager.Instance.PlayProgrammerSound("welcome", FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
    }

    #endregion

    #region Mixer

    [ContextMenu("Set Bus Volume")]
    public void SetBusVolume()
    {
        FMODManager.Instance.SetBusVolume(FMODBusList.Sample, Random.Range(0.0f, 1.0f));
    }

    [ContextMenu("Set VCA Volume")]
    public void SetVCAVolume()
    {
        FMODManager.Instance.SetVCAVolume(FMODVCAList.Sample, Random.Range(0.0f, 1.0f));
    }

    [ContextMenu("Pause Bus")]
    public void PauseBus()
    {
        FMODManager.Instance.PauseBus(FMODBusList.Sample);
    }

    [ContextMenu("UnPause Bus")]
    public void UnPauseBus()
    {
        FMODManager.Instance.UnPauseBus(FMODBusList.Sample);
    }

    [ContextMenu("Mute Bus")]
    public void MuteBus()
    {
        FMODManager.Instance.MuteBus(FMODBusList.Sample);
    }

    [ContextMenu("UnMute Bus")]
    public void UnMuteBus()
    {
        FMODManager.Instance.UnMuteBus(FMODBusList.Sample);
    }

    [ContextMenu("Stop All Bus Events")]
    public async void StopAllBusEvents()
    {
        await FMODManager.Instance.StopAllBusEvents(FMODBusList.Sample);
    }

    #endregion
}
