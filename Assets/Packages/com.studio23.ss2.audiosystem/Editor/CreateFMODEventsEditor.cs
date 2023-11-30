using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using FMOD.Studio;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using CodiceApp.EventTracking.Plastic;
using JetBrains.Annotations;

namespace Studio23.SS2.AudioSystem.Editor
{
    public class CreateFMODEventsEditor : EditorWindow
    {
        static Dictionary<string, List<string>> _eventList = new Dictionary<string, List<string>>();
        static Dictionary<string, List<string>> _parameterList = new Dictionary<string, List<string>>();
        private static string _className = "FMODEventTable";
        private static string _folderPath = "Assets/Resources/FMOD_Data";
        private static string _nameSpace = "Studio23.SS2.AudioSystem.Data";


        [MenuItem("Studio-23/Audio System/Generate Event Data")]
        public static void GetAllEvents()
        {
            foreach (var e in FMODUnity.EventManager.Events)
            {
                foreach (var b in e.Banks)
                {
                    if (_eventList.ContainsKey(b.Name))
                    {
                        List<string> valuesForKey1 = _eventList[b.Name];
                        valuesForKey1.Add(e.Path);
                    }
                    else
                    {
                        List<string> valuesForKey1 = new List<string> { e.Path };
                        _eventList.Add(b.Name, valuesForKey1);
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

            GenerateEventList();
            GenerateParameterList();
        }

        private static void GenerateEventList()
        {
            for (int i = 0; i < _eventList.Count; i++)
            {
                string scriptContent = $"namespace {_nameSpace}\n{{\n";

                string filename = $"FMODBank_{_eventList.ElementAt(i).Key}";

                scriptContent += $"\tpublic static class {filename}\n\t{{\n";

                foreach (var value in _eventList.ElementAt(i).Value)
                {
                    var eventName = value.Replace("event:/", "");
                    scriptContent += $"\t\tpublic static readonly string {eventName} = \"{value}\";\n";
                }

                scriptContent += "\t}\n";
                scriptContent += "}";


                if (!Directory.Exists(_folderPath))
                {
                    Directory.CreateDirectory(_folderPath);
                }

                string scriptPath = Path.Combine(_folderPath, $"{filename}.cs");
                if (File.Exists(scriptPath))
                {
                    File.Delete(scriptPath);
                }

                File.WriteAllText(scriptPath, scriptContent);
                AssetDatabase.Refresh();
            }
        }

        private static void GenerateParameterList()
        {
            /*for (int i = 0; i < _parameterList.Count; i++)
            {
                foreach (var p in _parameterList.ElementAt(i).Value)
                {
                    Debug.Log(p);
                }
            }*/

            string filename = "FMODParameterList";
            string scriptContent = $"namespace {_nameSpace}\n{{\n";

            scriptContent += $"\tpublic static class {filename}\n\t{{\n";

            for (int i = 0; i < _parameterList.Count; i++)
            {
                var eventName = _parameterList.ElementAt(i).Key.Replace("event:/", "");

                scriptContent += $"\t\tpublic static class {eventName}\n\t\t{{\n";

                foreach (var value in _parameterList.ElementAt(i).Value)
                {
                    var temp = value.Replace($"parameter:/{eventName}/", "");
                    var parameterName = temp.Replace(" ", "");

                    scriptContent += $"\t\t\tpublic static readonly string {parameterName} = \"{parameterName}\";\n";
                }
                scriptContent += "\t\t}\n";
            }
            scriptContent += "\t}\n";
            scriptContent += "}";


            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }
            string scriptPath = Path.Combine(_folderPath, $"{filename}.cs");
            if (File.Exists(scriptPath))
            {
                File.Delete(scriptPath);
            }

            File.WriteAllText(scriptPath, scriptContent);
            AssetDatabase.Refresh();
        }
    }
}
