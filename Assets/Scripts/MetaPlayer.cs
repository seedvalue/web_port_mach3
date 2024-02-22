using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class MetaPlayer : MetaObject
{
	public const string levelProperty = "level";

	public const string levelExpProperty = "levelExp";

	public const string levelExpLimitProperty = "levelExpLimit";

	public const string statsProperty = "stats";

	public const string statsDeltaForSelectedItemProperty = "statsDeltaForSelectedItem";

	public const string baseStatsProperty = "baseStats";

	public const string playerNameProperty = "playerName";

	public const string createdProperty = "created";

	public int levelUpGems = 10;

	public int levelUpCoins;

	[MetaData(1, 0)]
	private int _level;

	private int _levelExp;

	private int _levelExpLimit;

	private Stats _baseStats = new Stats();

	private Stats _stats = new Stats();

	private Stats _statsDeltaForSelectedItem = new Stats();

	[MetaData("", 0)]
	private string _playerName;

	[MetaData(null, 0)]
	private bool _created;

	public static MetaPlayer local
	{
		get;
		private set;
	}

	public int level
	{
		get
		{
			return _level;
		}
		private set
		{
			PropertySetter(ref _level, value, "level");
		}
	}

	public int levelExp
	{
		get
		{
			return _levelExp;
		}
		private set
		{
			PropertySetter(ref _levelExp, value, "levelExp");
		}
	}

	public int levelExpLimit
	{
		get
		{
			return _levelExpLimit;
		}
		private set
		{
			PropertySetter(ref _levelExpLimit, value, "levelExpLimit");
		}
	}

	public Stats stats
	{
		get
		{
			return _stats;
		}
		private set
		{
			PropertySetter(ref _stats, value, "stats");
		}
	}

	public Stats statsDeltaForSelectedItem
	{
		get
		{
			return _statsDeltaForSelectedItem;
		}
		private set
		{
			PropertySetter(ref _statsDeltaForSelectedItem, value, "statsDeltaForSelectedItem");
		}
	}

	public Stats baseStats
	{
		get
		{
			return _baseStats;
		}
		private set
		{
			PropertySetter(ref _baseStats, value, "baseStats");
		}
	}

	public bool created
	{
		get
		{
			return _created;
		}
		set
		{
			PropertySetter(ref _created, value, "created");
		}
	}

	public string playerName
	{
		get
		{
			return _playerName;
		}
		set
		{
			PropertySetter(ref _playerName, value, "playerName");
		}
	}

	public string id => "00-0000-00";

	private static Stats GetBaseStats(int level)
	{
		return SingletonComponent<Meta, MetaBalance>.Instance.playerLevelData.FindLast((MetaBalance.PlayerLevelData d) => d.Level <= level).BaseStats;
	}

	protected static void MetaStaticAwake()
	{
		MetaPlayer[] array = Singleton<Meta>.Instance.FindObjects<MetaPlayer>();
		if (array.Length > 1)
		{
			throw new ArgumentException($"Found {array.Length} MetaPlayers!");
		}
		if (array.Length == 1)
		{
			local = array[0];
		}
		SingletonComponent<Meta, MetaBalance>.Instance.playerLevelData.Sort();
	}

	protected virtual void MetaStart()
	{
		AdjustLevelToExp();
		baseStats = GetBaseStats(level);
		stats = CalculatePlayerStats();
		statsDeltaForSelectedItem = new Stats();
		MetaItem[] array = Singleton<Meta>.Instance.FindObjects<MetaItem>();
		for (int i = 0; i < array.Length; i++)
		{
			DependsOn(array[i]);
		}
		DependsOn(MetaResource.exp);
	}

	protected override void OnPropertyChanged(string propertyName, object before, object after)
	{
		base.OnPropertyChanged(propertyName, before, after);
		if (propertyName == "level")
		{
			baseStats = GetBaseStats(level);
		}
		else if (propertyName == "baseStats")
		{
			AdjustStats();
		}
	}

	protected override void OnDependencyChanged(MetaObject metaObject, string propertyName, object before, object after)
	{
		base.OnDependencyChanged(metaObject, propertyName, before, after);
		if (metaObject == MetaResource.exp)
		{
			AdjustLevelToExp();
		}
		else if (metaObject is MetaItem && (propertyName == "level" || propertyName == "slot" || propertyName == "selected"))
		{
			AdjustStats();
		}
	}

	private void AdjustLevelToExp()
	{
		int exp = MetaResource.exp.count;
		int num = SingletonComponent<Meta, MetaBalance>.Instance.playerLevelData.FindLastIndex((MetaBalance.PlayerLevelData d) => d.ExpTotal <= exp || d.Level <= level);
		MetaBalance.PlayerLevelData playerLevelData = SingletonComponent<Meta, MetaBalance>.Instance.playerLevelData.ElementAtOrDefault(num);
		MetaBalance.PlayerLevelData playerLevelData2 = SingletonComponent<Meta, MetaBalance>.Instance.playerLevelData.ElementAtOrDefault(num + 1);
		if (playerLevelData == null)
		{
			playerLevelData = SingletonComponent<Meta, MetaBalance>.Instance.playerLevelData.Last();
		}
		if (level < playerLevelData.Level)
		{
			OnLevelUp(level, playerLevelData.Level);
			level = playerLevelData.Level;
		}
		if (playerLevelData2 == null)
		{
			levelExp = 0;
			levelExpLimit = 0;
		}
		else
		{
			levelExp = exp - playerLevelData.ExpTotal;
			levelExpLimit = playerLevelData2.ExpTotal - playerLevelData.ExpTotal;
		}
	}

	private void AdjustStats()
	{
		stats = CalculatePlayerStats();
		if ((bool)MetaItem.selectedItem)
		{
			statsDeltaForSelectedItem = CalculatePlayerStatsWith(MetaItem.selectedItem) - CalculatePlayerStatsWithout(MetaItem.selectedItem);
		}
		else
		{
			statsDeltaForSelectedItem = new Stats();
		}
	}

	private Stats CalculatePlayerStats(MetaItem selectedItem = null)
	{
		MetaItem[] items = (from s in Singleton<Meta>.Instance.FindObjects<MetaItemSlot>()
			select s.item into i
			where i != null
			select i).ToArray();
		return CalculatePlayerStats(items);
	}

	private Stats CalculatePlayerStatsWith(MetaItem item)
	{
		if (item == null)
		{
			return CalculatePlayerStats();
		}
		MetaItem[] items = (from s in Singleton<Meta>.Instance.FindObjects<MetaItemSlot>()
			select s.item into i
			where i != null
			where i.itemType != item.itemType
			select i).Concat(new MetaItem[1]
		{
			item
		}).ToArray();
		return CalculatePlayerStats(items);
	}

	private Stats CalculatePlayerStatsWithout(MetaItem item)
	{
		if (item == null)
		{
			return CalculatePlayerStats();
		}
		MetaItem[] items = (from s in Singleton<Meta>.Instance.FindObjects<MetaItemSlot>()
			select s.item into i
			where i != null
			where i != item
			select i).ToArray();
		return CalculatePlayerStats(items);
	}

	private Stats CalculatePlayerStats(MetaItem[] items)
	{
		Stats lhs = new Stats();
		Stats lhs2 = new Stats();
		lhs2 += 100;
		foreach (MetaItem metaItem in items)
		{
			lhs += metaItem.stats;
			lhs2 += metaItem.percentStats;
		}
		return baseStats + lhs * lhs2 * 0.01f;
	}

	private void OnLevelUp(int from, int to)
	{
		MetaAnalytics instance = SingletonComponent<Meta, MetaAnalytics>.Instance;
		for (int i = from + 1; i < to + 1; i++)
		{
			AnalyticsManager.Design("LevelUp", "Level" + i.ToString("000"), new Dictionary<string, object>
			{
				{
					"Grinds",
					instance.grinds
				},
				{
					"GemsSpent",
					instance.gemsSpent
				},
				{
					"CoinsSpent",
					instance.coinsSpent
				},
				{
					"GemsCurrent",
					instance.gemsCurrent
				},
				{
					"CoinsCurrent",
					instance.coinsCurrent
				},
				{
					"ItemsUpgradable",
					instance.itemsUpgradable
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
		}
		int gems = (to - from) * levelUpGems;
		int coins = (to - from) * levelUpCoins;
		LevelUpWindowContext context = new LevelUpWindowContext();
		context.player = local;
		context.stats = GetBaseStats(from);
		context.newStats = GetBaseStats(to);
		context.newLevel = to;
		context.exp = levelExpLimit;
		context.unlockedIcon = null;
		context.unlockedText = string.Empty;
		context.rewardCoins = coins;
		context.rewardGems = gems;
		SequenceManager.Enqueue(delegate
		{
			Singleton<WindowManager>.Instance.OpenWindow<LevelUpWindow>(context);
		});
		SequenceManager.Enqueue(WaitForLevelUpWindowClose());
		if (gems > 0)
		{
			MetaResourceView metaResourceView = MetaView.FindMasterView<MetaResourceView>(MetaResource.gems);
			if ((bool)metaResourceView)
			{
				SequenceManager.Enqueue(metaResourceView.DropSequencePre(gems));
				SequenceManager.Enqueue(delegate
				{
					MetaResource.gems.count += gems;
				});
			}
			else
			{
				MetaResource.gems.count += gems;
			}
			AnalyticsManager.ResourceSource(MetaResource.gems.analyticsID, gems, "LevelUp", "LevelUp");
		}
		if (coins > 0)
		{
			MetaResourceView metaResourceView2 = MetaView.FindMasterView<MetaResourceView>(MetaResource.coins);
			if ((bool)metaResourceView2)
			{
				SequenceManager.Enqueue(metaResourceView2.DropSequencePre(coins));
				SequenceManager.Enqueue(delegate
				{
					MetaResource.coins.count += coins;
				});
			}
			else
			{
				MetaResource.coins.count += coins;
			}
			AnalyticsManager.ResourceSource(MetaResource.coins.analyticsID, coins, "LevelUp", "LevelUp");
		}
	}

	private IEnumerator WaitForLevelUpWindowClose()
	{
		yield return new WaitUntil(() => Singleton<WindowManager>.Instance.GetOpenWindow<LevelUpWindow>() == null);
	}
}
