using FMODUnity;
using Studio23.SS2.AudioSystem.fmod.Extensions;
using UnityEditor;
using UnityEngine;

namespace Studio23.SS2.AudioSystem.fmod.Editor
{
    [CustomEditor(typeof(FMODEmitterUtility))]
    [CanEditMultipleObjects]
    public class FMODEmitterUtilityEditor : UnityEditor.Editor
    {
        private SerializedProperty eventReferenceProp;
        private SerializedProperty gameObjectProp;
        private SerializedProperty parameterNameProp;
        private SerializedProperty startValueProp;
        private SerializedProperty endValueProp;
        private SerializedProperty durationProp;
        private SerializedProperty stopOnFadeOutProp;
        private SerializedProperty releaseOnFadeOutProp;

        // SerializedProperties for UnityEvents
        private SerializedProperty OnEventPlayed;
        private SerializedProperty OnEventSuspended;
        private SerializedProperty OnEventUnsuspended;
        private SerializedProperty OnEventPaused;
        private SerializedProperty OnEventUnPaused;
        private SerializedProperty OnEventStopped;
        private SerializedProperty OnEventCompleted;

        // SerializedProperties for randomization
        private SerializedProperty randomizeParametersProp;
        private SerializedProperty startValueRangeProp;
        private SerializedProperty endValueRangeProp;
        private SerializedProperty durationRangeProp;
        private SerializedProperty roundToIntProp;  // New property for round to integer

        private bool showEvents;

