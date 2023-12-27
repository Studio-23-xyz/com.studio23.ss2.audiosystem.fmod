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

        public delegate UniTask BankEvent(string bankName);
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
                OnBankLoaded?.Invoke(bankName);
                _bankList.Add(bankName, bank);
            }
        }

        /// <summary>
        /// Unloads a Bank.
        /// </summary>
        /// <param name="bankName"></param>
        public void UnloadBank(string bankName)
        {
            if (_bankList.TryGetValue(bankName, out Bank bank))
            {
                var result = bank.unload();
                if (result == RESULT.OK)
                {
                    bank.getPath(out string bankPath);
                    OnBankUnloaded?.Invoke(bankPath);
                    bank.unloadSampleData();
                    _bankList.Remove(bankName);
                }
                else
                {
                    Debug.LogError($"Failed to unload bank '{bankName}': {result}");
                }
            }
            else
            {
                Debug.LogWarning($"Bank '{bankName}' not found in the bank list.");
            }
        }

        /// <summary>
        /// Unloads all Banks loaded by user.
        /// Will not unload Banks loaded by FMOD Initialization.
        /// </summary>
        public void UnloadAllBanks()
        {
            List<string> banksToRemove = _bankList.Keys.ToList();
            foreach (var bank in banksToRemove)
            {
                UnloadBank(bank);
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
            if (_bankList.TryGetValue(bankName, out Bank bank))
            {
                var result = bank.loadSampleData();
                if (result != RESULT.OK)
                {
                    Debug.LogError($"Failed to load sample data for '{bankName}': {result}");
                }
            }
            else
            {
                Debug.LogWarning($"Bank '{bankName}' not found in the bank list.");
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
    }
}
