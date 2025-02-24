using Cysharp.Threading.Tasks;
using FMODUnity;
using Studio23.SS2.AudioSystem.fmod.Core;
using Studio23.SS2.AudioSystem.fmod.Data;
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
        private FMODEmitterData _emitter;

        private float _parameterValue;
        [SerializeField] private string _parameterName;
        [SerializeField] private float _startValue;
        [SerializeField] private float _endValue;
        [SerializeField] private float _duration;

        [SerializeField] private bool randomizeParameters;
        [SerializeField] private bool roundToInt = false;
        [SerializeField] private Vector2 _startValueRange = new Vector2(0f, 1f);
        [SerializeField] private Vector2 _endValueRange = new Vector2(0f, 1f);
        [SerializeField] private Vector2 _durationRange = new Vector2(0f, 1f);

        public bool StopOnFadeOut;
        public bool ReleaseOnFadeOut;

        public UnityEvent OnEventPlayed;
        public UnityEvent OnEventSuspended;
        public UnityEvent OnEventUnsuspended;
        public UnityEvent OnEventPaused;
        public UnityEvent OnEventUnPaused;
        public UnityEvent OnEventStopped;
        public UnityEvent OnEventCompleted;
        private bool _isSubscribed;

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

        private void SubscribeToEvents()
        {
            if (_emitter == null && !_isSubscribed)
            {
                Debug.LogWarning("FMODEmitterData is null, cannot subscribe to events.");
                return;
            }

            _isSubscribed = true;

            _emitter.OnEventSuspended.AddListener(OnSuspended);
            _emitter.OnEventUnsuspended.AddListener(OnUnsuspended);
            _emitter.OnEventPaused.AddListener(OnPaused);
            _emitter.OnEventUnPaused.AddListener(OnUnpaused);
            _emitter.OnEventStopped.AddListener(OnStopped);
            _emitter.OnEventCompleted.AddListener(OnCompleted);
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

        private void ApplyRandomization()
        {
            if (randomizeParameters)
            {
                _startValue = GetRandomValue(_startValueRange, roundToInt);
                _endValue = GetRandomValue(_endValueRange, roundToInt);
                _duration = GetRandomValue(_durationRange, roundToInt);
            }
        }

        private float GetRandomValue(Vector2 range, bool round)
        {
            float value = Random.Range(range.x, range.y);
            return round ? Mathf.Round(value) : value;
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
            _emitter = FMODManager.Instance.EventsManager.CreateEmitter(EventGUID, _gameObject);
            _emitter.Play();
            SubscribeToEvents();
            OnPlayed();
        }

        /// <summary>
        /// Creates an Emitter, plays the specified Event and sets a parameter for the event.
        /// </summary>
        [ContextMenu("PlayAndSetParameter")]
        public void PlayAndSetParameter()
        {
            ApplyRandomization();
            Play();
            SetLocalParameter(_startValue);
        }

        /// <summary>
        /// Plays the specified Event only if it isn't already playing.
        /// </summary>
        [ContextMenu("PlayIfNotPlaying")]
        public void PlayIfNotPlaying()
        {
            if (_emitter == null || _emitter.GetEventState() != FMODEventState.Playing)
            {
                Play();
            }
        }

        /// <summary>
        /// Plays the specified Event only if it isn't already playing and sets a parameter for the event.
        /// </summary>
        [ContextMenu("PlayIfNotPlayingAndSetParameter")]
        public void PlayIfNotPlayingAndSetParameter()
        {
            if (_emitter == null || _emitter.GetEventState() != FMODEventState.Playing)
            {
                PlayAndSetParameter();
            }
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
            FMODManager.Instance.EventsManager.Stop(EventGUID, _gameObject);
        }

        /// <summary>
        /// Stops all existing emitters of the specified Event.
        /// </summary>
        [ContextMenu("StopAllOfType")]
        public void StopAllOfType()
        {
            FMODManager.Instance.EventsManager.StopAllOfType(EventGUID);
        }

        /// <summary>
        /// Stops all existing Emitters in the scene.
        /// </summary>
        [ContextMenu("StopAll")]
        public void StopAll()
        {
            FMODManager.Instance.EventsManager.StopAll();
        }

        #endregion

        #region Release

        [ContextMenu("Release")]
        public void Release()
        {
            FMODManager.Instance.EventsManager.Release(EventGUID, _gameObject);
        }

        public void ReleaseTargetEmitters(string tag)
        {
            var emitters = GameObject.FindGameObjectsWithTag(tag);
            foreach (var emitter in emitters)
            {
                FMODManager.Instance.EventsManager.Release(EventGUID, emitter);
            }
        }

        [ContextMenu("ReleaseAllOfType")]
        public void ReleaseTargetEmitters()
        {
            FMODManager.Instance.EventsManager.ReleaseAllOfType(EventGUID);
        }

        [ContextMenu("ReleaseAll")]
        public void ReleaseAllEmitters()
        {
            FMODManager.Instance.EventsManager.ReleaseAll();
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
            ApplyRandomization();
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
            ApplyRandomization();
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

        #region Events

        /// <summary>
        /// Invoked when the event is played.
        /// </summary>
        private void OnPlayed()
        {
            OnEventPlayed.Invoke();
        }

        /// <summary>
        /// Invoked when the event is suspended.
        /// </summary>
        private void OnSuspended()
        {
            OnEventSuspended.Invoke();
        }

        /// <summary>
        /// Invoked when the event is unsuspended.
        /// </summary>
        private void OnUnsuspended()
        {
            OnEventUnsuspended.Invoke();
        }

        /// <summary>
        /// Invoked when the event is paused.
        /// </summary>
        private void OnPaused()
        {
            OnEventPaused.Invoke();
        }

        /// <summary>
        /// Invoked when the event is unpaused.
        /// </summary>
        private void OnUnpaused()
        {
            OnEventUnPaused.Invoke();
        }

        /// <summary>
        /// Invoked when the event is stopped.
        /// </summary>
        private void OnStopped()
        {
            OnEventStopped.Invoke();
        }

        /// <summary>
        /// Invoked when the event is completed.
        /// </summary>
        private void OnCompleted()
        {
            OnEventCompleted.Invoke();
        }

        #endregion
    }
}
