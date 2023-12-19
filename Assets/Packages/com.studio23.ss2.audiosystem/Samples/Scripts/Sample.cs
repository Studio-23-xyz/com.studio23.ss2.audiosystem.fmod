using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using Studio23.SS2.AudioSystem.Core;
using Studio23.SS2.AudioSystem.Data;

public class Sample : MonoBehaviour
{
    public bool isPaused;

    [ContextMenu("Play")]
    public void Play()
    {
        AudioManager.Instance.CreateEmitter(FMODBank_Test.Test, gameObject);
        AudioManager.Instance.Play(FMODBank_Test.Test, gameObject);
    }

    [ContextMenu("Release")]
    public void Release()
    {
        AudioManager.Instance.Release(FMODBank_Test.Test, gameObject);
    }

    [ContextMenu("Stop")]
    public void Stop()
    {
        AudioManager.Instance.Stop(FMODBank_Test.Test, gameObject);
    }

    [ContextMenu("Pause")]
    public void Pause()
    {
        AudioManager.Instance.Pause(FMODBank_Test.Test, gameObject);
    }

    [ContextMenu("UnPause")]
    public void UnPause()
    {
        AudioManager.Instance.UnPause(FMODBank_Test.Test, gameObject);
    }

    [ContextMenu("Toggle")]
    public void Toggle()
    {
        isPaused = !isPaused;
        AudioManager.Instance.TogglePauseAll(isPaused);
    }

    [ContextMenu("Load Bank")]
    public void LoadBank()
    {
        AudioManager.Instance.LoadBank(FMODBankList.Test);
    }

    [ContextMenu("Unload Bank")]
    public void UnloadBank()
    {
        AudioManager.Instance.UnloadBank(FMODBankList.Test);
    }

    [ContextMenu("Unload All Banks")]
    public void UnloadAllBanks()
    {
        AudioManager.Instance.UnloadAllBanks();
    }

    [ContextMenu("Get Bus Volume")]
    public void GetBusVolume()
    {
        Bus musicBus = RuntimeManager.GetBus(FMODBusList.Test);
        musicBus.getVolume(out float volume, out float finalVolume);
        Debug.Log($"Volume {volume} Final Volume {finalVolume}");
    }

    [ContextMenu("Set Bus Volume")]
    public void SetBusVolume()
    {
        Bus musicBus = RuntimeManager.GetBus(FMODBusList.Test);
        musicBus.setVolume(0.0f);
    }

    [ContextMenu("Load Test Snapshot")]
    public void LoadTestSnapshot()
    {
        AudioManager.Instance.CreateEmitter(FMODBank_Master.snapshot_Test, gameObject);
        AudioManager.Instance.Play(FMODBank_Master.snapshot_Test, gameObject);

    }

    [ContextMenu("Increase Bus Volume")]
    public void IncreaseBusVolume()
    {
        Bus musicBus = RuntimeManager.GetBus(FMODBusList.Test);
        musicBus.setVolume(1f);
    }

    [ContextMenu("Load Dialogue Bank")]
    public void LoadDialogueBank()
    {
        AudioManager.Instance.LoadBank(FMODBankList.Dialogue);
    }

    [ContextMenu("Unload Dialogue Bank")]
    public void UnloadDialogueBank()
    {
        AudioManager.Instance.UnloadBank(FMODBankList.Dialogue);
    }

    //[ContextMenu("Load EN Bank")]
    //public void LoadENBank()
    //{
    //    AudioManager.Instance.LoadBank(FMODBankList.DialogueTable_LOCALE_EN);
    //}

    //[ContextMenu("Unload EN Bank")]
    //public void UnloadENBank()
    //{
    //    AudioManager.Instance.UnloadBank(FMODBankList.DialogueTable_LOCALE_EN);
    //}

    //[ContextMenu("Load JP Bank")]
    //public void LoadJPBank()
    //{
    //    AudioManager.Instance.LoadBank(FMODBankList.DialogueTable_LOCALE_JP);
    //}

    //[ContextMenu("Unload JP Bank")]
    //public void UnloadJPBank()
    //{
    //    AudioManager.Instance.UnloadBank(FMODBankList.DialogueTable_LOCALE_JP);
    //}

    [ContextMenu("Load JP Bank")]
    public void LoadJPBank()
    {
        AudioManager.Instance.SwitchLocalization(FMODLocaleList.LanguageList[Language.EN], FMODLocaleList.LanguageList[Language.JP]);
    }

    [ContextMenu("Play Welcome Dialogue")]
    public void CreateDialogue()
    {
        AudioManager.Instance.PlayProgrammerSound("welcome", FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
    }

    [ContextMenu("Play Goodbye Dialogue")]
    public void PlayGoodbye()
    {
        AudioManager.Instance.PlayProgrammerSound("goodbye", FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
    }

    [ContextMenu("Pause")]
    public void PauseWelcome()
    {
        AudioManager.Instance.Pause(FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
    }

    [ContextMenu("UnPause")]
    public void UnPauseWelcome()
    {
        AudioManager.Instance.UnPause(FMODBank_Dialogue.Dialogue_Dialogue, gameObject);
    }

    [ContextMenu("StopAllBusEvents")]
    public async void StopAllBusEvents()
    {
        AudioManager.Instance.SetBusVolume(FMODBusList.Test, 1f);
        await AudioManager.Instance.StopAllBusEvents(FMODBusList.Test);
    }
}
