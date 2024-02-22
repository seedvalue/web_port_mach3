using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Utils;

[ExecuteInEditMode]
public class FullScreenCanvas : ExternalSceneElement, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
{
	public bool mainScreen;

	public bool resetScroll = true;

	public RectTransform[] keepScreenSize;

	[HideInInspector]
	public UnityEvent onGainFocus;

	[HideInInspector]
	public UnityEvent onLostFocus;

	[HideInInspector]
	public UnityEvent onEnable;

	private Vector3 dragOrigin;

	private Vector3 dragDifference;

	private Vector2 dragAxis;

	private float dragTime;

	private bool hasFocus;

	private DrivenRectTransformTracker rectTracker;

	private void OnEnable()
	{
		if (!Application.isPlaying)
		{
			AdjustKeepScreenSize();
		}
	}

	private void OnDisable()
	{
		rectTracker.Clear();
	}

	protected override void Start()
	{
		base.Start();
		if (Application.isPlaying)
		{
			float num = (float)Screen.width * 1f / (float)Screen.height;
			if ((bool)ScreenMgr.instance)
			{
				num = Mathf.Max(ScreenMgr.instance.minAspect, num);
			}
			RectTransform component = GetComponent<RectTransform>();
			RectTransform rectTransform = component;
			Vector2 sizeDelta = component.sizeDelta;
			float x = sizeDelta.y * num;
			Vector2 sizeDelta2 = component.sizeDelta;
			rectTransform.sizeDelta = new Vector2(x, sizeDelta2.y);
			if ((bool)base.transform.parent)
			{
				RectTransform rectTransform2 = component;
				float num2 = component.GetSiblingIndex();
				Vector2 sizeDelta3 = component.sizeDelta;
				rectTransform2.position = new Vector3(num2 * sizeDelta3.y * num, 0f, 0f);
			}
			dragAxis = new Vector2(1f, 0f);
			if (mainScreen)
			{
				Transform transform = Camera.main.transform;
				Vector3 position = component.position;
				transform.position = new Vector3(position.x, 0f, -100f);
			}
			AdjustKeepScreenSize();
		}
	}

