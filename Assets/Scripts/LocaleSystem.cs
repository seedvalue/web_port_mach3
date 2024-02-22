using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils;

[SingletonPrefab]
[SingletonInitializeOnLoad]
public class LocaleSystem : Singleton<LocaleSystem>
{
	public delegate string Resolver(string key, out bool mutable);

	public delegate string MutableResolver(string key);

	public delegate void LocaleChangedDelegate();

	public string workbook;

	public SystemLanguage defaultLanguage = SystemLanguage.Unknown;

	public List<LocaleData> locales;

	private LocaleData currentLocale;

	private readonly List<Resolver> resolvers = new List<Resolver>();

	private readonly Dictionary<string, string> constantKeys = new Dictionary<string, string>();

	public event LocaleChangedDelegate LocaleChanged;

	public void AddResolver(Resolver resolver)
	{
		if (!resolvers.Contains(resolver))
		{
			resolvers.Add(resolver);
		}
	}

	public void RemResolver(Resolver resolver)
	{
		resolvers.Remove(resolver);
	}

	public void SwitchLanguage(SystemLanguage language)
	{
		LocaleData locale = FindBestLocale(language);
		SwitchLocale(locale);
	}

	protected virtual void Awake()
	{
		SwitchLanguage(Application.systemLanguage);
	}

	protected LocaleData FindBestLocale(SystemLanguage language)
	{
		LocaleData localeData = (from l in locales
			where l.language == language
			select l).FirstOrDefault();
		if ((bool)localeData)
		{
			return localeData;
		}
		localeData = (from l in locales
			where l.language == defaultLanguage
			select l).FirstOrDefault();
		if ((bool)localeData)
		{
			return localeData;
		}
		return locales.FirstOrDefault();
	}

	protected void SwitchLocale(LocaleData locale)
	{
		if (!(currentLocale == locale))
		{
			constantKeys.Clear();
			for (int i = 0; i < locale.constantKeys.Count; i++)
			{
				constantKeys.Add(locale.constantKeys[i], locale.constantValues[i]);
			}
			currentLocale = locale;
			this.LocaleChanged?.Invoke();
			LocaleString.NotifyOnLocaleChanged();
			LocaleText.NotifyOnLocaleChanged();
		}
	}

	public string Resolve(string textOrKey, MutableResolver resolver = null)
	{
		string text = FindKeyValue(textOrKey, resolver);
		if (text != null)
		{
			return ResolveText(text, resolver);
		}
		return ResolveText(textOrKey, resolver);
	}

	public string ResolveText(string text, MutableResolver resolver = null)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		for (int num2 = text.IndexOf("$("); num2 >= 0; num2 = text.IndexOf("$(", num))
		{
			int num3 = text.IndexOf(")", num2 + "$(".Length);
			if (num3 < 0)
			{
				break;
			}
			stringBuilder.Append(text, num, num2 - num);
			num = num3 + ")".Length;
			string key = text.Substring(num2 + "$(".Length, num3 - num2 - 2);
			stringBuilder.Append(ResolveKey(key, resolver));
		}
		stringBuilder.Append(text, num, text.Length - num);
		return stringBuilder.ToString();
	}

	public string ResolveKey(string key, MutableResolver resolver = null)
	{
		string text = FindKeyValue(key, resolver);
		if (text != null)
		{
			return ResolveText(text, resolver);
		}
		return key;
	}

	private string FindKeyValue(string key, MutableResolver resolver = null)
	{
		string value = null;
		if (resolver != null)
		{
			value = resolver(key);
			if (!string.IsNullOrEmpty(value))
			{
				return value;
			}
		}
		for (int i = 0; i < resolvers.Count; i++)
		{
			bool mutable = false;
			value = resolvers[i](key, out mutable);
			if (!string.IsNullOrEmpty(value))
			{
				return value;
			}
		}
		if (constantKeys.TryGetValue(key, out value))
		{
			return value;
		}
		return value;
	}
}
