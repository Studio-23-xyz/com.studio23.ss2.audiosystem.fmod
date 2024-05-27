using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using Studio23.SS2.AudioSystem.fmod.Data;
using Studio23.SS2.AudioSystem.fmod.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

[assembly: InternalsVisibleTo("com.studio23.ss2.audiosystem.fmod.playmode.tests")]
namespace Studio23.SS2.AudioSystem.fmod.Core
{
    public class EventsManager
    {
        internal Dictionary<(string, string, int), FMODEmitterData> _emitterDataList;

        internal void Initialize()
        {
            _emitterDataList = new Dictionary<(string, string, int), FMODEmitterData>();
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
        /// Creates a Custom FMOD Studio Event Emitter for an Event on a GameObject.
        /// If the GameObject already has an Emitter attached, then pass the Emitter to initialize it.
        /// By default it will create an Emitter and the Event Instance's STOP_MODE is set to ALLOWFADEOUT.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        /// <param name="emitter"></param>
        /// <param name="stopModeType"></param>
        public FMODEmitterData CreateEmitter(FMODEventData eventData, GameObject referenceGameObject, CustomStudioEventEmitter emitter = null, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData != null) return fetchData;
            var newEmitter = new FMODEmitterData(eventData, referenceGameObject, emitter, stopModeType);
            _emitterDataList.Add(newEmitter.GetKey(), newEmitter);
            FMODCallBackHandler.InitializeCallBack(newEmitter);
            return newEmitter;
        }

        /// <summary>
        /// Loads the sample data of an event.
        /// It may be beneficial to load the sample data of an event that is frequently used,
        /// instead of loading/unloading every time the event is called.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        public void LoadEventSampleData(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            fetchData.LoadSampleData();
        }

