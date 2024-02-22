using System;
using System.Collections.Generic;
using UnityEngine;

public class MetaBalance : MetaComponent<MetaBalance>
{
	[Serializable]
	public class ItemStatsCurveData
	{
		public int Lvl;

		public float Common;

		public float Rare;

		public float Epic;

		public float Legendary;
	}

	[Serializable]
	public class ItemUpgradeCostData
	{
		public int Lvl;

		public int Common;

		public int Rare;

		public int Epic;

		public int Legendary;
	}

	[Serializable]
	public class ItemLevelUpRequirementsData
	{
		public int Lvl;

		public int Common;

		public int Rare;

		public int Epic;

		public int Legendary;
	}

	[Serializable]
	public class StageBattlesData
	{
		public MetaStage Stage;

		public int BattleIndex;

		public string[] Mobs;

		[WorkbookFlat]
		public MobStatsMultipliers Multipliers;
	}

	[Serializable]
	public class PlayerLevelData : IComparable<PlayerLevelData>
	{
		public int Level;

		public int ExpTotal;

		[WorkbookFlat]
		public Stats BaseStats;

		public int CompareTo(PlayerLevelData other)
		{
			return Level.CompareTo(other.Level);
		}
	}

	[HideInInspector]
	public List<Dictionary<int, float>> itemsStatsCurve;

	[HideInInspector]
	public List<Dictionary<int, int>> itemsUpgradeCost;

	[HideInInspector]
	public List<Dictionary<int, int>> itemsLevelUpRequirements;

	[WorkbookLoad("ItemStatsCurve")]
	public List<ItemStatsCurveData> itemStatsCurveData;

	[WorkbookLoad("ItemLevelUpCost")]
	public List<ItemUpgradeCostData> itemsUpgradeCostData;

	[WorkbookLoad("ItemLevelUpRequirements")]
	public List<ItemUpgradeCostData> itemsLevelUpRequirementsData;

	[WorkbookLoad("StageBattles")]
	public List<StageBattlesData> stageBattlesData;

	[WorkbookLoad("Player")]
	public List<PlayerLevelData> playerLevelData;

	private void Start()
	{
		LoadItemsCurve();
		LoadItemLevelUpRequirements();
		LoadItemsUpgradeCost();
	}

	private void LoadItemsCurve()
	{
		itemsStatsCurve = new List<Dictionary<int, float>>();
		for (int i = 0; i < 4; i++)
		{
			itemsStatsCurve.Add(new Dictionary<int, float>());
		}
		foreach (ItemStatsCurveData itemStatsCurveDatum in itemStatsCurveData)
		{
			itemsStatsCurve[0].Add(itemStatsCurveDatum.Lvl, itemStatsCurveDatum.Common);
			itemsStatsCurve[1].Add(itemStatsCurveDatum.Lvl, itemStatsCurveDatum.Rare);
			itemsStatsCurve[2].Add(itemStatsCurveDatum.Lvl, itemStatsCurveDatum.Epic);
			itemsStatsCurve[3].Add(itemStatsCurveDatum.Lvl, itemStatsCurveDatum.Legendary);
		}
	}

	private void LoadItemsUpgradeCost()
	{
		itemsUpgradeCost = new List<Dictionary<int, int>>();
		for (int i = 0; i < 4; i++)
		{
			itemsUpgradeCost.Add(new Dictionary<int, int>());
		}
		foreach (ItemUpgradeCostData itemsUpgradeCostDatum in itemsUpgradeCostData)
		{
			itemsUpgradeCost[0].Add(itemsUpgradeCostDatum.Lvl, itemsUpgradeCostDatum.Common);
			itemsUpgradeCost[1].Add(itemsUpgradeCostDatum.Lvl, itemsUpgradeCostDatum.Rare);
			itemsUpgradeCost[2].Add(itemsUpgradeCostDatum.Lvl, itemsUpgradeCostDatum.Epic);
			itemsUpgradeCost[3].Add(itemsUpgradeCostDatum.Lvl, itemsUpgradeCostDatum.Legendary);
		}
	}

	private void LoadItemLevelUpRequirements()
	{
		itemsLevelUpRequirements = new List<Dictionary<int, int>>();
		for (int i = 0; i < 4; i++)
		{
			itemsLevelUpRequirements.Add(new Dictionary<int, int>());
		}
		foreach (ItemUpgradeCostData itemsLevelUpRequirementsDatum in itemsLevelUpRequirementsData)
		{
			itemsLevelUpRequirements[0].Add(itemsLevelUpRequirementsDatum.Lvl, itemsLevelUpRequirementsDatum.Common);
			itemsLevelUpRequirements[1].Add(itemsLevelUpRequirementsDatum.Lvl, itemsLevelUpRequirementsDatum.Rare);
			itemsLevelUpRequirements[2].Add(itemsLevelUpRequirementsDatum.Lvl, itemsLevelUpRequirementsDatum.Epic);
			itemsLevelUpRequirements[3].Add(itemsLevelUpRequirementsDatum.Lvl, itemsLevelUpRequirementsDatum.Legendary);
		}
	}
}
