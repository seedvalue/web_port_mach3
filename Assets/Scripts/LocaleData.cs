using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Locale", menuName = "Locale")]
public class LocaleData : ScriptableObject
{
	private const string DevelopmentCode = "dev";

	public SystemLanguage language = SystemLanguage.Unknown;

	public List<string> constantKeys;

	public List<string> constantValues;

	public string languageCode
	{
		get
		{
			if (language != SystemLanguage.Unknown)
			{
				return LanguageHelper.To2DigitCode(language);
			}
			return "dev";
		}
	}

	public bool isDevelopmentLanguage => language == SystemLanguage.Unknown;
}
