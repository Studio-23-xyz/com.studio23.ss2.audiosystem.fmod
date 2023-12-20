using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Cysharp.Threading.Tasks;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Studio23.SS2.AudioSystem.Data;

[assembly: InternalsVisibleTo("com.studio23.ss2.audiosystem.Tests")]
namespace Studio23.SS2.AudioSystem.Core
{
    public class BanksHandler : MonoBehaviour
    {
        internal Dictionary<string, Bank> _bankList;

        public delegate UniTask BankEvent(string bankName);

        public BankEvent OnBankLoaded;
        public BankEvent OnBankUnloaded;

        public void Initialize()
        {
            _bankList = new Dictionary<string, Bank>();
        }

        public void LoadBank(string bankName, LOAD_BANK_FLAGS flag = LOAD_BANK_FLAGS.NORMAL)
        {
            var result = RuntimeManager.StudioSystem.loadBankFile(bankName, flag, out Bank bank);
            if (result == RESULT.OK && !_bankList.ContainsKey(bankName))
            {
                OnBankLoaded?.Invoke(bankName);
                _bankList.Add(bankName, bank);
            }
        }

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

        //private async UniTask ClearEmitter(string bankPath)
        //{
        //    for (int i = 0; i < _emitterDataList.Count; i++)
        //    {
        //        FMODEmitterData emitter = _emitterDataList[i];
        //        if (emitter.BankName.Equals(bankPath))
        //        {
        //            await emitter.ReleaseAsync();
        //            _emitterDataList.Remove(emitter);
        //        }
        //    }
        //}

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

        public void SwitchLocalization(string currentLocale, string targetLocale)
        {
            if (string.IsNullOrEmpty(targetLocale)) return;
            UnloadBank(currentLocale);
            LoadBank(targetLocale);
        }
    }
}
