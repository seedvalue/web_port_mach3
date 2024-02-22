using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class Armory : MonoBehaviour
{
	public MetaItemView itemPrefab;

	public MetaItemView itemToBeFoundPrefab;

	public ArmoryItemContainer itemContainerPrefab;

	public RectTransform itemContainerLabelPrefab;

	public GameObject contentNode;

	private ArmoryOrder currentOrder;

	private Dictionary<MetaItemType, ArmoryItemContainer> itemContainers;

	private Dictionary<MetaItemType, MetaItemSlotView> slotsView;

	private bool needRebuild;

	public ArmoryItemContainer GetItemContainer(MetaItemType slot)
	{
		if (itemContainers.ContainsKey(slot))
		{
			return itemContainers[slot];
		}
		return null;
	}

	public void SelectSlot(MetaItemType slot)
	{
		MetaItemSlotView metaItemSlotView = slotsView[slot];
		if (metaItemSlotView != null)
		{
			metaItemSlotView.Select();
		}
	}

	private void PrepareArmory(MetaItemType slot, ArmoryOrder order)
	{
		string text = string.Empty;
		if (contentNode == null || itemContainerLabelPrefab == null || itemContainerPrefab == null)
		{
			return;
		}
		ArmoryItemContainer armoryItemContainer = null;
		if (!itemContainers.ContainsKey(slot))
		{
			switch (slot)
			{
			case MetaItemType.Back:
				text = "Back";
				break;
			case MetaItemType.Companion:
				text = "Companion";
				break;
			case MetaItemType.Head:
				text = "Head";
				break;
			case MetaItemType.MainHand:
				text = "MainHand";
				break;
			case MetaItemType.OffHand:
				text = "OffHand";
				break;
			case MetaItemType.Torso:
				text = "Torso";
				break;
			}
			RectTransform rectTransform = UnityEngine.Object.Instantiate(itemContainerLabelPrefab, contentNode.transform);
			rectTransform.transform.localScale = Vector3.one;
			Text componentInChildren = rectTransform.GetComponentInChildren<Text>();
			Helpers.SetText(componentInChildren, "Items collection: " + text);
			armoryItemContainer = UnityEngine.Object.Instantiate(itemContainerPrefab, contentNode.transform);
			armoryItemContainer.transform.localScale = Vector3.one;
			armoryItemContainer.name = text;
			itemContainers.Add(slot, armoryItemContainer);
		}
		else
		{
			armoryItemContainer = itemContainers[slot];
			armoryItemContainer.ResetItemContainer();
		}
		List<MetaItem> source = (from x in Singleton<Meta>.Instance.FindObjects<MetaItem>()
			where x.itemType == slot
			where x.slot == null
			select x).ToList();
		List<MetaItem> list = new List<MetaItem>();
		switch (order)
		{
		case ArmoryOrder.Rarity:
			list = (from o in source
				orderby o.rarity
				select o into x
				orderby x.found descending
				orderby x.available descending
				select x).ToList();
			break;
		case ArmoryOrder.Level:
			list = (from o in source
				orderby o.level
				select o into x
				orderby x.found descending
				orderby x.available descending
				select x).ToList();
			break;
		}
		foreach (MetaItem item in list)
		{
			if (item.found)
			{
				armoryItemContainer.Add(item, itemPrefab);
			}
			else
			{
				armoryItemContainer.Add(item, itemToBeFoundPrefab);
			}
		}
		armoryItemContainer.InitializeItemContainer();
	}

	private void Start()
	{
		itemContainers = new Dictionary<MetaItemType, ArmoryItemContainer>();
		slotsView = new Dictionary<MetaItemType, MetaItemSlotView>();
		MetaItemSlotView[] componentsInChildren = GetComponentsInChildren<MetaItemSlotView>();
		MetaItemSlotView[] array = componentsInChildren;
		foreach (MetaItemSlotView metaItemSlotView in array)
		{
			slotsView.Add(metaItemSlotView.GetObject().acceptedItemType, metaItemSlotView);
		}
		List<MetaItem> list = Singleton<Meta>.Instance.FindObjects<MetaItem>().ToList();
		foreach (MetaItem item in list)
		{
			Singleton<Meta>.Instance.AddTypePropertyListener(typeof(MetaItem), "found", OnMetaItemFound);
			Singleton<Meta>.Instance.AddTypePropertyListener(typeof(MetaItem), "slot", OnItemChange);
		}
		currentOrder = ArmoryOrder.Rarity;
		Rebuild();
		FullScreenCanvas componentInParent = GetComponentInParent<FullScreenCanvas>();
		componentInParent.onLostFocus.AddListener(OnLostFocus);
		componentInParent.onGainFocus.AddListener(OnGainFocus);
		componentInParent.onEnable.AddListener(OnCanvasEnable);
	}

	private void OnLostFocus()
	{
		ScrollRectTweener componentInChildren = GetComponentInChildren<ScrollRectTweener>();
		if ((bool)componentInChildren)
		{
			ScrollRect component = componentInChildren.GetComponent<ScrollRect>();
			component.verticalNormalizedPosition = 1f;
		}
	}

	private void OnGainFocus()
	{
		ArmoryComponent instance = SingletonComponent<Meta, ArmoryComponent>.Instance;
		if ((bool)instance && instance.scrollArmory)
		{
			ScrollRectTweener componentInChildren = GetComponentInChildren<ScrollRectTweener>();
			if ((bool)componentInChildren)
			{
				componentInChildren.ScrollVertical(1f, fast: false);
			}
			instance.scrollArmory = false;
		}
	}

	private void OnCanvasEnable()
	{
		ArmoryComponent instance = SingletonComponent<Meta, ArmoryComponent>.Instance;
		if ((bool)instance && instance.scrollArmory)
		{
			ScrollRectTweener componentInChildren = GetComponentInChildren<ScrollRectTweener>();
			if ((bool)componentInChildren)
			{
				ScrollRect component = componentInChildren.GetComponent<ScrollRect>();
				component.verticalNormalizedPosition = 0f;
			}
		}
	}

	private void OnDestroy()
	{
		if (Singleton<Meta>.HasInstance)
		{
			Singleton<Meta>.Instance.RemTypePropertyListener(typeof(MetaItem), "count", OnMetaItemFound);
		}
	}

	private void OnMetaItemFound(MetaObject item, string propoerty)
	{
		needRebuild = true;
	}

	private void OnItemChange(MetaObject item, string propoerty)
	{
		needRebuild = true;
	}

	private void Rebuild()
	{
		PrepareArmory(MetaItemType.Head, currentOrder);
		PrepareArmory(MetaItemType.Back, currentOrder);
		PrepareArmory(MetaItemType.Torso, currentOrder);
		PrepareArmory(MetaItemType.MainHand, currentOrder);
		PrepareArmory(MetaItemType.OffHand, currentOrder);
		PrepareArmory(MetaItemType.Companion, currentOrder);
		needRebuild = false;
	}

	private void Update()
	{
		if (needRebuild)
		{
			Rebuild();
		}
	}

	public void OrderItems()
	{
		currentOrder++;
		if (currentOrder > ArmoryOrder.Level)
		{
			currentOrder = ArmoryOrder.Rarity;
		}
		foreach (KeyValuePair<MetaItemType, ArmoryItemContainer> itemContainer in itemContainers)
		{
			if (itemContainer.Value != null)
			{
				itemContainer.Value.OrderItems(currentOrder);
			}
		}
		ArmorySortButton[] componentsInChildren = GetComponentsInChildren<ArmorySortButton>();
		ArmorySortButton[] array = componentsInChildren;
		foreach (ArmorySortButton armorySortButton in array)
		{
			switch (currentOrder)
			{
			case ArmoryOrder.Rarity:
				armorySortButton.SetLabel("by Rarity");
				break;
			case ArmoryOrder.Level:
				armorySortButton.SetLabel("by Level");
				break;
			}
		}
	}

	public void ScrollToItems(MetaItemType itemType)
	{
		ScrollRectTweener componentInChildren = GetComponentInChildren<ScrollRectTweener>();
		if (!(componentInChildren == null))
		{
			ArmoryItemContainer itemContainer = GetItemContainer(itemType);
			componentInChildren.Scroll(itemContainer.GetComponent<RectTransform>(), fast: true);
		}
	}

	public void ScrollToTop()
	{
		ScrollRectTweener componentInChildren = GetComponentInChildren<ScrollRectTweener>();
		if (!(componentInChildren == null))
		{
			componentInChildren.ScrollVertical(1f, fast: true);
		}
	}

	public void UseAndSelectItem(MetaItem item)
	{
		StartCoroutine(useItem(item));
	}

	public IEnumerator useItem(MetaItem item)
	{
		float startTime = Time.time;
		ScrollRectTweener scroll = GetComponentInChildren<ScrollRectTweener>();
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
		item.InsertToSlot();
		yield return new WaitForEndOfFrame();
		SelectSlot(item.itemType);
	}
}
