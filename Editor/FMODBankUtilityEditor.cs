﻿using Studio23.SS2.AudioSystem.fmod;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace FMODUnity
{
    [CustomEditor(typeof(FMODBankUtility))]
    [CanEditMultipleObjects]
    public class FMODBankUtilityEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var load = serializedObject.FindProperty("LoadEvent");
            var unload = serializedObject.FindProperty("UnloadEvent");
            var tag = serializedObject.FindProperty("CollisionTag");
            var banks = serializedObject.FindProperty("Banks");
            var addressableBanks = serializedObject.FindProperty("AddressableBanks");
            var onBankLoadingComplete = serializedObject.FindProperty("OnBankLoadingComplete");
            var onBankUnloadingComplete = serializedObject.FindProperty("OnBankUnloadingComplete");

            // Reference to the target script
            FMODBankUtility utility = (FMODBankUtility)target;

            EditorGUILayout.PropertyField(load, new GUIContent("Load"));
            EditorGUILayout.PropertyField(unload, new GUIContent("Unload"));

            if ((load.enumValueIndex >= 3 && load.enumValueIndex <= 6) ||
                (unload.enumValueIndex >= 3 && unload.enumValueIndex <= 6))
            {
                tag.stringValue = EditorGUILayout.TagField("Collision Tag", tag.stringValue);
            }

            // Display the correct field based on LoadBanksUsingAddressable
            if (utility.LoadBanksUsingAddressable)
            {
                EditorGUILayout.PropertyField(addressableBanks, new GUIContent("Addressable Banks"), true);
            }
            else
            {
                // Display Banks list with Add and Delete functionality
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Banks");
                EditorGUILayout.BeginVertical();
                if (GUILayout.Button("Add Bank", GUILayout.ExpandWidth(false)))
                {
                    banks.InsertArrayElementAtIndex(banks.arraySize);
                    SerializedProperty newBank = banks.GetArrayElementAtIndex(banks.arraySize - 1);
                    newBank.stringValue = "";

                    EventBrowser browser = CreateInstance<EventBrowser>();
                    browser.titleContent = new GUIContent("Select FMOD Bank");
                    browser.ChooseBank(newBank);
                    browser.ShowUtility();
                }

                Texture deleteTexture = EditorUtils.LoadImage("Delete.png");
                GUIContent deleteContent = new GUIContent(deleteTexture, "Delete Bank");

                var buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.padding.top = buttonStyle.padding.bottom = 1;
                buttonStyle.margin.top = 2;
                buttonStyle.padding.left = buttonStyle.padding.right = 4;
                buttonStyle.fixedHeight = GUI.skin.textField.CalcSize(new GUIContent()).y;

                for (int i = 0; i < banks.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(banks.GetArrayElementAtIndex(i), GUIContent.none);

                    if (GUILayout.Button(deleteContent, buttonStyle, GUILayout.ExpandWidth(false)))
                    {
                        banks.DeleteArrayElementAtIndex(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                Event e = Event.current;
                if (e.type == EventType.DragPerform)
                {
                    if (DragAndDrop.objectReferences.Length > 0 &&
                        DragAndDrop.objectReferences[0] != null &&
                        DragAndDrop.objectReferences[0].GetType() == typeof(EditorBankRef))
                    {
                        int pos = banks.arraySize;
                        banks.InsertArrayElementAtIndex(pos);
                        var pathProperty = banks.GetArrayElementAtIndex(pos);

                        pathProperty.stringValue = ((EditorBankRef)DragAndDrop.objectReferences[0]).Name;

                        e.Use();
                    }
                }
                if (e.type == EventType.DragUpdated)
                {
                    if (DragAndDrop.objectReferences.Length > 0 &&
                        DragAndDrop.objectReferences[0] != null &&
                        DragAndDrop.objectReferences[0].GetType() == typeof(EditorBankRef))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                        DragAndDrop.AcceptDrag();
                        e.Use();
                    }
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Bank Events", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(onBankLoadingComplete, new GUIContent("On Bank Loading Complete"));
            EditorGUILayout.PropertyField(onBankUnloadingComplete, new GUIContent("On Bank Unloading Complete"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
