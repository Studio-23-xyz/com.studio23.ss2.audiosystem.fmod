using Studio23.SS2.AudioSystem.fmod;
using Studio23.SS2.AudioSystem.fmod.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FMODManager.Instance.EventsManager.Play(FMODBank_Sample.Test, gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
