using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UITweenerEx : MonoBehaviour
{
	private bool destroyOnEnd = true;

	private bool useWorldPos;

	private bool smooth;

	public static void flyTo(Component obj, Vector3 targetPos, float value = 0.5f, bool destroyOnEnd = true, bool time = true, bool local = true, bool smooth_step = false)
	{
		flyTo(obj.gameObject, targetPos, value, destroyOnEnd, time, local, smooth_step);
	}

	public static void flyTo(GameObject obj, Vector3 targetPos, float value = 0.5f, bool destroyOnEnd = true, bool time = true, bool local = true, bool smooth_step = false)
	{
		UITweenerEx uITweenerEx = obj.AddComponent<UITweenerEx>();
		uITweenerEx.destroyOnEnd = destroyOnEnd;
		uITweenerEx.flyTo(targetPos, value, time, local, smooth_step);
	}

	private void flyTo(Vector3 targetPos, float value = 0.5f, bool time = true, bool local = true, bool smooth_step = true)
	{
		StopAllCoroutines();
		smooth = smooth_step;
		useWorldPos = !local;
		Vector3 source = (!useWorldPos) ? base.transform.localPosition : base.transform.position;
		if (time)
		{
			StartCoroutine(moveObject(base.gameObject, source, targetPos, value));
		}
		else
		{
			StartCoroutine(moveObjectBySpeed(base.gameObject, source, targetPos, value));
		}
	}

	private IEnumerator moveObjectBySpeed(GameObject obj, Vector3 source, Vector3 target, float speed)
	{
		float magnitude = (target - source).magnitude;
		float duration = magnitude / speed;
		return moveObject(obj, source, target, duration);
	}

	private IEnumerator moveObject(GameObject obj, Vector3 source, Vector3 target, float duration)
	{
		float startTime = Time.time;
		while (Time.time < startTime + duration)
		{
			float t = (Time.time - startTime) / duration;
			if (smooth)
			{
				t = Helpers.simple_smooth_step(t);
			}
			if (useWorldPos)
			{
				obj.transform.position = Vector3.Lerp(source, target, t);
			}
			else
			{
				obj.transform.localPosition = Vector3.Lerp(source, target, t);
			}
			yield return null;
		}
		if (useWorldPos)
		{
			obj.transform.position = target;
		}
		else
		{
			obj.transform.localPosition = target;
		}
		if (destroyOnEnd)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	public static void fadeIn(Component obj, float time = 0.5f, bool destroyOnEnd = true)
	{
		fadeIn(obj.gameObject, time, destroyOnEnd);
	}

	public static void fadeOut(Component obj, float time = 0.5f, bool destroyOnEnd = true)
	{
		fadeOut(obj.gameObject, time, destroyOnEnd);
	}

	public static void fadeIn(GameObject obj, float time = 0.5f, bool destroyOnEnd = true)
	{
		fade(obj, 1f, time, destroyOnEnd);
	}

	public static void fadeOut(GameObject obj, float time = 0.5f, bool destroyOnEnd = true)
	{
		fade(obj, 0f, time, destroyOnEnd);
	}

	private static void fade(GameObject obj, float value, float time = 0.5f, bool destroyOnEnd = true)
	{
		if (obj.activeSelf)
		{
			UITweenerEx uITweenerEx = obj.AddComponent<UITweenerEx>();
			uITweenerEx.destroyOnEnd = destroyOnEnd;
			uITweenerEx.fade(value, time);
		}
	}

	private void fade(float value, float time = 0.5f)
	{
		StopAllCoroutines();
		StartCoroutine(fadeObject(base.gameObject, value, time));
	}

	private IEnumerator fadeObject(GameObject obj, float value, float duration)
	{
		Graphic graphic = null;
		CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
		if (!canvasGroup)
		{
			graphic = obj.GetComponent<Graphic>();
		}
		float num;
		if ((bool)canvasGroup)
		{
			num = canvasGroup.alpha;
		}
		else if ((bool)graphic)
		{
			Color color = graphic.color;
			num = color.a;
		}
		else
		{
			num = 1f;
		}
		float startAlpha = num;
		float startTime = Time.time;
		while (Time.time < startTime + duration)
		{
			float alpha = Mathf.Lerp(startAlpha, value, (Time.time - startTime) / duration);
			if ((bool)canvasGroup)
			{
				canvasGroup.alpha = alpha;
			}
			if ((bool)graphic)
			{
				Graphic graphic2 = graphic;
				Color color2 = graphic.color;
				float r = color2.r;
				Color color3 = graphic.color;
				float g = color3.g;
				Color color4 = graphic.color;
				graphic2.color = new Color(r, g, color4.b, alpha);
			}
			yield return null;
		}
		if ((bool)canvasGroup)
		{
			canvasGroup.alpha = value;
		}
		if ((bool)graphic)
		{
			Graphic graphic3 = graphic;
			Color color5 = graphic.color;
			float r2 = color5.r;
			Color color6 = graphic.color;
			float g2 = color6.g;
			Color color7 = graphic.color;
			graphic3.color = new Color(r2, g2, color7.b, value);
		}
		if (destroyOnEnd)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	public static void scaleTo(Component obj, float value, float time = 0.5f, bool destroyOnEnd = true)
	{
		scaleTo(obj.gameObject, value, time, destroyOnEnd);
	}

	public static void scaleTo(GameObject obj, float value, float time = 0.5f, bool destroyOnEnd = true)
	{
		scale(obj, value, time, destroyOnEnd);
	}

	public static void scaleIn(Component obj, float time = 0.5f, bool destroyOnEnd = true)
	{
		scaleIn(obj.gameObject, time, destroyOnEnd);
	}

	public static void scaleOut(Component obj, float time = 0.5f, bool destroyOnEnd = true)
	{
		scaleOut(obj.gameObject, time, destroyOnEnd);
	}

	public static void scaleIn(GameObject obj, float time = 0.5f, bool destroyOnEnd = true)
	{
		scale(obj, 1f, time, destroyOnEnd);
	}

	public static void scaleOut(GameObject obj, float time = 0.5f, bool destroyOnEnd = true)
	{
		scale(obj, 0f, time, destroyOnEnd);
	}

	private static void scale(GameObject obj, float value, float time = 0.5f, bool destroyOnEnd = true)
	{
		UITweenerEx uITweenerEx = obj.AddComponent<UITweenerEx>();
		uITweenerEx.destroyOnEnd = destroyOnEnd;
		uITweenerEx.scale(value, time);
	}

	private void scale(float value, float time = 0.5f)
	{
		StopAllCoroutines();
		StartCoroutine(scaleObject(base.gameObject, value, time));
	}

	private IEnumerator scaleObject(GameObject obj, float value, float duration)
	{
		Transform transform = obj.transform;
		Vector3 startScale = transform.localScale;
		float startTime = Time.time;
		while (Time.time < startTime + duration)
		{
			Vector3 scale = Vector3.Lerp(startScale, value * Vector3.one, (Time.time - startTime) / duration);
			scale.z = 1f;
			transform.localScale = scale;
			yield return null;
		}
		transform.localScale = value * Vector3.one;
		if (destroyOnEnd)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	public static void flip(Component obj, float turns, float time = 0.5f, bool destroyOnEnd = true)
	{
		flip(obj.gameObject, turns, time, destroyOnEnd);
	}

	public static void flip(GameObject obj, float turns, float time = 0.5f, bool destroyOnEnd = true)
	{
		_flip(obj, turns, time, destroyOnEnd);
	}

	private static void _flip(GameObject obj, float turns, float time = 0.5f, bool destroyOnEnd = true)
	{
		UITweenerEx uITweenerEx = obj.AddComponent<UITweenerEx>();
		uITweenerEx.destroyOnEnd = destroyOnEnd;
		uITweenerEx.flip(turns, time);
	}

	private void flip(float value, float time = 0.5f)
	{
		StopAllCoroutines();
		StartCoroutine(flipObject(base.gameObject, value, time));
	}

	private IEnumerator flipObject(GameObject obj, float value, float duration)
	{
		Transform transform = obj.transform;
		Vector3 localScale = transform.localScale;
		List<bool> childActive = new List<bool>();
		for (int i = 0; i < transform.childCount; i++)
		{
			childActive.Add(transform.GetChild(i).gameObject.activeSelf);
		}
		float startTime = Time.time;
		while (Time.time < startTime + duration)
		{
			float t = Helpers.simple_smooth_step((Time.time - startTime) / duration) * (float)Math.PI * 2f * value;
			Vector3 scale = transform.localScale = new Vector3(Mathf.Cos(t), 1f, 1f);
			for (int j = 0; j < transform.childCount; j++)
			{
				Helpers.SetActiveGameObject(transform.GetChild(j), scale.x >= 0f && childActive[j]);
			}
			yield return null;
		}
		transform.localScale = new Vector3(Mathf.Cos((float)Math.PI * 2f * value), 1f, 1f);
		for (int k = 0; k < transform.childCount; k++)
		{
			Helpers.SetActiveGameObject(transform.GetChild(k), childActive[k]);
		}
		if (destroyOnEnd)
		{
			UnityEngine.Object.Destroy(this);
		}
	}
}
