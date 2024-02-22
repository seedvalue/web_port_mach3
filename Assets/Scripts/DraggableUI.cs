using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
{
	public enum DragAxis
	{
		Any,
		Horizontal,
		Vertical
	}

	public DragAxis dragAxis;

	public Transform forcedParent;

	private IDragHandler draggableParent;

	private bool dragParent;

	private bool dragAxisFixed;

	private Vector3 dragStartWorldPos;

	public float dragAxisTreshold = 10f;

	private Vector2 GetDrag(Vector2 position)
	{
		switch (dragAxis)
		{
		case DragAxis.Horizontal:
			return new Vector2(position.x, 0f);
		case DragAxis.Vertical:
			return new Vector2(0f, position.y);
		default:
			return position;
		}
	}

	private DragAxis GetDragAxis(Vector2 drag)
	{
		if (Mathf.Abs(drag.x) < Mathf.Abs(drag.y))
		{
			return DragAxis.Vertical;
		}
		return DragAxis.Horizontal;
	}

	private void Start()
	{
		if (forcedParent != null)
		{
			draggableParent = forcedParent.GetComponentInParent<IDragHandler>();
		}
		else if ((bool)base.transform.parent)
		{
			draggableParent = base.transform.parent.GetComponentInParent<IDragHandler>();
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		dragParent = false;
		dragAxisFixed = false;
		dragStartWorldPos = Camera.main.ScreenToWorldPoint(eventData.position);
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (dragAxisFixed && dragParent)
		{
			if (draggableParent != null && draggableParent != null)
			{
				draggableParent.OnDrag(eventData);
			}
		}
		else
		{
			if (dragAxisFixed && !dragParent)
			{
				return;
			}
			Vector3 v = Camera.main.ScreenToWorldPoint(eventData.position) - dragStartWorldPos;
			if (!(v.sqrMagnitude > dragAxisTreshold))
			{
				return;
			}
			dragAxisFixed = true;
			if (GetDragAxis(v) == dragAxis)
			{
				dragParent = false;
				ScrollRect component = GetComponent<ScrollRect>();
				if ((bool)component)
				{
					component.enabled = true;
				}
				return;
			}
			dragParent = true;
			if (draggableParent != null)
			{
				(draggableParent as IBeginDragHandler).OnBeginDrag(eventData);
				ScrollRect component2 = GetComponent<ScrollRect>();
				if ((bool)component2)
				{
					component2.enabled = false;
				}
			}
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (dragAxisFixed && dragParent)
		{
			if (draggableParent != null)
			{
				(draggableParent as IEndDragHandler).OnEndDrag(eventData);
			}
			ScrollRect component = GetComponent<ScrollRect>();
			if ((bool)component)
			{
				component.enabled = false;
			}
		}
	}
}
