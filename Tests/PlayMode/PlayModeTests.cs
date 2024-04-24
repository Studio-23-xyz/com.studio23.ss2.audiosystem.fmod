//using Cysharp.Threading.Tasks;
//using FMODUnity;
//using NUnit.Framework;
//using Studio23.SS2.AudioSystem.fmod.Core;
//using Studio23.SS2.AudioSystem.fmod.Data;
//using System;
//using System.Collections;
//using UnityEngine;
//using UnityEngine.TestTools;
//using Object = UnityEngine.Object;

//namespace Studio23.SS2.AudioSystem.fmod.Tests.Playmode
//{
//    public class PlayModeTests
//    {
//        private FMODManager _fmodManager;

//        [UnityTest]
//        [Order(0)]
//        public IEnumerator LoadAudioManager()
//        {
//            _fmodManager = new GameObject().AddComponent<FMODManager>();
//            _fmodManager.InitializeOnStart = true;
//            Assert.IsTrue(_fmodManager != null);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(1)]
//        public IEnumerator LoadSingleBank()
//        {
//            _fmodManager.BanksManager.LoadBank(Test_FMODBankList.Test);
//            Assert.IsTrue(_fmodManager.BanksManager._bankList.ContainsKey(Test_FMODBankList.Test));
//            yield return null;
//        }

//        [UnityTest]
//        [Order(2)]
//        public IEnumerator LoadMultipleBanks()
//        {
//            _fmodManager.BanksManager.LoadBank(Test_FMODBankList.SFX);
//            _fmodManager.BanksManager.LoadBank(Test_FMODBankList.DialogueTable_LOCALE_EN);
//            Assert.IsTrue(_fmodManager.BanksManager._bankList.ContainsKey(Test_FMODBankList.SFX) &&
//                          _fmodManager.BanksManager._bankList.ContainsKey(Test_FMODBankList.DialogueTable_LOCALE_EN));
//            yield return null;
//        }

//        [UnityTest]
//        [Order(3)]
//        public IEnumerator SwitchLocalization()
//        {
//            _fmodManager.BanksManager.SwitchLocalization(Test_FMODLocaleList.LanguageList[Language.EN],
//                Test_FMODLocaleList.LanguageList[Language.JP]);
//            Assert.IsTrue(
//                _fmodManager.BanksManager._bankList.ContainsKey(Test_FMODLocaleList.LanguageList[Language.JP]));
//            yield return null;
//        }

//        [UnityTest]
//        [Order(4)]
//        public IEnumerator GetBus()
//        {
//            _fmodManager.MixerManager.SetBusVolume(Test_FMODBusList.SFX, 0.65f);
//            Assert.IsTrue(_fmodManager.MixerManager._busDataList[Test_FMODBusList.SFX] != null);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(5)]
//        public IEnumerator SetBusVolume()
//        {
//            _fmodManager.MixerManager.SetBusVolume(Test_FMODBusList.SFX, 0.8f);
//            Assert.IsTrue(_fmodManager.MixerManager._busDataList[Test_FMODBusList.SFX].CurrentVolume == 0.8f);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(6)]
//        public IEnumerator PauseBus()
//        {
//            _fmodManager.MixerManager.PauseBus(Test_FMODBusList.SFX);
//            Assert.IsTrue(_fmodManager.MixerManager._busDataList[Test_FMODBusList.SFX].IsPaused);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(7)]
//        public IEnumerator UnPauseBus()
//        {
//            _fmodManager.MixerManager.UnPauseBus(Test_FMODBusList.SFX);
//            Assert.IsFalse(_fmodManager.MixerManager._busDataList[Test_FMODBusList.SFX].IsPaused);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(8)]
//        public IEnumerator MuteBus()
//        {
//            _fmodManager.MixerManager.MuteBus(Test_FMODBusList.SFX);
//            Assert.IsTrue(_fmodManager.MixerManager._busDataList[Test_FMODBusList.SFX].IsMuted);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(9)]
//        public IEnumerator UnMuteBus()
//        {
//            _fmodManager.MixerManager.UnMuteBus(Test_FMODBusList.SFX);
//            Assert.IsFalse(_fmodManager.MixerManager._busDataList[Test_FMODBusList.SFX].IsMuted);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(10)]
//        public IEnumerator GetVCA()
//        {
//            _fmodManager.MixerManager.SetVCAVolume(Test_FMODVCAList.Player, 0.65f);
//            Assert.IsTrue(_fmodManager.MixerManager._VCADataList[Test_FMODVCAList.Player] != null);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(11)]
//        public IEnumerator SetVCAVolume()
//        {
//            _fmodManager.MixerManager.SetVCAVolume(Test_FMODVCAList.Player, 0.8f);
//            Assert.IsTrue(_fmodManager.MixerManager._VCADataList[Test_FMODVCAList.Player].CurrentVolume == 0.8f);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(12)]
//        public IEnumerator CreateEmitter()
//        {
//            _fmodManager.EventsManager.CreateEmitter(Test_FMODBank_Test.Test, _fmodManager.gameObject);
//            Assert.IsTrue(_fmodManager.EventsManager._emitterDataList[(Test_FMODBank_Test.Test.BankName, Test_FMODBank_Test.Test.EventName, _fmodManager.gameObject.GetInstanceID())] != null);
//            yield return null;
//        }

