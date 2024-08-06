using FMOD;
using FMOD.Studio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Studio23.SS2.AudioSystem.fmod.Editor
{
    public class CreateFMODDataEditor : EditorWindow
    {
        private const string AssemblyDefinitionKey = "CreateFolderWindow_AssemblyDefinition";
        private const string SelectedFolderKey = "CreateFolderWindow_SelectedFolder";
        private const string CustomNamespaceKey = "CreateFolderWindow_CustomNamespace";
        private const string SelectedOptionKey = "CreateFolderWindow_SelectedOption";

        private enum SelectedOption
        {
            None,
            AssemblyDefinition,
            Folder
        }

        private SelectedOption _selectedOption = SelectedOption.None;
        private AssemblyDefinitionAsset _assemblyDefinitionAsset;
        private DefaultAsset _selectedFolder;
        private string _selectedFolderPath = "";
        private string _customNamespace = "";

        private static Dictionary<string, string> _bankList = new Dictionary<string, string>();
        private static List<string> _busList = new List<string>();
        private static List<string> _VCAList = new List<string>();
        private static Dictionary<string, List<string>> _eventList = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> _parameterList = new Dictionary<string, List<string>>();

        [MenuItem("Studio-23/Audio System/Generate data from FMOD")]
        public static void ShowWindow()
        {
            GetWindow<CreateFMODDataEditor>("Generate data from FMOD");
        }

        private void OnEnable()
        {
            LoadPreferences();
        }

        private void OnDisable()
        {
            SavePreferences();
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            EditorGUILayout.LabelField($"Select your project's assembly definition. The data files will be created in the root folder containing the assembly definition.", EditorStyles.boldLabel);

            GUILayout.Space(5);

            EditorGUI.BeginChangeCheck();
            _assemblyDefinitionAsset = EditorGUILayout.ObjectField("Assembly Definition Asset", _assemblyDefinitionAsset, typeof(AssemblyDefinitionAsset), false) as AssemblyDefinitionAsset;
            if (EditorGUI.EndChangeCheck() && _assemblyDefinitionAsset != null)
            {
                _selectedOption = SelectedOption.AssemblyDefinition;
                _selectedFolder = null;
                _selectedFolderPath = "";
            }

            GUILayout.Space(5);

            // Display path of the assembly definition asset.
            if (_selectedOption == SelectedOption.AssemblyDefinition && _assemblyDefinitionAsset != null)
            {
                EditorGUI.BeginDisabledGroup(true);
                string assemblyDefinitionAssetPath = AssetDatabase.GetAssetPath(_assemblyDefinitionAsset);
                EditorGUILayout.TextField("Assembly Definition Path", Path.GetDirectoryName(assemblyDefinitionAssetPath));
                EditorGUI.EndDisabledGroup();
            }

            GUILayout.Space(15);

            EditorGUILayout.LabelField($"Or select a directory for the data files to be created in.", EditorStyles.boldLabel);

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            _selectedFolder = EditorGUILayout.ObjectField("Select Folder", _selectedFolder, typeof(DefaultAsset), false) as DefaultAsset;
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Convert the absolute path to a relative path
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        _selectedFolderPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                        _selectedFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(_selectedFolderPath);
                        _selectedOption = SelectedOption.Folder;
                        _assemblyDefinitionAsset = null;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            // Display path of selected folder.
            if (_selectedOption == SelectedOption.Folder && !string.IsNullOrEmpty(_selectedFolderPath))
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Selected Folder Path", _selectedFolderPath);
                EditorGUI.EndDisabledGroup();

                GUILayout.Space(5);

                _customNamespace = EditorGUILayout.TextField("Custom Namespace", _customNamespace);
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Generate"))
            {
                string targetFolderPath = null;

                if (_selectedOption == SelectedOption.AssemblyDefinition && _assemblyDefinitionAsset != null)
                {
                    string assemblyFolderPath = AssetDatabase.GetAssetPath(_assemblyDefinitionAsset);
                    string directory = Path.GetDirectoryName(assemblyFolderPath);
                    string folderPath = Path.Combine(directory, "FMOD_Data");

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                        Debug.Log("Folder created at: " + folderPath);
                    }
                    else
                    {
                        Debug.LogWarning("Folder already exists at: " + folderPath);
                    }

                    string nameSpace = GetRootNamespaceFromAssemblyDefinition(_assemblyDefinitionAsset);

                    GenerateData(folderPath, nameSpace);
                }
                else if (_selectedOption == SelectedOption.Folder && _selectedFolder != null)
                {
                    string selectedFolderPath = AssetDatabase.GetAssetPath(_selectedFolder);
                    targetFolderPath = Path.Combine(selectedFolderPath, "FMOD_Data");

                    if (!Directory.Exists(targetFolderPath))
                    {
                        Directory.CreateDirectory(targetFolderPath);
                        Debug.Log("Folder created at: " + targetFolderPath);
                    }
                    else
                    {
                        Debug.LogWarning("Folder already exists at: " + targetFolderPath);
                    }

                    GenerateData(targetFolderPath, _customNamespace);
                }
                else
                {
                    Debug.LogError("Please select the Assembly Definition Asset or a valid directory.");
                }
            }
        }

        private string GetRootNamespaceFromAssemblyDefinition(AssemblyDefinitionAsset assemblyDefinitionAsset)
        {
            string assemblyDefinitionPath = AssetDatabase.GetAssetPath(assemblyDefinitionAsset);
            string assemblyDefinitionText = File.ReadAllText(assemblyDefinitionPath);
            JObject assemblyDefinitionObject = JObject.Parse(assemblyDefinitionText);
            return (string)assemblyDefinitionObject["rootNamespace"];
        }

        private void SavePreferences()
        {
            if (_selectedOption == SelectedOption.AssemblyDefinition && _assemblyDefinitionAsset != null)
            {
                string assemblyDefinitionPath = AssetDatabase.GetAssetPath(_assemblyDefinitionAsset);
                EditorPrefs.SetString(AssemblyDefinitionKey, assemblyDefinitionPath);
                EditorPrefs.DeleteKey(SelectedFolderKey);
                EditorPrefs.DeleteKey(CustomNamespaceKey);
            }
            else if (_selectedOption == SelectedOption.Folder && !string.IsNullOrEmpty(_selectedFolderPath))
            {
                EditorPrefs.SetString(SelectedFolderKey, _selectedFolderPath);
                EditorPrefs.SetString(CustomNamespaceKey, _customNamespace);
                EditorPrefs.DeleteKey(AssemblyDefinitionKey);
            }
            else
            {
                EditorPrefs.DeleteKey(AssemblyDefinitionKey);
                EditorPrefs.DeleteKey(SelectedFolderKey);
                EditorPrefs.DeleteKey(CustomNamespaceKey);
            }

            EditorPrefs.SetInt(SelectedOptionKey, (int)_selectedOption);
        }

        private void LoadPreferences()
        {
            _selectedOption = (SelectedOption)EditorPrefs.GetInt(SelectedOptionKey, (int)SelectedOption.None);

            if (_selectedOption == SelectedOption.AssemblyDefinition && EditorPrefs.HasKey(AssemblyDefinitionKey))
            {
                string assemblyDefinitionPath = EditorPrefs.GetString(AssemblyDefinitionKey);
                _assemblyDefinitionAsset = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assemblyDefinitionPath);
            }
            else if (_selectedOption == SelectedOption.Folder && EditorPrefs.HasKey(SelectedFolderKey))
            {
                _selectedFolderPath = EditorPrefs.GetString(SelectedFolderKey);
                _selectedFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(_selectedFolderPath);

                if (EditorPrefs.HasKey(CustomNamespaceKey))
                {
                    _customNamespace = EditorPrefs.GetString(CustomNamespaceKey);
                }
            }
        }

        private static void GenerateData(string folderPath, string nameSpace)
        {
            _bankList.Clear();
            _busList.Clear();
            _VCAList.Clear();
            _eventList.Clear();
            _parameterList.Clear();
            GetAllData(folderPath, nameSpace);
        }

        private static void GetAllData(string folderPath, string nameSpace)
        {
            foreach (var b in FMODUnity.EventManager.Banks)
            {
                if (!_bankList.ContainsKey(b.Name))
                {
                    _bankList.Add(b.Name, b.Path);
                }
            }

            foreach (var e in FMODUnity.EventManager.Events)
            {
                var eventWithGuid = e.Path + "GUID" + e.Guid;
                foreach (var b in e.Banks)
                {
                    if (_eventList.ContainsKey(b.StudioPath))
                    {
                        List<string> valuesForKey1 = _eventList[b.StudioPath];

                        valuesForKey1.Add(eventWithGuid);
                    }
                    else
                    {
                        List<string> valuesForKey1 = new List<string> { eventWithGuid };
                        _eventList.Add(b.StudioPath, valuesForKey1);
                    }
                }

                foreach (var p in e.Parameters)
                {
                    if (_parameterList.ContainsKey(e.Path))
                    {
                        List<string> valuesForKey1 = _parameterList[e.Path];
                        valuesForKey1.Add(p.name);
                    }
                    else
                    {
                        List<string> valuesForKey1 = new List<string> { p.name };
                        _parameterList.Add(e.Path, valuesForKey1);
                    }
                }
            }
            GenerateBankList(folderPath, nameSpace);
            GenerateLocaleList(folderPath, nameSpace);
            GenerateEventList(folderPath, nameSpace);
            GenerateParameterList(folderPath, nameSpace);
        }

        private static void GenerateBankList(string folderPath, string nameSpace)
        {
            string filename = $"FMODBankList";

            string scriptContent = "";

            scriptContent += $"namespace {nameSpace}\n{{\n";

            scriptContent += $"\tpublic static class {filename}\n\t{{\n";

            for (int i = 0; i < _bankList.Count; i++)
            {
                string bankName = _bankList.ElementAt(i).Key.Replace(".", "_").Replace("(", "").Replace(")","");
                scriptContent += $"\t\tpublic static readonly string {bankName} = \"{_bankList.ElementAt(i).Key}\";\n";
            }
            scriptContent += "\t}\n";
            scriptContent += "}";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string scriptPath = Path.Combine(folderPath, $"{filename}.cs");
            if (File.Exists(scriptPath))
            {
                File.Delete(scriptPath);
            }

            File.WriteAllText(scriptPath, scriptContent);
            AssetDatabase.Refresh();
            GetMixerDataList(folderPath, nameSpace);
        }

        private static void GetMixerDataList(string folderPath, string nameSpace)
        {

            FMOD.Studio.System.create(out FMOD.Studio.System fmodSystem);
            fmodSystem.initialize(8, FMOD.Studio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, new IntPtr(0));

            for (int i = 0; i < _bankList.Count; i++)
            {
                fmodSystem.loadBankFile(_bankList.ElementAt(i).Value, LOAD_BANK_FLAGS.NORMAL, out Bank masterBank);
            }

            fmodSystem.getBankList(out FMOD.Studio.Bank[] allBanks);
            foreach (FMOD.Studio.Bank bank in allBanks)
            {
                var busResult = bank.getBusList(out Bus[] bus);
                if (busResult == RESULT.OK)
                {
                    bank.getBusCount(out int busCount);
                    if (busCount > 0)
                    {
                        foreach (var b in bus)
                        {
                            b.getPath(out string busPath);
                            if (!_busList.Contains(busPath)) _busList.Add(busPath);
                        }
                    }
                }
                var VCAResult = bank.getVCAList(out VCA[] VCA);
                if (VCAResult == RESULT.OK)
                {
                    bank.getVCACount(out int VCACount);
                    if (VCACount > 0)
                    {
                        foreach (var v in VCA)
                        {
                            v.getPath(out string VCAPath);
                            if (!_VCAList.Contains(VCAPath)) _VCAList.Add(VCAPath);
                        }
                    }
                }
            }
            fmodSystem.unloadAll();
            fmodSystem.release();
            GenerateBusList(folderPath, nameSpace);
            GenerateVCAList(folderPath, nameSpace);
        }

        private static void GenerateBusList(string folderPath, string nameSpace)
        {
            string filename = $"FMODBusList";

            string scriptContent = "";

            scriptContent += $"namespace {nameSpace}\n{{\n";

            scriptContent += $"\tpublic static class {filename}\n\t{{\n";

            for (int i = 0; i < _busList.Count; i++)
            {
                string busName = _busList[i].Replace("bus:/", "").Replace(" ", "_").Replace("/", "_").Replace("-", "_");
                if (String.IsNullOrEmpty(busName)) busName = "Master";
                scriptContent += $"\t\tpublic static readonly string {busName} = \"{_busList[i]}\";\n";
            }
            scriptContent += "\t}\n";
            scriptContent += "}";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string scriptPath = Path.Combine(folderPath, $"{filename}.cs");
            if (File.Exists(scriptPath))
            {
                File.Delete(scriptPath);
            }

            File.WriteAllText(scriptPath, scriptContent);
            AssetDatabase.Refresh();
        }

        private static void GenerateVCAList(string folderPath, string nameSpace)
        {
            string filename = $"FMODVCAList";

            string scriptContent = "";

            scriptContent += $"namespace {nameSpace}\n{{\n";

            scriptContent += $"\tpublic static class {filename}\n\t{{\n";


            for (int i = 0; i < _VCAList.Count; i++)
            {
                string VCAName = _VCAList[i].Replace("vca:/", "").Replace(" ", "_").Replace("/", "_").Replace("-", "_");
                if (String.IsNullOrEmpty(VCAName)) VCAName = "Master";
                scriptContent += $"\t\tpublic static readonly string {VCAName} = \"{_VCAList[i]}\";\n";
            }
            scriptContent += "\t}\n";
            scriptContent += "}";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string scriptPath = Path.Combine(folderPath, $"{filename}.cs");
            if (File.Exists(scriptPath))
            {
                File.Delete(scriptPath);
            }

            File.WriteAllText(scriptPath, scriptContent);
            AssetDatabase.Refresh();
        }

        private static void GenerateLocaleList(string folderPath, string nameSpace)
        {
            string filename = $"FMODLocaleList";

            string scriptContent = "";

            scriptContent += "using System.Collections.Generic;\n\n";
            scriptContent += $"namespace {nameSpace}\n{{\n";
            
            scriptContent += $"\tpublic static class {filename}\n\t{{\n";
            scriptContent += $"\t\tpublic static Dictionary<string, string> LanguageList = new Dictionary<string, string>\n";
            scriptContent += "\t\t{";
            scriptContent += "\n";

            for (int i = 0; i < _bankList.Count; i++)
            {
                if (_bankList.ElementAt(i).Key.Contains("LOCALE"))
                {
                    var temp = _bankList.ElementAt(i).Key.Split("LOCALE_")[1].Replace("_", " ");
                    scriptContent += "\t\t\t{";
                    scriptContent += $"\"{temp}\", \"{_bankList.ElementAt(i).Key}\"";
                    scriptContent += "},";
                    scriptContent += "\n";
                }
            }

            scriptContent += "\t\t";
            scriptContent += "};\n";
            scriptContent += "\t}\n";
            scriptContent += "}";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string scriptPath = Path.Combine(folderPath, $"{filename}.cs");
            if (File.Exists(scriptPath))
            {
                File.Delete(scriptPath);
            }

            using (StreamWriter writer = new StreamWriter(scriptPath, false))
            {
                writer.Write(scriptContent);
            }
            AssetDatabase.Refresh();
        }

        private static void GenerateEventList(string folderPath, string nameSpace)
        {
            for (int i = 0; i < _eventList.Count; i++)
            {
                string scriptContent = "";

                scriptContent += "using Studio23.SS2.AudioSystem.fmod.Data;\n\n";

                scriptContent += $"namespace {nameSpace}\n{{\n";

                string filename = $"FMODBank_{_eventList.ElementAt(i).Key.Replace("bank:/", "").Replace(" ", "_").Replace(":/", "_").Replace("/", "_").Replace("-", "_")}";

                scriptContent += $"\tpublic static class {filename}\n\t{{\n";

                foreach (var value in _eventList.ElementAt(i).Value)
                {
                    var guidValue = value.Split("GUID")[1];
                    var eventName = value.Split("GUID")[0].Replace("event:/", "").Replace(" ", "_").Replace(":/", "_").Replace("/", "_").Replace("-", "_");
                    scriptContent += $"\t\tpublic static string {eventName} = \"{guidValue}\";\n";
                }

                scriptContent += "\t}\n";
                scriptContent += "}";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                string scriptPath = Path.Combine(folderPath, $"{filename}.cs");
                if (File.Exists(scriptPath))
                {
                    File.Delete(scriptPath);
                }

                using (StreamWriter writer = new StreamWriter(scriptPath, false))
                {
                    writer.Write(scriptContent);
                }
                AssetDatabase.Refresh();
            }
        }

        private static void GenerateParameterList(string folderPath, string nameSpace)
        {
            string filename = $"FMODParameterList";

            string scriptContent = "";

            scriptContent += $"namespace {nameSpace}\n{{\n";

            scriptContent += $"\tpublic static class {filename}\n\t{{\n";

            for (int i = 0; i < _parameterList.Count; i++)
            {
                var eventName = _parameterList.ElementAt(i).Key.Replace("event:/", "").Replace(":/", "_").Replace("/", "_").Replace(" ", "_").Replace("-", "_");

                scriptContent += $"\t\tpublic static class {eventName}\n\t\t{{\n";

                foreach (var value in _parameterList.ElementAt(i).Value)
                {
                    var variableValue = value.Split("/").Last();
                    var parameterName = variableValue.Replace(" ", "_").Replace("-", "_");

                    scriptContent += $"\t\t\tpublic static readonly string {parameterName} = \"{variableValue}\";\n";
                }
                scriptContent += "\t\t}\n";
            }
            scriptContent += "\t}\n";
            scriptContent += "}";

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string scriptPath = Path.Combine(folderPath, $"{filename}.cs");
            if (File.Exists(scriptPath))
            {
                File.Delete(scriptPath);
            }

            File.WriteAllText(scriptPath, scriptContent);
            AssetDatabase.Refresh();
        }
    }
}
