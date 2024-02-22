using UnityEngine;

public static class Development
{
	private const string ValidPrefKey = "86b3f8500cd34bf7937b305c5563b930";

	public static bool installedFromStore
	{
		get;
		private set;
	}

	public static bool securedEnvironment
	{
		get;
		private set;
	}

	public static bool developmentMode
	{
		get;
		private set;
	}

	public static bool validUser
	{
		get;
		private set;
	}

	static Development()
	{
		installedFromStore = IsInstalledFromStore();
		securedEnvironment = IsSecuredEnvironment();
		developmentMode = IsDevelopmentMode();
		validUser = IsValidUser();
	}

	public static string checkDevelopmentMode()
	{
		string text = string.Empty;
		if (Debug.isDebugBuild)
		{
			text += " DB";
		}
		return text;
	}

	private static bool IsInstalledFromStore()
	{
		return true;
	}

	private static bool IsSecuredEnvironment()
	{
		return true;
	}

	private static bool IsDevelopmentMode()
	{
		return UnityEngine.Debug.isDebugBuild;
	}

	private static bool IsValidUser()
	{
		bool flag = PlayerPrefs.HasKey("86b3f8500cd34bf7937b305c5563b930");
		flag &= installedFromStore;
		flag &= securedEnvironment;
		flag &= !developmentMode;
		if (!flag)
		{
			PlayerPrefs.SetString("86b3f8500cd34bf7937b305c5563b930", SystemInfo.deviceUniqueIdentifier);
			PlayerPrefs.Save();
		}
		return flag;
	}
}
