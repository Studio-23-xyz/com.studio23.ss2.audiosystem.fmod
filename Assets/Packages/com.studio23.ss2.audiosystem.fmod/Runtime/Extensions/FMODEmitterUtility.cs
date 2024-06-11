using Cysharp.Threading.Tasks;
using Studio23.SS2.AudioSystem.fmod.Core;
using Studio23.SS2.AudioSystem.fmod.Data;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace Studio23.SS2
{
    public class FMODEmitterUtility : MonoBehaviour
    {
        [Header("FMOD Event")]
        [SerializeField] private string _eventName;
        [SerializeField] private string _eventGUID;
        public FMODEventData EventData => new FMODEventData(_eventName, _eventGUID);
        [SerializeField] private GameObject _gameObject;

        [Header("Parameter Settings")]
        [SerializeField] private string _parameterName;
        [SerializeField] private float _startValue;
        [SerializeField] private float _endValue;
        [SerializeField] private float _duration;
        private float _parameterValue;

        public bool StopOnFadeOut;
        public bool ReleaseOnFadeOut;

        private UnityEvent _onTweenUpdate;
        private UnityEvent _onTweenComplete;
        private CancellationTokenSource tweenCancellationTokenSource;

        private void Start()
        {
            _onTweenUpdate = new UnityEvent();
            _onTweenComplete = new UnityEvent();
            tweenCancellationTokenSource = new CancellationTokenSource();
            _onTweenComplete.AddListener(_onTweenUpdate.RemoveAllListeners);
        }

        private void OnDestroy()
        {
            _onTweenUpdate.RemoveAllListeners();
            _onTweenComplete.RemoveAllListeners();
        }

        private async void TweenParameter(float startValue, float endValue)
        {
            float currentTime = 0.0f;
            _parameterValue = startValue;
            while (currentTime < _duration && !tweenCancellationTokenSource.IsCancellationRequested)
            {
                currentTime += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(currentTime / _duration);
                _parameterValue = Mathf.Lerp(startValue, endValue, t);
                await UniTask.WaitForFixedUpdate(cancellationToken: tweenCancellationTokenSource.Token).SuppressCancellationThrow();
                _onTweenUpdate?.Invoke();
            }
            _onTweenComplete?.Invoke();
        }

        #region PlayBack

        [ContextMenu("Play")]
        public void Play()
        {
            FMODManager.Instance.EventsManager.Play(EventData, _gameObject);
        }

        [ContextMenu("PlayAllOfType")]
        public void PlayAllOfType()
        {
            FMODManager.Instance.EventsManager.PlayAllOfType(EventData);
        }

        [ContextMenu("Stop")]
        public void Stop()
        {
            FMODManager.Instance.EventsManager.Stop(EventData, _gameObject).Forget();
        }

        [ContextMenu("StopAllOfType")]
        public void StopAllOfType()
        {
            FMODManager.Instance.EventsManager.StopAllOfType(EventData).Forget();
        }

        [ContextMenu("StopAll")]
        public void StopAll()
        {
            FMODManager.Instance.EventsManager.StopAll().Forget();
        }

        [ContextMenu("FadeIn")]
        public void FadeIn()
        {
            SetLocalParameter(_startValue);
            Play();
            RampUpLocalParameter();
        }

        [ContextMenu("FadeInAllOfType")]
        public void FadeInAllOfType()
        {
            SetLocalParameterAllOfType(_startValue);
            PlayAllOfType();
            RampUpLocalParameterAllOfType();
        }

        [ContextMenu("FadeOut")]
        public void FadeOut()
        {
            RampDownLocalParameter();
        }

        [ContextMenu("FadeOutAllOfType")]
        public void FadeOutAllOfType()
        {
            RampDownLocalParameterAllOfType();
        }

        #endregion

        #region Release

        [ContextMenu("Release")]
        public void Release()
        {
            FMODManager.Instance.EventsManager.Release(EventData, _gameObject).Forget();
        }

        [ContextMenu("ReleaseAllOfType using tags")]
        public void ReleaseTargetEmitters(string tag)
        {
            var emitters = GameObject.FindGameObjectsWithTag(tag);
            foreach (var emitter in emitters)
            {
                FMODManager.Instance.EventsManager.Release(EventData, emitter).Forget();
            }
        }

        [ContextMenu("ReleaseAllOfType")]
        public void ReleaseTargetEmitters()
        {
            FMODManager.Instance.EventsManager.ReleaseAllOfType(EventData).Forget();
        }

        [ContextMenu("ReleaseAll")]
        public async void ReleaseAllEmitters()
        {
            await FMODManager.Instance.EventsManager.ReleaseAll();
        }

        #endregion

        #region Local Parameters

        [ContextMenu("SetLocalParameter")]
        public void SetLocalParameter(float value)
        {
            FMODManager.Instance.EventsManager.SetLocalParameterByName(EventData, gameObject, _parameterName, value);
        }

        [ContextMenu("SetLocalParameterAllOfType")]
        public void SetLocalParameterAllOfType(float value)
        {
            FMODManager.Instance.EventsManager.SetLocalParameterAllOfTypeByName(EventData, _parameterName, value);
        }

        private void RampLocalParameter(float startValue, float endValue)
        {
            _onTweenUpdate.AddListener((() =>
            {
                FMODManager.Instance.EventsManager.SetLocalParameterByName(EventData, gameObject, _parameterName, _parameterValue);
            }));
            TweenParameter(startValue, endValue);
        }

        private void RampLocalParameterAllOfType(float startValue, float endValue)
        {
            _onTweenUpdate.AddListener((() =>
            {
                FMODManager.Instance.EventsManager.SetLocalParameterAllOfTypeByName(EventData, _parameterName, _parameterValue);
            }));
            TweenParameter(startValue, endValue);
        }

        [ContextMenu("RampUpLocalParameter")]
        public void RampUpLocalParameter()
        {
            if (StopOnFadeOut) _onTweenComplete.RemoveListener(Stop);
            if (ReleaseOnFadeOut) _onTweenComplete.RemoveListener(Release);
            RampLocalParameter(_startValue, _endValue);
        }

        [ContextMenu("RampUpLocalParameterAllOfType")]
        public void RampUpLocalParameterAllOfType()
        {
            if (StopOnFadeOut) _onTweenComplete.RemoveListener(StopAllOfType);
            if (ReleaseOnFadeOut) _onTweenComplete.RemoveListener(ReleaseAllEmitters);
            RampLocalParameterAllOfType(_startValue, _endValue);
        }

        [ContextMenu("RampDownLocalParameter")]
        public void RampDownLocalParameter()
        {
            if (StopOnFadeOut) _onTweenComplete.AddListener(Stop);
            if (ReleaseOnFadeOut) _onTweenComplete.AddListener(Release);
            RampLocalParameter(_endValue, _startValue);
        }

        [ContextMenu("RampDownLocalParameterAllOfType")]
        public void RampDownLocalParameterAllOfType()
        {
            if (StopOnFadeOut) _onTweenComplete.AddListener(StopAllOfType);
            if (ReleaseOnFadeOut) _onTweenComplete.AddListener(ReleaseTargetEmitters);
            RampLocalParameterAllOfType(_endValue, _startValue);
        }

        #endregion

        #region Global Parameters

        [ContextMenu("SetGlobalParameter")]
        public void SetGlobalParameter()
        {
            FMODManager.Instance.EventsManager.SetGlobalParameterByName(_parameterName, _endValue);
        }
        
        private void RampGlobalParameter(float startValue, float endValue)
        {
            _onTweenUpdate.AddListener((() =>
            {
                FMODManager.Instance.EventsManager.SetGlobalParameterByName(_parameterName, _parameterValue);
            }));
            TweenParameter(startValue, endValue);
        }

        [ContextMenu("RampUpGlobalParameter")]
        public void RampUpGlobalParameter()
        {
            _onTweenComplete.RemoveListener(Release);
            RampGlobalParameter(_startValue, _endValue);
        }

        [ContextMenu("RampDownGlobalParameter")]
        public void RampDownGlobalParameter()
        {
            _onTweenComplete.AddListener(Release);
            RampGlobalParameter(_endValue, _startValue);
        }

        #endregion
    }
}
