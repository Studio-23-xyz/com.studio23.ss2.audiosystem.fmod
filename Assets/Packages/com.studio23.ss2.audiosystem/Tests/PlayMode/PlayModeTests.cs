using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cysharp.Threading.Tasks;
using Studio23.SS2.AudioSystem.Core;
using Studio23.SS2.AudioSystem.Data;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using System;
using Object = UnityEngine.Object;

public class PlayModeTests
{
    private AudioManager _audioManager;

    [UnityTest]
    [Order(0)]
    public IEnumerator LoadAudioManager() => UniTask.ToCoroutine(async () =>
    {
        _audioManager = new GameObject().AddComponent<AudioManager>();
        Assert.IsTrue(_audioManager != null);
    });

    [UnityTest]
    [Order(1)]
    public IEnumerator LoadSingleBank() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.LoadBank(Test_FMODBankList.Test);
        Assert.IsTrue(_audioManager._bankList.ContainsKey(Test_FMODBankList.Test));
    });

    [UnityTest]
    [Order(2)]
    public IEnumerator LoadMultipleBanks() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.LoadBank(Test_FMODBankList.SFX);
        _audioManager.LoadBank(Test_FMODBankList.Music);
        Assert.IsTrue(_audioManager._bankList.ContainsKey(Test_FMODBankList.SFX) && _audioManager._bankList.ContainsKey(Test_FMODBankList.Music));
    });

    [UnityTest]
    [Order(3)]
    public IEnumerator SwitchLocalization() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.SwitchLocalization(Test_FMODLocaleList.LanguageList[Language.EN], Test_FMODLocaleList.LanguageList[Language.EN]);
        Assert.IsTrue(_audioManager._bankList.ContainsKey(Test_FMODLocaleList.LanguageList[Language.EN]));
    });

    [UnityTest]
    [Order(4)]
    public IEnumerator GetBus() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.SetBusVolume(Test_FMODBusList.SFX , 0.65f);
        Assert.IsTrue(_audioManager._busDataList.FirstOrDefault(x => x.BusName.Equals(Test_FMODBusList.SFX)) != null);
    });

    [UnityTest]
    [Order(5)]
    public IEnumerator SetBusVolume() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.SetBusVolume(Test_FMODBusList.SFX, 0.8f);
        Assert.IsTrue(_audioManager._busDataList.FirstOrDefault(x => x.BusName.Equals(Test_FMODBusList.SFX)).CurrentVolume == 0.8f);
    });

    [UnityTest]
    [Order(6)]
    public IEnumerator PauseBus() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.PauseBus(Test_FMODBusList.SFX);
        Assert.IsTrue(_audioManager._busDataList.FirstOrDefault(x => x.BusName.Equals(Test_FMODBusList.SFX)).IsPaused);
    });

    [UnityTest]
    [Order(7)]
    public IEnumerator UnPauseBus() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.UnPauseBus(Test_FMODBusList.SFX);
        Assert.IsFalse(_audioManager._busDataList.FirstOrDefault(x => x.BusName.Equals(Test_FMODBusList.SFX)).IsPaused);
    });

    [UnityTest]
    [Order(8)]
    public IEnumerator MuteBus() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.MuteBus(Test_FMODBusList.SFX);
        Assert.IsTrue(_audioManager._busDataList.FirstOrDefault(x => x.BusName.Equals(Test_FMODBusList.SFX)).IsMuted);
    });

    [UnityTest]
    [Order(9)]
    public IEnumerator UnMuteBus() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.UnMuteBus(Test_FMODBusList.SFX);
        Assert.IsFalse(_audioManager._busDataList.FirstOrDefault(x => x.BusName.Equals(Test_FMODBusList.SFX)).IsMuted);
    });

    [UnityTest]
    [Order(10)]
    public IEnumerator GetVCA() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.SetVCAVolume(Test_FMODVCAList.Player, 0.65f);
        Assert.IsTrue(_audioManager._VCADataList.FirstOrDefault(x => x.VCAName.Equals(Test_FMODVCAList.Player)) != null);
    });

    [UnityTest]
    [Order(11)]
    public IEnumerator SetVCAVolume() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.SetVCAVolume(Test_FMODVCAList.Player, 0.8f);
        Assert.IsTrue(_audioManager._VCADataList.FirstOrDefault(x => x.VCAName.Equals(Test_FMODVCAList.Player)).CurrentVolume == 0.8f);
    });

    [UnityTest]
    [Order(12)]
    public IEnumerator CreateEmitter() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.CreateEmitter(Test_FMODBank_Test.Test, _audioManager.gameObject);
        Assert.IsTrue(_audioManager._emitterDataList.FirstOrDefault(x =>
            x.BankName.Equals(Test_FMODBank_Test.Test.BankName) && x.EventName.Equals(Test_FMODBank_Test.Test.EventName) && x.ReferenceGameObject == _audioManager.gameObject) != null);
    });

    [UnityTest]
    [Order(13)]
    public IEnumerator PlaySound() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.Play(Test_FMODBank_Test.Test, _audioManager.gameObject);
        Assert.IsTrue(_audioManager._emitterDataList.FirstOrDefault(x =>
            x.BankName.Equals(Test_FMODBank_Test.Test.BankName) && x.EventName.Equals(Test_FMODBank_Test.Test.EventName) && x.ReferenceGameObject == _audioManager.gameObject).EventState == FMODEventState.Playing);
    });

    [UnityTest]
    [Order(14)]
    public IEnumerator PauseSound() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.Pause(Test_FMODBank_Test.Test, _audioManager.gameObject);
        Assert.IsTrue(_audioManager._emitterDataList.FirstOrDefault(x =>
            x.BankName.Equals(Test_FMODBank_Test.Test.BankName) && x.EventName.Equals(Test_FMODBank_Test.Test.EventName) && x.ReferenceGameObject == _audioManager.gameObject).EventState == FMODEventState.Suspended);
    });

    [UnityTest]
    [Order(15)]
    public IEnumerator UnPauseSound() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.UnPause(Test_FMODBank_Test.Test, _audioManager.gameObject);
        Assert.IsTrue(_audioManager._emitterDataList.FirstOrDefault(x =>
            x.BankName.Equals(Test_FMODBank_Test.Test.BankName) && x.EventName.Equals(Test_FMODBank_Test.Test.EventName) && x.ReferenceGameObject == _audioManager.gameObject).EventState == FMODEventState.Playing);
    });

    [UnityTest]
    [Order(16)]
    public IEnumerator TogglePause() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.TogglePauseAll(true);
        Assert.IsTrue(_audioManager._emitterDataList.FirstOrDefault(x =>
            x.BankName.Equals(Test_FMODBank_Test.Test.BankName) && x.EventName.Equals(Test_FMODBank_Test.Test.EventName) && x.ReferenceGameObject == _audioManager.gameObject).EventState == FMODEventState.Paused);
    });

    [UnityTest]
    [Order(17)]
    public IEnumerator ToggleUnPause() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.TogglePauseAll(false);
        Assert.IsTrue(_audioManager._emitterDataList.FirstOrDefault(x =>
            x.BankName.Equals(Test_FMODBank_Test.Test.BankName) && x.EventName.Equals(Test_FMODBank_Test.Test.EventName) && x.ReferenceGameObject == _audioManager.gameObject).EventState == FMODEventState.Playing);
    });

    [UnityTest]
    [Order(18)]
    public IEnumerator StopSound() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.Stop(Test_FMODBank_Test.Test, _audioManager.gameObject);
        Assert.IsTrue(_audioManager._emitterDataList.FirstOrDefault(x =>
            x.BankName.Equals(Test_FMODBank_Test.Test.BankName) && x.EventName.Equals(Test_FMODBank_Test.Test.EventName) && x.ReferenceGameObject == _audioManager.gameObject).EventState == FMODEventState.Stopped);
    });

    [UnityTest]
    [Order(19)]
    public IEnumerator SetLocalParameter() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.SetLocalParameter(Test_FMODBank_Test.Test, _audioManager.gameObject, Test_FMODParameterList.Test.TestParameter, 0.5f);
        _audioManager._emitterDataList.FirstOrDefault(x =>
                x.BankName.Equals(Test_FMODBank_Test.Test.BankName) &&
                x.EventName.Equals(Test_FMODBank_Test.Test.EventName) &&
                x.ReferenceGameObject == _audioManager.gameObject)
            .Emitter.EventInstance
            .getParameterByName(Test_FMODParameterList.Test.TestParameter, out float parameterValue);
        Assert.IsTrue(parameterValue == 0.5f);
    });

    [UnityTest]
    [Order(20)]
    public IEnumerator SetGlobalParameter() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.SetGlobalParameter(Test_FMODParameterList.snapshot_Test.GlobalTest, 0.5f);
        RuntimeManager.StudioSystem.getParameterByName(Test_FMODParameterList.snapshot_Test.GlobalTest, out float parameterValue);
        Assert.IsTrue(parameterValue == 0.5f);
    });

    [UnityTest]
    [Order(21)]
    public IEnumerator StopAllBusEvents() => UniTask.ToCoroutine(async () =>
    {
        await _audioManager.StopAllBusEvents(Test_FMODBusList.Test);
        await UniTask.Delay(TimeSpan.FromSeconds(5));
        Assert.IsTrue(_audioManager._emitterDataList.FirstOrDefault(x => 
                x.BankName.Equals(Test_FMODBank_Test.Test.BankName) &&
                x.EventName.Equals(Test_FMODBank_Test.Test.EventName) &&
                x.ReferenceGameObject == _audioManager.gameObject)
            .EventState == FMODEventState.Stopped);
    });

    /*[UnityTest]
    [Order(22)]
    public IEnumerator LoadBankSampleData() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.LoadBankSampleData(Test_FMODBankList.Test);
        _audioManager.Play(Test_FMODBank_Test.Test, _audioManager.gameObject);
        _audioManager._bankList[Test_FMODBankList.Test].getSampleLoadingState(out LOADING_STATE loadingState);
        await UniTask.WaitUntil(() => loadingState == LOADING_STATE.LOADED);
        Debug.Log(loadingState);
        Assert.IsTrue(loadingState == LOADING_STATE.LOADED);
    });*/

    [UnityTest]
    [Order(23)]
    public IEnumerator ReleaseSound() => UniTask.ToCoroutine(async () =>
    {
        await _audioManager.Release(Test_FMODBank_Test.Test, _audioManager.gameObject);
        Assert.IsTrue(_audioManager._emitterDataList.FirstOrDefault(x =>
            x.BankName.Equals(Test_FMODBank_Test.Test.BankName) && x.EventName.Equals(Test_FMODBank_Test.Test.EventName) && x.ReferenceGameObject == _audioManager.gameObject) == null);
    });

    [UnityTest]
    [Order(24)]
    public IEnumerator UnloadSingleBank() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.UnloadBank(Test_FMODBankList.Test);
        Assert.IsTrue(!_audioManager._bankList.ContainsKey(Test_FMODBankList.Test));
    });

    [UnityTest]
    [Order(25)]
    public IEnumerator UnloadAllBanks() => UniTask.ToCoroutine(async () =>
    {
        _audioManager.UnloadAllBanks();
        Assert.IsTrue(_audioManager._bankList.Count == 0);
    });

    [UnityTest]
    [Order(26)]
    public IEnumerator DestroyAudioManager() => UniTask.ToCoroutine(async () =>
    {
        Object.DestroyImmediate(_audioManager);
        Assert.IsTrue(_audioManager == null);
    });
}