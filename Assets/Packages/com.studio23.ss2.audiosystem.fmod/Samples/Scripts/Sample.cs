//using UnityEngine;
//using Studio23.SS2.AudioSystem.Core;
//using Studio23.SS2.AudioSystem.Data;

//public class Sample : MonoBehaviour
//{
//    public bool isPaused;
//    public Language currentLocale;

//    #region Basic Audio

//    [ContextMenu("Play")]
//    public void Play()
//    {
//        FMODManager.Instance.EventsHandler.CreateEmitter(FMODBank_Sample.Test, gameObject);
//        FMODManager.Instance.EventsHandler.Play(FMODBank_Sample.Test, gameObject);
//    }

//    [ContextMenu("Pause")]
//    public void Pause()
//    {
//        FMODManager.Instance.EventsHandler.Pause(FMODBank_Sample.Test, gameObject);
//    }

//    [ContextMenu("UnPause")]
//    public void UnPause()
//    {
//        FMODManager.Instance.EventsHandler.UnPause(FMODBank_Sample.Test, gameObject);
//    }

//    [ContextMenu("Toggle")]
//    public void Toggle()
//    {
//        isPaused = !isPaused;
//        FMODManager.Instance.EventsHandler.TogglePauseAll(isPaused);
//    }

//    [ContextMenu("Change Parameter")]
//    public void ChangeParameter()
//    {
//        FMODManager.Instance.EventsHandler.SetLocalParameter(FMODBank_Sample.Test, gameObject, FMODParameterList.Test.TestParameter, Random.Range(0.0f, 1.0f));
//    }

//    [ContextMenu("Stop")]
//    public async void Stop()
//    {
//        await FMODManager.Instance.EventsHandler.Stop(FMODBank_Sample.Test, gameObject);
//    }

//    [ContextMenu("Release")]
//    public async void Release()
//    {
//        await FMODManager.Instance.EventsHandler.Release(FMODBank_Sample.Test, gameObject);
//    }

//    #endregion

//    #region Banks

//    [ContextMenu("Play Sound")]
//    public void PlaySound()
//    {
//        FMODManager.Instance.EventsHandler.CreateEmitter(FMODBank_Test.Test_3, gameObject);
//        FMODManager.Instance.EventsHandler.Play(FMODBank_Test.Test_3, gameObject);
//    }

//    [ContextMenu("Stop Sound")]
//    public async void StopSound()
//    {
//        await FMODManager.Instance.EventsHandler.Stop(FMODBank_Test.Test_3, gameObject);
//    }

//    [ContextMenu("Load Bank")]
//    public void LoadBank()
//    {
//        FMODManager.Instance.BanksHandler.LoadBank(FMODBankList.Test);
//    }

//    [ContextMenu("Unload Bank")]
//    public void UnloadBank()
//    {
//        FMODManager.Instance.BanksHandler.UnloadBank(FMODBankList.Test);
//    }

//    [ContextMenu("Unload All Banks")]
//    public void UnloadAllBanks()
//    {
//        FMODManager.Instance.BanksHandler.UnloadAllBanks();
//    }

//    #endregion

//    #region Dialogue

//    [ContextMenu("Play EN")]
//    public void PlayEN()
//    {
//        FMODManager.Instance.BanksHandler.SwitchLocalization(FMODLocaleList.LanguageList[currentLocale], FMODLocaleList.LanguageList[Language.EN]);
//        currentLocale = Language.EN;
//        FMODManager.Instance.EventsHandler.PlayProgrammerSound("welcome", FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
//    }

//    [ContextMenu("Play JP")]
//    public void PlayJP()
//    {
//        FMODManager.Instance.BanksHandler.SwitchLocalization(FMODLocaleList.LanguageList[currentLocale], FMODLocaleList.LanguageList[Language.JP]);
//        currentLocale = Language.JP;
//        FMODManager.Instance.EventsHandler.PlayProgrammerSound("welcome", FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
//    }

//    [ContextMenu("Switch to CN")]
//    public void SwitchToCN()
//    {
//        FMODManager.Instance.BanksHandler.SwitchLocalization(FMODLocaleList.LanguageList[currentLocale], FMODLocaleList.LanguageList[Language.CN]);
//        currentLocale = Language.CN;
//    }

//    [ContextMenu("Play CN")]
//    public void PlayCN()
//    {
//        FMODManager.Instance.EventsHandler.PlayProgrammerSound("welcome", FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
//    }

//    #endregion

//    #region Mixer

//    [ContextMenu("Set Bus Volume")]
//    public void SetBusVolume()
//    {
//        FMODManager.Instance.MixerHandler.SetBusVolume(FMODBusList.Sample, Random.Range(0.0f, 1.0f));
//    }

//    [ContextMenu("Set VCA Volume")]
//    public void SetVCAVolume()
//    {
//        FMODManager.Instance.MixerHandler.SetVCAVolume(FMODVCAList.Sample, Random.Range(0.0f, 1.0f));
//    }

//    [ContextMenu("Pause Bus")]
//    public void PauseBus()
//    {
//        FMODManager.Instance.MixerHandler.PauseBus(FMODBusList.Sample);
//    }

//    [ContextMenu("UnPause Bus")]
//    public void UnPauseBus()
//    {
//        FMODManager.Instance.MixerHandler.UnPauseBus(FMODBusList.Sample);
//    }

//    [ContextMenu("Mute Bus")]
//    public void MuteBus()
//    {
//        FMODManager.Instance.MixerHandler.MuteBus(FMODBusList.Sample);
//    }

//    [ContextMenu("UnMute Bus")]
//    public void UnMuteBus()
//    {
//        FMODManager.Instance.MixerHandler.UnMuteBus(FMODBusList.Sample);
//    }

//    [ContextMenu("Stop All Bus Events")]
//    public async void StopAllBusEvents()
//    {
//        await FMODManager.Instance.MixerHandler.StopAllBusEvents(FMODBusList.Sample);
//    }

//    #endregion
//}
