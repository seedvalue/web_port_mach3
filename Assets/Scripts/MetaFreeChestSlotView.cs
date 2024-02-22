using UnityEngine;
using Utils;

public class MetaFreeChestSlotView : MetaView
{
	[Header("Meta Free Chest Slot")]
	public GameObject emptyGroup;

	public GameObject openGroup;

	public GameObject[] grayChests;

	public GameObject[] normalChests;

	public TimeText currentOpenTime;

	public new MetaFreeChestSlot GetObject()
	{
		return base.GetObject() as MetaFreeChestSlot;
	}

	protected override void OnInteract()
	{
		MetaFreeChestSlot @object = GetObject();
		if (@object.chestCount > 0)
		{
			OpenChestWindowContext openChestWindowContext = new OpenChestWindowContext();
			openChestWindowContext.chest = @object.connectedChest;
			Singleton<WindowManager>.Instance.OpenWindow<OpenChestWindow>(openChestWindowContext);
			@object.chestCount--;
		}
	}

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
	}

	protected override void OnChanged()
	{
		base.OnChanged();
		MetaFreeChestSlot @object = GetObject();
		Helpers.SetActive(emptyGroup, @object.chestCount <= 0);
		Helpers.SetActive(openGroup, @object.chestCount > 0);
		for (int i = 0; i < grayChests.Length; i++)
		{
			Helpers.SetActive(grayChests[i], @object.chestCount < i + 1);
		}
		for (int j = 0; j < normalChests.Length; j++)
		{
			Helpers.SetActive(normalChests[j], @object.chestCount >= j + 1);
		}
		Helpers.SetTime(currentOpenTime, @object.nextChestTime, countdown: true);
	}
}
