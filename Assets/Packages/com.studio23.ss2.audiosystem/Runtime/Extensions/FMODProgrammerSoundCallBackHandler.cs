using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;
using AOT;
using Cysharp.Threading.Tasks;
using FMOD;
using FMOD.Studio;
using Studio23.SS2.AudioSystem.Data;
using static Studio23.SS2.AudioSystem.Extensions.FMODCallBackHandler;

public static class FMODProgrammerSoundCallBackHandler
{
    public delegate void ProgrammerSoundEvent();
    public static ProgrammerSoundEvent OnDialogueStarted;
    public static ProgrammerSoundEvent OnDialogueComplete;

    public static async void InitializeDialogueCallback(FMODEmitterData eventData, string key, bool assignCallBack = false)
    {
        var data = await LoadExternalSound(eventData, key);
        if (assignCallBack)
        {
            EVENT_CALLBACK eventCallback = new EVENT_CALLBACK(ProgrammerSoundCallbackHandler);
            eventData.Emitter.EventInstance.setCallback(eventCallback);
        }
        GCHandle eventGcHandle = GCHandle.Alloc(data);
        eventData.Emitter.EventInstance.setUserData(GCHandle.ToIntPtr(eventGcHandle));
        eventData.Play();
    }

    private static async UniTask<SoundData> LoadExternalSound(FMODEmitterData eventData, string key)
    {
        MODE soundMode = MODE.LOOP_NORMAL | MODE.CREATECOMPRESSEDSAMPLE | MODE.NONBLOCKING;
        Sound sound;
        FMOD.Studio.SOUND_INFO soundInfo = new SOUND_INFO(){subsoundindex = -1};

        if (key.Contains("."))
        {
            var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(Application.streamingAssetsPath + "/" + key, soundMode, out sound);
            if (soundResult != FMOD.RESULT.OK)
            {
                Debug.LogError("Couldn't find external sound with key: " + key);
                await UniTask.Yield();
            }
        }
        else
        {
            // Load Sound Path
            FMOD.RESULT keyResult = FMODUnity.RuntimeManager.StudioSystem.getSoundInfo(key, out soundInfo);
            if (keyResult != FMOD.RESULT.OK)
            {
                Debug.LogError("Couldn't find dialogue with key: " + key);
                await UniTask.Yield();
            }

            // Load Sound
            FMOD.RESULT soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(soundInfo.name_or_data, soundMode | soundInfo.mode, ref soundInfo.exinfo, out sound);
            if (soundResult != FMOD.RESULT.OK)
            {
                Debug.LogError("Couldn't load dialogue sound: " + key);
                await UniTask.Yield();
            }
        }

        //Wait to Load
        int maxFrameWait = 120;
        FMOD.OPENSTATE openstate = FMOD.OPENSTATE.BUFFERING;
        while (openstate != FMOD.OPENSTATE.READY)
        {
            await UniTask.Yield();
            sound.getOpenState(out openstate, out uint percentbuffered, out bool starving, out bool diskbusy);
            if (--maxFrameWait <= 0)
            {
                sound.release();
                Debug.LogError("Failed to load sound " + key);
                await UniTask.Yield();
            }
        }
        return new SoundData(eventData, soundInfo, sound);
    }

    [MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    public static RESULT ProgrammerSoundCallbackHandler(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        EventInstance instance = new EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr EventDataPtr;
        RESULT result = instance.getUserData(out EventDataPtr);

        if (result != RESULT.OK)
        {
            Debug.LogError("Programmer Sound Callback error: " + result);
        }
        else if (EventDataPtr != IntPtr.Zero)
        {
            // Get the object to store the FMODEmitterData
            GCHandle eventDataHandle = GCHandle.FromIntPtr(EventDataPtr);
            SoundData eventData = (SoundData)eventDataHandle.Target;

            eventData.EmitterData.Emitter.EventDescription.getUserProperty("IsLooping", out USER_PROPERTY UserProperties);
            eventData.EmitterData.CurrentCallbackType = type;
            switch (type)
            {
                case EVENT_CALLBACK_TYPE.CREATED:
                {
                    OnDialogueStarted?.Invoke();
                    break;
                }
                case EVENT_CALLBACK_TYPE.STARTED:
                {
                    eventData.EmitterData.EventState = FMODEventState.Playing;
                    break;
                }
                case EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                {
                    IsLoopingCheck(UserProperties, eventData.EmitterData);
                    var parameter = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));

                    parameter.sound = eventData.Sound.handle;
                    parameter.subsoundIndex = eventData.SoundInfo.subsoundindex;
                    Marshal.StructureToPtr(parameter, parameterPtr, false);
                    break;
                }
                case EVENT_CALLBACK_TYPE.SOUND_STOPPED:
                {
                    IsLoopingCheck(UserProperties, eventData.EmitterData);
                    OnDialogueComplete?.Invoke();
                    break;
                }
                case EVENT_CALLBACK_TYPE.STOPPED:
                {
                    eventData.EmitterData.EventState = FMODEventState.Stopped;
                    break;
                }
                case EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                {
                    var parameter = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));
                    var sound = new Sound(parameter.sound);
                    sound.release();
                    break;
                }
                case EVENT_CALLBACK_TYPE.DESTROYED:
                {
                    eventDataHandle.Free();
                    break;
                }
            }
        }
        return RESULT.OK;
    }

    private class SoundData
    {
        public FMODEmitterData EmitterData;
        public FMOD.Studio.SOUND_INFO SoundInfo;
        public FMOD.Sound Sound;

        public SoundData(FMODEmitterData emitterData, FMOD.Studio.SOUND_INFO soundInfo, FMOD.Sound sound)
        {
            EmitterData = emitterData;
            SoundInfo = soundInfo;
            Sound = sound;
        }
    }
}
