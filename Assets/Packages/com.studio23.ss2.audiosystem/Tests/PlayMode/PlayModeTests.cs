using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cysharp.Threading.Tasks;
using Studio23.SS2.AudioSystem.Core;
using Studio23.SS2.AudioSystem.Data;
using System.Linq;
using FMODUnity;
using System;
using Object = UnityEngine.Object;

public class PlayModeTests
{
    private FMODManager _fmodManager;

    [UnityTest]
    [Order(0)]
    public IEnumerator LoadAudioManager()
    {
        _fmodManager = new GameObject().AddComponent<FMODManager>();
        _fmodManager.InitializeOnStart = true;
        Assert.IsTrue(_fmodManager != null);
        yield return null;
    }

    [UnityTest]
    [Order(1)]
    public IEnumerator LoadSingleBank()
    {
        _fmodManager.BanksHandler.LoadBank(Test_FMODBankList.Test);
        Assert.IsTrue(_fmodManager.BanksHandler._bankList.ContainsKey(Test_FMODBankList.Test));
        yield return null;
    }

    [UnityTest]
    [Order(2)]
    public IEnumerator LoadMultipleBanks()
    {
        _fmodManager.BanksHandler.LoadBank(Test_FMODBankList.SFX);
        _fmodManager.BanksHandler.LoadBank(Test_FMODBankList.Music);
        Assert.IsTrue(_fmodManager.BanksHandler._bankList.ContainsKey(Test_FMODBankList.SFX) && _fmodManager.BanksHandler._bankList.ContainsKey(Test_FMODBankList.Music));
        yield return null;
    }

    [UnityTest]
    [Order(3)]
    public IEnumerator SwitchLocalization()
    {
        _fmodManager.BanksHandler.SwitchLocalization(Test_FMODLocaleList.LanguageList[Language.EN], Test_FMODLocaleList.LanguageList[Language.EN]);
        Assert.IsTrue(_fmodManager.BanksHandler._bankList.ContainsKey(Test_FMODLocaleList.LanguageList[Language.EN]));
        yield return null;

    }

    [UnityTest]
    [Order(4)]
    public IEnumerator GetBus()
    {
        _fmodManager.MixerHandler.SetBusVolume(Test_FMODBusList.SFX, 0.65f);
        Assert.IsTrue(_fmodManager.MixerHandler._busDataList.FirstOrDefault(x => x.BusName.Equals(Test_FMODBusList.SFX)) != null);
        yield return null;

    }

    [UnityTest]
    [Order(5)]
    public IEnumerator SetBusVolume()
    {
        _fmodManager.MixerHandler.SetBusVolume(Test_FMODBusList.SFX, 0.8f);
        Assert.IsTrue(_fmodManager.MixerHandler._busDataList.FirstOrDefault(x => x.BusName.Equals(Test_FMODBusList.SFX)).CurrentVolume == 0.8f);
        yield return null;

    }

    [UnityTest]
    [Order(6)]
    public IEnumerator PauseBus()
    {
        _fmodManager.MixerHandler.PauseBus(Test_FMODBusList.SFX);
        Assert.IsTrue(_fmodManager.MixerHandler._busDataList.FirstOrDefault(x => x.BusName.Equals(Test_FMODBusList.SFX)).IsPaused);
        yield return null;

    }

    [UnityTest]
    [Order(7)]
    public IEnumerator UnPauseBus()
    {
        _fmodManager.MixerHandler.UnPauseBus(Test_FMODBusList.SFX);
        Assert.IsFalse(_fmodManager.MixerHandler._busDataList.FirstOrDefault(x => x.BusName.Equals(Test_FMODBusList.SFX)).IsPaused);
        yield return null;
    }

    [UnityTest]
    [Order(8)]
    public IEnumerator MuteBus()
    {
        _fmodManager.MixerHandler.MuteBus(Test_FMODBusList.SFX);
        Assert.IsTrue(_fmodManager.MixerHandler._busDataList.FirstOrDefault(x => x.BusName.Equals(Test_FMODBusList.SFX)).IsMuted);
        yield return null;
    }

    [UnityTest]
    [Order(9)]
    public IEnumerator UnMuteBus()
    {
        _fmodManager.MixerHandler.UnMuteBus(Test_FMODBusList.SFX);
        Assert.IsFalse(_fmodManager.MixerHandler._busDataList.FirstOrDefault(x => x.BusName.Equals(Test_FMODBusList.SFX)).IsMuted);
        yield return null;
    }

