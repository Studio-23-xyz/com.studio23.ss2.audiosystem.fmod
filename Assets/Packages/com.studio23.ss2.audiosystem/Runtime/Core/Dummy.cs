using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Studio23.SS2.AudioSystem.Data;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public bool isPaused;

    [ContextMenu("Play")]
    public void Play()
    {
        AudioManager.Instance.CreateEmitter(FMODBank_Sample.Test, gameObject);
        AudioManager.Instance.Play(FMODBank_Sample.Test, gameObject);

    }

    [ContextMenu("Play2")]
    public void Play2()
    {
        AudioManager.Instance.CreateEmitter(FMODBank_Sample2.Test2, gameObject);
        AudioManager.Instance.Play(FMODBank_Sample2.Test2, gameObject);
    }

    [ContextMenu("Release")]
    public void Release()
    {
        AudioManager.Instance.Release(FMODBank_Sample.Test, gameObject);
    }

    [ContextMenu("Pause")]
    public void Pause()
    {
        AudioManager.Instance.Pause(FMODBank_Sample.Test, gameObject);
    }

    [ContextMenu("UnPause")]
    public void UnPause()
    {
        AudioManager.Instance.UnPause(FMODBank_Sample.Test, gameObject);
    }

    [ContextMenu("Toggle")]
    public void Toggle()
    {
        isPaused = !isPaused;
        AudioManager.Instance.TogglePauseAll(isPaused);
        
    }
}
