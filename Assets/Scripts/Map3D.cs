using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Map3D : MonoBehaviour
{
	public float radius;

	public float segmentAngle;

	public float segmentBaseAngle;

	public float startRotation;

	public int beginSegments;

	public int endSegments;

	public Transform mapRoot;

	public Camera mapCamera;

	public ScrollRectTweener scrollRect;

	private readonly Dictionary<MetaObject, Transform> metaTransforms = new Dictionary<MetaObject, Transform>();

	private void Start()
	{
		MetaView[] componentsInChildren = GetComponentsInChildren<MetaView>();
		foreach (MetaView metaView in componentsInChildren)
		{
			metaTransforms.Add(metaView.GetObject(), metaView.transform);
		}
	}

	private void Update()
	{
		mapRoot.transform.localPosition = new Vector3(0f, 0f - radius, 0f);
		for (int i = 0; i < mapRoot.transform.childCount; i++)
		{
			PrepareSegment(mapRoot.transform.GetChild(i), mapRoot.transform.childCount - i - 1);
		}
	}

	public float GetNormalizePosition(Transform refObject)
	{
		Transform parent = refObject.parent;
		while (parent.parent != mapRoot)
		{
			parent = parent.parent;
		}
		int num = mapRoot.transform.childCount - beginSegments - endSegments;
		int num2 = mapRoot.transform.childCount - parent.GetSiblingIndex() - 1 - beginSegments;
		return Mathf.Clamp01(((float)num2 - 0.1f) / (float)num);
	}

	public float GetNormalizePosition(MetaObject metaObject)
	{
		Transform value = null;
		if (metaTransforms.TryGetValue(metaObject, out value))
		{
			return GetNormalizePosition(value);
		}
		return 0f;
	}

	public float GetMapWidth()
	{
		return (float)(mapRoot.transform.childCount - beginSegments - endSegments) * segmentAngle;
	}

	public void SetNormalizePosition(float normalizedPos)
	{
		float b = (float)(-(mapRoot.transform.childCount - endSegments)) * segmentAngle + startRotation;
		float x = Mathf.LerpUnclamped(startRotation - segmentAngle * (float)beginSegments, b, normalizedPos);
		mapRoot.transform.localRotation = Quaternion.Euler(new Vector3(x, 0f, 0f));
	}

	private void PrepareSegment(Transform segment, int index)
	{
		float num = (0f - segmentAngle) * (float)index;
		Vector3 localPosition = segment.localPosition;
		segment.localPosition = new Vector3(localPosition.x, Mathf.Sin((float)Math.PI / 180f * num) * radius, radius * Mathf.Cos((float)Math.PI / 180f * num));
		segment.localRotation = Quaternion.Euler(new Vector3(0f - Mathf.DeltaAngle(0f, num + segmentBaseAngle), 0f, 0f));
	}

	public float ScrollToShow(Transform target)
	{
		if ((bool)scrollRect)
		{
			float normalizePosition = GetNormalizePosition(target);
			float result = Mathf.Abs(scrollRect.GetComponent<ScrollRect>().verticalNormalizedPosition - normalizePosition);
			scrollRect.ScrollVertical(normalizePosition, fast: false);
			return result;
		}
		return 0f;
	}

	public float ScrollToShow(MetaObject metaObject)
	{
		Transform value = null;
		if (metaTransforms.TryGetValue(metaObject, out value))
		{
			return ScrollToShow(value);
		}
		return 0f;
	}
}
