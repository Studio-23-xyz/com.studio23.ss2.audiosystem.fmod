using AOT;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Studio23.SS2.AudioSystem.fmod.Core;
using Studio23.SS2.AudioSystem.fmod.Data;
using System;
using System.Runtime.InteropServices;
using Debug = UnityEngine.Debug;

namespace Studio23.SS2.AudioSystem.fmod.Extensions
{
    public static class FMODCallBackHandler
    {
        /// <summary>
        /// Initializes a default CallBack for all Event Instances.
        /// </summary>
        /// <param name="eventData"></param>
        public static void InitializeCallBack(FMODEmitterData eventData)
        {
            EVENT_CALLBACK EventCallback = new EVENT_CALLBACK(EventCallbackHandler);
            GCHandle EventGCHandle = GCHandle.Alloc(eventData);
            eventData.Emitter.EventInstance.setUserData(GCHandle.ToIntPtr(EventGCHandle));
            eventData.Emitter.EventInstance.setCallback(EventCallback);
        }

        [MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
        private static RESULT EventCallbackHandler(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
        {
            EventInstance instance = new EventInstance(instancePtr);

            // Retrieve the user data
            IntPtr EventDataPtr;
            RESULT result = instance.getUserData(out EventDataPtr);

            if (result != RESULT.OK)
            {
                Debug.LogError("Event State Callback error: " + result);
            }
            else if (EventDataPtr != IntPtr.Zero)
            {
                // Get the object to store the FMODEmitterData
                GCHandle eventDataHandle = GCHandle.FromIntPtr(EventDataPtr);
                FMODEmitterData eventData = (FMODEmitterData)eventDataHandle.Target;

                eventData.Emitter.EventDescription.getUserProperty("IsLooping", out USER_PROPERTY UserProperties);
                eventData.CurrentCallbackType = type;

#if UNITY_EDITOR
                if (FMODManager.Instance.Debug)
                {
                    RuntimeManager.StudioSystem.lookupPath(GUID.Parse(eventData.EventGUID), out string path);
                    Debug.Log($"{path} Event Callback Type {type}");
                }
#endif

                switch (type)
                {
                    case EVENT_CALLBACK_TYPE.STARTED:
                        {
                            eventData.EventState = FMODEventState.Playing;
                            break;
                        }
                    case EVENT_CALLBACK_TYPE.SOUND_STOPPED:
                        {
                            IsLoopingCheck(UserProperties, eventData);
                            break;
                        }
                    case EVENT_CALLBACK_TYPE.STOPPED:
                        {
                            eventData.EventState = FMODEventState.Stopped;
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

        /// <summary>
        /// Checks if the Event Instance is looping.
        /// If the Event is looping, define a variable called "IsLooping" under the User Properties of an Event from FMOD Studio and set its value to 1.
        /// </summary>
        /// <param name="userProperties"></param>
        /// <param name="eventData"></param>
        public static void IsLoopingCheck(USER_PROPERTY userProperties, FMODEmitterData eventData)
        {
            bool isLooping = Convert.ToBoolean(userProperties.intValue());
            if (isLooping)
                eventData.EventState = FMODEventState.Playing;
            else
                eventData.EventState = FMODEventState.Stopped;
        }
    }
}
