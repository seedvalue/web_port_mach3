using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

[WorkbookLoad("Locations")]
[WorkbookAssetPath("Locations")]
public class MetaLocation : MetaObject, IAnalyticsItem
{
	public string displayName;

	public Sprite bkgImage;

	[NonSerialized]
	public List<MetaStage> stages = new List<MetaStage>();

	public int settingIndex;

	public int index;

	public static MetaLocation[] globalLocations;

	public const string easyUnlockedProperty = "easyUnlocked";

	public const string mediumUnlockedProperty = "mediumUnlocked";

	public const string hardUnlockedProperty = "hardUnlocked";

	public const string refreshViewProperty = "refreshView";

	public const string currentDifficultyLevelProperty = "currentDifficultyLevel";

	public int easyBronzeReward;

	public int easySilverReward;

	public int easyGoldReward;

	public int mediumBronzeReward;

	public int mediumSilverReward;

	public int mediumGoldReward;

	public int hardBronzeReward;

	public int hardSilverReward;

	public int hardGoldReward;

	[MetaData(false, 9)]
	private bool[] rewardCollected;

	private int _refreshView;

	[MetaData(null, 0)]
	private bool _easyUnlocked;

	[MetaData(null, 0)]
	private bool _mediumUnlocked;

	[MetaData(null, 0)]
	private bool _hardUnlocked;

	private MetaStageDifficulty _currentDifficultyLevel;

	public int globalIndex
	{
		get;
		private set;
	}

	public bool easyUnlocked
	{
		get
		{
			return _easyUnlocked;
		}
		set
		{
			PropertySetter(ref _easyUnlocked, value, "easyUnlocked");
		}
	}

	public bool mediumUnlocked
	{
		get
		{
			return _mediumUnlocked;
		}
		set
		{
			PropertySetter(ref _mediumUnlocked, value, "mediumUnlocked");
		}
	}

	public bool hardUnlocked
	{
		get
		{
			return _hardUnlocked;
		}
		set
		{
			PropertySetter(ref _hardUnlocked, value, "hardUnlocked");
		}
	}

	public int refreshView
	{
		get
		{
			return _refreshView;
		}
		set
		{
			PropertySetter(ref _refreshView, value, "refreshView");
		}
	}

	public bool anyUnlocked => easyUnlocked || mediumUnlocked || hardUnlocked;

	public MetaStageDifficulty currentDifficultyLevel
	{
		get
		{
			return _currentDifficultyLevel;
		}
		set
		{
			PropertySetter(ref _currentDifficultyLevel, value, "currentDifficultyLevel");
		}
	}

	public int totalStars => 3 * stages.Count;

	public string analyticsID
	{
		get;
		private set;
	}

	public string analyticsType
	{
		get;
		private set;
	}

	public bool GetUnlocked(MetaStageDifficulty difficulty)
	{
		switch (difficulty)
		{
		default:
			return easyUnlocked;
		case MetaStageDifficulty.Medium:
			return mediumUnlocked;
		case MetaStageDifficulty.Hard:
			return hardUnlocked;
		}
	}

	public float GetProgress(MetaStageDifficulty difficulty, bool onlyShown = false)
	{
		int collectedStars = GetCollectedStars(difficulty, onlyShown);
		if (collectedStars > GetRequiredStars(MetaLocationState.Silver))
		{
			int requiredStars = GetRequiredStars(MetaLocationState.Silver);
			return 0.75f + 0.25f * (float)(collectedStars - requiredStars) / (float)(totalStars - requiredStars);
		}
		if (collectedStars <= GetRequiredStars(MetaLocationState.Bronze))
		{
			return 0.5f * (float)collectedStars / (float)GetRequiredStars(MetaLocationState.Bronze);
		}
		int requiredStars2 = GetRequiredStars(MetaLocationState.Bronze);
		return 0.5f + 0.25f * (float)(collectedStars - requiredStars2) / (float)(GetRequiredStars(MetaLocationState.Silver) - requiredStars2);
	}

	public int GetCollectedStars(MetaStageDifficulty difficulty, bool onlyShown = false)
	{
		switch (difficulty)
		{
		default:
			return stages.Sum((MetaStage x) => (!onlyShown) ? x.easyStars : x.easyStarsShown);
		case MetaStageDifficulty.Medium:
			return stages.Sum((MetaStage x) => (!onlyShown) ? x.mediumStars : x.mediumStarsShown);
		case MetaStageDifficulty.Hard:
			return stages.Sum((MetaStage x) => (!onlyShown) ? x.hardStars : x.hardStarsShown);
		}
	}

	public int GetRequiredStars(MetaLocationState reward)
	{
		switch (reward)
		{
		default:
			return (int)((float)totalStars * 0.5f);
		case MetaLocationState.Silver:
			return (int)((float)totalStars * 0.75f);
		case MetaLocationState.Gold:
			return totalStars;
		}
	}

	public int GetRewardValue(MetaStageDifficulty difficulty, MetaLocationState reward)
	{
		switch (reward)
		{
		case MetaLocationState.Bronze:
			switch (difficulty)
			{
			default:
				return easyBronzeReward;
			case MetaStageDifficulty.Medium:
				return mediumBronzeReward;
			case MetaStageDifficulty.Hard:
				return hardBronzeReward;
			}
		case MetaLocationState.Silver:
			switch (difficulty)
			{
			default:
				return easySilverReward;
			case MetaStageDifficulty.Medium:
				return mediumSilverReward;
			case MetaStageDifficulty.Hard:
				return hardSilverReward;
			}
		case MetaLocationState.Gold:
			switch (difficulty)
			{
			default:
				return easyGoldReward;
			case MetaStageDifficulty.Medium:
				return mediumGoldReward;
			case MetaStageDifficulty.Hard:
				return hardGoldReward;
			}
		default:
			return 0;
		}
	}

