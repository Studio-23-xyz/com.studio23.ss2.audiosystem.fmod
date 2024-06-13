using FMOD;
using FMOD.Studio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

public class CreateFMODDataTestsEditor : EditorWindow
{
    private static Dictionary<string, string> _bankList = new Dictionary<string, string>();
    private static List<string> _busList = new List<string>();
    private static List<string> _VCAList = new List<string>();
    private static Dictionary<string, List<string>> _eventList = new Dictionary<string, List<string>>();
    private static Dictionary<string, List<string>> _parameterList = new Dictionary<string, List<string>>();

    private static string _testFolderPath = "Assets/Packages/com.studio23.ss2.audiosystem.fmod/Tests/PlayMode/FMOD_Test_Data";
    private static string _testNameSpace = "Studio23.SS2.AudioSystem.fmod.Tests";

    [MenuItem("Studio-23/Audio System/Generate Test Data")]
    public static void GenerateData()
    {
        _bankList.Clear();
        _busList.Clear();
        _VCAList.Clear();
        _eventList.Clear();
        _parameterList.Clear();
        GetAllData(_testFolderPath, _testNameSpace, "Test_");
    }

    private static void GetAllData(string folderPath, string nameSpace, string prefix = "")
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
        GenerateBankList(folderPath, nameSpace, prefix);
        GenerateLocaleList(folderPath, nameSpace, prefix);
        GenerateEventList(folderPath, nameSpace, prefix);
        GenerateParameterList(folderPath, nameSpace, prefix);
    }

    private static void GenerateBankList(string folderPath, string nameSpace, string prefix)
    {
        string filename = $"{prefix}FMODBankList";

        string scriptContent = "";

        scriptContent += "using Studio23.SS2.AudioSystem.fmod.Data;\n\n";

        scriptContent += $"namespace {nameSpace}\n{{\n";

        scriptContent += $"\tpublic static class {filename}\n\t{{\n";

        for (int i = 0; i < _bankList.Count; i++)
        {
            string bankName = _bankList.ElementAt(i).Key.Replace(".", "_");
            scriptContent += $"\t\tpublic static readonly string {bankName} = \"{_bankList.ElementAt(i).Value}\";\n";
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
        GetMixerDataList(folderPath, nameSpace, prefix);
    }

    private static void GetMixerDataList(string folderPath, string nameSpace, string prefix)
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
        GenerateBusList(folderPath, nameSpace, prefix);
        GenerateVCAList(folderPath, nameSpace, prefix);
    }

    private static void GenerateBusList(string folderPath, string nameSpace, string prefix)
    {
        string filename = $"{prefix}FMODBusList";

        string scriptContent = "";
        scriptContent += "using Studio23.SS2.AudioSystem.fmod.Data;\n\n";

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

    private static void GenerateVCAList(string folderPath, string nameSpace, string prefix)
    {
        string filename = $"{prefix}FMODVCAList";

        string scriptContent = "";

        scriptContent += "using Studio23.SS2.AudioSystem.fmod.Data;\n\n";

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

    private static void GenerateLocaleList(string folderPath, string nameSpace, string prefix)
    {
        string filename = $"{prefix}FMODLocaleList";

        string scriptContent = "";

        scriptContent += "using Studio23.SS2.AudioSystem.fmod.Data;\n";

        scriptContent += "using System.Collections.Generic;\n\n";
        scriptContent += $"namespace {nameSpace}\n{{\n";
        scriptContent += $"\tpublic enum Language\n";
        scriptContent += "\t{";
        scriptContent += "\n";

        for (int i = 0; i < _bankList.Count; i++)
        {
            if (_bankList.ElementAt(i).Key.Contains("LOCALE"))
            {
                var temp = _bankList.ElementAt(i).Key.Split("LOCALE_")[1];
                scriptContent += $"\t\t{temp},\n";
            }
        }
        scriptContent += "\t}";
        scriptContent += "\n\n";

        scriptContent += $"\tpublic static class {filename}\n\t{{\n";
        scriptContent += $"\t\tpublic static Dictionary<Language, string> LanguageList = new Dictionary<Language, string>\n";
        scriptContent += "\t\t{";
        scriptContent += "\n";

        for (int i = 0; i < _bankList.Count; i++)
        {
            if (_bankList.ElementAt(i).Key.Contains("LOCALE"))
            {
                var temp = _bankList.ElementAt(i).Key.Split("LOCALE_")[1];
                scriptContent += "\t\t\t{";
                scriptContent += $"Language.{temp}, \"{_bankList.ElementAt(i).Value}\"";
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

    private static void GenerateEventList(string folderPath, string nameSpace, string prefix)
    {
        for (int i = 0; i < _eventList.Count; i++)
        {
            string scriptContent = "";

            scriptContent += "using Studio23.SS2.AudioSystem.fmod.Data;\n\n";

            scriptContent += $"namespace {nameSpace}\n{{\n";

            //string filename = $"FMODBank_{_eventList.ElementAt(i).Key.Split("/").Last().Replace(".bank", "").Replace(" ", "_").Replace("-", "_")}";

            string filename = $"{prefix}FMODBank_{_eventList.ElementAt(i).Key.Replace("bank:/", "").Replace(" ", "_").Replace(":/", "_").Replace("/", "_").Replace("-", "_")}";

            scriptContent += $"\tpublic static class {filename}\n\t{{\n";

            foreach (var value in _eventList.ElementAt(i).Value)
            {
                var guidValue = value.Split("GUID")[1];
                var eventName = value.Split("GUID")[0].Replace("event:/", "").Replace(" ", "_").Replace(":/", "_").Replace("/", "_").Replace("-", "_");
                scriptContent += $"\t\tpublic static FMODEventData {eventName} = new FMODEventData(\"{_eventList.ElementAt(i).Key}\", \"{value.Split("GUID")[0]}\", \"{guidValue}\");\n";
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

    private static void GenerateParameterList(string folderPath, string nameSpace, string prefix)
    {
        string filename = $"{prefix}FMODParameterList";

        string scriptContent = "";

        scriptContent += "using Studio23.SS2.AudioSystem.fmod.Data;\n\n";

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
