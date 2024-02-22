using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using Utils;

[WorkbookLoad("IAPs")]
[WorkbookAssetPath("Products")]
public class MetaProduct : MetaObject, IPurchaseHandler, IAnalyticsItem
{
	public const string activeProperty = "active";

	public const string priceTextProperty = "priceText";

	public string displayName;

	public Sprite icon;

	public MetaProductType productType;

	public int price;

	public int value;

	public string iAPID;

	private bool _active;

	private string _priceText;

	private static MetaProduct[] coinProducts;

	public string productID => iAPID;

	public bool active
	{
		get
		{
			return _active;
		}
		private set
		{
			PropertySetter(ref _active, value, "active");
		}
	}

	public string priceText
	{
		get
		{
			return _priceText;
		}
		private set
		{
			PropertySetter(ref _priceText, value, "priceText");
		}
	}

	public string analyticsID
	{
		get;
		private set;
	}

	public string analyticsType
	{
		get;
		private set;
	}

	protected virtual void Awake()
	{
		analyticsID = AnalyticsManager.ResolveID(base.metaID);
		analyticsType = AnalyticsManager.ResolveType(base.metaID);
	}

	protected static void MetaStaticAwake()
	{
		coinProducts = (from p in Singleton<Meta>.Instance.FindObjects<MetaProduct>()
			where p.productType == MetaProductType.Coins
			orderby p.value
			select p).ToArray();
	}

	public static int CalculateGemPriceForCoins(int requiredCoins)
	{
		if (coinProducts[0].value >= requiredCoins)
		{
			return Mathf.CeilToInt((float)coinProducts[0].price / (float)coinProducts[0].value * (float)requiredCoins);
		}
		for (int i = 1; i < coinProducts.Length; i++)
		{
			MetaProduct metaProduct = coinProducts[i];
			if (metaProduct.value >= requiredCoins)
			{
				MetaProduct metaProduct2 = coinProducts[i - 1];
				float a = (float)metaProduct2.price / (float)metaProduct2.value;
				float b = (float)metaProduct.price / (float)metaProduct.value;
				float t = Mathf.InverseLerp(metaProduct2.value, metaProduct.value, requiredCoins);
				float num = Mathf.Lerp(a, b, t);
				return Mathf.CeilToInt(num * (float)requiredCoins);
			}
		}
		return Mathf.CeilToInt((float)coinProducts[coinProducts.Length - 1].price / (float)coinProducts[coinProducts.Length - 1].value * (float)requiredCoins);
	}

	public void Purchase()
	{
		if (productType == MetaProductType.Coins)
		{
			Singleton<WindowManager>.Instance.OpenWindow<ShopCoinsWindow>(this);
		}
		else if (productType == MetaProductType.Gems)
		{
			PurchaseManager.Purchase(iAPID);
		}
	}

	public void InitProduct(Product product)
	{
		priceText = product.metadata.localizedPriceString;
		active = true;
	}

	public void HandlePurchase(Product product)
	{
		MetaResource.gems.count += value;
		Singleton<Meta>.Instance.ForceSave();
		AnalyticsManager.Business(productType.ToString(), analyticsID, "Shop", product);
		AnalyticsManager.ResourceSource(MetaResource.gems.analyticsID, value, this);
	}

	protected virtual void MetaStart()
	{
		if (productType == MetaProductType.Coins)
		{
			priceText = MetaResource.gems.quadText + " " + price.ToString();
			active = true;
		}
		else if (productType == MetaProductType.Gems)
		{
			PurchaseManager.AddHandler(this);
		}
	}

	protected virtual void OnDisable()
	{
		if (productType == MetaProductType.Gems)
		{
			PurchaseManager.RemHandler(this);
		}
	}
}
