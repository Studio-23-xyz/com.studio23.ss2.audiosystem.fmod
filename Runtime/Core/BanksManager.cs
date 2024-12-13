using System;
using Cysharp.Threading.Tasks;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;


[assembly: InternalsVisibleTo("com.studio23.ss2.audiosystem.fmod.playmode.tests")]
namespace Studio23.SS2.AudioSystem.fmod.Core
{
    public class BanksManager
    {
        internal Dictionary<string, Bank> _bankList;
        internal List<AssetReference> _bankAssetReferences;

        public delegate UniTask BankEvent(Bank bank);
        public BankEvent OnBankLoaded;
        public BankEvent OnBankUnloaded;

        internal void Initialize()
        {
            _bankList = new Dictionary<string, Bank>();
            _bankAssetReferences = new List<AssetReference>();
        }

        /// <summary>
        /// Loads a Bank.
        /// By default LOAD_BANK_FLAGS is set to NORMAL.
        /// </summary>
        /// <param name="bankName"></param>
        /// <param name="flag"></param>
        public void LoadBank(string bankName, LOAD_BANK_FLAGS flag = LOAD_BANK_FLAGS.NORMAL)
        {
            if (_bankList.ContainsKey(bankName))
            {
                if (FMODManager.Instance.Debug) Debug.Log($"{bankName} bank is already loaded");
                return;
            }

#if !UNITY_EDITOR
            var result = RuntimeManager.StudioSystem.loadBankFile($"{Application.streamingAssetsPath}/{FMODUnity.Settings.Instance.TargetSubFolder}/{bankName}.bank", flag, out Bank bank);
#elif UNITY_EDITOR
            var result = RuntimeManager.StudioSystem.loadBankFile($"{FMODUnity.Settings.Instance.SourceBankPath}/{bankName}.bank", flag, out Bank bank);
#endif

            if (result == RESULT.OK && !_bankList.ContainsKey(bankName))
            {
                OnBankLoaded?.Invoke(bank);
                _bankList[bankName] = bank;
                if (FMODManager.Instance.Debug) Debug.Log($"{bankName} bank has been loaded.");
            }
            else if (result == RESULT.ERR_EVENT_ALREADY_LOADED)
            {
                if (FMODManager.Instance.Debug) Debug.Log($"{bankName} bank is already loaded");
            }
            else Debug.LogError($"Failed to load bank {bankName}: {result}");
        }

        //public void LoadBank(string bankName, bool loadSamples = false)
        //{
        //    if (_bankList.ContainsKey(bankName))
        //    {
        //        if (FMODManager.Instance.Debug) Debug.Log($"{bankName} bank is already loaded");
        //        return;
        //    }

        //    RuntimeManager.LoadBank(bankName, loadSamples);

        //    RuntimeManager.StudioSystem.getBank(bankName, out Bank bank);
        //    OnBankLoaded?.Invoke(bank);
        //    _bankList[bankName] = bank;
        //    if (FMODManager.Instance.Debug) Debug.Log($"{bankName} bank has been loaded.");
        //}

        public void LoadBank(AssetReference assetReference, bool loadSamples = false, System.Action completionCallback = null)
        {
            var bankName = assetReference.ToString().Split(":")[1];
            if (_bankList.ContainsKey(bankName))
            {
                if (FMODManager.Instance.Debug) Debug.Log($"{bankName} bank is already loaded");
                return;
            }

            RuntimeManager.LoadBank(assetReference, loadSamples, completionCallback);

            Debug.Log($"bank:/{bankName}");
            RuntimeManager.StudioSystem.getBank($"bank:/{bankName}", out Bank bank);
            OnBankLoaded?.Invoke(bank);
            _bankList[bankName] = bank;
            _bankAssetReferences.Add(assetReference);
            if (FMODManager.Instance.Debug) Debug.Log($"{bankName} bank has been loaded.");
        }

        /// <summary>
        /// Unloads a Bank.
        /// </summary>
        /// <param name="bankName"></param>
        public void UnloadBank(string bankName)
        {
            if (!_bankList.ContainsKey(bankName))
            {
                if (FMODManager.Instance.Debug) Debug.Log($"{bankName} bank has not been loaded yet or has already been unloaded");
                return;
            }
            Bank bank = BankExists(bankName);
            var result = bank.unload();
            if (result == RESULT.OK)
            {
                OnBankUnloaded?.Invoke(bank);
                bank.unloadSampleData();
                _bankList.Remove(bankName);
                if (FMODManager.Instance.Debug) Debug.Log($"{bankName} bank has been unloaded.");
            }
            else Debug.LogError($"Failed to unload bank '{bankName}': {result}");
        }

