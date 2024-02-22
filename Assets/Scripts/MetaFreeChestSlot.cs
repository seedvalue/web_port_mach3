using System;
using UnityEngine;
using Utils;

public class MetaFreeChestSlot : MetaChestSlot
{
	public MetaFreeChest connectedChest;

	[WorkbookAlias("Value")]
	public int maxChests = 2;

	[WorkbookAlias("TimeHours")]
	public float generationHours = 1f;

	public const string lastChestTimeProperty = "lastChestTime";

	public const string chestCountProperty = "chestCount";

	[MetaData(null, 0)]
	private DateTime? _lastChestTime;

	[MetaData(null, 0)]
	private int _chestCount;

	public DateTime? nextChestTime
	{
		get
		{
			DateTime? result;
			if (!this.lastChestTime.HasValue)
			{
				result = null;
			}
			else
			{
				DateTime? lastChestTime = this.lastChestTime;
				result = ((!lastChestTime.HasValue) ? null : new DateTime?(lastChestTime.GetValueOrDefault() + TimeSpan.FromHours(generationHours)));
			}
			return result;
		}
	}

	public DateTime? lastChestTime
	{
		get
		{
			return _lastChestTime;
		}
		private set
		{
			PropertySetter(ref _lastChestTime, value, "lastChestTime");
		}
	}

	public int chestCount
	{
		get
		{
			return _chestCount;
		}
		set
		{
			PropertySetter(ref _chestCount, value, "chestCount");
		}
	}

	protected virtual void MetaAwake()
	{
	}

	protected virtual void MetaUpdate()
	{
		int num = maxChests - chestCount;
		if (num <= 0)
		{
			return;
		}
		DateTime utcNow = SingletonComponent<Meta, MetaTimeManager>.Instance.utcNow;
		DateTime? lastChestTime = _lastChestTime;
		if (!lastChestTime.HasValue)
		{
			_lastChestTime = utcNow;
			return;
		}
		int num2 = Mathf.FloorToInt((float)(utcNow - this.lastChestTime.Value).TotalHours / generationHours);
		if (num2 >= num)
		{
			chestCount = maxChests;
			this.lastChestTime = null;
		}
		else if (num2 > 0)
		{
			chestCount += num2;
			DateTime? lastChestTime2 = this.lastChestTime;
			this.lastChestTime = ((!lastChestTime2.HasValue) ? null : new DateTime?(lastChestTime2.GetValueOrDefault() + TimeSpan.FromHours(generationHours * (float)num2)));
		}
	}
}
