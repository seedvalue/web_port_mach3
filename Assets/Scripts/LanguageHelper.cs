using System;
using UnityEngine;

public static class LanguageHelper
{
	public static string To2DigitCode(SystemLanguage systemLanguage)
	{
		switch (systemLanguage)
		{
		case SystemLanguage.Afrikaans:
			return "af";
		case SystemLanguage.Arabic:
			return "ar";
		case SystemLanguage.Basque:
			return "eu";
		case SystemLanguage.Belarusian:
			return "by";
		case SystemLanguage.Bulgarian:
			return "bg";
		case SystemLanguage.Catalan:
			return "ca";
		case SystemLanguage.Chinese:
			return "zh";
		case SystemLanguage.Czech:
			return "cs";
		case SystemLanguage.Danish:
			return "da";
		case SystemLanguage.Dutch:
			return "nl";
		case SystemLanguage.English:
			return "en";
		case SystemLanguage.Estonian:
			return "et";
		case SystemLanguage.Faroese:
			return "fo";
		case SystemLanguage.Finnish:
			return "fi";
		case SystemLanguage.French:
			return "fr";
		case SystemLanguage.German:
			return "de";
		case SystemLanguage.Greek:
			return "el";
		case SystemLanguage.Hebrew:
			return "iw";
		case SystemLanguage.Hungarian:
			return "hu";
		case SystemLanguage.Icelandic:
			return "is";
		case SystemLanguage.Indonesian:
			return "in";
		case SystemLanguage.Italian:
			return "it";
		case SystemLanguage.Japanese:
			return "ja";
		case SystemLanguage.Korean:
			return "ko";
		case SystemLanguage.Latvian:
			return "lv";
		case SystemLanguage.Lithuanian:
			return "lt";
		case SystemLanguage.Norwegian:
			return "no";
		case SystemLanguage.Polish:
			return "pl";
		case SystemLanguage.Portuguese:
			return "pt";
		case SystemLanguage.Romanian:
			return "ro";
		case SystemLanguage.Russian:
			return "ru";
		case SystemLanguage.SerboCroatian:
			return "sh";
		case SystemLanguage.Slovak:
			return "sk";
		case SystemLanguage.Slovenian:
			return "sl";
		case SystemLanguage.Spanish:
			return "es";
		case SystemLanguage.Swedish:
			return "sv";
		case SystemLanguage.Thai:
			return "th";
		case SystemLanguage.Turkish:
			return "tr";
		case SystemLanguage.Ukrainian:
			return "uk";
		case SystemLanguage.Vietnamese:
			return "vi";
		default:
			throw new ArgumentException("Unknown SystemLanguage");
		}
	}
}
