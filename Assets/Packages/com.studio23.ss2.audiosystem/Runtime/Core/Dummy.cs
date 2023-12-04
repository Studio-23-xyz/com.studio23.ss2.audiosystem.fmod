using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Studio23.SS2.AudioSystem.Data;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public StudioEventEmitter emitter;
    public int tempIndex;
    public float time;
    public bool startTime;
    public int maxCount;

    // Start is called before the first frame update
    void Start()
    {
        //AudioManager.Instance.CreateInstance(FMODBank_Sample.Test, gameObject);
        //AudioManager.Instance.Play(FMODBank_Sample.Test, gameObject);
        

    }

    [ContextMenu("Play")]
    public void Demo()
    {
        startTime = true;

        for (tempIndex = 0; tempIndex <= maxCount; tempIndex++)
        {
            var instance = RuntimeManager.CreateInstance(FMODBank_Sample.Test);
        }


        /*StudioEventEmitter emitter2 = new StudioEventEmitter
        {
            EventReference = EventReference.Find(FMODBank_Sample.Test)
        };
        emitter2.EventInstance.start();*/

        /*AudioManager.Instance.CreateEmitter(FMODBank_Sample.Test, gameObject, emitter);
        AudioManager.Instance.Play(FMODBank_Sample.Test, gameObject, emitter);*/
    }

    [ContextMenu("Play2")]
    public void Demo2()
    {
        startTime = true;
        for (tempIndex = 0; tempIndex <= maxCount; tempIndex++)
        {
            StudioEventEmitter emitter2 = new StudioEventEmitter
            {
                EventReference = EventReference.Find(FMODBank_Sample.Test)
                
            };
            emitter2.Play();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (startTime)
        {
            time += Time.deltaTime;
            if (tempIndex >= maxCount)
            {
                Debug.Log($"Time consumed: {time}");
                tempIndex = 0;
                startTime = false;
                time = 0.0f;
            }
        }
    }
}
