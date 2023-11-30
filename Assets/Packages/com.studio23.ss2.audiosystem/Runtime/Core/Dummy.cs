using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Studio23.SS2.AudioSystem.Data;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public StudioEventEmitter emitter;

    // Start is called before the first frame update
    void Start()
    {
        /*AudioManager.Instance.CreateInstance(FMODBank_Sample.Test, gameObject);
        AudioManager.Instance.Play(FMODBank_Sample.Test, gameObject);*/

        
    }

    [ContextMenu("Play")]
    public void Demo()
    {
        AudioManager.Instance.CreateEmitter(FMODBank_Sample.Test, gameObject, emitter);
        AudioManager.Instance.Play(FMODBank_Sample.Test, gameObject, emitter);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