//        //[UnityTest]
//        //[Order(13)]
//        //public IEnumerator LoadSampleDataForEvent() => UniTask.ToCoroutine(async () =>
//        //{
//        //    _fmodManager.EventsManager.LoadEventSampleData(Test_FMODBank_Test.Test, _fmodManager.gameObject);
//        //    _fmodManager.EventsManager._emitterDataList[(Test_FMODBank_Test.Test.BankName, Test_FMODBank_Test.Test.EventName, _fmodManager.gameObject.GetInstanceID())].Emitter.EventDescription.getSampleLoadingState(out LOADING_STATE loadingState);
//        //    await UniTask.WaitUntil(() => loadingState == LOADING_STATE.LOADED);
//        //    Assert.IsTrue(loadingState == LOADING_STATE.LOADED);
//        //});

//        [UnityTest]
//        [Order(14)]
//        public IEnumerator PlaySound()
//        {
//            _fmodManager.EventsManager.Play(Test_FMODBank_Test.Test, _fmodManager.gameObject);
//            Assert.IsTrue(_fmodManager.EventsManager._emitterDataList[(Test_FMODBank_Test.Test.BankName, Test_FMODBank_Test.Test.EventName, _fmodManager.gameObject.GetInstanceID())].EventState == FMODEventState.Playing);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(15)]
//        public IEnumerator PauseSound()
//        {
//            _fmodManager.EventsManager.Pause(Test_FMODBank_Test.Test, _fmodManager.gameObject);
//            Assert.IsTrue(_fmodManager.EventsManager._emitterDataList[(Test_FMODBank_Test.Test.BankName, Test_FMODBank_Test.Test.EventName, _fmodManager.gameObject.GetInstanceID())].EventState == FMODEventState.Suspended);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(16)]
//        public IEnumerator UnPauseSound()
//        {
//            _fmodManager.EventsManager.UnPause(Test_FMODBank_Test.Test, _fmodManager.gameObject);
//            Assert.IsTrue(_fmodManager.EventsManager._emitterDataList[(Test_FMODBank_Test.Test.BankName, Test_FMODBank_Test.Test.EventName, _fmodManager.gameObject.GetInstanceID())].EventState == FMODEventState.Playing);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(17)]
//        public IEnumerator TogglePause()
//        {
//            _fmodManager.EventsManager.TogglePauseAll(true);
//            Assert.IsTrue(_fmodManager.EventsManager._emitterDataList[(Test_FMODBank_Test.Test.BankName, Test_FMODBank_Test.Test.EventName, _fmodManager.gameObject.GetInstanceID())].EventState == FMODEventState.Paused);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(18)]
//        public IEnumerator ToggleUnPause()
//        {
//            _fmodManager.EventsManager.TogglePauseAll(false);
//            Assert.IsTrue(_fmodManager.EventsManager._emitterDataList[(Test_FMODBank_Test.Test.BankName, Test_FMODBank_Test.Test.EventName, _fmodManager.gameObject.GetInstanceID())].EventState == FMODEventState.Playing);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(19)]
//        public IEnumerator StopSound() => UniTask.ToCoroutine(async () =>
//        {
//            await _fmodManager.EventsManager.Stop(Test_FMODBank_Test.Test, _fmodManager.gameObject);
//            Assert.IsTrue(_fmodManager.EventsManager._emitterDataList[(Test_FMODBank_Test.Test.BankName, Test_FMODBank_Test.Test.EventName, _fmodManager.gameObject.GetInstanceID())].EventState == FMODEventState.Stopped);
//        });

