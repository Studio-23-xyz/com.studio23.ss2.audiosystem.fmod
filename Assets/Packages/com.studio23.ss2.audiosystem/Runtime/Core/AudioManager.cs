using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Studio23.SS2.AudioSystem.Data;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public List<FMODInstanceData> InstanceDataList;
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

    public void CreateInstance(string eventName, GameObject referenceGameobject)
    {
        var newInstance = new FMODInstanceData(eventName, referenceGameobject);
        InstanceDataList.Add(newInstance);
    }

    public void Play(string eventName, GameObject referenceGameobject)
    {
        var temp = InstanceDataList.FirstOrDefault(x =>
            x.EventName.Equals(eventName) && x.ReferenceGameObject == referenceGameobject);
        if (temp != null)
        {
            temp.Play();
        }
    }

    public void CreateEmitter(string eventName, GameObject referenceGameobject, StudioEventEmitter emitter)
    {
        var newEmitter = new FMODEmitterData(eventName, referenceGameobject, emitter);
        EmitterDataList.Add(newEmitter);
    }

    public void Play(string eventName, GameObject referenceGameobject, StudioEventEmitter emitter)
    {
        var temp = EmitterDataList.FirstOrDefault(x =>
            x.EventName.Equals(eventName) && x.ReferenceGameObject == referenceGameobject && x.Emitter == emitter);
        if (temp != null)
        {
            temp.Play();
        }
    }

}
