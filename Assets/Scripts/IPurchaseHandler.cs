using UnityEngine.Purchasing;

public interface IPurchaseHandler
{
	string productID
	{
		get;
	}

	void InitProduct(Product product);

	void HandlePurchase(Product product);
}
