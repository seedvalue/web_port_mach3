using UnityEngine;
using UnityEngine.UI;
using Utils;

public class MetaVictoryChestSlotView : MetaView
{
	[Header("Meta Victory Chest Slot")]
	public GameObject disabledGroup;

	public GameObject enabledGroup;

	public GameObject openGroup;

	public Image progressBar;

	public Text progressText;

	public TimeText currentEnableTime;

	public new MetaVictoryChestSlot GetObject()
	{
		return base.GetObject() as MetaVictoryChestSlot;
	}

	protected override void OnInteract()
	{
		MetaVictoryChestSlot @object = GetObject();
		if (@object.state == MetaVictoryChestSlotState.WithChest)
		{
			OpenChestWindowContext openChestWindowContext = new OpenChestWindowContext();
			openChestWindowContext.chest = @object.connectedChest;
			Singleton<WindowManager>.Instance.OpenWindow<OpenChestWindow>(openChestWindowContext);
			@object.state = MetaVictoryChestSlotState.Disabled;
		}
	}

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
	}

	protected override void OnChanged()
	{
		base.OnChanged();
		MetaVictoryChestSlot @object = GetObject();
		Helpers.SetActive(disabledGroup, @object.state == MetaVictoryChestSlotState.Disabled);
		Helpers.SetActive(enabledGroup, @object.state == MetaVictoryChestSlotState.Enabled);
		Helpers.SetActive(openGroup, @object.state == MetaVictoryChestSlotState.WithChest);
		Helpers.SetTime(currentEnableTime, @object.nextEnableTime, countdown: true);
		Helpers.SetText(progressText, @object.currentStars + "/" + @object.requiredStars);
		if ((bool)progressBar)
		{
			progressBar.fillAmount = (float)@object.currentStars / (float)@object.requiredStars;
		}
	}
}
