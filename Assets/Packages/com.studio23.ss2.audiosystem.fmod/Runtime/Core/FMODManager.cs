using UnityEngine;

namespace Studio23.SS2.AudioSystem.fmod.Core
{
    public class FMODManager : MonoBehaviour
    {
        public EventsManager EventsManager;
        public BanksManager BanksManager;
        public MixerManager MixerManager;

        /// <summary>
        /// Set true to Initialize on Start.
        /// </summary>
        public bool InitializeOnStart;
        
        private static FMODManager _instance;
        public static FMODManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<FMODManager>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("FMODManager");
                        _instance = obj.AddComponent<FMODManager>();
                        obj.name = "FMODManager";
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
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

        private void Initialize()
        {
            EventsManager.Initialize();
            BanksManager.Initialize();
            MixerManager.Initialize();
        }

        private void OnDestroy()
        {
            BanksManager.UnloadAllBanks();
        }
    }
}
