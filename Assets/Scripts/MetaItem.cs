using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

[WorkbookLoad("Items")]
[WorkbookAssetPath("Items")]
public class MetaItem : MetaObject, IAnalyticsItem
{
	public string displayName;

	public Sprite icon;

	public MetaItemRarity rarity;

	public MetaItemType itemType;

	public MetaItemClass itemClass;

	public List<MetaPerk> perks;

	public MetaSkill skill;

	public string description;

	public GameObject vis3D;

	public HeadVariants headVariant;

	public bool availableSoon;

	public const string countProperty = "count";

	public const string levelProperty = "level";

	public const string foundProperty = "found";

	public const string justFoundProperty = "justFound";

	public const string slotProperty = "slot";

	public const string expireTimeProperty = "expireTime";

	public const string purchasedCountProperty = "purchasedCount";

	public const string selectedProperty = "selected";

	[WorkbookFlat]
	public Stats baseStats;

	[WorkbookFlat(postfix = "%")]
	public Stats basePercentStats;

	[MetaData(null, 0)]
	private int _count;

	[MetaData(1, 0)]
	private int _level;

	[MetaData(null, 0)]
	private bool _found;

	[MetaData(null, 0)]
	private bool _justFound;

	[MetaData(null, 0)]
	private DateTime _expireTime;

	private bool _selected;

	[MetaData(null, 0)]
	private int _purchasedCount;

	private MetaItemSlot _slot;

	public int count
	{
		get
		{
			return _count;
		}
		set
		{
			PropertySetter(ref _count, value, "count");
		}
	}

	public int level
	{
		get
		{
			return _level;
		}
		set
		{
			PropertySetter(ref _level, value, "level");
		}
	}

	public bool found
	{
		get
		{
			return _found;
		}
		set
		{
			PropertySetter(ref _found, value, "found");
		}
	}

	public bool justFound
	{
		get
		{
			return _justFound;
		}
		set
		{
			PropertySetter(ref _justFound, value, "justFound");
		}
	}

	public MetaItemSlot slot
	{
		get
		{
			return _slot;
		}
		private set
		{
			PropertySetter(ref _slot, value, "slot");
		}
	}

	public DateTime expireTime
	{
		get
		{
			return _expireTime;
		}
		set
		{
			PropertySetter(ref _expireTime, value, "expireTime");
		}
	}

	public bool selected
	{
		get
		{
			return _selected;
		}
		set
		{
			PropertySetter(ref _selected, value, "selected");
		}
	}

	public static MetaItem selectedItem => (from i in Singleton<Meta>.Instance.FindObjects<MetaItem>()
		where i.selected
		select i).FirstOrDefault();

	public int firstPurchasePrice
	{
		get
		{
			switch (rarity)
			{
			case MetaItemRarity.Rare:
				return 20;
			case MetaItemRarity.Epic:
				return 2000;
			case MetaItemRarity.Legendary:
				return 40000;
			default:
				return 2;
			}
		}
	}

	public int toBuyCount
	{
		get
		{
			switch (rarity)
			{
			case MetaItemRarity.Rare:
				return 50;
			case MetaItemRarity.Epic:
				return 10;
			case MetaItemRarity.Legendary:
				return 3;
			default:
				return 100;
			}
		}
	}

	public int purchasedCount
	{
		get
		{
			return _purchasedCount;
		}
		set
		{
			PropertySetter(ref _purchasedCount, value, "purchasedCount");
		}
	}

	public Stats stats => GetMetaItemStats(this, level);

	public Stats percentStats => GetMetaItemPercentStats(this, level);

	public bool available => !availableSoon;

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

	public bool canUpgrade
	{
		get
		{
			if (level < GetMaxItemLevel())
			{
				return count >= GetCardsNumRequiredToUpdate(level + 1, rarity);
			}
			return false;
		}
	}

	protected virtual void Awake()
	{
		analyticsID = AnalyticsManager.ResolveID(base.metaID);
		analyticsType = AnalyticsManager.ResolveType(base.metaID);
	}

	protected virtual void MetaReset()
	{
		MetaAwake();
	}

	protected virtual void MetaAwake()
	{
	}

	protected virtual void MetaStart()
	{
		MetaItemSlot metaItemSlot = (from x in Singleton<Meta>.Instance.FindObjects<MetaItemSlot>()
			where x.acceptedItemType == itemType
			select x).FirstOrDefault();
		if (metaItemSlot != null)
		{
			slot = ((!(metaItemSlot.item == this)) ? null : metaItemSlot);
			DependsOn(metaItemSlot);
			if ((bool)slot && !found)
			{
				found = true;
				justFound = false;
			}
		}
	}

