using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class M3AttackWarning : MonoBehaviour
{
	public Image glow;

	public Image exclamationMark;

	public float pulseFrequency = 1f;

	public float fadeDuration = 0.5f;

	public float scaleAmplitude = 0.1f;

	private bool active;

	public void Fade(bool fadeIn, bool instant = false)
	{
		Image component = GetComponent<Image>();
		Color colorFrom = new Color(1f, 1f, 1f, 1f);
		Color colorTo = new Color(1f, 1f, 1f, 1f);
		if (fadeIn)
		{
			colorFrom.a = 0f;
		}
		else
		{
			colorTo.a = 0f;
		}
		float fadeTime = (!instant) ? fadeDuration : 0f;
		StartCoroutine(FadeImage(component, colorFrom, colorTo, fadeTime));
		if ((bool)exclamationMark)
		{
			StartCoroutine(FadeImage(exclamationMark, colorFrom, colorTo, fadeTime));
		}
		if ((bool)glow && !fadeIn)
		{
			StartCoroutine(FadeImage(glow, glow.color, colorTo, fadeTime));
		}
		active = fadeIn;
	}

	private IEnumerator FadeImage(Image image, Color colorFrom, Color colorTo, float fadeTime)
	{
		if (fadeTime > float.Epsilon)
		{
			float time = 0f;
			do
			{
				yield return null;
				time += Time.deltaTime;
				Color c = image.color = Helpers.SmoothStepColor(colorFrom, colorTo, time / fadeTime);
			}
			while (time < fadeTime);
		}
		else
		{
			image.color = colorTo;
		}
	}

	private void Update()
	{
		if (active)
		{
			float num = Mathf.Sin(Time.time * 2f * (float)Math.PI * pulseFrequency);
			if ((bool)exclamationMark)
			{
				exclamationMark.transform.localScale = new Vector3(1f + num * scaleAmplitude, 1f + num * scaleAmplitude, 1f);
			}
			if ((bool)glow)
			{
				Helpers.SetImageAlpha(glow, 0.3f * num + 0.7f);
			}
		}
	}
}