        //public async void UnloadBank(string bankName)
        //{
        //    if (!_bankList.ContainsKey(bankName))
        //    {
        //        if (FMODManager.Instance.Debug) Debug.Log($"{bankName} bank has not been loaded yet or has already been unloaded");
        //        return;
        //    }
        //    RuntimeManager.StudioSystem.getBank(bankName, out Bank bank);
        //    await UniTask.Delay(TimeSpan.FromSeconds(5));
        //    OnBankUnloaded?.Invoke(bank);
        //    RuntimeManager.UnloadBank(bankName);
        //    bank.unloadSampleData();
        //    _bankList.Remove(bankName);
        //    if (FMODManager.Instance.Debug) Debug.Log($"{bankName} bank has been unloaded.");
        //}

        public void UnloadBank(AssetReference assetReference)
        {
            var bankName = assetReference.ToString().Split(":")[1];
            if (!_bankList.ContainsKey(bankName))
            {
                if (FMODManager.Instance.Debug) Debug.Log($"{bankName} bank has not been loaded yet or has already been unloaded");
                return;
            }

            RuntimeManager.UnloadBank(assetReference);

            Debug.Log($"bank:/{bankName}");
            RuntimeManager.StudioSystem.getBank($"bank:/{bankName}", out Bank bank);
            
            OnBankUnloaded?.Invoke(bank);
            bank.unloadSampleData();
            _bankList.Remove(bankName);
            _bankAssetReferences.Remove(assetReference);
            if (FMODManager.Instance.Debug) Debug.Log($"{bankName} bank has been unloaded.");
        }

        /// <summary>
        /// Unloads all Banks loaded by user.
        /// Will not unload Banks loaded by FMOD Initialization.
        /// </summary>
        public void UnloadAllBanks()
        {
            if (FMODUnity.Settings.Instance.ImportType == ImportType.StreamingAssets)
            {
                List<string> banksToRemove = _bankList.Keys.ToList();
                foreach (var bankName in banksToRemove)
                {
                    UnloadBank(bankName);
                }
                _bankList.Clear();
            }
            else
            {
                List<AssetReference> banksToRemove = _bankAssetReferences.ToList();
                foreach (var bankName in banksToRemove)
                {
                    UnloadBank(bankName);
                }
                _bankList.Clear();
                _bankAssetReferences.Clear();
            }
        }

        /// <summary>
        /// Returns true if the specified Bank has been loaded.
        /// </summary>
        /// <param name="bankName"></param>
        /// <returns></returns>
        public bool HasBankLoaded(string bankName)
        {
            if (RuntimeManager.HasBankLoaded(bankName))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if all the previously loaded banks have been unloaded.
        /// </summary>
        /// <returns></returns>
        public bool HasAllBanksUnLoaded()
        {
            foreach (var bank in _bankList)
            {
                if (HasBankLoaded(bank.Key)) return false;
            }
            return true;
        }

        /// <summary>
        /// Loads all non-streaming Sample Data for a Bank.
        /// Make sure to load corresponding Bank before loading the Sample Data.
        /// </summary>
        /// <param name="bankName"></param>
        public void LoadBankSampleData(string bankName)
        {
            Bank bank = BankExists(bankName);
            var result = bank.loadSampleData();
            if (result != RESULT.OK)
            {
                Debug.LogError($"Failed to load sample data for '{bankName}': {result}");
            }
        }

        /// <summary>
        /// Switches localization for audio.
        /// </summary>
        /// <param name="currentLocale"></param>
        /// <param name="targetLocale"></param>
        public void SwitchLocalization(string currentLocale, string targetLocale)
        {
            if (string.IsNullOrEmpty(targetLocale)) return;
            UnloadBank(currentLocale);
            LoadBank(targetLocale);
        }

        private Bank BankExists(string bankName)
        {
            var key = bankName;
            _bankList.TryGetValue(key, out var bank);
            return bank;
        }
    }
}
