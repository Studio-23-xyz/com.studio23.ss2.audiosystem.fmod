using FMOD.Studio;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Studio23.SS2.AudioSystem.Editor
{
    public class CreateFMODEventsEditor : EditorWindow
    {
        private static Dictionary<string, string> _bankList = new Dictionary<string, string>();
        static Dictionary<string, List<string>> _eventList = new Dictionary<string, List<string>>();
        static Dictionary<string, List<string>> _parameterList = new Dictionary<string, List<string>>();
        private static string _className = "FMODEventTable";
        private static string _folderPath = "Assets/Resources/FMOD_Data";
        private static string _nameSpace = "Studio23.SS2.AudioSystem.Data";


        [MenuItem("Studio-23/Audio System/Generate All FMOD Data")]
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

                    if (!_bankList.ContainsKey(b.Name))
                    {
                        _bankList.Add(b.Name, b.Path);
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
            GenerateBankList();
            GenerateEventList();
            GenerateParameterList();
        }

        private static void GenerateBankList()
        {
            string filename = "FMODBankList";
            string scriptContent = $"namespace {_nameSpace}\n{{\n";

            scriptContent += $"\tpublic static class {filename}\n\t{{\n";

            for (int i = 0; i < _bankList.Count; i++)
            {
                scriptContent += $"\t\tpublic static readonly string {_bankList.ElementAt(i).Key} = \"{_bankList.ElementAt(i).Value}\";\n";
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

        private static void GenerateEventList()
        {
            for (int i = 0; i < _eventList.Count; i++)
            {
                string scriptContent = $"namespace {_nameSpace}\n{{\n";

                string filename = $"FMODBank_{_eventList.ElementAt(i).Key}";

                scriptContent += $"\tpublic static class {filename}\n\t{{\n";

                foreach (var value in _eventList.ElementAt(i).Value)
                {
                    var eventName = value.Replace("event:/", "").Replace(" ", "_").Replace(":/", "_").Replace("/", "_").Replace("-", "_");
                    var newData = new EventData(_eventList.ElementAt(i).Key, value);
                    scriptContent += $"\t\tpublic static readonly EventData {eventName} = \"{newData}\";\n";
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
            string filename = "FMODParameterList";
            string scriptContent = $"namespace {_nameSpace}\n{{\n";

            scriptContent += $"\tpublic static class {filename}\n\t{{\n";

            for (int i = 0; i < _parameterList.Count; i++)
            {
                var eventName = _parameterList.ElementAt(i).Key.Replace("event:/", "").Replace("/", "_").Replace(" ", "_").Replace("-", "_");

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

    public class EventData
    {
        public string BankRef;
        public string EventName;

        public EventData(string bank, string eventName)
        {
            BankRef = bank;
            EventName = eventName;
        }
    }
}
