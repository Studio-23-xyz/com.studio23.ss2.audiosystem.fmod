using FMODUnity;
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

            // Draw other fields
            EditorGUILayout.PropertyField(startValueProp, new GUIContent("Start Value"));
            EditorGUILayout.PropertyField(endValueProp, new GUIContent("End Value"));
            EditorGUILayout.PropertyField(durationProp, new GUIContent("Duration"));
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(stopOnFadeOutProp, new GUIContent("Stop On Fade Out"));
            EditorGUILayout.PropertyField(releaseOnFadeOutProp, new GUIContent("Release On Fade Out"));

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
