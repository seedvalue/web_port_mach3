using System;
using System.Collections;
using UnityEngine;

public static class CoroutineHelper
{
	public static IEnumerator AnimateInTime(float duration, Action<float> action)
	{
		float startTime = Time.time;
		yield return null;
		while (Time.time < startTime + duration)
		{
			float t = (Time.time - startTime) / duration;
			action(t);
			yield return null;
		}
		action(1f);
	}
}
