using GameAnalyticsSDK;
using GameAnalyticsSDK.Setup;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Purchasing;
using Utils;

[SingletonInitializeOnLoad]
[SingletonPrefab]
public class AnalyticsManager : Singleton<AnalyticsManager>
{
	[Serializable]
	public struct Target
	{
		public string GAGameKey;

		public string GASecretKey;
	}

	[SerializeField]
	private Target production = default(Target);

	[SerializeField]
	private Target development = default(Target);

	private string playerID;

	private Target target;

	public static string ResolveID(string name)
	{
		int num = name.IndexOf('_');
		if (num < 0)
		{
			return "Unknown";
		}
		return name.Substring(num + 1);
	}

	public static string ResolveType(string name)
	{
		int num = name.IndexOf('_');
		if (num < 0)
		{
			return "Unknown";
		}
		return name.Substring(0, num);
	}

	public static void Business(IAnalyticsItem item, string cartType, Product product)
	{
		Business(item.analyticsType, item.analyticsID, cartType, product);
	}

	public static void Business(string itemType, string itemId, string cartType, Product product)
	{
		string isoCurrencyCode = product.metadata.isoCurrencyCode;
		decimal localizedPrice = product.metadata.localizedPrice;
		int amount = (int)(localizedPrice * 100m);
		string json = null;
		string signature = null;
		if (ParseGooglePlayReceipt(product, out json, out signature))
		{
			GameAnalytics.NewBusinessEventGooglePlay(isoCurrencyCode, amount, itemType, itemId, cartType, json, signature);
			Analytics.Transaction(itemId, localizedPrice, isoCurrencyCode, json, signature);
		}
		else
		{
			GameAnalytics.NewBusinessEvent(isoCurrencyCode, amount, itemType, itemId, cartType);
			Analytics.Transaction(itemId, localizedPrice, isoCurrencyCode);
		}
	}

	public static void ResourceSink(string resource, float amount, IAnalyticsItem item)
	{
		ResourceSink(resource, amount, item.analyticsType, item.analyticsID);
	}

