using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public class MetaItemSlotView : MetaView, IPointerClickHandler, IPointerDownHandler, IEventSystemHandler
{
	public Image iconImage;

	public Image backgroundImage;

	protected MetaItemView itemView;

	public MetaItemView itemViewPrefab;

	public HeroPresentation heroPresentation;

	protected override void Start()
	{
		base.Start();
	}

	public virtual void MetaStart()
	{
	}

	public new MetaItemSlot GetObject()
	{
		return base.GetObject() as MetaItemSlot;
	}

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
	}

	protected override void OnChanged()
	{
		base.OnChanged();
		MetaItemSlot @object = GetObject();
		Helpers.SetImage(iconImage, @object.icon);
		if ((bool)backgroundImage)
		{
			backgroundImage.enabled = (@object.item == null);
		}
		if ((bool)iconImage)
		{
			iconImage.gameObject.SetActive(@object.item == null);
		}
		if ((bool)@object.item)
		{
			if (!itemView)
			{
				itemView = Object.Instantiate(itemViewPrefab, base.transform);
				itemView.transform.SetAsFirstSibling();
				itemView.transform.localPosition = Vector3.zero;
				MetaItemViewSelectable component = itemView.GetComponent<MetaItemViewSelectable>();
				Helpers.AddListenerOnClick(component.upgradeButton, OnUpgradeClicked);
				Helpers.AddListenerOnClick(component.viewButton, OnViewClicked);
				Helpers.AddListenerOnClick(component.swapButton, OnSwapClicked);
				component.onSelect.AddListener(OnSelect);
				component.onDeselect.AddListener(OnDeselect);
			}
			itemView.gameObject.SetActive(value: true);
			MetaView[] components = itemView.GetComponents<MetaView>();
			MetaView[] array = components;
			foreach (MetaView metaView in array)
			{
				metaView.SetObject(@object.item);
			}
		}
		else if ((bool)itemView)
		{
			itemView.gameObject.SetActive(value: false);
		}
	}

	public void Select()
	{
		MetaItemViewSelectable component = itemView.GetComponent<MetaItemViewSelectable>();
		if ((bool)component)
		{
			component.Select();
		}
	}

	private void OnSelect()
	{
		if ((bool)heroPresentation)
		{
			heroPresentation.SlotSelected(GetObject().acceptedItemType);
		}
	}

	private void OnDeselect()
	{
		if ((bool)heroPresentation)
		{
			heroPresentation.SlotDeselected();
		}
	}

	private void OnSwapContent(Window window, object returnValue)
	{
		MetaItem metaItem = returnValue as MetaItem;
		if (metaItem != null)
		{
			MetaItemSlot @object = GetObject();
			@object.item = metaItem;
		}
	}

	private void OnViewClicked()
	{
		MetaItemSlot @object = GetObject();
		ItemWindowContext itemWindowContext = new ItemWindowContext();
		itemWindowContext.item = @object.item;
		itemWindowContext.armory = GetComponentInParent<Armory>();
		Singleton<WindowManager>.Instance.OpenWindow<ItemWindow>(itemWindowContext);
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
		MetaItemSlot @object = GetObject();
		Armory componentInParent = GetComponentInParent<Armory>();
		if ((bool)componentInParent)
		{
			componentInParent.ScrollToItems(@object.acceptedItemType);
		}
		MetaItemViewSelectable.TryDeselect();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		MetaItemViewSelectable.TryDeselect();
		OnSwapClicked();
		AudioManager.Play(AudioMechanics.Click);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}
}
