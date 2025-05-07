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
        internal Dictionary<string, AssetReference> _bankAssetReferences;

        public delegate UniTask BankEvent(Bank bank);
        public BankEvent OnBankLoaded;
        public BankEvent OnBankUnloaded;

        internal void Initialize()
        {
            _bankList = new Dictionary<string, Bank>();
            _bankAssetReferences = new Dictionary<string, AssetReference>();
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

        public async UniTask LoadBank(AssetReferenceT<TextAsset> assetReference, bool loadSamples = false, System.Action completionCallback = null)
        {
            if (_bankAssetReferences.ContainsValue(assetReference))
            {
                if (FMODManager.Instance.Debug) Debug.Log($"{assetReference} asset reference is already loaded");
                return;
            }

            var handle = assetReference.LoadAssetAsync<TextAsset>();
            handle.WaitForCompletion();
            var bankToLoad = handle.Result;
            var bankName = bankToLoad.name;
            assetReference.ReleaseAsset();

            if (_bankList.ContainsKey(bankName))
            {
                if (FMODManager.Instance.Debug) Debug.Log($"{bankName} bank is already loaded");
                return;
            }

            Debug.Log($"To Load bank:/{bankName}");
            RuntimeManager.LoadBank(assetReference, loadSamples, completionCallback);
            await UniTask.WaitUntil(() => RuntimeManager.HasBankLoaded(assetReference.AssetGUID));
            Bank bank = GetBank(bankName);

            OnBankLoaded?.Invoke(bank);
            _bankList[bankName] = bank;
            _bankAssetReferences[bankName] = assetReference;
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
            Bank bank = GetBank(bankName);
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
            var handle = assetReference.LoadAssetAsync<TextAsset>();
            handle.WaitForCompletion();
            var bankToLoad = handle.Result;
            var bankName = bankToLoad.name;
            assetReference.ReleaseAsset();

            if (!_bankList.ContainsKey(bankName))
            {
                if (FMODManager.Instance.Debug) Debug.Log($"{bankName} bank has not been loaded yet or has already been unloaded");
                return;
            }

            RuntimeManager.UnloadBank(assetReference);
            //await UniTask.WaitUntil(() => RuntimeManager.HasBankLoaded(assetReference.AssetGUID) == false);

            OnBankUnloaded?.Invoke(_bankList[bankName]);
            _bankList[bankName].unloadSampleData();
            _bankList.Remove(bankName);
            _bankAssetReferences.Remove(bankName);
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
                List<AssetReference> banksToRemove = _bankAssetReferences.Values.ToList();
                foreach (var bankName in banksToRemove)
                {
                    UnloadBank(bankName);
                }
                _bankList.Clear();
                _bankAssetReferences.Clear();
            }
        }

        /// <summary>
        /// Loads all non-streaming Sample Data for a bank.
        /// Make sure to load the corresponding bank before loading the Sample Data.
        /// </summary>
        /// <param name="bankName"></param>
        public void LoadBankSampleData(string bankName)
        {
            Bank bank = GetBank(bankName);
            var result = bank.loadSampleData();
            if (result != RESULT.OK)
            {
                Debug.LogError($"Failed to load sample data for '{bankName}': {result}");
            }
        }

        /// <summary>
        /// Switches localization current locale audio table and loading the target locale audio table.
        /// </summary>
        /// <param name="currentLocale"></param>
        /// <param name="targetLocale"></param>
        public void SwitchLocalization(string currentLocale, string targetLocale)
        {
            if (string.IsNullOrEmpty(currentLocale) || string.IsNullOrEmpty(targetLocale)) return;
            UnloadBank(currentLocale);
            LoadBank(targetLocale);
        }

        /// <summary>
        /// Switches localization current locale audio table and loading the target locale audio table.
        /// </summary>
        /// <param name="currentLocale"></param>
        /// <param name="targetLocale"></param>
        public async UniTask SwitchLocalization(AssetReferenceT<TextAsset> currentLocale, AssetReferenceT<TextAsset> targetLocale)
        {
            if (currentLocale == null || targetLocale == null) return;
            UnloadBank(currentLocale);
            await LoadBank(targetLocale);
        }

        private Bank GetBank(string bankName)
        {
            if (FMODUnity.Settings.Instance.ImportType == ImportType.StreamingAssets)
            {
                var key = bankName;
                _bankList.TryGetValue(key, out var bank);
                return bank;
            }

            RuntimeManager.StudioSystem.getBankList(out Bank[] bankList);
            foreach (var bank in bankList)
            {
                bank.getPath(out string path);
                if (path ==  null) continue;
                if (path.Contains(bankName)) return bank;
            }
            return new Bank();
        }

        public Dictionary<string, AssetReference> GetBankAssetReferenceList()
        {
            return _bankAssetReferences;
        }

        #region Debug
        /// <summary>
        /// Prints the names of the banks loaded by FMOD.
        /// </summary>
        public void PrintFMODBankList()
        {
            RuntimeManager.StudioSystem.getBankList(out Bank[] bankList);
            foreach (var bank in bankList)
            {
                bank.getPath(out string path);
                Debug.Log($"FMOD Bank List contains: {path}");
            }
        }

        /// <summary>
        /// Prints the names of the banks stored in this manager when loading banks.
        /// </summary>
        public void PrintBankList()
        {
            foreach (var bank in _bankList)
            {
                bank.Value.getPath(out string name);
                Debug.Log($"Bank List contains: {bank.Key}, {bank.Value}: {name}");
            }
        }

        /// <summary>
        /// Prints the names of the asset references stored when loading banks.
        /// </summary>
        public void PrintBankAssetReferenceList()
        {
            foreach (var bank in _bankAssetReferences)
            {
                Debug.Log($"Asset Reference Bank List contains: {bank.Key}");
            }
        }
        #endregion
    }
}
