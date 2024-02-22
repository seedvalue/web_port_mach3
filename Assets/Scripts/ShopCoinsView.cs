using UnityEngine.EventSystems;

public class ShopCoinsView : MetaProductView, IPointerClickHandler, IPointerDownHandler, IEventSystemHandler
{
	protected override void OnInteract()
	{
		base.OnInteract();
		MetaProduct @object = GetObject();
		@object.Purchase();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OnInteract();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}
}
