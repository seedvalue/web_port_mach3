using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class M3WoundFlare : MonoBehaviour
{
	public float duration;

	public AnimationCurve alphaAmplitude;

	private List<Image> images = new List<Image>();

	private M3InterpolatedFloat time;

	private bool active;

	public void Hit()
	{
		time.SlideUp(duration, adjustTime: false, forceFullCycle: true);
		ToggleEnabled(enabled: true);
		active = true;
	}

	private void Start()
	{
		GetComponentsInChildren(images);
		time = new M3InterpolatedFloat(this, M3InterpolationMode.Linear);
		ToggleEnabled(enabled: false);
	}

	private void ToggleEnabled(bool enabled)
	{
		foreach (Image image in images)
		{
			image.enabled = enabled;
			Helpers.SetImageAlpha(image, 0f);
		}
	}

	private void Update()
	{
		if (active)
		{
			float alpha = alphaAmplitude.Evaluate(time.Value);
			foreach (Image image in images)
			{
				Helpers.SetImageAlpha(image, alpha);
			}
			active = (1f - time.Value > float.Epsilon);
			if (!active)
			{
				ToggleEnabled(enabled: false);
			}
		}
	}
}