    [UnityTest]
    [Order(10)]
    public IEnumerator GetVCA()
    {
        _fmodManager.MixerHandler.SetVCAVolume(Test_FMODVCAList.Player, 0.65f);
        Assert.IsTrue(_fmodManager.MixerHandler._VCADataList.FirstOrDefault(x => x.VCAName.Equals(Test_FMODVCAList.Player)) != null);
        yield return null;
    }

    [UnityTest]
    [Order(11)]
    public IEnumerator SetVCAVolume()
    {
        _fmodManager.MixerHandler.SetVCAVolume(Test_FMODVCAList.Player, 0.8f);
        Assert.IsTrue(_fmodManager.MixerHandler._VCADataList.FirstOrDefault(x => x.VCAName.Equals(Test_FMODVCAList.Player)).CurrentVolume == 0.8f);
        yield return null;
    }

    [UnityTest]
    [Order(12)]
    public IEnumerator CreateEmitter()
    {
        _fmodManager.EventsHandler.CreateEmitter(Test_FMODBank_Test.Test, _fmodManager.gameObject);
        Assert.IsTrue(_fmodManager.EventsHandler._emitterDataList.FirstOrDefault(x =>
            x.BankName.Equals(Test_FMODBank_Test.Test.BankName) &&
            x.EventName.Equals(Test_FMODBank_Test.Test.EventName) &&
            x.ReferenceGameObject == _fmodManager.gameObject) != null);
        yield return null;

    }

    [UnityTest]
    [Order(13)]
    public IEnumerator PlaySound()
    {
        _fmodManager.EventsHandler.Play(Test_FMODBank_Test.Test, _fmodManager.gameObject);
        Assert.IsTrue(_fmodManager.EventsHandler._emitterDataList.FirstOrDefault(x =>
            x.BankName.Equals(Test_FMODBank_Test.Test.BankName) &&
            x.EventName.Equals(Test_FMODBank_Test.Test.EventName) &&
            x.ReferenceGameObject == _fmodManager.gameObject)
            .EventState == FMODEventState.Playing);
        yield return null;
    }

    [UnityTest]
    [Order(14)]
    public IEnumerator PauseSound()
    {
        _fmodManager.EventsHandler.Pause(Test_FMODBank_Test.Test, _fmodManager.gameObject);
        Assert.IsTrue(_fmodManager.EventsHandler._emitterDataList.FirstOrDefault(x =>
            x.BankName.Equals(Test_FMODBank_Test.Test.BankName) &&
            x.EventName.Equals(Test_FMODBank_Test.Test.EventName) &&
            x.ReferenceGameObject == _fmodManager.gameObject)
            .EventState == FMODEventState.Suspended);
        yield return null;
    }

    [UnityTest]
    [Order(15)]
    public IEnumerator UnPauseSound()
    {
        _fmodManager.EventsHandler.UnPause(Test_FMODBank_Test.Test, _fmodManager.gameObject);
        Assert.IsTrue(_fmodManager.EventsHandler._emitterDataList.FirstOrDefault(x =>
            x.BankName.Equals(Test_FMODBank_Test.Test.BankName) &&
            x.EventName.Equals(Test_FMODBank_Test.Test.EventName) &&
            x.ReferenceGameObject == _fmodManager.gameObject)
            .EventState == FMODEventState.Playing);
        yield return null;
    }

    [UnityTest]
    [Order(16)]
    public IEnumerator TogglePause()
    {
        _fmodManager.EventsHandler.TogglePauseAll(true);
        Assert.IsTrue(_fmodManager.EventsHandler._emitterDataList.FirstOrDefault(x =>
            x.BankName.Equals(Test_FMODBank_Test.Test.BankName) &&
            x.EventName.Equals(Test_FMODBank_Test.Test.EventName) &&
            x.ReferenceGameObject == _fmodManager.gameObject)
            .EventState == FMODEventState.Paused);
        yield return null;
    }

    [UnityTest]
    [Order(17)]
    public IEnumerator ToggleUnPause()
    {
        _fmodManager.EventsHandler.TogglePauseAll(false);
        Assert.IsTrue(_fmodManager.EventsHandler._emitterDataList.FirstOrDefault(x =>
            x.BankName.Equals(Test_FMODBank_Test.Test.BankName) &&
            x.EventName.Equals(Test_FMODBank_Test.Test.EventName) &&
            x.ReferenceGameObject == _fmodManager.gameObject)
            .EventState == FMODEventState.Playing);
        yield return null;
    }

