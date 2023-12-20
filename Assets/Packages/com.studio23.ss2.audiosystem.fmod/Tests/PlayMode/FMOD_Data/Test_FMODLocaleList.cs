using System.Collections.Generic;

namespace Studio23.SS2.AudioSystem.FMOD.Data
{
	public enum Language
	{
		EN,
		BN,
		CN,
		JP,
	}

	public static class Test_FMODLocaleList
	{
		public static Dictionary<Language, string> LanguageList = new Dictionary<Language, string>
		{
            {Language.BN, "Assets/Packages/com.studio23.ss2.audiosystem.fmod/Samples/Sample_FMOD_Project/Build/Desktop/DialogueTable_LOCALE_BN.bank"},
            {Language.CN, "Assets/Packages/com.studio23.ss2.audiosystem.fmod/Samples/Sample_FMOD_Project/Build/Desktop/DialogueTable_LOCALE_CN.bank"},
            {Language.EN, "Assets/Packages/com.studio23.ss2.audiosystem.fmod/Samples/Sample_FMOD_Project/Build/Desktop/DialogueTable_LOCALE_EN.bank"},
            {Language.JP, "Assets/Packages/com.studio23.ss2.audiosystem.fmod/Samples/Sample_FMOD_Project/Build/Desktop/DialogueTable_LOCALE_JP.bank"},
        };
	}
}