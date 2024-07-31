using Cysharp.Threading.Tasks;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Debug = UnityEngine.Debug;

[assembly: InternalsVisibleTo("com.studio23.ss2.audiosystem.fmod.playmode.tests")]
namespace Studio23.SS2.AudioSystem.fmod.Core
{
    public class BanksManager
    {
        internal Dictionary<string, Bank> _bankList;

        public delegate UniTask BankEvent(Bank bank);
        public BankEvent OnBankLoaded;
        public BankEvent OnBankUnloaded;

        internal void Initialize()
        {
            _bankList = new Dictionary<string, Bank>();
        }

        /// <summary>
        /// Loads a Bank.
        /// By default LOAD_BANK_FLAGS is set to NORMAL.
        /// </summary>
        /// <param name="bankName"></param>
        /// <param name="flag"></param>
        public void LoadBank(string bankName, LOAD_BANK_FLAGS flag = LOAD_BANK_FLAGS.NORMAL)
        {
            var result = RuntimeManager.StudioSystem.loadBankFile(bankName, flag, out Bank bank);
            if (result == RESULT.OK && !_bankList.ContainsKey(bankName))
            {
                OnBankLoaded?.Invoke(bank);
                _bankList[bankName] = bank;
                if (FMODManager.Instance.Debug) Debug.Log($"{bankName.Split($"{FMODUnity.Settings.Instance.SourceBankPath}/")[1]} has been loaded. Path: {bankName}");
            }
            else Debug.LogError($"Failed to load bank '{bankName.Split($"{FMODUnity.Settings.Instance.SourceBankPath}/")[1]}': {result}");
        }

        /// <summary>
        /// Unloads a Bank.
        /// </summary>
        /// <param name="bankName"></param>
        public void UnloadBank(string bankName)
        {
            Bank bank = BankExists(bankName);
            var result = bank.unload();
            if (result == RESULT.OK)
            {
                bank.getPath(out string bankPath);
                OnBankUnloaded?.Invoke(bank);
                bank.unloadSampleData();
                _bankList.Remove(bankName);
                if (FMODManager.Instance.Debug) Debug.Log($"{bankName.Split($"{FMODUnity.Settings.Instance.SourceBankPath}/")[1]} has been unloaded.");
            }
            else Debug.LogError($"Failed to unload bank '{bankName.Split($"{FMODUnity.Settings.Instance.SourceBankPath}/")[1]}': {result}");
        }

        /// <summary>
        /// Unloads all Banks loaded by user.
        /// Will not unload Banks loaded by FMOD Initialization.
        /// </summary>
        public void UnloadAllBanks()
        {
            List<string> banksToRemove = _bankList.Keys.ToList();
            foreach (var bankName in banksToRemove)
            {
                UnloadBank(bankName);
            }
            _bankList.Clear();
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
