using FMOD.Studio;
using Studio23.SS2.AudioSystem.Data;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class FMODCallBackHandler
{
    public static void InitializeCallback(FMODEmitterData eventData)
    {
        FMOD.Studio.EVENT_CALLBACK EventCallback = new FMOD.Studio.EVENT_CALLBACK(FMODCallBackHandler.EventCallbackHandler);
        GCHandle EventGCHandle = GCHandle.Alloc(eventData);
        eventData.Emitter.EventInstance.setUserData(GCHandle.ToIntPtr(EventGCHandle));
        eventData.Emitter.EventInstance.setCallback(EventCallback);
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    public static FMOD.RESULT EventCallbackHandler(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        // Retrieve the user data
        IntPtr EventStateInfoPtr;
        FMOD.RESULT result = instance.getUserData(out EventStateInfoPtr);

        if (result != FMOD.RESULT.OK)
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
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                    {
                        eventStateHandle.Free();
                        break;
                    }
            }
        }
        return FMOD.RESULT.OK;
    }
}
