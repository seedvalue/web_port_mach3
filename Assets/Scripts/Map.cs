using UnityEngine;
using UnityEngine.EventSystems;

public class Map : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
{
	private Vector3 dragOrigin;

	public Vector3 dragAxis;

	private FullScreenCanvas canvas;

	private bool dragCanvas;

	private void Start()
	{
		canvas = GetComponentInParent<FullScreenCanvas>();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		dragCanvas = false;
		Vector2 delta = eventData.delta;
		float num = Mathf.Abs(delta.x);
		Vector2 delta2 = eventData.delta;
		if (num <= Mathf.Abs(delta2.y))
		{
			dragCanvas = true;
			canvas.OnBeginDrag(eventData);
			return;
		}
		Camera main = Camera.main;
		Vector2 position = eventData.position;
		float x = position.x * dragAxis.x;
		Vector2 position2 = eventData.position;
		dragOrigin = main.ScreenToWorldPoint(new Vector2(x, position2.y * dragAxis.y)) - base.transform.position;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (dragCanvas)
		{
			canvas.OnDrag(eventData);
			return;
		}
		Transform transform = base.transform;
		Camera main = Camera.main;
		Vector2 position = eventData.position;
		float x = position.x * dragAxis.x;
		Vector2 position2 = eventData.position;
		transform.position = main.ScreenToWorldPoint(new Vector2(x, position2.y * dragAxis.y)) - dragOrigin;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (dragCanvas)
		{
			canvas.OnEndDrag(eventData);
		}
	}
}
