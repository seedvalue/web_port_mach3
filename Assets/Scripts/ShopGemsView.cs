using UnityEngine.EventSystems;

public class ShopGemsView : MetaProductView, IPointerClickHandler, IPointerDownHandler, IEventSystemHandler
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
