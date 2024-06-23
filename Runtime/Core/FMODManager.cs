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
        }

        private void OnDestroy()
        {
            BanksManager.UnloadAllBanks();
        }
    }
}
