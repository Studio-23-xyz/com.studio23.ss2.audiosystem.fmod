using Studio23.SS2.AudioSystem.Core;
using Studio23.SS2.AudioSystem.Data;
using UnityEngine;

public class Sample : MonoBehaviour
{
    public bool isPaused;

    [ContextMenu("Play")]
    public void Play()
    {
        AudioManager.Instance.CreateEmitter(FMODBank_Test.Test, gameObject);
        AudioManager.Instance.Play(FMODBank_Test.Test, gameObject);
    }

    [ContextMenu("Play2")]
    public void Play2()
    {
        AudioManager.Instance.CreateEmitter(FMODBank_SFX.UI_Cancel, gameObject);
        AudioManager.Instance.Play(FMODBank_SFX.UI_Cancel, gameObject);
    }

    [ContextMenu("Release")]
    public void Release()
    {
        AudioManager.Instance.Release(FMODBank_SFX.Test, gameObject);
    }

    [ContextMenu("Pause")]
    public void Pause()
    {
        AudioManager.Instance.Pause(FMODBank_SFX.UI_Cancel, gameObject);
    }

    [ContextMenu("UnPause")]
    public void UnPause()
    {
        AudioManager.Instance.UnPause(FMODBank_SFX.UI_Cancel, gameObject);
    }

    [ContextMenu("Toggle")]
    public void Toggle()
    {
        isPaused = !isPaused;
        AudioManager.Instance.TogglePauseAll(isPaused);
    }

    [ContextMenu("Load Bank")]
    public void LoadBank()
    {
        AudioManager.Instance.LoadBank(FMODBankList.Test);
    }

    [ContextMenu("Unload Bank")]
    public void UnloadBank()
    {
        AudioManager.Instance.UnloadBank(FMODBankList.Test);
    }

    [ContextMenu("Unload All Banks")]
    public void UnloadAllBanks()
    {
        AudioManager.Instance.UnloadAllBanks();
    }
}
