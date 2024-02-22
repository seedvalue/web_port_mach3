using UnityEngine;
using UnityEngine.UI;
using Utils;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasDistanceFader : MonoBehaviour
{
	private const float OutlineFade = 0.5f;

	[Header("Distances")]
	public float aheadDistance = 40f;

	public float fade1Range = 1f;

	public float insideRange = 35f;

	public float fade2Range = 1f;

	[Header("Alpha")]
	public float ahead = 1f;

	public float inside;

	public float behind;

	[Header("Features")]
	public bool fadeOutlines;

	private Transform canvasTransform;

	private CanvasGroup canvasGroup;

	private void Start()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		Canvas componentInParent = GetComponentInParent<Canvas>();
		if (!componentInParent)
		{
			base.enabled = false;
			return;
		}
		componentInParent = componentInParent.rootCanvas;
		canvasTransform = componentInParent.transform;
		if ((bool)componentInParent.worldCamera)
		{
			canvasTransform = componentInParent.worldCamera.transform;
		}
	}

	private void Update()
	{
		float distance = CalculateDistance();
		canvasGroup.alpha = CalculateAlpha(distance);
		canvasGroup.blocksRaycasts = (canvasGroup.alpha > 0.5f);
		if (fadeOutlines)
		{
			float t = 1f;
			if (canvasGroup.alpha < 0.5f)
			{
				t = 0f;
			}
			else if (canvasGroup.alpha >= 0.5f)
			{
				t = (canvasGroup.alpha - 0.5f) / 0.5f;
			}
			Outline[] componentsInChildren = GetComponentsInChildren<Outline>();
			foreach (Outline outline in componentsInChildren)
			{
				Outline outline2 = outline;
				Color effectColor = outline.effectColor;
				float r = effectColor.r;
				Color effectColor2 = outline.effectColor;
				float g = effectColor2.g;
				Color effectColor3 = outline.effectColor;
				outline2.effectColor = new Color(r, g, effectColor3.b, Helpers.simple_smooth_step(t));
			}
		}
	}

	private float CalculateDistance()
	{
		if (!canvasTransform)
		{
			return 0f;
		}
		Vector3 vector = canvasTransform.InverseTransformPoint(base.transform.position);
		return Mathf.Abs(vector.z);
	}

	private float CalculateAlpha(float distance)
	{
		distance -= aheadDistance;
		if (distance < 0f)
		{
			return ahead;
		}
		distance -= fade1Range;
		if (distance < 0f)
		{
			float t = 1f + distance / fade1Range;
			return Mathf.Lerp(ahead, inside, t);
		}
		distance -= insideRange;
		if (distance < 0f)
		{
			return inside;
		}
		distance -= fade2Range;
		if (distance < 0f)
		{
			float t2 = 1f + distance / fade2Range;
			return Mathf.Lerp(inside, behind, t2);
		}
		return behind;
	}
}
