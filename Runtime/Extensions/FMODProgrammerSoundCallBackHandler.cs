using AOT;
using Cysharp.Threading.Tasks;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Studio23.SS2.AudioSystem.fmod.Core;
using Studio23.SS2.AudioSystem.fmod.Data;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Studio23.SS2.AudioSystem.fmod.Extensions
{
    public static class FMODProgrammerSoundCallBackHandler
    {
        /// <summary>
        /// Initializes a CallBack for all Event Instances with programmer sounds.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="key"></param>
        /// <param name="assignCallBack"></param>
        public static async UniTask InitializeProgrammerCallback(FMODEmitterData eventData, string key, bool assignCallBack = false)
        {
            if (assignCallBack)
            {
                EVENT_CALLBACK eventCallback = new EVENT_CALLBACK(ProgrammerSoundCallbackHandler);
                eventData.Emitter.EventInstance.setCallback(eventCallback);
            }
            SoundData data = await LoadExternalSound(eventData, key);
            if (data == null) return;
            GCHandle eventGcHandle = GCHandle.Alloc(data);
            eventData.Emitter.EventInstance.setUserData(GCHandle.ToIntPtr(eventGcHandle));
            eventData.Play();
        }

        /// <summary>
        /// Loads the external audio file before it is played.
        /// </summary>
        /// <param name="eventData"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static async UniTask<SoundData> LoadExternalSound(FMODEmitterData eventData, string key)
        {
            MODE soundMode = MODE.LOOP_NORMAL | MODE.CREATECOMPRESSEDSAMPLE | MODE.NONBLOCKING;

            Sound sound;
            SOUND_INFO soundInfo = new SOUND_INFO() { subsoundindex = -1 };

            if (key.Contains("."))
            {
                var soundResult =
                    RuntimeManager.CoreSystem.createSound(Application.streamingAssetsPath + "/" + key,
                        soundMode, out sound);
                if (soundResult != RESULT.OK)
                {
                    Debug.LogWarning("Couldn't find external audio file with key: " + key);
                    return null;
                }
            }
            else
            {
                // Load Sound Path
                RESULT keyResult = RuntimeManager.StudioSystem.getSoundInfo(key, out soundInfo);
                if (keyResult != RESULT.OK)
                {
                    Debug.LogWarning("Couldn't find dialogue with key: " + key);
                    return null;
                }

                // Load Sound
                RESULT soundResult = RuntimeManager.CoreSystem.createSound(soundInfo.name_or_data,
                    soundMode | soundInfo.mode, ref soundInfo.exinfo, out sound);
                if (soundResult != RESULT.OK)
                {
                    Debug.LogWarning("Couldn't load dialogue sound: " + key);
                    return null;
                }
            }

            //Wait to Load
            int maxFrameWait = 120;
            OPENSTATE openstate = OPENSTATE.BUFFERING;
            while (openstate != OPENSTATE.READY)
            {
                await UniTask.Yield();
                sound.getOpenState(out openstate, out uint percentbuffered, out bool starving, out bool diskbusy);
                if (--maxFrameWait <= 0)
                {
                    sound.release();
                    Debug.LogWarning("Failed to load sound " + key);
                    return null;
                }
            }

            return new SoundData(eventData, soundInfo, sound);
        }

        [MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
        private static RESULT ProgrammerSoundCallbackHandler(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
        {
            EventInstance instance = new EventInstance(instancePtr);

            // Retrieve the user data
            IntPtr EventDataPtr;
            RESULT result = instance.getUserData(out EventDataPtr);

            uint soundLength = 0;

            if (result != RESULT.OK)
            {
                Debug.LogError("Programmer Sound Callback error: " + result);
            }
            else if (EventDataPtr != IntPtr.Zero)
            {
                // Get the object to store the FMODEmitterData
                GCHandle eventDataHandle = GCHandle.FromIntPtr(EventDataPtr);
                SoundData soundData = (SoundData)eventDataHandle.Target;

                soundData.EmitterData.Emitter.EventDescription.getUserProperty("IsLooping", out USER_PROPERTY userProperties);
                soundData.EmitterData.CurrentCallbackType = type;

//#if UNITY_EDITOR
                string eventPath = "";
                if (FMODManager.Instance.Debug)
                {
                    RuntimeManager.StudioSystem.lookupPath(GUID.Parse(soundData.EmitterData.EventGUID), out eventPath);
                    Debug.Log($"{eventPath} Event Callback Type {type}");
                }
//#endif

                switch (type)
                {
                    case EVENT_CALLBACK_TYPE.STARTED:
                        {
                            soundData.EmitterData.EventState = FMODEventState.Playing;
                            break;
                        }
                    case EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                        {
                            FMODCallBackHandler.IsLoopingCheck(userProperties, soundData.EmitterData);
                            var parameter = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));

                            parameter.sound = soundData.Sound.handle;
                            parameter.subsoundIndex = soundData.SoundInfo.subsoundindex;
                            Marshal.StructureToPtr(parameter, parameterPtr, false);
                            break;
                        }
                    case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                        {
                            var parameter = (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_MARKER_PROPERTIES));
                            soundData.LastMarker = parameter.name;
                            soundData.Position = parameter.position;

//#if UNITY_EDITOR
                            if (FMODManager.Instance.Debug)
                            {
                                Debug.Log($"{eventPath}, Marker Name: {(string)soundData.LastMarker}, Marker Position: {soundData.Position}ms");
                            }
//#endif

                            break;
                        }
                    case EVENT_CALLBACK_TYPE.STOPPED:
                        {
                            soundData.EmitterData.EventState = FMODEventState.Stopped;
                            break;
                        }
                    case EVENT_CALLBACK_TYPE.SOUND_STOPPED:
                        {
                            FMODCallBackHandler.IsLoopingCheck(userProperties, soundData.EmitterData);

                            Sound sound = new Sound(parameterPtr);
                            sound.getName(out string name, 256);
                            sound.getLength(out soundLength, TIMEUNIT.MS);
                            sound.release();

//#if UNITY_EDITOR
                            if (FMODManager.Instance.Debug)
                            {
                                if (name != null) Debug.Log($"Programmer Sound Name: {name}, Length: {soundLength}");
                                Debug.Log($"{eventPath}, Timeline Position: {soundData.EmitterData.GetTimelinePosition()}ms, Length: {soundLength} ms");
                            }
//#endif

                            if (soundData.EmitterData.GetTimelinePosition() >= soundLength)
                            {
                                soundData.EmitterData.CompleteEvent();
                            }
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
            public SOUND_INFO SoundInfo;
            public Sound Sound;
            public FMOD.StringWrapper LastMarker;
            public int Position;

            public SoundData(FMODEmitterData emitterData, SOUND_INFO soundInfo, Sound sound)
            {
                EmitterData = emitterData;
                SoundInfo = soundInfo;
                Sound = sound;
                LastMarker = new StringWrapper();
                Position = 0;
            }
        }
    }
}