        private void OnEnable()
        {
            eventReferenceProp = serializedObject.FindProperty("EventReference");
            gameObjectProp = serializedObject.FindProperty("_gameObject");
            parameterNameProp = serializedObject.FindProperty("_parameterName");
            startValueProp = serializedObject.FindProperty("_startValue");
            endValueProp = serializedObject.FindProperty("_endValue");
            durationProp = serializedObject.FindProperty("_duration");
            stopOnFadeOutProp = serializedObject.FindProperty("StopOnFadeOut");
            releaseOnFadeOutProp = serializedObject.FindProperty("ReleaseOnFadeOut");

            // Initialize UnityEvent properties
            OnEventPlayed = serializedObject.FindProperty("OnEventPlayed");
            OnEventSuspended = serializedObject.FindProperty("OnEventSuspended");
            OnEventUnsuspended = serializedObject.FindProperty("OnEventUnsuspended");
            OnEventPaused = serializedObject.FindProperty("OnEventPaused");
            OnEventUnPaused = serializedObject.FindProperty("OnEventUnPaused");
            OnEventStopped = serializedObject.FindProperty("OnEventStopped");
            OnEventCompleted = serializedObject.FindProperty("OnEventCompleted");

            // Initialize randomization properties
            randomizeParametersProp = serializedObject.FindProperty("randomizeParameters");
            startValueRangeProp = serializedObject.FindProperty("_startValueRange");
            endValueRangeProp = serializedObject.FindProperty("_endValueRange");
            durationRangeProp = serializedObject.FindProperty("_durationRange");
            roundToIntProp = serializedObject.FindProperty("roundToInt");  // Initialize the round to integer property
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw the EventReference property
            EditorGUILayout.PropertyField(eventReferenceProp, new GUIContent("Event Reference"));

            // Draw the GameObject field
            EditorGUILayout.PropertyField(gameObjectProp, new GUIContent("GameObject"));
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            // Draw the parameter selection
            DrawParameterSelection();

            // Draw randomization toggle
            EditorGUILayout.PropertyField(randomizeParametersProp, new GUIContent("Randomize Parameters"));

            // If randomization is enabled, show the range fields and the round to integer toggle
            if (randomizeParametersProp.boolValue)
            {
                EditorGUILayout.PropertyField(startValueRangeProp, new GUIContent("Start Value Range"));
                EditorGUILayout.PropertyField(endValueRangeProp, new GUIContent("End Value Range"));
                EditorGUILayout.PropertyField(durationRangeProp, new GUIContent("Duration Range"));
                EditorGUILayout.PropertyField(roundToIntProp, new GUIContent("Round to Integer"));  // Display the round to integer option
            }
            else
            {
                EditorGUILayout.PropertyField(startValueProp, new GUIContent("Start Value"));
                EditorGUILayout.PropertyField(endValueProp, new GUIContent("End Value"));
                EditorGUILayout.PropertyField(durationProp, new GUIContent("Duration"));
            }

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(stopOnFadeOutProp, new GUIContent("Stop On Fade Out"));
            EditorGUILayout.PropertyField(releaseOnFadeOutProp, new GUIContent("Release On Fade Out"));
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            // UnityEvents foldout
            showEvents = EditorGUILayout.Foldout(showEvents, "Events");
            if (showEvents)
            {
                EditorGUILayout.PropertyField(OnEventPlayed, new GUIContent("On Event Played"));
                EditorGUILayout.PropertyField(OnEventSuspended, new GUIContent("On Event Suspended"));
                EditorGUILayout.PropertyField(OnEventUnsuspended, new GUIContent("On Event Unsuspended"));
                EditorGUILayout.PropertyField(OnEventPaused, new GUIContent("On Event Paused"));
                EditorGUILayout.PropertyField(OnEventUnPaused, new GUIContent("On Event UnPaused"));
                EditorGUILayout.PropertyField(OnEventStopped, new GUIContent("On Event Stopped"));
                EditorGUILayout.PropertyField(OnEventCompleted, new GUIContent("On Event Completed"));
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawParameterSelection()
        {
            string eventPath = eventReferenceProp.FindPropertyRelative("Path").stringValue;
            EditorEventRef editorEvent = EventManager.EventFromPath(eventPath);

            if (editorEvent != null)
            {
                // Display a dropdown to select a parameter
                string[] parameterNames = new string[editorEvent.Parameters.Count];
                for (int i = 0; i < editorEvent.Parameters.Count; i++)
                {
                    parameterNames[i] = editorEvent.Parameters[i].Name;
                }

                int selectedIndex = -1;
                if (!string.IsNullOrEmpty(parameterNameProp.stringValue))
                {
                    selectedIndex = System.Array.IndexOf(parameterNames, parameterNameProp.stringValue);
                }

                selectedIndex = EditorGUILayout.Popup("Parameter", selectedIndex, parameterNames);

                bool isGlobal = false;
                ParameterType parameterType = ParameterType.Continuous;
                float defaultValue = 0f;
                float minValue = 0f;
                float maxValue = 0f;
                string[] parameterLabels = new string[editorEvent.Parameters.Count];
                string parameterLabelNames = null;

                if (selectedIndex >= 0 && selectedIndex < parameterNames.Length)
                {
                    parameterNameProp.stringValue = parameterNames[selectedIndex];

                    // Assign min and max values to startValue and endValue
                    var selectedParam = editorEvent.Parameters[selectedIndex];
                    isGlobal = selectedParam.IsGlobal;
                    parameterType = selectedParam.Type;
                    defaultValue = selectedParam.Default;
                    minValue = selectedParam.Min;
                    maxValue = selectedParam.Max;
                    parameterLabels = selectedParam.Labels;
                    parameterLabelNames = string.Join(", ", parameterLabels);
                }
                else
                {
                    parameterNameProp.stringValue = string.Empty;
                }

                EditorGUILayout.HelpBox($"{(isGlobal ? "Global Parameter" : "Local Parameter")}\n" +
                                        $"Parameter Type: {parameterType}\n" +
                                        $"Default Value: {defaultValue}, Min Value: {minValue}, Max Value: {maxValue} \n" +
                                        $"{(parameterLabels.Length <= 0 ? "" : $"Parameter Labels: {parameterLabelNames}")}", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("No event selected or event not found.", MessageType.Warning);
            }
        }
    }
}
