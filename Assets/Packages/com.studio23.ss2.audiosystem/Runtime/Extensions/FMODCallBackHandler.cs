using System;
using System.Runtime.InteropServices;
using AOT;
using FMOD;
using UnityEngine;
using FMOD.Studio;
using Studio23.SS2.AudioSystem.Data;
using Debug = UnityEngine.Debug;

namespace Studio23.SS2.AudioSystem.Extensions
{
    public static class FMODCallBackHandler
    {
        public static void InitializeCallback(FMODEmitterData eventData)
        {
            EVENT_CALLBACK EventCallback =
                new EVENT_CALLBACK(EventCallbackHandler);
            GCHandle EventGCHandle = GCHandle.Alloc(eventData);
            eventData.Emitter.EventInstance.setUserData(GCHandle.ToIntPtr(EventGCHandle));
            eventData.Emitter.EventInstance.setCallback(EventCallback);
        }

        [MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
        public static RESULT EventCallbackHandler(EVENT_CALLBACK_TYPE type, IntPtr instancePtr,
            IntPtr parameterPtr)
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
                GCHandle eventStateHandle = GCHandle.FromIntPtr(EventStateInfoPtr);
                FMODEmitterData eventStateInfo = (FMODEmitterData)eventStateHandle.Target;

                eventStateInfo.Emitter.EventDescription.getUserProperty("IsLooping", out USER_PROPERTY UserProperties);

                switch (type)
                {
                    case EVENT_CALLBACK_TYPE.SOUND_STOPPED:
                    {
                        bool isLooping = Convert.ToBoolean(UserProperties.intValue());
                        if (isLooping)
                            eventStateInfo.EventState = FMODEventState.Playing;
                        else
                            eventStateInfo.EventState = FMODEventState.Stopped;
                        break;
                    }
                    case EVENT_CALLBACK_TYPE.DESTROYED:
                    {
                        eventStateHandle.Free();
                        break;
                    }
                }
            }

            return RESULT.OK;
        }
    }
}
