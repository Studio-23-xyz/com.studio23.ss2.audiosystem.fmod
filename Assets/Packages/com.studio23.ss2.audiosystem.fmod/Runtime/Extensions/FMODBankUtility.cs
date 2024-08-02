using Studio23.SS2.AudioSystem.fmod.Core;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace Studio23.SS2.AudioSystem.fmod
{
    public class FMODBankUtility : MonoBehaviour
    {
        public LoaderGameEvent LoadEvent;
        public LoaderGameEvent UnloadEvent;
        [BankRef] public List<string> Banks;
        public string CollisionTag;
        private bool isQuitting;

        private void HandleGameEvent(LoaderGameEvent gameEvent)
        {
            if (LoadEvent == gameEvent)
            {
                LoadBank();
            }
            if (UnloadEvent == gameEvent)
            {
                UnloadBank();
            }
        }

        private void Start()
        {
            RuntimeUtils.EnforceLibraryOrder();
            HandleGameEvent(LoaderGameEvent.ObjectStart);
        }

        private void OnApplicationQuit()
        {
            isQuitting = true;
        }

        private void OnDestroy()
        {
            if (!isQuitting)
            {
                HandleGameEvent(LoaderGameEvent.ObjectDestroy);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (string.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
            {
                HandleGameEvent(LoaderGameEvent.TriggerEnter);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (string.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
            {
                HandleGameEvent(LoaderGameEvent.TriggerExit);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (string.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
            {
                HandleGameEvent(LoaderGameEvent.TriggerEnter2D);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (string.IsNullOrEmpty(CollisionTag) || other.CompareTag(CollisionTag))
            {
                HandleGameEvent(LoaderGameEvent.TriggerExit2D);
            }
        }

        private void OnEnable()
        {
            HandleGameEvent(LoaderGameEvent.ObjectEnable);
        }

        private void OnDisable()
        {
            HandleGameEvent(LoaderGameEvent.ObjectDisable);
        }

        [ContextMenu("Load")]
        public void LoadBank()
        {
            foreach (var b in Banks)
            {
                FMODManager.Instance.BanksManager.LoadBank($"{FMODUnity.Settings.Instance.SourceBankPath}/{b}.bank");
            }
        }

        [ContextMenu("Unload")]
        public void UnloadBank()
        {
            foreach (var b in Banks)
            {
                FMODManager.Instance.BanksManager.UnloadBank($"{FMODUnity.Settings.Instance.SourceBankPath}/{b}.bank");
            }
        }

        [ContextMenu("UnloadAll")]
        public void UnloadAllBanks()
        {
            FMODManager.Instance.BanksManager.UnloadAllBanks();
        }
    }
}
