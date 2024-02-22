using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class MetaChestSequence : MetaComponent<MetaChestSequence>
{
	[Serializable]
	public class SequenceData
	{
		public MetaRewardChest Chest;
	}

	[Serializable]
	private class OverrideData
	{
		public int index;

		public MetaRewardChest chest;

		public OverrideData(int index, MetaRewardChest chest)
		{
			this.index = index;
			this.chest = chest;
		}
	}

	[MetaData(-1, 0)]
	private int current;

	[MetaData(-1, 0)]
	private int count;

	[WorkbookLoad("ChestSequence")]
	public List<SequenceData> sequence;

	[MetaData(null, 0)]
	private List<OverrideData> overrides;

	public MetaRewardChest nextReward
	{
		get;
		private set;
	}

	public MetaRewardChest GetReward()
	{
		MetaRewardChest nextReward = this.nextReward;
		current++;
		if (current >= count)
		{
			GenerateSequence();
		}
		GenerateNextReward();
		return nextReward;
	}

	protected virtual void MetaStart()
	{
		if (overrides == null)
		{
			overrides = new List<OverrideData>();
		}
		if (!IsValid())
		{
			GenerateSequence();
		}
		GenerateNextReward();
	}

	private void GenerateNextReward()
	{
		MetaRewardChest metaRewardChest = (from d in overrides
			where d.index == current
			select d.chest).FirstOrDefault();
		if ((bool)metaRewardChest)
		{
			nextReward = metaRewardChest;
		}
		else
		{
			nextReward = sequence[current].Chest;
		}
	}

	private void GenerateSequence()
	{
		current = 0;
		count = sequence.Count;
		List<MetaRewardChest> list = new List<MetaRewardChest>();
		MetaRewardChest[] array = Singleton<Meta>.Instance.FindObjects<MetaRewardChest>();
		foreach (MetaRewardChest metaRewardChest in array)
		{
			metaRewardChest.currentForcedInSequence += metaRewardChest.forcedInSequence;
			int num = Mathf.FloorToInt(metaRewardChest.currentForcedInSequence);
			metaRewardChest.currentForcedInSequence -= num;
			for (int j = 0; j < num; j++)
			{
				list.Add(metaRewardChest);
			}
		}
		if (list.Count > count)
		{
			UnityEngine.Debug.LogError("To many overriden Chests in sequence!");
			list.RemoveRange(count, list.Count - count);
		}
		while (list.Count < count)
		{
			list.Add(null);
		}
		Rand.Shuffle(list);
		overrides.Clear();
		for (int k = 0; k < list.Count; k++)
		{
			if (!(list[k] == null))
			{
				overrides.Add(new OverrideData(k, list[k]));
			}
		}
	}

	private bool IsValid()
	{
		return count == sequence.Count;
	}
}