	private int GetRewardIndex(MetaStageDifficulty difficulty, MetaLocationState reward)
	{
		return (int)((int)difficulty * 3 + reward - 2);
	}

	public bool IsRewardCollected(MetaStageDifficulty difficulty, MetaLocationState reward)
	{
		return rewardCollected[GetRewardIndex(difficulty, reward)];
	}

	public bool IsRewardAvailable(MetaStageDifficulty difficulty, MetaLocationState reward)
	{
		if (IsRewardCollected(difficulty, reward))
		{
			return false;
		}
		return GetCollectedStars(difficulty) >= GetRequiredStars(reward);
	}

	public bool IsAnyRewardAvailable()
	{
		if (IsRewardAvailable(MetaStageDifficulty.Easy, MetaLocationState.Bronze))
		{
			return true;
		}
		if (IsRewardAvailable(MetaStageDifficulty.Easy, MetaLocationState.Silver))
		{
			return true;
		}
		if (IsRewardAvailable(MetaStageDifficulty.Easy, MetaLocationState.Gold))
		{
			return true;
		}
		if (IsRewardAvailable(MetaStageDifficulty.Medium, MetaLocationState.Gold))
		{
			return true;
		}
		if (IsRewardAvailable(MetaStageDifficulty.Medium, MetaLocationState.Silver))
		{
			return true;
		}
		if (IsRewardAvailable(MetaStageDifficulty.Medium, MetaLocationState.Gold))
		{
			return true;
		}
		if (IsRewardAvailable(MetaStageDifficulty.Hard, MetaLocationState.Bronze))
		{
			return true;
		}
		if (IsRewardAvailable(MetaStageDifficulty.Hard, MetaLocationState.Silver))
		{
			return true;
		}
		if (IsRewardAvailable(MetaStageDifficulty.Hard, MetaLocationState.Gold))
		{
			return true;
		}
		return false;
	}

	public MetaLocationState GetState(MetaStageDifficulty difficulty)
	{
		if (!GetUnlocked(difficulty))
		{
			return MetaLocationState.Locked;
		}
		int collectedStars = GetCollectedStars(difficulty);
		if (collectedStars >= GetRequiredStars(MetaLocationState.Gold))
		{
			return MetaLocationState.Gold;
		}
		if (collectedStars >= GetRequiredStars(MetaLocationState.Silver))
		{
			return MetaLocationState.Silver;
		}
		if (collectedStars >= GetRequiredStars(MetaLocationState.Bronze))
		{
			return MetaLocationState.Bronze;
		}
		return MetaLocationState.Unlocked;
	}

	public bool IsRewardAvailable(MetaStageDifficulty difficulty, int collectedStars, MetaLocationState reward)
	{
		if (IsRewardCollected(difficulty, reward))
		{
			return false;
		}
		return collectedStars >= GetRequiredStars(reward);
	}

	public bool CollectReward(MetaStageDifficulty difficulty, MetaLocationState reward)
	{
		if (IsRewardAvailable(difficulty, reward) && !IsRewardCollected(difficulty, reward))
		{
			int rewardValue = GetRewardValue(difficulty, reward);
			MetaResource.gems.count += GetRewardValue(difficulty, reward);
			rewardCollected[GetRewardIndex(difficulty, reward)] = true;
			AnalyticsManager.ResourceSource(MetaResource.gems.analyticsID, rewardValue, "LocationMastering", analyticsID);
			return true;
		}
		return false;
	}

	public MetaLocation FindNextLocation()
	{
		if (globalIndex < globalLocations.Length - 1)
		{
			return globalLocations[globalIndex + 1];
		}
		return null;
	}

	protected virtual void Awake()
	{
		analyticsID = AnalyticsManager.ResolveID(base.metaID);
		analyticsType = AnalyticsManager.ResolveType(base.metaID);
	}

	protected static void MetaStaticAwake()
	{
		globalLocations = (from s in Singleton<Meta>.Instance.FindObjects<MetaLocation>()
			orderby s.index
			orderby s.settingIndex
			select s).ToArray();
		for (int i = 0; i < globalLocations.Length; i++)
		{
			globalLocations[i].globalIndex = i;
		}
	}

	protected virtual void MetaStart()
	{
		if (globalIndex <= 0)
		{
			_easyUnlocked = true;
			_mediumUnlocked = true;
			_hardUnlocked = true;
		}
		stages = (from s in stages
			orderby s.index
			select s).ToList();
		foreach (MetaStage stage in stages)
		{
			DependsOn(stage);
		}
		DependsOn(MetaResource.gems);
	}

	protected override void OnDependencyChanged(MetaObject metaObject, string propertyName, object before, object after)
	{
		base.OnDependencyChanged(metaObject, propertyName, before, after);
		if (propertyName == "easyUnlocked")
		{
			easyUnlocked = true;
		}
		else if (propertyName == "mediumUnlocked")
		{
			mediumUnlocked = true;
		}
		else if (propertyName == "hardUnlocked")
		{
			hardUnlocked = true;
		}
		if (metaObject is MetaStage)
		{
			refreshView++;
		}
		if (metaObject is MetaResource)
		{
			refreshView++;
		}
	}
}
