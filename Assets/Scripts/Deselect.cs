using UnityEngine;
using UnityEngine.EventSystems;

public class Deselect : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
{
	public void OnPointerDown(PointerEventData eventData)
	{
		MetaItemViewSelectable.TryDeselect();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
