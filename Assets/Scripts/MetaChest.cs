using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

[WorkbookLoad("Chests")]
[WorkbookAssetPath("Chests")]
public abstract class MetaChest : MetaObject, IAnalyticsItem
{
	public class LevelData
	{
		public int PriceGems;

		public int GoldMin;

		public int GoldMax;

		public int CardCount;

		public float RareMin;

		public float RareMax;

		public float EpicMin;

		public float EpicMax;

		public float LegendMin;

		public float LegendMax;
	}

	public const int CommGroups = 3;

	public const int RareGroups = 2;

	public const int EpicGroups = 2;

	public const float CommGroupMul = 2f;

	public const float CommGroupDev = 0.5f;

	public const float RareGroupMul = 2f;

	public const float RareGroupDev = 0.5f;

	public const float EpicGroupMul = 2f;

	public const float EpicGroupDev = 0.5f;

	private const float ChanceItemMul = 4f;

	public string displayName;

	public Sprite icon;

	public GameObject vis3D;

	public float forcedInSequence;

	public float openTimeHours;

	public int openGems;

	public float priceMul;

	public float goldMul = 1f;

	public float cardMul = 1f;

	public float rareMul = 1f;

	public float epicMul = 1f;

	public float legendMul = 1f;

	public float goldXMul;

	public float goldDisp;

	public float goldBase;

	public float goldDiv;

	public float cardXMul;

	public float cardDisp;

	public float cardBase;

	public float rareProb;

	public float epicProb;

	public float legendProb;

	public const string levelProperty = "level";

	private int _level;

	[MetaData(null, 0)]
	private float _currentForcedInSequence;

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

	public LevelData levelData => CalcLevelData(MetaPlayer.local.level);

	public float currentForcedInSequence
	{
		get
		{
			return _currentForcedInSequence;
		}
		set
		{
			_currentForcedInSequence = value;
		}
	}

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

	protected virtual void Awake()
	{
		analyticsID = AnalyticsManager.ResolveID(base.metaID);
		analyticsType = AnalyticsManager.ResolveType(base.metaID);
	}

	protected virtual void MetaStart()
	{
		DependsOn(MetaPlayer.local);
		_level = MetaPlayer.local.level;
	}

	protected override void OnDependencyChanged(MetaObject metaObject, string propertyName, object before, object after)
	{
		base.OnDependencyChanged(metaObject, propertyName, before, after);
		if (MetaPlayer.local == metaObject)
		{
			level = MetaPlayer.local.level;
		}
	}

	public List<MetaLink> PickRewards(int level, MetaItem chanceItem = null)
	{
		List<MetaLink> result = new List<MetaLink>();
		LevelData levelData = CalcLevelData(level);
		if (levelData == null)
		{
			return result;
		}
		return PickRewards(levelData, chanceItem);
	}

	public LevelData CalcLevelData(int level)
	{
		float num = Mathf.Log((float)level * goldXMul + goldDisp, goldBase) * priceMul;
		float num2 = Mathf.Log((float)level * goldXMul + goldDisp, goldBase) * goldMul;
		float num3 = num2 * goldDiv;
		float num4 = Mathf.Log((float)level * cardXMul + cardDisp, cardBase) * cardMul;
		float num5 = num4 * rareProb * rareMul;
		float num6 = num4 * epicProb * epicMul;
		float num7 = num4 * legendProb * legendMul;
		LevelData levelData = new LevelData();
		levelData.PriceGems = Mathf.RoundToInt(num * 0.1f) * 10;
		levelData.GoldMin = Mathf.RoundToInt(num2 - num3);
		levelData.GoldMax = Mathf.RoundToInt(num2 + num3);
		levelData.CardCount = Mathf.RoundToInt(num4);
		levelData.RareMin = Mathf.FloorToInt(num5);
		levelData.RareMax = levelData.RareMin + (num5 - levelData.RareMin) * 2f;
		levelData.EpicMin = Mathf.FloorToInt(num6);
		levelData.EpicMax = levelData.EpicMin + (num6 - levelData.EpicMin) * 2f;
		levelData.LegendMin = Mathf.FloorToInt(num7);
		levelData.LegendMax = levelData.LegendMin + (num7 - levelData.LegendMin) * 2f;
		return levelData;
	}

