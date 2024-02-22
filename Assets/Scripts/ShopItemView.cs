using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public class ShopItemView : MetaView, IPointerClickHandler, IPointerDownHandler, IEventSystemHandler
{
	public Text priceText;

	public Material soldOutMaterial;

	public Image buyButtonBkg;

	public new MetaItem GetObject()
	{
		return base.GetObject() as MetaItem;
	}

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
		MetaItem @object = GetObject();
		if (@object.toBuyCount == @object.purchasedCount)
		{
			Helpers.SetImageMaterial(buyButtonBkg, soldOutMaterial);
			Helpers.SetTextWithQuads(priceText, "SOLD OUT");
		}
		else
		{
			Helpers.SetTextWithQuads(priceText, MetaResource.coins.quadText + " " + GetPrice().ToString());
			Helpers.SetImageMaterial(buyButtonBkg, null);
		}
	}

	protected override void OnInteract()
	{
		base.OnInteract();
		MetaItem @object = GetObject();
		if (@object.toBuyCount - @object.purchasedCount > 0)
		{
			ShopItemWindowContext shopItemWindowContext = new ShopItemWindowContext();
			shopItemWindowContext.item = GetObject();
			shopItemWindowContext.itemView = this;
			Singleton<WindowManager>.Instance.OpenWindow<ShopItemWindow>(shopItemWindowContext);
		}
	}

	public int GetPrice()
	{
		MetaItem @object = GetObject();
		return @object.firstPurchasePrice * (@object.purchasedCount + 1);
	}

	public int GetTotalPrice()
	{
		MetaItem @object = GetObject();
		int price = GetPrice();
		int num = @object.firstPurchasePrice * @object.toBuyCount;
		return (price + num) * (@object.toBuyCount - @object.purchasedCount) / 2;
	}

	protected override void OnChanged()
	{
		base.OnChanged();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OnInteract();
	}

	public void BuyOneCard()
	{
		MetaItem @object = GetObject();
		int price = GetPrice();
		MetaResource.coins.count -= price;
		@object.purchasedCount++;
		@object.count++;
		AnalyticsManager.ResourceSink(MetaResource.coins.analyticsID, price, "ItemBuy", @object.analyticsID);
		Helpers.SetTextWithQuads(priceText, MetaResource.coins.quadText + " " + GetPrice().ToString());
		if (@object.toBuyCount == 0)
		{
			Helpers.SetTextWithQuads(priceText, "SOLD OUT");
			Helpers.SetImageMaterial(buyButtonBkg, soldOutMaterial);
		}
	}

	public void BuyAllCards()
	{
		MetaItem @object = GetObject();
		int totalPrice = GetTotalPrice();
		MetaResource.coins.count -= totalPrice;
		@object.count += @object.toBuyCount - @object.purchasedCount;
		@object.purchasedCount = @object.toBuyCount;
		AnalyticsManager.ResourceSink(MetaResource.coins.analyticsID, totalPrice, "ItemBuy", @object.analyticsID);
		Helpers.SetTextWithQuads(priceText, "SOLD OUT");
		Helpers.SetImageMaterial(buyButtonBkg, soldOutMaterial);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}
}
