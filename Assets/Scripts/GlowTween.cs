using UnityEngine;
using UnityEngine.UI;

public class GlowTween : MonoBehaviour
{
	[Header("Effect")]
	[Range(0f, 1f)]
	public float amplitude = 0.4f;

	[Range(-1f, 1f)]
	public float offset = -0.4f;

	[Range(0f, 50f)]
	public float speed = 25f;

	private float baseAlpha;

	private CanvasGroup canvasGroup;

	private Graphic graphic;

	protected void OnEnable()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		if (!canvasGroup)
		{
			graphic = GetComponent<Graphic>();
		}
		if (!graphic)
		{
			base.enabled = false;
		}
		else
		{
			baseAlpha = GetAlpha();
		}
	}

	protected void OnDisable()
	{
		SetAlpha(baseAlpha);
	}

	protected void Update()
	{
		float num = CalcDistortion(Time.time * speed);
		float alpha = Mathf.Clamp01(baseAlpha + offset + amplitude * num);
		SetAlpha(alpha);
	}

	protected float CalcDistortion(float time)
	{
		float num = Mathf.Sin(time);
		float num2 = Mathf.Sin(time * 0.5f);
		float num3 = Mathf.Sin(time * 0.33f);
		return (num + num2 + num3) / 3f;
	}

	protected float GetAlpha()
	{
		if ((bool)canvasGroup)
		{
			return canvasGroup.alpha;
		}
		if ((bool)graphic)
		{
			Color color = graphic.color;
			return color.a;
		}
		return 0f;
	}

	protected void SetAlpha(float alpha)
	{
		if ((bool)canvasGroup)
		{
			canvasGroup.alpha = alpha;
		}
		else if ((bool)graphic)
		{
			Color color = graphic.color;
			color.a = alpha;
			graphic.color = color;
		}
	}
}