        /// <summary>
        /// Use to play FMOD Programmer Sounds. Localized audio tables for dialogues can be played.
        /// Any external audio not within FMOD can be played.
        /// If the GameObject already has an Emitter attached, then pass the Emitter to initialize it.
        /// By default it will create an Emitter and the Event Instance's STOP_MODE is set to ALLOWFADEOUT.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        /// <param name="emitter"></param>
        /// <param name="stopModeType"></param>
        public async void PlayProgrammerSound(string key, FMODEventData eventData, GameObject referenceGameObject, CustomStudioEventEmitter emitter = null, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData != null)
            {
                await FMODProgrammerSoundCallBackHandler.InitializeProgrammerCallback(fetchData, key);
                return;
            }
            var newEmitter = new FMODEmitterData(eventData, referenceGameObject, emitter, stopModeType);
            _emitterDataList.Add(newEmitter.GetKey(), newEmitter);
            await FMODProgrammerSoundCallBackHandler.InitializeProgrammerCallback(newEmitter, key, true);
        }

        /// <summary>
        /// Plays the Emitter.
        /// If the GameObject already has an Emitter attached, then pass the Emitter to initialize it.
        /// By default it will create an Emitter and the Event Instance's STOP_MODE is set to ALLOWFADEOUT.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        public async void Play(FMODEventData eventData, GameObject referenceGameObject, CustomStudioEventEmitter emitter = null, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null)
            {
                fetchData = CreateEmitter(eventData, referenceGameObject, emitter, stopModeType);
            }
            if (fetchData.EventState == FMODEventState.Playing) await fetchData.StopAsync(STOP_MODE.IMMEDIATE);
            fetchData.Play();
        }

        /// <summary>
        /// Pause the Emitter.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        public void Pause(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            fetchData.Pause();
        }

        /// <summary>
        /// UnPause the Emitter
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        public void UnPause(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            fetchData.UnPause();
        }

        /// <summary>
        /// Pause all Emitters.
        /// </summary>
        /// <param name="isGamePaused"></param>
        public void TogglePauseAll(bool isGamePaused)
        {
            foreach (var emitter in _emitterDataList)
            {
                emitter.Value.TogglePause(isGamePaused);
            }
        }

        /// <summary>
        /// Stops the Emitter.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        /// <param name="stopMode"></param>
        /// <returns></returns>
        public async UniTask Stop(FMODEventData eventData, GameObject referenceGameObject, STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            await fetchData.StopAsync(stopMode);
        }

        /// <summary>
        /// Stops all Emitters of the same type.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="stopMode"></param>
        /// <returns></returns>
        public async UniTask StopAllOfType(FMODEventData eventData, STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)
        {
            var fetchData = EventEmitterExists(eventData);
            if (fetchData == null) return;
            List<UniTask> stopTasks = new List<UniTask>();
            foreach (var emitter in fetchData)
            {
                stopTasks.Add(Stop(emitter.GetEventData(), emitter.ReferencedGameObject));
            }
            await UniTask.WhenAll(stopTasks);
        }

        /// <summary>
        /// Stops all currently playing Emitters.
        /// </summary>
        /// <param name="stopMode"></param>
        /// <returns></returns>
        public async UniTask StopAll(STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)
        {
            List<UniTask> stopTasks = new List<UniTask>();
            foreach (var emitter in _emitterDataList)
            {
                stopTasks.Add(Stop(emitter.Value.GetEventData(), emitter.Value.ReferencedGameObject));
            }
            await UniTask.WhenAll(stopTasks);
        }

        /// <summary>
        /// Release the Event Instance and destroys the Emitter.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        /// <returns></returns>
        public async UniTask Release(FMODEventData eventData, GameObject referenceGameObject)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            await fetchData.ReleaseAsync();
            _emitterDataList.Remove(fetchData.GetKey());
        }

        /// <summary>
        /// Releases and destroys all Emitters of the same type.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        /// <returns></returns>
        public async UniTask ReleaseAllOfType(FMODEventData eventData)
        {
            var fetchData = EventEmitterExists(eventData);
            if (fetchData == null) return;
            List<UniTask> releaseTasks = new List<UniTask>();
            for (int i = fetchData.Count - 1; i >= 0; i--)
            {
                var value = fetchData[i];
                releaseTasks.Add(Release(value.GetEventData(), value.ReferencedGameObject));
            }
            await UniTask.WhenAll(releaseTasks);
        }

        /// <summary>
        /// Releases all existing Emitters.
        /// </summary>
        /// <returns></returns>
        public async UniTask ReleaseAll()
        {
            List<UniTask> releaseTasks = new List<UniTask>();
            for (int i = _emitterDataList.Count - 1; i >= 0; i--)
            {
                var value = _emitterDataList.ElementAt(i).Value;
                releaseTasks.Add(Release(value.GetEventData(), value.ReferencedGameObject));
            }
            await UniTask.WhenAll(releaseTasks);
        }

        /// <summary>
        /// Sets a Local parameter value by name.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public void SetLocalParameter(FMODEventData eventData, GameObject referenceGameObject, string parameterName,
            float parameterValue)
        {
            var fetchData = EventEmitterExists(eventData, referenceGameObject);
            if (fetchData == null) return;
            fetchData.SetParameter(parameterName, parameterValue);
        }

        /// <summary>
        /// Sets a Global parameter value by name.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        public void SetGlobalParameter(string parameterName, float parameterValue)
        {
            RuntimeManager.StudioSystem.setParameterByName(parameterName, parameterValue);
        }

        /// <summary>
        /// Returns an Emitter if it has exists.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="referenceGameObject"></param>
        /// <returns></returns>
        public FMODEmitterData EventEmitterExists(FMODEventData eventData, GameObject referenceGameObject)
        {
            var key = (eventData.EventName, eventData.EventGUID, referenceGameObject.GetInstanceID());
            _emitterDataList.TryGetValue(key, out var emitterData);
            return emitterData;
        }

        /// <summary>
        /// Returns a list of the same type of Emitters if they exist.
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public List<FMODEmitterData> EventEmitterExists(FMODEventData eventData)
        {
            List<FMODEmitterData> emitterDatas = new List<FMODEmitterData>();
            var key = (eventData.EventName, eventData.EventGUID);
            foreach (var emitter in _emitterDataList)
            {
                var data = emitter.Key;
                if (data.Item1.Equals(key.EventName) && data.Item2.Equals(key.EventGUID))
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

                var foundMatchData = _emitterDataList.Where(k => k.Key.Item2.Equals(id.ToString())).ToList();

                for (int i = foundMatchData.Count - 1; i >= 0; i--)
                {
                    releaseTasks.Add(foundMatchData[i].Value.ReleaseAsync());
                    _emitterDataList.Remove(foundMatchData[i].Key);
                }

                await UniTask.WhenAll(releaseTasks);
            }
        }
    }
}