    [UnityTest]
    [Order(18)]
    public IEnumerator StopSound()
    {
        _fmodManager.EventsHandler.Stop(Test_FMODBank_Test.Test, _fmodManager.gameObject);
        Assert.IsTrue(_fmodManager.EventsHandler._emitterDataList.FirstOrDefault(x =>
            x.BankName.Equals(Test_FMODBank_Test.Test.BankName) &&
            x.EventName.Equals(Test_FMODBank_Test.Test.EventName) &&
            x.ReferenceGameObject == _fmodManager.gameObject)
            .EventState == FMODEventState.Stopped);
        yield return null;
    }

    [UnityTest]
    [Order(19)]
    public IEnumerator SetLocalParameter()
    {
        _fmodManager.EventsHandler.SetLocalParameter(Test_FMODBank_Test.Test, _fmodManager.gameObject, Test_FMODParameterList.Test.TestParameter, 0.5f);
        _fmodManager.EventsHandler._emitterDataList.FirstOrDefault(x =>
                x.BankName.Equals(Test_FMODBank_Test.Test.BankName) &&
                x.EventName.Equals(Test_FMODBank_Test.Test.EventName) &&
                x.ReferenceGameObject == _fmodManager.gameObject)
            .Emitter.EventInstance
            .getParameterByName(Test_FMODParameterList.Test.TestParameter, out float parameterValue);
        Assert.IsTrue(parameterValue == 0.5f);
        yield return null;
    }

    [UnityTest]
    [Order(20)]
    public IEnumerator SetGlobalParameter()
    {
        _fmodManager.EventsHandler.SetGlobalParameter(Test_FMODParameterList.snapshot_Test.GlobalTest, 0.5f);
        RuntimeManager.StudioSystem.getParameterByName(Test_FMODParameterList.snapshot_Test.GlobalTest, out float parameterValue);
        Assert.IsTrue(parameterValue == 0.5f);
        yield return null;
    }

    [UnityTest]
    [Order(21)]
    public IEnumerator StopAllBusEvents() => UniTask.ToCoroutine(async () =>
    {
        await _fmodManager.MixerHandler.StopAllBusEvents(Test_FMODBusList.Test);
        await UniTask.Delay(TimeSpan.FromSeconds(5));
        Assert.IsTrue(_fmodManager.EventsHandler._emitterDataList.FirstOrDefault(x =>
                x.BankName.Equals(Test_FMODBank_Test.Test.BankName) &&
                x.EventName.Equals(Test_FMODBank_Test.Test.EventName) &&
                x.ReferenceGameObject == _fmodManager.gameObject)
            .EventState == FMODEventState.Stopped);
    });

    /*[UnityTest]
    [Order(22)]
    public IEnumerator LoadBankSampleData() => UniTask.ToCoroutine(async () =>
    {
        _fmodManager.LoadBankSampleData(Test_FMODBankList.Test);
        _fmodManager.Play(Test_FMODBank_Test.Test, _fmodManager.gameObject);
        _fmodManager._bankList[Test_FMODBankList.Test].getSampleLoadingState(out LOADING_STATE loadingState);
        await UniTask.WaitUntil(() => loadingState == LOADING_STATE.LOADED);
        Debug.Log(loadingState);
        Assert.IsTrue(loadingState == LOADING_STATE.LOADED);
    });*/

    [UnityTest]
    [Order(23)]
    public IEnumerator ReleaseSound() => UniTask.ToCoroutine(async () =>
    {
        await _fmodManager.EventsHandler.Release(Test_FMODBank_Test.Test, _fmodManager.gameObject);
        Assert.IsTrue(_fmodManager.EventsHandler._emitterDataList.FirstOrDefault(x =>
            x.BankName.Equals(Test_FMODBank_Test.Test.BankName) && x.EventName.Equals(Test_FMODBank_Test.Test.EventName) && x.ReferenceGameObject == _fmodManager.gameObject) == null);
    });

    [UnityTest]
    [Order(24)]
    public IEnumerator UnloadSingleBank()
    {
        _fmodManager.BanksHandler.UnloadBank(Test_FMODBankList.Test);
        Assert.IsTrue(!_fmodManager.BanksHandler._bankList.ContainsKey(Test_FMODBankList.Test));
        yield return null;
    }

    [UnityTest]
    [Order(25)]
    public IEnumerator UnloadAllBanks()
    {
        _fmodManager.BanksHandler.UnloadAllBanks();
        Assert.IsTrue(_fmodManager.BanksHandler._bankList.Count == 0);
        yield return null;
    }

    [UnityTest]
    [Order(26)]
    public IEnumerator DestroyAudioManager()
    {
        Object.DestroyImmediate(_fmodManager);
        Assert.IsTrue(_fmodManager == null);
        yield return null;
    }
}