using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using Studio23.SS2.AudioSystem.fmod.Data;
using Studio23.SS2.AudioSystem.fmod.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("com.studio23.ss2.audiosystem.fmod.playmode.tests")]
namespace Studio23.SS2.AudioSystem.fmod.Core
{
    public class EventsManager
    {
        internal Dictionary<(string, int), FMODEmitterData> _emitterDataList;

        public delegate void EmitterEvent();
        public EmitterEvent OnPauseAllOfType;
        public EmitterEvent OnUnPauseAllOfType;
        public EmitterEvent OnPauseAll;
        public EmitterEvent OnUnPauseAll;
        public EmitterEvent OnStopAllOfType;
        public EmitterEvent OnStopAll;
        public EmitterEvent OnReleaseAllOfType;
        public EmitterEvent OnReleaseAll;

        internal void Initialize()
        {
            _emitterDataList = new Dictionary<(string, int), FMODEmitterData>();
        }

        //public FMODEmitterData CreateEventData(CustomStudioEventEmitter emitter)
        //{
        //    var eventData = new FMODEventData(emitter.EventReference.Path, emitter.EventReference.Guid.ToString());
        //    return CreateEmitter(eventData, emitter.gameObject, emitter);
        //}

        //public FMODEmitterData CreateEventData(EventReference eventReference, GameObject referenceGameObject)
        //{
        //    var eventData = new FMODEventData(eventReference.Path, eventReference.Guid.ToString());
        //    return CreateEmitter(eventData, referenceGameObject);
        //}

        //public FMODEmitterData CreateEventData(EventReference eventReference, CustomStudioEventEmitter emitter)
        //{
        //    var eventData = new FMODEventData(eventReference.Path, eventReference.Guid.ToString());
        //    return CreateEmitter(eventData, emitter.gameObject, emitter);
        //}

        /// <summary>
        /// Creates an FMOD Studio Event Emitter for an Event on a GameObject.
        /// If the GameObject already has an Emitter attached, then pass the Emitter to initialize it.
        /// An Emitter can only be initialized once. So multiple Events will require multiple Emitters.
        /// By default it will create an Emitter and its AllowFadeout is set to true.
        /// Returns an FMODEmitterData.
        /// </summary>
        /// <param name="eventGUID"></param>
        /// <param name="referenceGameObject"></param>
        /// <param name="emitter"></param>
        /// <param name="allowFadeout"></param>
        public FMODEmitterData CreateEmitter(string eventGUID, GameObject referenceGameObject, CustomStudioEventEmitter emitter = null, bool allowFadeout = true)
        {
            var fetchData = EventEmitterExists(eventGUID, referenceGameObject);
            if (fetchData != null) return fetchData;
            var newEmitter = new FMODEmitterData(eventGUID, referenceGameObject, emitter, allowFadeout);
            _emitterDataList.Add(newEmitter.GetKey(), newEmitter);
            FMODCallBackHandler.InitializeCallBack(newEmitter);
            return newEmitter;
        }

        /// <summary>
        /// Plays the Emitter.
        /// If the GameObject already has an Emitter attached, then pass the Emitter to initialize it.
        /// An Emitter can only be initialized once. So multiple Events will require multiple Emitters.
        /// By default it will create an Emitter and its AllowFadeout is set to true.
        /// </summary>
        /// <param name="eventGUID"></param>
        /// <param name="referenceGameObject"></param>
        /// <param name="emitter"></param>
        /// <param name="allowFadeout"></param>
        public void Play(string eventGUID, GameObject referenceGameObject, CustomStudioEventEmitter emitter = null, bool allowFadeout = true)
        {
            var fetchData = EventEmitterExists(eventGUID, referenceGameObject);
            if (fetchData == null)
            {
                fetchData = CreateEmitter(eventGUID, referenceGameObject, emitter, allowFadeout);
            }
            //if (fetchData.EventState == FMODEventState.Playing) await fetchData.StopAsync(false);
            fetchData.Play();
        }

