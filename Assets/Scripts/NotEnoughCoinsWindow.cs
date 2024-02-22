using UnityEngine.UI;
using Utils;

public class NotEnoughCoinsWindow : Window
{
	public Button buyButton;

	public TextWithQuads priceText;

	private int requiredGems;

	private int coinsToBuy;

	protected override void Start()
	{
		base.Start();
		Helpers.AddListenerOnClick(buyButton, BuyCoins);
	}

	private void BuyCoins()
	{
		if (MetaResource.gems.count >= requiredGems)
		{
			MetaResource.gems.count -= requiredGems;
			MetaResource.coins.count += coinsToBuy;
			AnalyticsManager.ResourceSink(MetaResource.gems.analyticsID, requiredGems, "IAP", "NotEnoughCoins");
			AnalyticsManager.ResourceSource(MetaResource.coins.analyticsID, coinsToBuy, "IAP", "NotEnoughCoins");
			CloseWindow();
		}
		else
		{
			Singleton<WindowManager>.Instance.OpenWindow<GoToShopWindow>(null, OnGoToShopWindowClosed);
		}
	}

	private void OnGoToShopWindowClosed(Window window, object returnValue)
	{
		if (returnValue != null)
		{
			CloseWindow(true);
		}
	}

	protected virtual void OnEnable()
	{
		NotEnoughCoinsWindowContext notEnoughCoinsWindowContext = base.context as NotEnoughCoinsWindowContext;
		if (notEnoughCoinsWindowContext != null)
		{
			InitWithContext(notEnoughCoinsWindowContext);
		}
	}

	private void InitWithContext(NotEnoughCoinsWindowContext context)
	{
		requiredGems = context.requiredGems;
		coinsToBuy = context.coinsToBuy;
		Helpers.SetTextWithQuads(priceText, MetaResource.gems.quadText + requiredGems.ToString());
	}
}
