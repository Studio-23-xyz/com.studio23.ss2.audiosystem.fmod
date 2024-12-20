using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace Studio23.SS2.AudioSystem.fmod.Core
{
    public class FMODManager : MonoBehaviour
    {
        public EventsManager EventsManager;
        public BanksManager BanksManager;
        public MixerManager MixerManager;

        public UnityEvent OnInitializeComplete;

        public List<AssetReferenceT<TextAsset>> Banks = new List<AssetReferenceT<TextAsset>>();

        /// <summary>
        /// Set true to Initialize on Start.
        /// </summary>
        public bool InitializeOnStart;
        public bool Debug;

        public static FMODManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
            else if (Instance != this)
            {
                DestroyImmediate(this);
            }

            EventsManager = new EventsManager();
            BanksManager = new BanksManager();
            MixerManager = new MixerManager();
        }

        private void OnEnable()
        {
            BanksManager.OnBankUnloaded += EventsManager.ClearEmitter;
        }

        private void OnDisable()
        {
            BanksManager.OnBankUnloaded -= EventsManager.ClearEmitter;
        }

        private void Start()
        {
            if (InitializeOnStart) Initialize();
        }

        public void Initialize()
        {
            EventsManager.Initialize();
            BanksManager.Initialize();
            MixerManager.Initialize();

            OnInitializeComplete?.Invoke();
        }

        [ContextMenu("Print All Lists")]
        public void PrintAllLists()
        {
            BanksManager.PrintFMODBankList();
            BanksManager.PrintBankList();
            BanksManager.PrintBankAssetReferenceList();
        }

        [ContextMenu("Print FMOD Bank List")]
        public void PrintFMODBankList()
        {
            BanksManager.PrintFMODBankList();
        }

        [ContextMenu("Print Bank List")]
        public void PrintBankList()
        {
            BanksManager.PrintBankList();
        }

        [ContextMenu("Print Bank Asset Reference List")]
        public void PrintBankAssetReferenceList()
        {
            BanksManager.PrintBankAssetReferenceList();
        }

        [ContextMenu("Remove Banks")]
        public void RemoveBanks()
        {
            foreach (var b in Banks)
            {
                RuntimeManager.UnloadBank(b);
            }
        }
    }
}
