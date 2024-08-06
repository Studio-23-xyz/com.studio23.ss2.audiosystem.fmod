using Cysharp.Threading.Tasks;
using FMODUnity;
using Studio23.SS2.AudioSystem.fmod.Core;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace Studio23.SS2.AudioSystem.fmod.Extensions
{
    public class FMODEmitterUtility : MonoBehaviour
    {
        public EventReference EventReference;
        public string EventGUID => EventReference.Guid.ToString();
        [SerializeField] private GameObject _gameObject;

        private float _parameterValue;
        [SerializeField] private string _parameterName;
        [SerializeField] private float _startValue;
        [SerializeField] private float _endValue;
        [SerializeField] private float _duration;

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

        public void SetParameterName(string name)
        {
            _parameterName = name;
        }

        #region PlayBack

        /// <summary>
        /// Creates an Emitter and plays the specified Event.
        /// </summary>
        [ContextMenu("Play")]
        public void Play()
        {
            FMODManager.Instance.EventsManager.Play(EventGUID, _gameObject);
        }

        /// <summary>
        /// Plays all existing emitters of the specified Event.
        /// </summary>
        [ContextMenu("PlayAllOfType")]
        public void PlayAllOfType()
        {
            FMODManager.Instance.EventsManager.PlayAllOfType(EventGUID);
        }

        /// <summary>
        /// Pauses the specified Emitter.
        /// </summary>
        [ContextMenu("Pause")]
        public void Pause()
        {
            FMODManager.Instance.EventsManager.Pause(EventGUID, _gameObject);
        }

        /// <summary>
        /// UnPauses the specified Emitter.
        /// </summary>
        [ContextMenu("Unpause")]
        public void Unpause()
        {
            FMODManager.Instance.EventsManager.Unpause(EventGUID, _gameObject);
        }

        /// <summary>
        /// Pauses all existing emitters of the specified Event.
        /// </summary>
        [ContextMenu("PauseAllOfType")]
        public void PauseAllOfType()
        {
            FMODManager.Instance.EventsManager.PauseAllOfType(EventGUID);
        }

        /// <summary>
        /// UnPauses all existing emitters of the specified Event.
        /// </summary>
        [ContextMenu("UnpauseAllOfType")]
        public void UnpauseAllOfType()
        {
            FMODManager.Instance.EventsManager.UnpauseAllOfType(EventGUID);
        }

        /// <summary>
        /// Pauses/UnPauses all existing Emitters in the scene.
        /// </summary>
        /// <param name="isGamePaused"></param>
        public void TogglePause(bool isGamePaused)
        {
            FMODManager.Instance.EventsManager.TogglePause(isGamePaused);
        }

        /// <summary>
        /// Stops the specified Emitter.
        /// </summary>
        [ContextMenu("Stop")]
        public void Stop()
        {
            FMODManager.Instance.EventsManager.Stop(EventGUID, _gameObject).Forget();
        }

        /// <summary>
        /// Stops all existing emitters of the specified Event.
        /// </summary>
        [ContextMenu("StopAllOfType")]
        public void StopAllOfType()
        {
            FMODManager.Instance.EventsManager.StopAllOfType(EventGUID).Forget();
        }

        /// <summary>
        /// Stops all existing Emitters in the scene.
        /// </summary>
        [ContextMenu("StopAll")]
        public void StopAll()
        {
            FMODManager.Instance.EventsManager.StopAll().Forget();
        }

        #endregion

        #region Release

        [ContextMenu("Release")]
        public void Release()
        {
            FMODManager.Instance.EventsManager.Release(EventGUID, _gameObject).Forget();
        }

        public void ReleaseTargetEmitters(string tag)
        {
            var emitters = GameObject.FindGameObjectsWithTag(tag);
            foreach (var emitter in emitters)
            {
                FMODManager.Instance.EventsManager.Release(EventGUID, emitter).Forget();
            }
        }

        [ContextMenu("ReleaseAllOfType")]
        public void ReleaseTargetEmitters()
        {
            FMODManager.Instance.EventsManager.ReleaseAllOfType(EventGUID).Forget();
        }

        [ContextMenu("ReleaseAll")]
        public async void ReleaseAllEmitters()
        {
            await FMODManager.Instance.EventsManager.ReleaseAll();
        }

        #endregion

        #region Volume Fade In/Out
        // Use these methods for fading in/out your audio. 
        // The event must have some parameter that controls the master volume of the event.
        // Should not be used for ramping other parameters. May not work as expected.
        // For the intended behavior, _startValue should be set to 0, _endValue should be set to 1. 

        // The bools StopOnFadeOut and ReleaseOnFadeOut can be marked as true to stop or release the event respectively after fading out. 
        // Ideally these should not be true when ramping any other parameters other than volume. May not work as expected.


        /// <summary>
        /// Sets the event volume parameter to 0.
        /// Plays the event.
        /// Ramps parameter from 0 to 1.
        /// No need to call Play() separately when fading in.
        /// </summary>
        [ContextMenu("FadeIn")]
        public void FadeIn()
        {
            SetLocalParameter(_startValue);
            Play();
            RampUpLocalParameter();
        }

        /// <summary>
        /// Sets the event volume parameter to 0.
        /// Plays the event.
        /// Ramps parameter from 0 to 1.
        /// Fade in for all events of the same type.
        /// No need to call Play() separately when fading in.
        /// </summary>
        [ContextMenu("FadeInAllOfType")]
        public void FadeInAllOfType()
        {
            SetLocalParameterAllOfType(_startValue);
            PlayAllOfType();
            RampUpLocalParameterAllOfType();
        }

        /// <summary>
        /// Ramps parameter from 0 to 1.
        /// </summary>
        [ContextMenu("FadeOut")]
        public void FadeOut()
        {
            RampDownLocalParameter();
        }

        /// <summary>
        /// Ramps parameter from 0 to 1.
        /// Fade out for all events of the same type.
        /// </summary>
        [ContextMenu("FadeOutAllOfType")]
        public void FadeOutAllOfType()
        {
            RampDownLocalParameterAllOfType();
        }

        #endregion

        #region Local Parameters

        /// <summary>
        /// Sets the specified local parameter for the current event.
        /// </summary>
        /// <param name="value"></param>
        public void SetLocalParameter(float value)
        {
            FMODManager.Instance.EventsManager.SetLocalParameterByName(EventGUID, gameObject, _parameterName, value);
        }

        /// <summary>
        /// Sets the specified local parameter for all events of the same type.
        /// </summary>
        /// <param name="value"></param>
        public void SetLocalParameterAllOfType(float value)
        {
            FMODManager.Instance.EventsManager.SetLocalParameterAllOfTypeByName(EventGUID, _parameterName, value);
        }

        private void RampLocalParameter(float startValue, float endValue)
        {
            _onTweenUpdate.AddListener((() =>
            {
                FMODManager.Instance.EventsManager.SetLocalParameterByName(EventGUID, gameObject, _parameterName, _parameterValue);
            }));
            TweenParameter(startValue, endValue);
        }

        private void RampLocalParameterAllOfType(float startValue, float endValue)
        {
            _onTweenUpdate.AddListener((() =>
            {
                FMODManager.Instance.EventsManager.SetLocalParameterAllOfTypeByName(EventGUID, _parameterName, _parameterValue);
            }));
            TweenParameter(startValue, endValue);
        }

        /// <summary>
        /// Ramps up local parameter from _startValue to _endValue.
        /// </summary>
        [ContextMenu("RampUpLocalParameter")]
        public void RampUpLocalParameter()
        {
            if (StopOnFadeOut) _onTweenComplete.RemoveListener(Stop);
            if (ReleaseOnFadeOut) _onTweenComplete.RemoveListener(Release);
            RampLocalParameter(_startValue, _endValue);
        }

        /// <summary>
        /// Ramps up local parameter from _startValue to _endValue for all events of the same type.
        /// </summary>
        [ContextMenu("RampUpLocalParameterAllOfType")]
        public void RampUpLocalParameterAllOfType()
        {
            if (StopOnFadeOut) _onTweenComplete.RemoveListener(StopAllOfType);
            if (ReleaseOnFadeOut) _onTweenComplete.RemoveListener(ReleaseAllEmitters);
            RampLocalParameterAllOfType(_startValue, _endValue);
        }

        /// <summary>
        /// Ramps down local parameter from _endValue to _startValue.
        /// </summary>
        [ContextMenu("RampDownLocalParameter")]
        public void RampDownLocalParameter()
        {
            if (StopOnFadeOut) _onTweenComplete.AddListener(Stop);
            if (ReleaseOnFadeOut) _onTweenComplete.AddListener(Release);
            RampLocalParameter(_endValue, _startValue);
        }

        /// <summary>
        /// Ramps down local parameter from _endValue to _startValue for all events of the same type.
        /// </summary>
        [ContextMenu("RampDownLocalParameterAllOfType")]
        public void RampDownLocalParameterAllOfType()
        {
            if (StopOnFadeOut) _onTweenComplete.AddListener(StopAllOfType);
            if (ReleaseOnFadeOut) _onTweenComplete.AddListener(ReleaseTargetEmitters);
            RampLocalParameterAllOfType(_endValue, _startValue);
        }

        #endregion

        #region Global Parameters

        /// <summary>
        /// Sets the specified global parameter
        /// </summary>
        /// <param name="value"></param>
        public void SetGlobalParameter(float value)
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

        /// <summary>
        /// Ramps up global parameter from _startValue to _endValue.
        /// </summary>
        [ContextMenu("RampUpGlobalParameter")]
        public void RampUpGlobalParameter()
        {
            if (StopOnFadeOut) _onTweenComplete.RemoveListener(Stop);
            if (ReleaseOnFadeOut) _onTweenComplete.RemoveListener(Release);
            RampGlobalParameter(_startValue, _endValue);
        }

        /// <summary>
        /// Ramps down global parameter from _startValue to _endValue.
        /// </summary>
        [ContextMenu("RampDownGlobalParameter")]
        public void RampDownGlobalParameter()
        {
            if (StopOnFadeOut) _onTweenComplete.AddListener(Stop);
            if (ReleaseOnFadeOut) _onTweenComplete.AddListener(Release);
            RampGlobalParameter(_endValue, _startValue);
        }

        #endregion
    }
}
