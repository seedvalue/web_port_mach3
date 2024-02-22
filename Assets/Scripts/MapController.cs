using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public class MapController : MonoBehaviour, IPointerDownHandler, IMetaScrollable, IEventSystemHandler
{
	public Map3D map;

	public float dragFactor;

	[Header("Required scroll tolerance")]
	public float nearTolerance = -0.024f;

	public float farTolerance = 0.1f;

	private ScrollRect scrollRect;

	private RectTransform rect;

	private bool dirty;

	public void OnPointerDown(PointerEventData eventData)
	{
	}

	private void Start()
	{
		scrollRect = GetComponentInParent<ScrollRect>();
		rect = GetComponent<RectTransform>();
		dirty = true;
	}

	private void Update()
	{
		if ((bool)map && dirty)
		{
			RectTransform rectTransform = rect;
			Vector2 sizeDelta = rect.sizeDelta;
			rectTransform.sizeDelta = new Vector2(sizeDelta.x, map.GetMapWidth() * dragFactor);
			MetaLocation lastLocation = SingletonComponent<Meta, MapComponent>.Instance.lastLocation;
			if ((bool)lastLocation)
			{
				scrollRect.verticalNormalizedPosition = map.GetNormalizePosition(lastLocation);
			}
			else
			{
				scrollRect.verticalNormalizedPosition = 0f;
			}
			dirty = false;
		}
	}

	private void LateUpdate()
	{
		if ((bool)map && (bool)scrollRect)
		{
			map.SetNormalizePosition(scrollRect.verticalNormalizedPosition);
		}
	}

	public float RequiredScrollToShow(MetaObject metaObject)
	{
		if ((bool)scrollRect)
		{
			float num = map.GetNormalizePosition(metaObject) - scrollRect.verticalNormalizedPosition;
			if (num > nearTolerance && num < farTolerance)
			{
				return 0f;
			}
			return num;
		}
		return 0f;
	}
}
