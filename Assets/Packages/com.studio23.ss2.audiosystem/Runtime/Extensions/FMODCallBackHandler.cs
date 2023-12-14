using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;
using AOT;
using FMOD;
using FMOD.Studio;
using Studio23.SS2.AudioSystem.Data;

namespace Studio23.SS2.AudioSystem.Extensions
{
    public static class FMODCallBackHandler
    {
        public static void InitializeCallback(FMODEmitterData eventData)
        {
            EVENT_CALLBACK EventCallback = new EVENT_CALLBACK(EventCallbackHandler);
            GCHandle EventGCHandle = GCHandle.Alloc(eventData);
            eventData.Emitter.EventInstance.setUserData(GCHandle.ToIntPtr(EventGCHandle));
            eventData.Emitter.EventInstance.setCallback(EventCallback);
        }

        [MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
        public static RESULT EventCallbackHandler(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
        {
            EventInstance instance = new EventInstance(instancePtr);

            // Retrieve the user data
            IntPtr EventStateInfoPtr;
            RESULT result = instance.getUserData(out EventStateInfoPtr);

            if (result != RESULT.OK)
            {
                Debug.LogError("Event State Callback error: " + result);
            }
            else if (EventStateInfoPtr != IntPtr.Zero)
            {
                // Get the object to store the FMODEmitterData
                GCHandle eventInfoHandle = GCHandle.FromIntPtr(EventStateInfoPtr);
                FMODEmitterData eventInfo = (FMODEmitterData)eventInfoHandle.Target;

                eventInfo.Emitter.EventDescription.getUserProperty("IsLooping", out USER_PROPERTY UserProperties);

                switch (type)
                {
                    case FMOD.Studio.EVENT_CALLBACK_TYPE.SOUND_STOPPED:
                    {
                        IsLoopingCheck(UserProperties, eventInfo);
                        break;
                    }
                    case FMOD.Studio.EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                    {
                        IsLoopingCheck(UserProperties, eventInfo);

                        MODE soundMode = MODE.LOOP_NORMAL | MODE.CREATECOMPRESSEDSAMPLE | MODE.NONBLOCKING;
                        var parameter = (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(PROGRAMMER_SOUND_PROPERTIES));

                        if (eventInfo.key.Contains("."))
                        {
                            Sound dialogueSound;
                            var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(Application.streamingAssetsPath + "/" + eventInfo.key, soundMode, out dialogueSound);
                            if (soundResult == RESULT.OK)
                            {
                                parameter.sound = dialogueSound.handle;
                                parameter.subsoundIndex = -1;
                                Marshal.StructureToPtr(parameter, parameterPtr, false);
                            }
                        }
                        else
                        {
                            SOUND_INFO dialogueSoundInfo;
                            var keyResult = FMODUnity.RuntimeManager.StudioSystem.getSoundInfo(eventInfo.key, out dialogueSoundInfo);
                            if (keyResult != RESULT.OK)
                            {
                                break;
                            }
                            Sound dialogueSound;
                            var soundResult = FMODUnity.RuntimeManager.CoreSystem.createSound(dialogueSoundInfo.name_or_data, soundMode | dialogueSoundInfo.mode, ref dialogueSoundInfo.exinfo, out dialogueSound);
                            if (soundResult == RESULT.OK)
                            {
                                Debug.Log("Playing Dialogue");
                                parameter.sound = dialogueSound.handle;
                                parameter.subsoundIndex = dialogueSoundInfo.subsoundindex;
                                Marshal.StructureToPtr(parameter, parameterPtr, false);
                            }
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
                        eventInfoHandle.Free();
                        break;
                    }
                }
            }
            return RESULT.OK;
        }

        private static void IsLoopingCheck(USER_PROPERTY userProperties, FMODEmitterData eventData)
        {
            bool isLooping = Convert.ToBoolean(userProperties.intValue());
            if (isLooping)
                eventData.EventState = FMODEventState.Playing;
            else
                eventData.EventState = FMODEventState.Stopped;
        }
    }
}
