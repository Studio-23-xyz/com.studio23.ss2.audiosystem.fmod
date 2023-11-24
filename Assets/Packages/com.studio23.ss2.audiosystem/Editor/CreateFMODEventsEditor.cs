using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using FMOD.Studio;

namespace Studio23.SS2.AudioSystem.Editor
{
    public class CreateFMODEventsEditor : EditorWindow
    {
        static Dictionary<string,List<string>> _eventList = new Dictionary<string,List<string>>();
        private static string _className = "FMODEventTable";
        private static string _folderPath = "Assets/Resources/FMOD_BankEvents";
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
                        List<string> valuesForKey1 = new List<string> {e.Path};
                        _eventList.Add(b.Name, valuesForKey1);
                    }

                }
            }

            GenerateStringProperties();
        }

        private static void GenerateStringProperties()
        {
            for (int i = 0; i < _eventList.Count; i++)
            {
                string scriptContent = $"namespace {_nameSpace}\n{{\n";

                scriptContent += $"\tpublic static class {_eventList.ElementAt(i).Key}\n\t{{\n";

                foreach(var value in _eventList.ElementAt(i).Value)
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
                string scriptPath = Path.Combine(_folderPath, $"{_eventList.ElementAt(i).Key}.cs");
                if (File.Exists(scriptPath))
                {
                    File.Delete(scriptPath);
                }

                File.WriteAllText(scriptPath, scriptContent);
                AssetDatabase.Refresh();
            }
        }
    }
}