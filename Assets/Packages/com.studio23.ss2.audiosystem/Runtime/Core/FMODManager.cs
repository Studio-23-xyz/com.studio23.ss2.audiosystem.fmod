using UnityEngine;

namespace Studio23.SS2.AudioSystem.Core
{
    public class FMODManager : MonoBehaviour
    {
        public EventsHandler EventsHandler;
        public BanksHandler BanksHandler;
        public MixerHandler MixerHandler;

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
            BanksHandler = new BanksHandler();
            EventsHandler = new EventsHandler();
            MixerHandler = new MixerHandler();
        }

        private void OnEnable()
        {
            BanksHandler.OnBankUnloaded += EventsHandler.ClearEmitter;
        }

        private void OnDisable()
        {
            BanksHandler.OnBankUnloaded -= EventsHandler.ClearEmitter;
        }

        private void Start()
        {
            if (InitializeOnStart) Initialize();
        }

        private void Initialize()
        {
            EventsHandler.Initialize();
            BanksHandler.Initialize();
            MixerHandler.Initialize();
        }

        private void OnDestroy()
        {
            BanksHandler.UnloadAllBanks();
        }
    }
}
