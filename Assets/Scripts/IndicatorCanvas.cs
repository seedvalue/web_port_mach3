using UnityEngine;

[RequireComponent(typeof(Indicator), typeof(Canvas))]
public class IndicatorCanvas : MonoBehaviour
{
	public int sortingOrderOffset = 1;

	private Indicator indicator;

	private Canvas canvas;

	protected virtual void OnEnable()
	{
		indicator = GetComponent<Indicator>();
		canvas = GetComponent<Canvas>();
	}

	protected virtual void LateUpdate()
	{
		if ((bool)indicator.followTransform)
		{
			Canvas componentInParent = indicator.followTransform.GetComponentInParent<Canvas>();
			if ((bool)componentInParent)
			{
				canvas.overrideSorting = true;
				canvas.sortingLayerID = componentInParent.sortingLayerID;
				canvas.sortingOrder = componentInParent.sortingOrder + sortingOrderOffset;
			}
		}
	}
}
