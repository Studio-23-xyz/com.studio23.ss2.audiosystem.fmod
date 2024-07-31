using Studio23.SS2.AudioSystem.fmod.Core;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace Studio23.SS2.AudioSystem.fmod
{
    public class FMODBankUtility : MonoBehaviour
    {
        [SerializeField]
        [BankRef]
        public List<string> Banks;

        [ContextMenu("LoadBanks")]
        public void LoadBanks()
        {
            foreach (var b in Banks)
            {
                FMODManager.Instance.BanksManager.LoadBank($"{FMODUnity.Settings.Instance.SourceBankPath}/{b}.bank");
                if (FMODManager.Instance.Debug) Debug.Log($"{b}.bank has been loaded. Path: {FMODUnity.Settings.Instance.SourceBankPath}/{b}.bank");
            }
        }

        [ContextMenu("UnloadBanks")]
        public void UnloadBanks()
        {
            foreach (var b in Banks)
            {
                FMODManager.Instance.BanksManager.UnloadBank($"{FMODUnity.Settings.Instance.SourceBankPath}/{b}.bank");
            }
        }

        [ContextMenu("UnloadAllBanks")]
        public void UnloadAllBanks()
        {
            FMODManager.Instance.BanksManager.UnloadAllBanks();
        }
    }
}