        /// <summary>
        /// Plays all existing emitters of the same type of event.
        /// This does not create any emitters on its own.
        /// </summary>
        /// <param name="eventGUID"></param>
        public void PlayAllOfType(string eventGUID)
        {
            var fetchData = EventEmitterExists(eventGUID);
            if (fetchData == null) return;
            foreach (var emitter in fetchData)
            {
                //if (emitter.EventState == FMODEventState.Playing) await emitter.StopAsync(false);
                emitter.Play();
            }
        }

        /// <summary>
        /// Use to play FMOD Programmer Sounds.
        /// Localized audio tables can be played, which is useful for playing localized dialogues.
        /// Any external audio not within FMOD can also be played.
        /// If the GameObject already has an Emitter attached, then pass the Emitter to initialize it.
        /// An Emitter can only be initialized once. So multiple Events will require multiple Emitters.
        /// By default it will create an Emitter and its AllowFadeout is set to true.
        /// Do not call Stop when trying to stop Programmer sounds. Call Release instead.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="eventGUID"></param>
        /// <param name="referenceGameObject"></param>
        /// <param name="emitter"></param>
        /// <param name="allowFadeout"></param>
        public async void PlayProgrammerSound(string key, string eventGUID, GameObject referenceGameObject, CustomStudioEventEmitter emitter = null, bool allowFadeout = true)
        {
            var fetchData = EventEmitterExists(eventGUID, referenceGameObject);
            if (fetchData != null)
            {
                await FMODProgrammerSoundCallBackHandler.InitializeProgrammerCallback(fetchData, key);
                return;
            }
            var newEmitter = new FMODEmitterData(eventGUID, referenceGameObject, emitter, allowFadeout);
            _emitterDataList.Add(newEmitter.GetKey(), newEmitter);
            await FMODProgrammerSoundCallBackHandler.InitializeProgrammerCallback(newEmitter, key, true);
        }

        /// <summary>
        /// Loads the sample data of an event.
        /// It may be beneficial to load the sample data of an event that is frequently used,
        /// instead of loading/unloading every time the event is called.
        /// </summary>
        /// <param name="eventGUID"></param>
        /// <param name="referenceGameObject"></param>
        public void LoadEventSampleData(string eventGUID, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventGUID, referenceGameObject);
            if (fetchData == null) return;
            fetchData.LoadSampleData();
        }

