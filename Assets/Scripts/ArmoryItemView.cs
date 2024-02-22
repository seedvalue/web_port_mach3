using System.Collections;
using System.Linq;
using UnityEngine;
using Utils;

[RequireComponent(typeof(MetaItemViewSelectable))]
public class ArmoryItemView : MetaView
{
	public bool deselctOnAction;

	private MetaItemViewSelectable selectabeItemView;

	public new MetaItem GetObject()
	{
		return base.GetObject() as MetaItem;
	}

	protected override void Start()
	{
		base.Start();
		selectabeItemView = GetComponent<MetaItemViewSelectable>();
		Helpers.AddListenerOnClick(selectabeItemView.upgradeButton, OnUpgradeClicked);
		Helpers.AddListenerOnClick(selectabeItemView.viewButton, OnViewClicked);
		Helpers.AddListenerOnClick(selectabeItemView.useButton, OnUseClicked);
		Helpers.AddListenerOnClick(selectabeItemView.swapButton, OnSwapClicked);
	}

	public IEnumerator useItem()
	{
		float startTime = Time.time;
		ScrollRectTweener scroll = GetComponentInParent<ScrollRectTweener>();
		if ((bool)scroll)
		{
			scroll.ScrollVertical(1f, fast: true);
		}
		while (scroll.scrollRect.verticalNormalizedPosition < 0.99f)
		{
			yield return null;
		}
		while (Time.time < startTime + 0.2f)
		{
			yield return null;
		}
		MetaItem obj = GetObject();
		obj.InsertToSlot();
		yield return new WaitForEndOfFrame();
		Armory armory = GetComponentInParent<Armory>();
		if ((bool)armory)
		{
			armory.SelectSlot(obj.itemType);
		}
	}

	private void OnUseClicked()
	{
		if (deselctOnAction)
		{
			selectabeItemView.Deselect();
		}
		StopAllCoroutines();
		StartCoroutine(useItem());
	}

	private void OnViewClicked()
	{
		ItemWindowContext itemWindowContext = new ItemWindowContext();
		itemWindowContext.item = GetObject();
		itemWindowContext.armory = GetComponentInParent<Armory>();
		Singleton<WindowManager>.Instance.OpenWindow<ItemWindow>(itemWindowContext, OnItemWindowClosed);
	}

	private void OnUpgradeClicked()
	{
		OnViewClicked();
	}

	private void OnItemWindowClosed(Window window, object returnValue)
	{
	}

	private void OnSwapClicked()
	{
		MetaItem obj = GetObject();
		PickItemWindowContext pickItemWindowContext = new PickItemWindowContext();
		pickItemWindowContext.title = obj.displayName;
		pickItemWindowContext.items = (from x in Singleton<Meta>.Instance.FindObjects<MetaItem>()
			where x.itemType == obj.itemType
			where x.slot == null
			where x.found
			select x).ToList();
		Singleton<WindowManager>.Instance.OpenWindow<PickItemWindow>(pickItemWindowContext, OnSwap);
	}

	private void OnSwap(Window window, object returnValue)
	{
		MetaItem metaItem = returnValue as MetaItem;
		if (metaItem != null)
		{
			metaItem.InsertToSlot();
		}
		if (deselctOnAction)
		{
			selectabeItemView.Deselect();
		}
	}
}