//        [UnityTest]
//        [Order(20)]
//        public IEnumerator SetLocalParameter()
//        {
//            _fmodManager.EventsManager.SetLocalParameter(Test_FMODBank_Test.Test, _fmodManager.gameObject,
//                Test_FMODParameterList.Test.TestParameter, 0.5f);
//            _fmodManager.EventsManager._emitterDataList[(Test_FMODBank_Test.Test.BankName, Test_FMODBank_Test.Test.EventName, _fmodManager.gameObject.GetInstanceID())].Emitter.EventInstance
//                .getParameterByName(Test_FMODParameterList.Test.TestParameter, out float parameterValue);
//            Assert.IsTrue(parameterValue == 0.5f);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(21)]
//        public IEnumerator SetGlobalParameter()
//        {
//            _fmodManager.EventsManager.SetGlobalParameter(Test_FMODParameterList.snapshot_Test.GlobalTest, 0.5f);
//            RuntimeManager.StudioSystem.getParameterByName(Test_FMODParameterList.snapshot_Test.GlobalTest,
//                out float parameterValue);
//            Assert.IsTrue(parameterValue == 0.5f);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(22)]
//        public IEnumerator StopAllBusEvents() => UniTask.ToCoroutine(async () =>
//        {
//            await _fmodManager.MixerManager.StopAllBusEvents(Test_FMODBusList.Sample);
//            await UniTask.Delay(TimeSpan.FromSeconds(5));
//            Assert.IsTrue(_fmodManager.EventsManager._emitterDataList[(Test_FMODBank_Test.Test.BankName, Test_FMODBank_Test.Test.EventName, _fmodManager.gameObject.GetInstanceID())].EventState == FMODEventState.Stopped);
//        });

//        //[UnityTest]
//        //[Order(24)]
//        //public IEnumerator LoadBankSampleData() => UniTask.ToCoroutine(async () =>
//        //{
//        //    _fmodManager.BanksManager.LoadBankSampleData(Test_FMODBankList.Test);
//        //    _fmodManager.EventsManager.Play(Test_FMODBank_Test.Test, _fmodManager.gameObject);
//        //    _fmodManager.BanksManager._bankList[Test_FMODBankList.Test].getSampleLoadingState(out LOADING_STATE loadingState);
//        //    await UniTask.WaitUntil(() => loadingState == LOADING_STATE.LOADED);
//        //    Debug.Log(loadingState);
//        //    Assert.IsTrue(loadingState == LOADING_STATE.LOADED);
//        //});

//        [UnityTest]
//        [Order(25)]
//        public IEnumerator ReleaseSound() => UniTask.ToCoroutine(async () =>
//        {
//            await _fmodManager.EventsManager.Release(Test_FMODBank_Test.Test, _fmodManager.gameObject);
//            Assert.IsFalse(_fmodManager.EventsManager._emitterDataList.ContainsKey((Test_FMODBank_Test.Test.BankName, Test_FMODBank_Test.Test.EventName, _fmodManager.gameObject.GetInstanceID())));
//        });

//        [UnityTest]
//        [Order(26)]
//        public IEnumerator UnloadSingleBank()
//        {
//            _fmodManager.BanksManager.UnloadBank(Test_FMODBankList.Test);
//            Assert.IsTrue(!_fmodManager.BanksManager._bankList.ContainsKey(Test_FMODBankList.Test));
//            yield return null;
//        }

//        [UnityTest]
//        [Order(27)]
//        public IEnumerator UnloadAllBanks()
//        {
//            _fmodManager.BanksManager.UnloadAllBanks();
//            Assert.IsTrue(_fmodManager.BanksManager._bankList.Count == 0);
//            yield return null;
//        }

//        [UnityTest]
//        [Order(28)]
//        public IEnumerator DestroyAudioManager()
//        {
//            Object.DestroyImmediate(_fmodManager);
//            Assert.IsTrue(_fmodManager == null);
//            yield return null;
//        }
//    }
//}