	public static void ResourceSink(string resource, float amount, string itemType, string itemId)
	{
		GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, resource, amount, itemType, itemId);
	}

	public static void ResourceSource(string resource, float amount, IAnalyticsItem item)
	{
		ResourceSource(resource, amount, item.analyticsType, item.analyticsID);
	}

	public static void ResourceSource(string resource, float amount, string itemType, string itemId)
	{
		GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, resource, amount, itemType, itemId);
	}

	public static void Progression(AnalyticsProgression status, string progression01)
	{
		Progression(status, progression01, null, null);
	}

	public static void Progression(AnalyticsProgression status, string progression01, string progression02)
	{
		Progression(status, progression01, progression02, null);
	}

	public static void Progression(AnalyticsProgression status, string progression01, string progression02, string progression03)
	{
		GameAnalytics.NewProgressionEvent(ToGAProgressionStatus(status), progression01, progression02, progression03);
	}

	public static void Progression(AnalyticsProgression status, string progression01, int score)
	{
		Progression(status, progression01, null, null, score);
	}

	public static void Progression(AnalyticsProgression status, string progression01, string progression02, int score)
	{
		Progression(status, progression01, progression02, null, score);
	}

	public static void Progression(AnalyticsProgression status, string progression01, string progression02, string progression03, int score)
	{
		GameAnalytics.NewProgressionEvent(ToGAProgressionStatus(status), progression01, progression02, progression03, score);
	}

	public static void Design(string name1, IDictionary<string, object> data = null)
	{
		AddDesign(data, name1);
	}

	public static void Design(string name1, string name2, IDictionary<string, object> data = null)
	{
		AddDesign(data, name1, name2);
	}

	public static void Design(string name1, string name2, string name3, IDictionary<string, object> data = null)
	{
		AddDesign(data, name1, name2, name3);
	}

	public static void Design(string name1, string name2, string name3, string name4, IDictionary<string, object> data = null)
	{
		AddDesign(data, name1, name2, name3, name4);
	}

	public static void Design(string name1, string name2, string name3, string name4, string name5, IDictionary<string, object> data = null)
	{
		AddDesign(data, name1, name2, name3, name4, name5);
	}

	private static void AddDesign(IDictionary<string, object> data, params string[] names)
	{
		if (string.IsNullOrEmpty(names[0]))
		{
			throw new ArgumentException("Design event first name must be not null or empty");
		}
		string str = names[0];
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 1; i < names.Length; i++)
		{
			if (!string.IsNullOrEmpty(names[i]))
			{
				stringBuilder.Append(':');
				stringBuilder.Append(names[i]);
			}
		}
		string text = stringBuilder.ToString();
		bool flag = false;
		if (data != null)
		{
			foreach (KeyValuePair<string, object> datum in data)
			{
				float result = 0f;
				if (float.TryParse(datum.Value.ToString(), out result))
				{
					flag = true;
					GameAnalytics.NewDesignEvent(str + "_" + datum.Key + text, result);
				}
			}
		}
		if (!flag)
		{
			GameAnalytics.NewDesignEvent(str + text);
		}
		Analytics.CustomEvent(str + text, data);
	}

	private static string UnescapeJSON(string value)
	{
		string text = value.Replace("\\\"", "\"");
		text = text.Replace("\\/", "/");
		return text.Replace("\\\\", "\\");
	}

	private static bool ParseGooglePlayReceipt(Product product, out string json, out string signature)
	{
		string resultJson = null;
		string resultSignature = null;
		if (!string.IsNullOrEmpty(product.receipt))
		{
			JSONObject jSONObject = new JSONObject(product.receipt);
			jSONObject.GetField("Payload", delegate(JSONObject jsonPayload)
			{
				JSONObject jSONObject2 = new JSONObject(UnescapeJSON(jsonPayload.str));
				jSONObject2.GetField("json", delegate(JSONObject jsonPayloadJson)
				{
					resultJson = UnescapeJSON(jsonPayloadJson.str);
				});
				jSONObject2.GetField("signature", delegate(JSONObject jsonPayloadSignature)
				{
					resultSignature = UnescapeJSON(jsonPayloadSignature.str);
				});
			});
		}
		if (string.IsNullOrEmpty(resultJson) || string.IsNullOrEmpty(resultSignature))
		{
			json = null;
			signature = null;
			return false;
		}
		json = resultJson;
		signature = resultSignature;
		return true;
	}

	private static GAProgressionStatus ToGAProgressionStatus(AnalyticsProgression value)
	{
		switch (value)
		{
		case AnalyticsProgression.Start:
			return GAProgressionStatus.Start;
		case AnalyticsProgression.Complete:
			return GAProgressionStatus.Complete;
		case AnalyticsProgression.Fail:
			return GAProgressionStatus.Fail;
		default:
			return GAProgressionStatus.Undefined;
		}
	}

	protected virtual void Awake()
	{
		target = ((!Development.developmentMode) ? production : development);
		playerID = SystemInfo.deviceUniqueIdentifier;
		ConfigureUnityAnalytics();
		ConfigureGameAnalytics();
	}

	protected virtual void Start()
	{
		//StartGameAnalytics();
	}

	private void ConfigureUnityAnalytics()
	{
		/*Analytics.SetUserId(playerID);
		Analytics.CustomEvent("User", new Dictionary<string, object>
		{
			{
				"Valid",
				Development.validUser
			}
		});*/
	}

	private void ConfigureGameAnalytics()
	{
		Settings settingsGA = GameAnalytics.SettingsGA;
		for (int num = settingsGA.Platforms.Count; num > 0; num--)
		{
			settingsGA.RemovePlatformAtIndex(num - 1);
		}
		settingsGA.AddPlatform(Application.platform);
		settingsGA.UpdateGameKey(0, target.GAGameKey);
		settingsGA.UpdateSecretKey(0, target.GASecretKey);
		settingsGA.Build[0] = Application.version;
	}

	private void StartGameAnalytics()
	{
		GameAnalytics.SetCustomId(playerID);
		GameAnalytics.SetCustomDimension01((!Development.validUser) ? "Invalid" : "Valid");
	}
}
