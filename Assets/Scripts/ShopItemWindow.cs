using System.Collections.Generic;
using UnityEngine.UI;
using Utils;

public class ShopItemWindow : MetaWindow
{
	public List<MetaItemView> itemIcons;

	public Text titleText;

	public Button buyButton;

	public Button buyAllButton;

	public Text priceText;

	public Text priceAllText;

	public Text cardsLeftText;

	public Text cardsLeftButtonAllText;

	private ShopItemView shopItemView;

	public ItemProgressBar levelBar;

	public new MetaItem GetObject()
	{
		return base.GetObject() as MetaItem;
	}

	[WindowTestMethod]
	public static void TestWindow()
	{
		MetaProduct context = Singleton<Meta>.Instance.FindRandomObject<MetaProduct>();
		Singleton<WindowManager>.Instance.OpenWindow<ItemWindow>(context);
	}

	protected virtual void OnEnable()
	{
		ShopItemWindowContext shopItemWindowContext = base.context as ShopItemWindowContext;
		if (shopItemWindowContext != null)
		{
			InitWithContext(shopItemWindowContext);
		}
	}

	protected override void Start()
	{
		base.Start();
		Helpers.AddListenerOnClick(buyButton, OnBuyClicked);
		Helpers.AddListenerOnClick(buyAllButton, OnBuyAllClicked);
	}

	private void InitWithContext(ShopItemWindowContext context)
	{
		SetObject(context.item);
		shopItemView = context.itemView;
	}

	protected override void OnChanged()
	{
		base.OnChanged();
		MetaItem @object = GetObject();
		Helpers.SetText(titleText, "Get " + @object.displayName);
		foreach (MetaItemView itemIcon in itemIcons)
		{
			itemIcon.SetObject(@object);
		}
		Helpers.SetText(cardsLeftText, "x" + (@object.toBuyCount - @object.purchasedCount).ToString());
		Helpers.SetText(cardsLeftButtonAllText, "Buy " + (@object.toBuyCount - @object.purchasedCount).ToString() + " cards");
		Helpers.SetTextWithQuads(priceAllText, MetaResource.coins.quadText + shopItemView.GetTotalPrice().ToString());
		Helpers.SetTextWithQuads(priceText, MetaResource.coins.quadText + shopItemView.GetPrice().ToString());
		if (@object.GetMaxItemLevel() == @object.level)
		{
			Helpers.SetActive(levelBar.gameObject, value: false);
		}
		else if ((bool)levelBar)
		{
			int cardsNumRequiredToUpdate = @object.GetCardsNumRequiredToUpdate();
			levelBar.SetProgress((float)@object.count / (float)cardsNumRequiredToUpdate, @object.count.ToString() + "/" + cardsNumRequiredToUpdate.ToString());
		}
	}

	private void OnBuyClicked()
	{
		if (MetaResource.coins.count < shopItemView.GetPrice())
		{
			NotEnoughCoinsWindowContext notEnoughCoinsWindowContext = new NotEnoughCoinsWindowContext();
			notEnoughCoinsWindowContext.coinsToBuy = shopItemView.GetPrice() - MetaResource.coins.count;
			notEnoughCoinsWindowContext.requiredGems = MetaProduct.CalculateGemPriceForCoins(notEnoughCoinsWindowContext.coinsToBuy);
			Singleton<WindowManager>.Instance.OpenWindow<NotEnoughCoinsWindow>(notEnoughCoinsWindowContext, OnNotEnoughCoinsWindowClosed);
			return;
		}
		shopItemView.BuyOneCard();
		MetaItem @object = GetObject();
		if (@object.toBuyCount - @object.purchasedCount > 0)
		{
			OnChanged();
		}
		else
		{
			CloseWindow();
		}
	}

	private void OnBuyAllClicked()
	{
		if (MetaResource.coins.count < shopItemView.GetTotalPrice())
		{
			NotEnoughCoinsWindowContext notEnoughCoinsWindowContext = new NotEnoughCoinsWindowContext();
			notEnoughCoinsWindowContext.coinsToBuy = shopItemView.GetTotalPrice() - MetaResource.coins.count;
			notEnoughCoinsWindowContext.requiredGems = MetaProduct.CalculateGemPriceForCoins(notEnoughCoinsWindowContext.coinsToBuy);
			Singleton<WindowManager>.Instance.OpenWindow<NotEnoughCoinsWindow>(notEnoughCoinsWindowContext, OnNotEnoughCoinsWindowClosed);
		}
		else
		{
			shopItemView.BuyAllCards();
			CloseWindow();
		}
	}

	public void OnNotEnoughCoinsWindowClosed(Window window, object returnValue)
	{
		if (returnValue != null)
		{
			CloseWindow();
		}
	}
}
