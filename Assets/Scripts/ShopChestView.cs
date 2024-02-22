using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public class ShopChestView : MetaChestView, IPointerClickHandler, IPointerDownHandler, IEventSystemHandler
{
	public Text displayText;

	public Text priceText;

	public Image iconImage;

	public new MetaChest GetObject()
	{
		return base.GetObject();
	}

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
		MetaChest @object = GetObject();
		Helpers.SetText(displayText, @object.displayName);
		Helpers.SetImage(iconImage, @object.icon);
	}

	protected override void OnChanged()
	{
		base.OnChanged();
		MetaChest @object = GetObject();
		Helpers.SetTextWithQuads(priceText, MetaResource.gems.quadText + " " + @object.levelData.PriceGems.ToString());
	}

	protected override void OnInteract()
	{
		base.OnInteract();
		ChestWindowContext chestWindowContext = new ChestWindowContext();
		chestWindowContext.chest = GetObject();
		chestWindowContext.state = ChestWindowState.BuyFromShop;
		Singleton<WindowManager>.Instance.OpenWindow<ChestWindow>(chestWindowContext);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OnInteract();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}
}
