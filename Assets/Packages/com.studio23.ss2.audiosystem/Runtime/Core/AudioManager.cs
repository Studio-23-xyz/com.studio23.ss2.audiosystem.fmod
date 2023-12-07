using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;
using Studio23.SS2.AudioSystem.Data;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public List<FMODEmitterData> EmitterDataList;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    public void CreateEmitter(string eventName, GameObject referenceGameObject, StudioEventEmitter emitter = null, STOP_MODE stopModeType = STOP_MODE.ALLOWFADEOUT)
    {
        var fetchData = EventEmitterExists(eventName, referenceGameObject);
        if (fetchData != null) return;
        var newEmitter = new FMODEmitterData(eventName, referenceGameObject, emitter, stopModeType);
        EmitterDataList.Add(newEmitter);
    }

    public void Play(string eventName, GameObject referenceGameObject)
    {
        var fetchData = EventEmitterExists(eventName, referenceGameObject);
        if (fetchData == null) return;
        fetchData.Play();
    }

    public void Pause(string eventName, GameObject referenceGameObject)
    {
        var fetchData = EventEmitterExists(eventName, referenceGameObject);
        if (fetchData == null) return;
        fetchData.Pause();
    }

    public void UnPause(string eventName, GameObject referenceGameObject)
    {
        var fetchData = EventEmitterExists(eventName, referenceGameObject);
        if (fetchData == null) return;
        fetchData.UnPause();
    }

    public void TogglePauseAll(bool isGamePaused)
    {
        foreach (var emitter in EmitterDataList)
        {
            emitter.TogglePause(isGamePaused);
        }
    }

    public void Release(string eventName, GameObject referenceGameObject)
    {
        var fetchData = EventEmitterExists(eventName, referenceGameObject);
        if (fetchData == null) return;
        fetchData.Release();
        EmitterDataList.Remove(fetchData);
    }

    private FMODEmitterData EventEmitterExists(string eventName, GameObject referenceGameObject)
    {
        return EmitterDataList.FirstOrDefault(x =>
            x.EventName.Equals(eventName) && x.ReferenceGameObject == referenceGameObject);
    }
}
