using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectTweener : MonoBehaviour, IDragHandler, IEventSystemHandler
{
	private const float Threshold = 0.001f;

	private Vector2 startPos;

	private Vector2 targetPos;

	private bool wasHorizontal;

	private bool wasVertical;

	public RectOffset bounds = new RectOffset();

	public float moveSpeed = 1000f;

	public float moveSpeedFast = 5000f;

	public bool disableDragWhileTweening;

	public ScrollRect scrollRect
	{
		get;
		private set;
	}

	private void Awake()
	{
		scrollRect = GetComponent<ScrollRect>();
		wasHorizontal = scrollRect.horizontal;
		wasVertical = scrollRect.vertical;
	}

	public void ScrollHorizontal(float normalizedX, bool fast)
	{
		Scroll(new Vector2(normalizedX, scrollRect.verticalNormalizedPosition), fast);
	}

	public void ScrollHorizontal(float normalizedX, float duration)
	{
		Scroll(new Vector2(normalizedX, scrollRect.verticalNormalizedPosition), duration);
	}

	public void ScrollVertical(float normalizedY, bool fast)
	{
		Scroll(new Vector2(scrollRect.horizontalNormalizedPosition, normalizedY), fast);
	}

	public void ScrollVertical(float normalizedY, float duration)
	{
		Scroll(new Vector2(scrollRect.horizontalNormalizedPosition, normalizedY), duration);
	}

	public void Scroll(Vector2 normalizedPos, bool fast)
	{
		Scroll(normalizedPos, GetScrollDuration(normalizedPos, fast));
	}

	public void Scroll(Vector2 normalizedPos, float duration)
	{
		startPos = GetCurrentPos();
		targetPos = normalizedPos;
		if (disableDragWhileTweening)
		{
			LockScrollability();
		}
		StopAllCoroutines();
		StartCoroutine(DoMove(duration));
	}

	public void Scroll(RectTransform target, bool fast)
	{
		Scroll(CalculatePosition(target), fast);
	}

	public void Scroll(RectTransform target, float duration)
	{
		Scroll(CalculatePosition(target), duration);
	}

	public Vector2 CalculateDistanceToShow(RectTransform target)
	{
		Vector2 vector = CalculateMinPosition(target);
		Vector2 vector2 = CalculateMaxPosition(target);
		Vector2 zero = Vector2.zero;
		if (scrollRect.horizontal)
		{
			if (scrollRect.horizontalNormalizedPosition > vector.x + 0.001f)
			{
				zero.x = scrollRect.horizontalNormalizedPosition - vector.x;
			}
			else if (scrollRect.horizontalNormalizedPosition < vector2.x - 0.001f)
			{
				zero.x = scrollRect.horizontalNormalizedPosition - vector2.x;
			}
		}
		if (scrollRect.vertical)
		{
			if (scrollRect.verticalNormalizedPosition > vector.y + 0.001f)
			{
				zero.y = scrollRect.verticalNormalizedPosition - vector.y;
			}
			else if (scrollRect.verticalNormalizedPosition < vector2.y - 0.001f)
			{
				zero.y = scrollRect.verticalNormalizedPosition - vector2.y;
			}
		}
		return zero;
	}

	public Vector2 CalculatePosition(RectTransform target, float xAnchor = 0f, float yAnchor = 1f)
	{
		Vector2 vector = CalculateMinPosition(target);
		Vector2 vector2 = CalculateMaxPosition(target);
		return new Vector2(Mathf.Lerp(vector.x, vector2.x, xAnchor), Mathf.Lerp(vector.y, vector2.y, yAnchor));
	}

	public Vector2 CalculateMaxPosition(RectTransform target)
	{
		Rect rect = scrollRect.viewport.rect;
		Rect rect2 = scrollRect.content.rect;
		Rect rect3 = target.rect;
		Vector2 vector = rect2.size - rect.size;
		vector.x = Mathf.Max(0f, vector.x);
		vector.y = Mathf.Max(0f, vector.y);
		Vector2 a = (Vector2)scrollRect.transform.InverseTransformPoint(scrollRect.content.position) - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);
		a -= new Vector2(bounds.right, bounds.top);
		Vector2 result = new Vector2(scrollRect.horizontalNormalizedPosition, scrollRect.verticalNormalizedPosition);
		if (scrollRect.horizontal && vector.x > 0f)
		{
			Vector2 pivot = scrollRect.content.pivot;
			result.x = Mathf.Clamp01(pivot.x - a.x / vector.x);
		}
		if (scrollRect.vertical && vector.y > 0f)
		{
			Vector2 pivot2 = scrollRect.content.pivot;
			result.y = Mathf.Clamp01(pivot2.y - a.y / vector.y);
		}
		return result;
	}

	public Vector2 CalculateMinPosition(RectTransform target)
	{
		Rect rect = scrollRect.viewport.rect;
		Rect rect2 = scrollRect.content.rect;
		Rect rect3 = target.rect;
		Vector2 vector = rect2.size - rect.size;
		vector.x = Mathf.Max(0f, vector.x);
		vector.y = Mathf.Max(0f, vector.y);
		Vector2 a = (Vector2)scrollRect.transform.InverseTransformPoint(scrollRect.content.position) - (Vector2)scrollRect.transform.InverseTransformPoint(target.position) - rect.size + rect3.size;
		a += new Vector2(bounds.left, bounds.bottom);
		Vector2 result = new Vector2(scrollRect.horizontalNormalizedPosition, scrollRect.verticalNormalizedPosition);
		if (scrollRect.horizontal && vector.x > 0f)
		{
			Vector2 pivot = scrollRect.content.pivot;
			result.x = Mathf.Clamp01(pivot.x - a.x / vector.x);
		}
		if (scrollRect.vertical && vector.y > 0f)
		{
			Vector2 pivot2 = scrollRect.content.pivot;
			result.y = Mathf.Clamp01(pivot2.y - a.y / vector.y);
		}
		return result;
	}

	private float GetScrollDuration(Vector2 normalizedPos, bool fast)
	{
		float num = (!fast) ? moveSpeed : moveSpeedFast;
		Vector2 currentPos = GetCurrentPos();
		return Vector2.Distance(DeNormalize(currentPos), DeNormalize(normalizedPos)) / num;
	}

	private Vector2 DeNormalize(Vector2 normalizedPos)
	{
		return new Vector2(normalizedPos.x * scrollRect.content.rect.width, normalizedPos.y * scrollRect.content.rect.height);
	}

	private Vector2 GetCurrentPos()
	{
		return new Vector2(scrollRect.horizontalNormalizedPosition, scrollRect.verticalNormalizedPosition);
	}

	private IEnumerator DoMove(float duration)
	{
		if (!(duration < 0.05f))
		{
			Vector2 posOffset = targetPos - startPos;
			float currentTime = 0f;
			while (currentTime < duration)
			{
				currentTime += Time.deltaTime;
				scrollRect.normalizedPosition = EaseVector(currentTime, startPos, posOffset, duration);
				yield return null;
			}
			scrollRect.normalizedPosition = targetPos;
			if (disableDragWhileTweening)
			{
				RestoreScrollability();
			}
		}
	}

	public Vector2 EaseVector(float currentTime, Vector2 startValue, Vector2 changeInValue, float duration)
	{
		return new Vector2(changeInValue.x * Mathf.Sin(currentTime / duration * ((float)Math.PI / 2f)) + startValue.x, changeInValue.y * Mathf.Sin(currentTime / duration * ((float)Math.PI / 2f)) + startValue.y);
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!disableDragWhileTweening)
		{
			StopScroll();
		}
	}

	private void StopScroll()
	{
		StopAllCoroutines();
		if (disableDragWhileTweening)
		{
			RestoreScrollability();
		}
	}

	private void LockScrollability()
	{
		scrollRect.horizontal = false;
		scrollRect.vertical = false;
	}

	private void RestoreScrollability()
	{
		scrollRect.horizontal = wasHorizontal;
		scrollRect.vertical = wasVertical;
	}
}