        /// <summary>
        /// Pauses the Emitter.
        /// </summary>
        /// <param name="eventGUID"></param>
        /// <param name="referenceGameObject"></param>
        public void Pause(string eventGUID, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventGUID, referenceGameObject);
            if (fetchData == null) return;
            fetchData.Pause();
        }

        /// <summary>
        /// UnPauses the Emitter.
        /// </summary>
        /// <param name="eventGUID"></param>
        /// <param name="referenceGameObject"></param>
        public void Unpause(string eventGUID, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventGUID, referenceGameObject);
            if (fetchData == null) return;
            fetchData.Unpause();
        }

        /// <summary>
        /// Pauses all Emitters of the same type.
        /// </summary>
        /// <param name="eventGUID"></param>
        public void PauseAllOfType(string eventGUID)
        {
            var fetchData = EventEmitterExists(eventGUID);
            if (fetchData == null) return;
            foreach (var emitter in fetchData)
            {
                emitter.Pause();
            }
            OnPauseAllOfType?.Invoke();
        }

        /// <summary>
        /// UnPauses all Emitters of the same type.
        /// </summary>
        /// <param name="eventGUID"></param>
        public void UnpauseAllOfType(string eventGUID)
        {
            var fetchData = EventEmitterExists(eventGUID);
            if (fetchData == null) return;
            foreach (var emitter in fetchData)
            {
                emitter.Unpause();
            }
            OnUnPauseAllOfType?.Invoke();
        }

        /// <summary>
        /// Pause/UnPauses all Emitters.
        /// </summary>
        /// <param name="isGamePaused"></param>
        public void TogglePause(bool isGamePaused)
        {
            foreach (var emitter in _emitterDataList)
            {
                emitter.Value.TogglePause(isGamePaused);
            }

            if (isGamePaused)
            {
                OnPauseAll?.Invoke();
            }
            else
            {
                OnUnPauseAll?.Invoke();
            }
        }

        /// <summary>
        /// Stops the Emitter.
        /// </summary>
        /// <param name="eventGUID"></param>
        /// <param name="referenceGameObject"></param>
        /// <param name="allowFadeOut"></param>
        /// <returns></returns>
        public void Stop(string eventGUID, GameObject referenceGameObject, bool allowFadeOut = true)
        {
            var fetchData = EventEmitterExists(eventGUID, referenceGameObject);
            if (fetchData == null) return;
            fetchData.Stop(allowFadeOut);
        }

        /// <summary>
        /// Stops all Emitters of the same type.
        /// </summary>
        /// <param name="eventGUID"></param>
        /// <returns></returns>
        public void StopAllOfType(string eventGUID, bool allowFadeOut = true)
        {
            var fetchData = EventEmitterExists(eventGUID);
            if (fetchData == null) return;
            List<UniTask> stopTasks = new List<UniTask>();
            foreach (var emitter in fetchData)
            {
                emitter.Stop(allowFadeOut);
            }
            //await UniTask.WhenAll(stopTasks);
            OnStopAllOfType?.Invoke();
        }

        /// <summary>
        /// Stops all currently playing Emitters.
        /// </summary>
        /// <param name="allowFadeOut"></param>
        /// <returns></returns>
        public void StopAll(bool allowFadeOut = true)
        {
            List<UniTask> stopTasks = new List<UniTask>();
            foreach (var emitter in _emitterDataList)
            {
                emitter.Value.Stop(allowFadeOut);
            }
            //await UniTask.WhenAll(stopTasks);
            OnStopAll?.Invoke();
        }

        /// <summary>
        /// Release the Event Instance and destroys the Emitter.
        /// </summary>
        /// <param name="eventGUID"></param>
        /// <param name="referenceGameObject"></param>
        /// <returns></returns>
        public void Release(string eventGUID, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventGUID, referenceGameObject);
            if (fetchData == null) return;
            fetchData.Release();
            _emitterDataList.Remove(fetchData.GetKey());
        }

        /// <summary>
        /// Releases and destroys all Emitters of the same type.
        /// </summary>
        /// <param name="eventGUID"></param>
        /// <returns></returns>
        public void ReleaseAllOfType(string eventGUID)
        {
            var fetchData = EventEmitterExists(eventGUID);
            if (fetchData == null) return;
            var foundMatchData = _emitterDataList.Where(k => k.Key.Item1.Equals(eventGUID)).ToList();
            List<UniTask> releaseTasks = new List<UniTask>();
            for (int i = fetchData.Count - 1; i >= 0; i--)
            {
                var value = fetchData[i];
                value.Release();
                _emitterDataList.Remove(foundMatchData[i].Key);
            }
            //await UniTask.WhenAll(releaseTasks);
            OnReleaseAllOfType?.Invoke();
        }

        /// <summary>
        /// Releases all existing Emitters.
        /// </summary>
        /// <returns></returns>
        public void ReleaseAll()
        {
            List<UniTask> releaseTasks = new List<UniTask>();
            for (int i = _emitterDataList.Count - 1; i >= 0; i--)
            {
                var value = _emitterDataList.ElementAt(i).Value;
                value.Release();
            }
            //await UniTask.WhenAll(releaseTasks);
            _emitterDataList.Clear();
            OnReleaseAll?.Invoke();
        }

        /// <summary>
        /// Sets a Local parameter value by name.
        /// </summary>
        /// <param name="eventGUID"></param>
        /// <param name="referenceGameObject"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public void SetLocalParameterByName(string eventGUID, GameObject referenceGameObject, string parameterName, float parameterValue)
        {
            var fetchData = EventEmitterExists(eventGUID, referenceGameObject);
            if (fetchData == null) return;
            fetchData.SetParameterByName(parameterName, parameterValue);
        }

        /// <summary>
        /// Sets a Local Parameter value for all active instances of that event.
        /// </summary>
        /// <param name="eventGUID"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public void SetLocalParameterAllOfTypeByName(string eventGUID, string parameterName, float parameterValue)
        {
            var fetchData = EventEmitterExists(eventGUID);
            if (fetchData == null) return;
            foreach (var emitter in fetchData)
            {
                SetLocalParameterByName(eventGUID, emitter.GetReferencedGameObject(), parameterName, parameterValue);
            }
        }

        /// <summary>
        /// Gets a Local parameter value by name.
        /// </summary>
        /// <param name="eventGUID"></param>
        /// <param name="referenceGameObject"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public float GetLocalParameterValueByName(string eventGUID, GameObject referenceGameObject, string parameterName)
        {
            var fetchData = EventEmitterExists(eventGUID, referenceGameObject);
            if (fetchData == null) return new float();
            return fetchData.GetParameterValueByName(parameterName);
        }

        /// <summary>
        /// Gets a Local parameter final value by name.
        /// </summary>
        /// <param name="eventGUID"></param>
        /// <param name="referenceGameObject"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public float GetLocalParameterFinalValueByName(string eventGUID, GameObject referenceGameObject, string parameterName)
        {
            var fetchData = EventEmitterExists(eventGUID, referenceGameObject);
            if (fetchData == null) return new float();
            return fetchData.GetParameterFinalValueByName(parameterName);
        }

        /// <summary>
        /// Sets a Global parameter value by name.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public void SetGlobalParameterByName(string parameterName, float parameterValue)
        {
            RuntimeManager.StudioSystem.setParameterByName(parameterName, parameterValue);
        }

        /// <summary>
        /// Gets a Global parameter value by name.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public float GetGlobalParameterValueByName(string parameterName)
        {
            RuntimeManager.StudioSystem.getParameterByName(parameterName, out float value);
            return value;
        }

        /// <summary>
        /// Gets a Global parameter final value by name.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public float GetGlobalParameterFinalValueByName(string parameterName)
        {
            RuntimeManager.StudioSystem.getParameterByName(parameterName, out float value, out float finalValue);
            return finalValue;
        }

        /// <summary>
        /// Returns an Emitter if it exists.
        /// </summary>
        /// <param name="eventGUID"></param>
        /// <param name="referenceGameObject"></param>
        /// <returns></returns>
        public FMODEmitterData EventEmitterExists(string eventGUID, GameObject referenceGameObject)
        {
            if (referenceGameObject == null)
            {
                Debug.LogError($"Referenced GameObject not found");
                var fetchData = EventEmitterExists(eventGUID);
                if (fetchData == null) return null;
                foreach (var emitter in fetchData)
                {
                    Debug.Log($"Referenced Emitter: GUID {eventGUID}, GameObject {emitter.GetReferencedGameObjectName()}, Scene {emitter.GetReferencedGameObjectSceneName()}\n");
                }
                return null;
            }
//#if UNITY_EDITOR
            if (FMODManager.Instance.Debug)
            {
                Debug.Log($"Referenced Emitter: GUID {eventGUID}, GameObject {referenceGameObject.name}, Scene {referenceGameObject.scene.name}");
            }
//#endif
            var key = (eventGUID, referenceGameObject.GetInstanceID());
            _emitterDataList.TryGetValue(key, out var emitterData);
            return emitterData;
        }

        /// <summary>
        /// Returns a list of the same type of Emitters if they exist.
        /// </summary>
        /// <param name="eventGUID"></param>
        /// <returns></returns>
        public List<FMODEmitterData> EventEmitterExists(string eventGUID)
        {
            List<FMODEmitterData> emitterDatas = new List<FMODEmitterData>();

            foreach (var emitter in _emitterDataList)
            {
                var data = emitter.Key;
                if (data.Item1.Equals(eventGUID))
                {
                    _emitterDataList.TryGetValue(data, out var emitterData);
                    emitterDatas.Add(emitterData);
                }
            }
            return emitterDatas;
        }

        internal async UniTask ClearEmitter(Bank bank)
        {
            bank.getEventList(out EventDescription[] list);
            var data = list.ToList();
            foreach (var key in data)
            {
                key.getID(out var id);
                List<UniTask> releaseTasks = new List<UniTask>();

                var foundMatchData = _emitterDataList.Where(k => k.Key.Item1.Equals(id.ToString())).ToList();

                for (int i = foundMatchData.Count - 1; i >= 0; i--)
                {
                    foundMatchData[i].Value.Release();
                    _emitterDataList.Remove(foundMatchData[i].Key);
                }

                await UniTask.WhenAll(releaseTasks);
            }
        }
    }
}
