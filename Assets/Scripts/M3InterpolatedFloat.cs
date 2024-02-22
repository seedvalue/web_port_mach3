using System.Collections;
using UnityEngine;

public class M3InterpolatedFloat
{
	private float value;

	private float amplitude;

	private float offset;

	private MonoBehaviour parent;

	private M3InterpolationMode interpolationMode;

	private IEnumerator coroutine;

	public float Value => value * amplitude + offset;

	public M3InterpolatedFloat(MonoBehaviour _parent, M3InterpolationMode mode = M3InterpolationMode.Smooth, float _amplitude = 1f, float _offset = 0f)
	{
		interpolationMode = mode;
		parent = _parent;
		amplitude = _amplitude;
		offset = _offset;
	}

	public void SlideUp(float time, bool adjustTime = false, bool forceFullCycle = false)
	{
		if (coroutine != null)
		{
			parent.StopCoroutine(coroutine);
		}
		coroutine = Slide((!forceFullCycle) ? value : 0f, 1f, (!adjustTime) ? time : (time * (1f - value)));
		parent.StartCoroutine(coroutine);
	}

	public void SlideDown(float time, bool adjustTime = false, bool forceFullCycle = false)
	{
		if (coroutine != null)
		{
			parent.StopCoroutine(coroutine);
		}
		coroutine = Slide((!forceFullCycle) ? value : 1f, 0f, (!adjustTime) ? time : (time * value));
		parent.StartCoroutine(coroutine);
	}

	private IEnumerator Slide(float slideFrom, float slideTo, float slideDuration)
	{
		for (float time = 0f; time < slideDuration; time += Time.deltaTime)
		{
			if (interpolationMode == M3InterpolationMode.Smooth)
			{
				value = Mathf.SmoothStep(slideFrom, slideTo, time / slideDuration);
			}
			else
			{
				value = Mathf.Lerp(slideFrom, slideTo, time / slideDuration);
			}
			yield return null;
		}
		value = slideTo;
		coroutine = null;
	}
}
