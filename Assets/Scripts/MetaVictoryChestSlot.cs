using System;
using System.Linq;
using Utils;

public class MetaVictoryChestSlot : MetaChestSlot
{
	public MetaVictoryChest connectedChest;

	[WorkbookAlias("Value")]
	public int requiredStars = 10;

	[WorkbookAlias("TimeHours")]
	public float enableHours = 24f;

	public const string lastEnableTimeProperty = "lastEnableTime";

	public const string currentStarsProperty = "currentStars";

	public const string stateProperty = "state";

	[MetaData(null, 0)]
	private DateTime? _lastEnableTime;

	[MetaData(null, 0)]
	private int _currentStars;

	[MetaData(null, 0)]
	private MetaVictoryChestSlotState _state;

	private static MetaVictoryChestSlot[] allSlots;

	public DateTime? nextEnableTime
	{
		get
		{
			DateTime? result;
			if (!this.lastEnableTime.HasValue)
			{
				result = null;
			}
			else
			{
				DateTime? lastEnableTime = this.lastEnableTime;
				result = ((!lastEnableTime.HasValue) ? null : new DateTime?(lastEnableTime.GetValueOrDefault() + TimeSpan.FromHours(enableHours)));
			}
			return result;
		}
	}

	public DateTime? lastEnableTime
	{
		get
		{
			return _lastEnableTime;
		}
		private set
		{
			PropertySetter(ref _lastEnableTime, value, "lastEnableTime");
		}
	}

	public int currentStars
	{
		get
		{
			return _currentStars;
		}
		set
		{
			PropertySetter(ref _currentStars, value, "currentStars");
		}
	}

	public MetaVictoryChestSlotState state
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

	protected static void MetaStaticAwake()
	{
		allSlots = Singleton<Meta>.Instance.FindObjects<MetaVictoryChestSlot>();
	}

	protected virtual void MetaUpdate()
	{
		if (state != 0)
		{
			return;
		}
		if (this.nextEnableTime.HasValue)
		{
			DateTime? nextEnableTime = this.nextEnableTime;
			if (!nextEnableTime.HasValue || !(nextEnableTime.GetValueOrDefault() <= SingletonComponent<Meta, MetaTimeManager>.Instance.utcNow))
			{
				return;
			}
		}
		state = MetaVictoryChestSlotState.Enabled;
	}

	protected override void OnPropertyChanged(string propertyName, object before, object after)
	{
		base.OnPropertyChanged(propertyName, before, after);
		if (propertyName == "state")
		{
			if (state == MetaVictoryChestSlotState.Enabled)
			{
				lastEnableTime = SingletonComponent<Meta, MetaTimeManager>.Instance.utcNow;
			}
			else if (state == MetaVictoryChestSlotState.WithChest)
			{
				currentStars = 0;
			}
		}
		else if (propertyName == "currentStars" && currentStars >= requiredStars)
		{
			state = MetaVictoryChestSlotState.WithChest;
		}
	}

	public static void AddStars(int count)
	{
		foreach (MetaVictoryChestSlot item in from s in allSlots
			where s.state == MetaVictoryChestSlotState.Enabled
			select s)
		{
			item.currentStars += count;
		}
	}
}