	protected override void OnDependencyChanged(MetaObject metaObject, string propertyName, object before, object after)
	{
		base.OnDependencyChanged(metaObject, propertyName, before, after);
		MetaItemSlot metaItemSlot = metaObject as MetaItemSlot;
		if (metaItemSlot != null)
		{
			slot = ((!(metaItemSlot.item == this)) ? null : metaItemSlot);
		}
	}

	protected override void OnPropertyChanged(string propertyName, object before, object after)
	{
		base.OnPropertyChanged(propertyName, before, after);
		if (propertyName == "count")
		{
			if (count > 0 && !found)
			{
				found = true;
				count--;
			}
		}
		else if (propertyName == "found")
		{
			justFound = true;
			MetaItemSlot metaItemSlot = (from x in Singleton<Meta>.Instance.FindObjects<MetaItemSlot>()
				where x.acceptedItemType == itemType
				where x.item == null
				select x).FirstOrDefault();
			if (metaItemSlot != null)
			{
				metaItemSlot.item = this;
			}
		}
		else if (propertyName == "selected")
		{
			justFound = false;
		}
	}

	public int GetMaxItemLevel()
	{
		return GetMaxItemLevel(rarity);
	}

	public static int GetMaxItemLevel(MetaItemRarity rarity)
	{
		switch (rarity)
		{
		case MetaItemRarity.Common:
			return 13;
		case MetaItemRarity.Rare:
			return 11;
		case MetaItemRarity.Epic:
			return 8;
		case MetaItemRarity.Legendary:
			return 5;
		default:
			return 1;
		}
	}

	public static Stats GetMetaItemStats(MetaItem item, int level)
	{
		return item.baseStats * GetStatsMultiplier(level, item.rarity);
	}

	public static Stats GetMetaItemPercentStats(MetaItem item, int level)
	{
		return item.basePercentStats * GetStatsMultiplier(level, item.rarity);
	}

	public static float GetStatsMultiplier(int itemLevel, MetaItemRarity rarity)
	{
		return SingletonComponent<Meta, MetaBalance>.Instance.itemsStatsCurve[(int)rarity][itemLevel];
	}

	public static int GetCardsNumRequiredToUpdate(int newLevel, MetaItemRarity rarity)
	{
		return SingletonComponent<Meta, MetaBalance>.Instance.itemsLevelUpRequirements[(int)rarity][newLevel];
	}

	public static int GetCoinsRequiredToUpdate(int newLevel, MetaItemRarity rarity)
	{
		return SingletonComponent<Meta, MetaBalance>.Instance.itemsUpgradeCost[(int)rarity][newLevel];
	}

	public bool Upgrade()
	{
		int coinsRequiredToUpdate = GetCoinsRequiredToUpdate();
		int cardsNumRequiredToUpdate = GetCardsNumRequiredToUpdate();
		if (MetaResource.coins.count >= coinsRequiredToUpdate && count >= cardsNumRequiredToUpdate)
		{
			count -= cardsNumRequiredToUpdate;
			MetaResource.coins.count -= coinsRequiredToUpdate;
			level++;
			MetaAnalytics instance = SingletonComponent<Meta, MetaAnalytics>.Instance;
			AnalyticsManager.ResourceSink(MetaResource.coins.analyticsID, coinsRequiredToUpdate, "ItemUpgrade", analyticsID);
			AnalyticsManager.Design("ItemUpgrade", analyticsID, "Level" + level.ToString("000"), new Dictionary<string, object>
			{
				{
					"Grinds",
					instance.grinds
				},
				{
					"RealTime",
					instance.realTime
				},
				{
					"GameTime",
					instance.gameTime
				}
			});
			return true;
		}
		return false;
	}

	public int GetCardsNumRequiredToUpdate()
	{
		if (level < GetMaxItemLevel())
		{
			return GetCardsNumRequiredToUpdate(level + 1, rarity);
		}
		return 0;
	}

	public int GetCoinsRequiredToUpdate()
	{
		return GetCoinsRequiredToUpdate(level + 1, rarity);
	}

	public MetaItemSlot InsertToSlot()
	{
		List<MetaItemSlot> list = (from x in Singleton<Meta>.Instance.FindObjects<MetaItemSlot>().ToList()
			where x.acceptedItemType == itemType
			select x).ToList();
		if (list.Count != 1)
		{
			UnityEngine.Debug.LogError("Czemu mamy 2 takie same sloty?");
			return null;
		}
		list[0].item = this;
		return list[0];
	}

	public bool IsWorn()
	{
		List<MetaItemSlot> list = (from x in Singleton<Meta>.Instance.FindObjects<MetaItemSlot>()
			where x.acceptedItemType == itemType
			select x).ToList();
		if (list.Count != 1)
		{
			UnityEngine.Debug.LogError("Czemu mamy 2 takie same sloty?");
			return false;
		}
		return list[0].item == this;
	}
}
