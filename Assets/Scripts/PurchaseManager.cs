using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using Utils;

public class PurchaseManager : Singleton<PurchaseManager>, IStoreListener
{
	private class ProductData
	{
		public Product product;

		public IPurchaseHandler handler;

		public bool purchased;
	}

	private IStoreController controller;

	private readonly Dictionary<string, ProductData> products = new Dictionary<string, ProductData>();

	public static void Purchase(string productID)
	{
		Singleton<PurchaseManager>.Instance.InvokePurchase(productID);
	}

	public static void AddHandler(IPurchaseHandler handler)
	{
		Singleton<PurchaseManager>.Instance.InvokeAddHandler(handler);
	}

	public static void RemHandler(IPurchaseHandler handler)
	{
		if (Singleton<PurchaseManager>.HasInstance)
		{
			Singleton<PurchaseManager>.Instance.InvokeRemHandler(handler);
		}
	}

	private void InvokePurchase(string productID)
	{
		if (controller != null)
		{
			ProductData productData = FindData(productID);
			if (productData != null && productData.product != null)
			{
				controller.InitiatePurchase(productData.product);
			}
		}
	}

	private void InvokeAddHandler(IPurchaseHandler handler)
	{
		if (handler == null)
		{
			throw new ArgumentNullException("handler");
		}
		ProductData productData = FindData(handler.productID);
		if (productData == null)
		{
			UnityEngine.Debug.LogErrorFormat("PurchaseManager unable to find product '{0}'", handler.productID);
		}
		else
		{
			HandleDataHandler(productData, handler);
		}
	}

	private void InvokeRemHandler(IPurchaseHandler handler)
	{
		ProductData productData = FindData(handler.productID);
		if (productData != null)
		{
			HandleDataHandler(productData, null);
		}
	}

	protected virtual void Awake()
	{
		ProductCatalog productCatalog = ProductCatalog.LoadDefaultCatalog();
		StandardPurchasingModule first = StandardPurchasingModule.Instance();
		ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(first);
		foreach (ProductCatalogItem allProduct in productCatalog.allProducts)
		{
			products.Add(allProduct.id, new ProductData());
			if (allProduct.allStoreIDs.Count > 0)
			{
				IDs ds = new IDs();
				foreach (StoreID allStoreID in allProduct.allStoreIDs)
				{
					ds.Add(allStoreID.id, allStoreID.store);
				}
				configurationBuilder.AddProduct(allProduct.id, allProduct.type, ds);
			}
			else
			{
				configurationBuilder.AddProduct(allProduct.id, allProduct.type);
			}
		}
		StartCoroutine(InitializeStore(configurationBuilder));
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		products.Clear();
	}

	private IEnumerator InitializeStore(ConfigurationBuilder builder)
	{
		UnityPurchasing.Initialize(this, builder);
		yield break;
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		this.controller = controller;
		foreach (KeyValuePair<string, ProductData> product in products)
		{
			HandleDataProduct(product.Value, controller.products.WithID(product.Key));
		}
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		UnityEngine.Debug.LogErrorFormat("PurchaseManager failed to initialize. Reason: {0}", error.ToString());
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		ProductData productData = FindData(e.purchasedProduct.definition.id);
		if (productData != null && HandleDataPurchase(productData))
		{
			return PurchaseProcessingResult.Complete;
		}
		return PurchaseProcessingResult.Pending;
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
	{
		UnityEngine.Debug.LogWarningFormat("PurchaseManager failed to purchase '{0}'. Reason: {1}", product.definition.id, reason.ToString());
	}

	private ProductData FindData(string productID)
	{
		ProductData value = null;
		products.TryGetValue(productID, out value);
		return value;
	}

	private void HandleDataProduct(ProductData data, Product product)
	{
		data.product = product;
		if (data.handler != null)
		{
			data.handler.InitProduct(data.product);
			if (data.purchased)
			{
				data.handler.HandlePurchase(data.product);
				data.purchased = false;
				controller.ConfirmPendingPurchase(data.product);
			}
		}
	}

	private void HandleDataHandler(ProductData data, IPurchaseHandler handler)
	{
		data.handler = handler;
		if (handler != null && data.product != null)
		{
			data.handler.InitProduct(data.product);
			if (data.purchased)
			{
				data.handler.HandlePurchase(data.product);
				data.purchased = false;
				controller.ConfirmPendingPurchase(data.product);
			}
		}
	}

	private bool HandleDataPurchase(ProductData data)
	{
		data.purchased = true;
		if (data.handler != null && data.product != null)
		{
			data.handler.HandlePurchase(data.product);
			data.purchased = false;
			return true;
		}
		return false;
	}

    void IStoreListener.OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }
}