	private void Update()
	{
		AdjustKeepScreenSize();
		if (!Application.isPlaying)
		{
			return;
		}
		RectTransform component = GetComponent<RectTransform>();
		Vector2 sizeDelta = component.sizeDelta;
		float num = sizeDelta.x * 0.95f;
		Canvas component2 = GetComponent<Canvas>();
		bool enabled = component2.enabled;
		if ((Camera.main.transform.position - base.transform.position).sqrMagnitude > num * num)
		{
			if (!isDragged())
			{
				if (hasFocus)
				{
					UnityEngine.Debug.Log(base.name + ": onLostFocus");
					onLostFocus.Invoke();
				}
				component2.enabled = false;
				hasFocus = false;
			}
		}
		else
		{
			component2.enabled = true;
			if (!enabled)
			{
				onEnable.Invoke();
			}
		}
		if (!hasFocus && !Singleton<SceneLoader>.Instance.loading && component2.enabled && !isDragged())
		{
			FlyTo component3 = Camera.main.GetComponent<FlyTo>();
			if (!component3.isFlaying)
			{
				hasFocus = true;
				onGainFocus.Invoke();
				UnityEngine.Debug.Log(base.name + ": onGainFocus");
			}
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		Camera main = Camera.main;
		Vector2 position = eventData.position;
		float x = position.x * dragAxis.x;
		Vector2 position2 = eventData.position;
		dragOrigin = main.ScreenToWorldPoint(new Vector2(x, position2.y * dragAxis.y));
		dragTime = 0f;
	}

	public void OnDrag(PointerEventData eventData)
	{
		Camera main = Camera.main;
		Vector2 position = eventData.position;
		float x = position.x * dragAxis.x;
		Vector2 position2 = eventData.position;
		dragDifference = main.ScreenToWorldPoint(new Vector2(x, position2.y * dragAxis.y)) - Camera.main.transform.position;
		Camera.main.transform.position = ClampCameraPos(dragOrigin - dragDifference);
		dragTime += Time.deltaTime;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		Camera main = Camera.main;
		Vector2 position = eventData.position;
		float x = position.x * dragAxis.x;
		Vector2 position2 = eventData.position;
		Vector3 b = main.ScreenToWorldPoint(new Vector2(x, position2.y * dragAxis.y));
		float magnitude = (dragOrigin - b).magnitude;
		if (magnitude > 35f && (double)dragTime < 0.25)
		{
			ShowNeighborScreen(b.x < dragOrigin.x);
		}
		else
		{
			FitCameraToScreen();
		}
	}

	private void FitCameraToScreen()
	{
		RectTransform component = GetComponent<RectTransform>();
		Vector2 sizeDelta = component.sizeDelta;
		float x = sizeDelta.x;
		Vector3 position = Camera.main.transform.position;
		int num = (int)(position.x / x + 0.49f);
		FlyTo component2 = Camera.main.GetComponent<FlyTo>();
		FlyTo flyTo = component2;
		float x2 = (float)num * x;
		Vector3 position2 = Camera.main.transform.position;
		float y = position2.y;
		Vector3 position3 = Camera.main.transform.position;
		flyTo.flyTo(new Vector3(x2, y, position3.z), 0.25f, time: true, local: false);
	}

	private Vector3 ClampCameraPos(Vector3 newCameraPos)
	{
		Vector3 result = newCameraPos;
		RectTransform component = GetComponent<RectTransform>();
		Vector2 sizeDelta = component.sizeDelta;
		float num = sizeDelta.x * 0.5f;
		float num2 = base.transform.parent.childCount;
		Vector2 sizeDelta2 = component.sizeDelta;
		float num3 = num2 * sizeDelta2.x;
		result.x = Mathf.Clamp(result.x, 0f - num, num3 - num);
		return result;
	}

	private void ShowNeighborScreen(bool next)
	{
		RectTransform component = GetComponent<RectTransform>();
		Vector2 sizeDelta = component.sizeDelta;
		float x = sizeDelta.x;
		Vector3 position = Camera.main.transform.position;
		int num = (int)(position.x / x + 0.49f);
		int num2 = num;
		num2 += (next ? 1 : (-1));
		num2 = Mathf.Clamp(num2, 0, base.transform.parent.childCount - 1);
		FlyTo component2 = Camera.main.GetComponent<FlyTo>();
		FlyTo flyTo = component2;
		float x2 = (float)num2 * x;
		Vector3 position2 = Camera.main.transform.position;
		float y = position2.y;
		Vector3 position3 = Camera.main.transform.position;
		flyTo.flyTo(new Vector3(x2, y, position3.z), 0.25f, time: true, local: false);
	}

	private bool isDragged()
	{
		return UnityEngine.Input.touchCount > 0;
	}

	private void AdjustKeepScreenSize()
	{
		rectTracker.Clear();
		if (keepScreenSize == null)
		{
			return;
		}
		RectTransform component = GetComponent<RectTransform>();
		if (!component)
		{
			return;
		}
		float num = (float)Screen.width * 1f / (float)Screen.height;
		Vector2 sizeDelta = component.sizeDelta;
		float x = sizeDelta.y * num;
		Vector2 sizeDelta2 = component.sizeDelta;
		Vector2 sizeDelta3 = new Vector2(x, sizeDelta2.y);
		RectTransform[] array = keepScreenSize;
		foreach (RectTransform rectTransform in array)
		{
			if ((bool)rectTransform)
			{
				rectTracker.Add(this, rectTransform, DrivenTransformProperties.All);
				rectTransform.position = base.transform.position;
				rectTransform.sizeDelta = sizeDelta3;
				rectTransform.localScale = Vector3.one;
				rectTransform.anchorMin = Vector2.one * 0.5f;
				rectTransform.anchorMax = Vector2.one * 0.5f;
				rectTransform.pivot = Vector2.one * 0.5f;
			}
		}
	}
}
