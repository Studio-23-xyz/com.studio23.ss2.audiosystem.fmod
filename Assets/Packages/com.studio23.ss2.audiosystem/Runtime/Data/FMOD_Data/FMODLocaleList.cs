using System.Collections.Generic;

namespace Studio23.SS2.AudioSystem.Data
{
	public enum Language
	{
		EN,
		BN,
		CN,
		JP,
	}

	public static class FMODLocaleList
	{
		public static Dictionary<Language, string> LanguageList = new Dictionary<Language, string>
		{
			{Language.EN, "FMODBankList.DialogueTable_LOCALE_EN"},
			{Language.BN, "FMODBankList.DialogueTable_LOCALE_BN"},
			{Language.CN, "FMODBankList.DialogueTable_LOCALE_CN"},
			{Language.JP, "FMODBankList.DialogueTable_LOCALE_JP"},
		};
	}
}