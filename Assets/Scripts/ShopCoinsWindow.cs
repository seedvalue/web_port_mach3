using UnityEngine.UI;
using Utils;

public class ShopCoinsWindow : MetaWindow
{
	public Image iconImage;

	public Text titleText;

	public Text descText;

	public Button buyButton;

	public Text priceText;

	public new MetaProduct GetObject()
	{
		return base.GetObject() as MetaProduct;
	}

	[WindowTestMethod]
	public static void TestWindow()
	{
		MetaProduct context = Singleton<Meta>.Instance.FindRandomObject<MetaProduct>();
		Singleton<WindowManager>.Instance.OpenWindow<ItemWindow>(context);
	}

	protected virtual void OnEnable()
	{
		MetaProduct metaProduct = base.context as MetaProduct;
		if (metaProduct != null)
		{
			InitWithContext(metaProduct);
		}
	}

	protected override void Start()
	{
		base.Start();
		Helpers.AddListenerOnClick(buyButton, OnBuyClicked);
	}

	private void InitWithContext(MetaProduct product)
	{
		SetObject(product);
	}

	protected override void OnChanged()
	{
		base.OnChanged();
		MetaProduct @object = GetObject();
		Helpers.SetText(titleText, "Get " + @object.displayName);
		Helpers.SetText(descText, "And get " + @object.value.ToString() + " gold");
		Helpers.SetTextWithQuads(priceText, MetaResource.gems.quadText + " " + @object.price.ToString());
		Helpers.SetImage(iconImage, @object.icon);
	}

	private void OnBuyClicked()
	{
		MetaProduct @object = GetObject();
		if (MetaResource.gems.count >= @object.price)
		{
			MetaResource.gems.count -= @object.price;
			MetaResource.coins.count += @object.value;
			AnalyticsManager.ResourceSink(MetaResource.gems.analyticsID, @object.price, @object);
			AnalyticsManager.ResourceSource(MetaResource.coins.analyticsID, @object.value, @object);
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
			CloseWindow();
		}
	}
}
