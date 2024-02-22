using System;
using UnityEngine;
using Utils;

public class MetaRewardChestSlot : MetaChestSlot
{
	public const string chestProperty = "chest";

	public const string contentProperty = "content";

	public const string chanceItemProperty = "chanceItem";

	public const string contentLevelProperty = "contentLevel";

	public const string openTimeProperty = "openTime";

	public const string stateProperty = "state";

	public const string openNowPriceProperty = "openNowPrice";

	public const string ableToUnlockProperty = "ableToUnlock";

	[MetaData(null, 0)]
	private MetaRewardChest _chest;

	[MetaData(null, 0)]
	private MetaChestContent _content;

	[MetaData(null, 0)]
	private MetaItem _chanceItem;

	[MetaData(null, 0)]
	private int _contentLevel;

	[MetaData(null, 0)]
	private DateTime? _openTime;

	[MetaData(null, 0)]
	private MetaRewardChestSlotState _state;

	private int _openNowPrice;

	private bool _ableToUnlock;

	private static MetaRewardChestSlot[] allSlots;

	public MetaRewardChest chest
	{
		get
		{
			return _chest;
		}
		set
		{
			PropertySetter(ref _chest, value, "chest");
		}
	}

	public int contentLevel
	{
		get
		{
			return _contentLevel;
		}
		private set
		{
			PropertySetter(ref _contentLevel, value, "contentLevel");
		}
	}

	public DateTime? openTime
	{
		get
		{
			return _openTime;
		}
		private set
		{
			PropertySetter(ref _openTime, value, "openTime");
		}
	}

	public MetaRewardChestSlotState state
	{
		get
		{
			return _state;
		}
		set
		{
			PropertySetter(ref _state, value, "state");
		}
	}

	public MetaChestContent content
	{
		get
		{
			return _content;
		}
		set
		{
			PropertySetter(ref _content, value, "content");
		}
	}

	public MetaItem chanceItem
	{
		get
		{
			return _chanceItem;
		}
		set
		{
			PropertySetter(ref _chanceItem, value, "chanceItem");
		}
	}

	public int openNowPrice
	{
		get
		{
			return _openNowPrice;
		}
		private set
		{
			PropertySetter(ref _openNowPrice, value, "openNowPrice");
		}
	}

	public bool ableToUnlock
	{
		get
		{
			return _ableToUnlock;
		}
		private set
		{
			PropertySetter(ref _ableToUnlock, value, "ableToUnlock");
		}
	}

	protected static void MetaStaticAwake()
	{
		allSlots = Singleton<Meta>.Instance.FindObjects<MetaRewardChestSlot>();
	}

	protected static void MetaStaticStart()
	{
		UpdateAbleToOpen();
	}

	protected virtual void MetaUpdate()
	{
		if (state == MetaRewardChestSlotState.OpeningChest)
		{
			DateTime utcNow = SingletonComponent<Meta, MetaTimeManager>.Instance.utcNow;
			if (openTime <= utcNow)
			{
				state = MetaRewardChestSlotState.WithOpenChest;
				return;
			}
			float num = (float)(openTime.Value - utcNow).TotalHours / chest.openTimeHours;
			openNowPrice = Mathf.CeilToInt((float)chest.openGems * num);
		}
	}

	protected override bool OnPropertyWillChange(string propertyName, object before, object after)
	{
		if (!base.OnPropertyWillChange(propertyName, before, after))
		{
			return false;
		}
		if (propertyName == "state" && (MetaRewardChestSlotState)after == MetaRewardChestSlotState.OpeningChest && FindOpeningSlot() != null)
		{
			return false;
		}
		return true;
	}

	protected override void OnPropertyChanged(string propertyName, object before, object after)
	{
		base.OnPropertyChanged(propertyName, before, after);
		if (propertyName == "chest")
		{
			if (chest != null)
			{
				contentLevel = MetaPlayer.local.level;
				openTime = null;
				state = MetaRewardChestSlotState.WithChest;
				content = null;
				chanceItem = null;
				openNowPrice = 0;
			}
			else
			{
				contentLevel = 0;
				openTime = null;
				state = MetaRewardChestSlotState.Empty;
				content = null;
				chanceItem = null;
				openNowPrice = 0;
			}
		}
		else
		{
			if (!(propertyName == "state"))
			{
				return;
			}
			UpdateAbleToOpen();
			if (state == MetaRewardChestSlotState.OpeningChest)
			{
				MetaChest.LevelData levelData = chest.levelData;
				if (levelData == null)
				{
					openTime = SingletonComponent<Meta, MetaTimeManager>.Instance.utcNow;
					return;
				}
				openTime = SingletonComponent<Meta, MetaTimeManager>.Instance.utcNow + TimeSpan.FromHours(chest.openTimeHours);
				openNowPrice = chest.openGems;
			}
			else if (state == MetaRewardChestSlotState.WithOpenChest)
			{
				openTime = null;
			}
			else if (state == MetaRewardChestSlotState.Empty)
			{
				chest = null;
				content = null;
				chanceItem = null;
				openNowPrice = 0;
			}
		}
	}

	public static MetaRewardChestSlot FindEmptySlot()
	{
		return Array.Find(allSlots, (MetaRewardChestSlot v) => v.chest == null);
	}

	public static MetaRewardChestSlot FindOpeningSlot()
	{
		return Array.Find(allSlots, (MetaRewardChestSlot v) => v.state == MetaRewardChestSlotState.OpeningChest);
	}

	public static bool AddChest(MetaRewardChest chest, MetaChestContent content = null)
	{
		return AddChest(chest, content, null);
	}

	public static bool AddChest(MetaRewardChest chest, MetaItem chanceItem)
	{
		return AddChest(chest, null, chanceItem);
	}

	private static bool AddChest(MetaRewardChest chest, MetaChestContent content, MetaItem chanceItem)
	{
		MetaRewardChestSlot metaRewardChestSlot = FindEmptySlot();
		if (metaRewardChestSlot == null)
		{
			return false;
		}
		if (chest == null)
		{
			return false;
		}
		metaRewardChestSlot.chest = chest;
		metaRewardChestSlot.content = content;
		metaRewardChestSlot.chanceItem = chanceItem;
		return true;
	}

	private static void UpdateAbleToOpen()
	{
		MetaRewardChestSlot x = FindOpeningSlot();
		MetaRewardChestSlot[] array = allSlots;
		foreach (MetaRewardChestSlot metaRewardChestSlot in array)
		{
			metaRewardChestSlot.ableToUnlock = (x == null || x == metaRewardChestSlot);
		}
	}
}
