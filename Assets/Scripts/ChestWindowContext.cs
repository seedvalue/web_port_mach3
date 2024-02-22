using System;

public class ChestWindowContext
{
	public MetaChest chest;

	public int chestLevel = -1;

	public MetaChestContent content;

	public MetaItem chanceItem;

	public DateTime? openTime;

	public MetaRewardChestSlot slot;

	public ChestWindowState state = ChestWindowState.OpenNow;
}
