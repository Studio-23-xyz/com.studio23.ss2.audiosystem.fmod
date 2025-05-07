using AOT;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Studio23.SS2.AudioSystem.fmod.Core;
using Studio23.SS2.AudioSystem.fmod.Data;
using System;
using System.Collections.Generic;
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
            SoundData data = CreateSoundData(eventData);
            if (data == null) return;
            GCHandle EventGCHandle = GCHandle.Alloc(data);
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
                Debug.LogError("Event Callback error: " + result);
            }
            else if (EventDataPtr != IntPtr.Zero)
            {
                // Get the object to store the FMODEmitterData
                GCHandle eventDataHandle = GCHandle.FromIntPtr(EventDataPtr);
                SoundData soundData = (SoundData)eventDataHandle.Target;

                USER_PROPERTY userProperties;
                if (soundData.EmitterData.Emitter.EventDescription.getUserProperty("IsLooping", out userProperties) != RESULT.OK) userProperties = default;
                soundData.EmitterData.CurrentCallbackType = type;

                //#if UNITY_EDITOR
                string eventPath = "";
                if (FMODManager.Instance.Debug)
                {
                    RuntimeManager.StudioSystem.lookupPath(GUID.Parse(soundData.EmitterData.EventGUID), out eventPath);
                    Debug.Log($"{eventPath}, Event Callback Type {type}");
                }
                //#endif

                switch (type)
                {
                    case EVENT_CALLBACK_TYPE.STARTED:
                        {
                            soundData.EmitterData.EventState = FMODEventState.Playing;
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
                            IsLoopingCheck(userProperties, soundData.EmitterData);

                            //#if UNITY_EDITOR
                            if (FMODManager.Instance.Debug)
                            {
                                Debug.Log($"{eventPath}, Timeline Position: {soundData.EmitterData.GetTimelinePosition()}ms, Length: {soundData.EmitterData.GetLength()}ms");
                            }
                            //#endif

                            if (soundData.EmitterData.GetTimelinePosition() >= soundData.EmitterData.GetLength())
                            {
                                soundData.EmitterData.CompleteEvent();
                            }
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

        private static SoundData CreateSoundData(FMODEmitterData eventData)
        {
            return new SoundData(eventData);
        }

        /// <summary>
        /// Checks if the Event Instance is looping.
        /// If the Event is looping, define a variable called "IsLooping" under the User Properties of an Event from FMOD Studio and set its value to 1.
        /// </summary>
        /// <param name="userProperties"></param>
        /// <param name="eventData"></param>
        public static void IsLoopingCheck(USER_PROPERTY userProperties, FMODEmitterData eventData)
        {
            if (EqualityComparer<USER_PROPERTY>.Default.Equals(userProperties, default(USER_PROPERTY)))
            {
                return;
            }

            bool isLooping = Convert.ToBoolean(userProperties.intValue());
            if (isLooping)
                eventData.EventState = FMODEventState.Playing;
            else
                eventData.EventState = FMODEventState.Stopped;
        }

        private class SoundData
        {
            public FMODEmitterData EmitterData;
            public FMOD.StringWrapper LastMarker;
            public int Position;

            public SoundData(FMODEmitterData emitterData)
            {
                EmitterData = emitterData;
                LastMarker = new StringWrapper();
                Position = 0;
            }
        }
    }

}