	public static List<MetaLink> PickRewards(LevelData data, MetaItem chanceItem = null)
	{
		List<MetaLink> list = new List<MetaLink>();
		MetaLink metaLink = new MetaLink(MetaResource.coins);
		metaLink.SetProperty("count", Random.Range(data.GoldMin, data.GoldMax + 1));
		list.Add(metaLink);
		int cardCount = data.CardCount;
		cardCount -= PickItems(list, MetaItemRarity.Legendary, chanceItem, data.LegendMin, data.LegendMax);
		cardCount -= PickItems(list, MetaItemRarity.Epic, chanceItem, data.EpicMin, data.EpicMax, 2, 2f, 0.5f);
		cardCount -= PickItems(list, MetaItemRarity.Rare, chanceItem, data.RareMin, data.RareMax, 2, 2f, 0.5f);
		PickItems(list, MetaItemRarity.Common, chanceItem, cardCount, 3, 2f, 0.5f);
		return list.OrderBy(delegate(MetaLink l)
		{
			MetaItem metaItem = l.metaObject as MetaItem;
			return (!metaItem) ? 0f : (SingletonComponent<Meta, MetaConsts>.Instance.elitarismByRarity[(int)metaItem.rarity] * (float)l.GetPropertyOrDefault<int>("count"));
		}).ToList();
	}

	private static int[] PickGroups(float min, float max, int groups, float groupMul, float groupDev)
	{
		int count = Rand.NormalRangeInt(min, max);
		return PickGroups(count, groups, groupMul, groupDev);
	}

	private static int[] PickGroups(int count, int groups, float groupMul, float groupDev)
	{
		groups = Mathf.Min(count, groups);
		switch (groups)
		{
		case 0:
			return new int[0];
		case 1:
		{
			int[] array3 = new int[groups];
			array3[0] = count;
			return array3;
		}
		default:
		{
			float[] array = new float[groups];
			array[0] = 1f;
			float num = 1f;
			for (int i = 1; i < groups; i++)
			{
				array[i] = array[i - 1] * Rand.NormalRange(groupMul - groupDev, groupMul + groupDev);
				num += array[i];
			}
			int[] array2 = new int[groups];
			int num2 = 0;
			for (int j = 0; j < groups; j++)
			{
				array[j] = (float)count * array[j] / num;
				array2[j] = Mathf.FloorToInt(array[j]);
				num2 += array2[j];
			}
			for (int k = 0; k < groups; k++)
			{
				if (num2 >= count)
				{
					break;
				}
				if (array[k] - (float)array2[k] < Rand.uniform)
				{
					array2[k]++;
					num2++;
				}
			}
			if (num2 < count)
			{
				array2[groups - 1] += count - num2;
			}
			return (from r in array2
				where r > 0
				select r).ToArray();
		}
		}
	}

	private static int PickItems(List<MetaLink> result, MetaItemRarity rarity, MetaItem chanceItem, int count, int groups = 1, float groupMul = 2f, float groupDev = 0f)
	{
		MetaItem[] array = (from item in Singleton<Meta>.Instance.FindObjects<MetaItem>()
			where item.rarity == rarity
			where item.available
			select item).ToArray();
		groups = Mathf.Min(groups, array.Length);
		int[] groupCount = PickGroups(count, groups, groupMul, groupDev);
		return PickItemsForGroups(result, array, chanceItem, groupCount);
	}

	private static int PickItems(List<MetaLink> result, MetaItemRarity rarity, MetaItem chanceItem, float min, float max, int groups = 1, float groupMul = 2f, float groupDev = 0f)
	{
		MetaItem[] array = (from item in Singleton<Meta>.Instance.FindObjects<MetaItem>()
			where item.rarity == rarity
			where item.available
			select item).ToArray();
		groups = Mathf.Min(groups, array.Length);
		int[] groupCount = PickGroups(min, max, groups, groupMul, groupDev);
		return PickItemsForGroups(result, array, chanceItem, groupCount);
	}

	private static int PickItemsForGroups(List<MetaLink> result, MetaItem[] items, MetaItem chanceItem, int[] groupCount)
	{
		if (groupCount.Length == 0)
		{
			return 0;
		}
		float[] list = (from i in items
			select (!(i == chanceItem)) ? 1f : 4f).ToArray();
		int[] array = Rand.PickIndicesUniqueByWeight(list, groupCount.Length);
		for (int j = 0; j < groupCount.Length; j++)
		{
			MetaLink metaLink = new MetaLink(items[array[j]]);
			metaLink.SetProperty("count", groupCount[j]);
			result.Add(metaLink);
		}
		return groupCount.Sum();
	}
}
