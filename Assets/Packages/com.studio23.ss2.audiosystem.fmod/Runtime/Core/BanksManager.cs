using Cysharp.Threading.Tasks;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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
            for (int i = 0; i < _bankList.Count; i++)
            {
                if (_bankList.ElementAt(i).Key.Equals(bankName))
                {
                    RemoveBank(i);
                }
            }
        }

        /// <summary>
        /// Unloads all Banks loaded by user.
        /// Will not unload Banks loaded by FMOD at Game start.
        /// </summary>
        public void UnloadAllBanks()
        {
            for (int i = 0; i < _bankList.Count; i++)
            {
                RemoveBank(i);
            }

            _bankList.Clear();
        }

        private void RemoveBank(int index)
        {
            var bank = _bankList.ElementAt(index).Value;
            bank.getPath(out string bankPath);
            OnBankUnloaded?.Invoke(bankPath);
            bank.unloadSampleData();
            bank.unload();
            _bankList.Remove(_bankList.ElementAt(index).Key);
        }

        /// <summary>
        /// Loads Sample Data for a Bank.
        /// Make sure to load the Sample Data for a Bank that has already been loaded.
        /// </summary>
        /// <param name="bankName"></param>
        public void LoadBankSampleData(string bankName)
        {
            for (int i = 0; i < _bankList.Count; i++)
            {
                if (_bankList.ElementAt(i).Key.Equals(bankName))
                {
                    _bankList.ElementAt(i).Value.loadSampleData();
                    break;
                }
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
