using UnityEngine;

[ExecuteInEditMode]
public class ItemViewScaleHelper : MonoBehaviour
{
	private void CalculateScale()
	{
		RectTransform component = GetComponent<RectTransform>();
		RectTransform rectTransform = (!base.transform.parent) ? component : base.transform.parent.GetComponent<RectTransform>();
		if (rectTransform == null)
		{
			rectTransform = component;
		}
		component.localScale = new Vector3(rectTransform.rect.width / component.rect.width, rectTransform.rect.height / component.rect.height, 1f);
	}

	private void Start()
	{
		CalculateScale();
	}

	private void Update()
	{
		CalculateScale();
	}
}
