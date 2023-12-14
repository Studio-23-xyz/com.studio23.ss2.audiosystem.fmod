using FMOD.Studio;
using FMODUnity;
using Studio23.SS2.AudioSystem.Core;
using Studio23.SS2.AudioSystem.Data;
using UnityEngine;

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

    [ContextMenu("Generate Bus List")]
    public void GenerateBusList()
    {
        FMODUnity.RuntimeManager.StudioSystem.getBankList(out FMOD.Studio.Bank[] loadedBanks);
        foreach (FMOD.Studio.Bank bank in loadedBanks)
        {
            var busListOk = bank.getBusList(out Bus[] myBuses);
            bank.getBusCount(out int busCount);
            if (busCount > 0)
            {
                foreach (var bus in myBuses)
                {
                    bus.getPath(out string busPath);
                    Debug.Log(busPath);
                }
            }
        }
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

    [ContextMenu("Set Local Parameter Increase")]
    public void SetParameter1()
    {
        AudioManager.Instance.SetLocalParameter(FMODBank_Test.Test, gameObject, FMODParameterList.Test.TestParameter, 0f);
    }

    [ContextMenu("Set Local Parameter Decrease")]
    public void SetParameter2()
    {
        AudioManager.Instance.SetLocalParameter(FMODBank_Test.Test, gameObject, FMODParameterList.Test.TestParameter, 1f);
    }

    [ContextMenu("Set Global Parameter Increase")]
    public void SetParameter3()
    {
        AudioManager.Instance.SetGlobalParameter(FMODParameterList.snapshot_Test.GlobalTest, 1f);
    }

    [ContextMenu("Set Global Parameter Decrease")]
    public void SetParameter4()
    {
        AudioManager.Instance.SetGlobalParameter(FMODParameterList.snapshot_Test.GlobalTest, 0f);
    }

}
