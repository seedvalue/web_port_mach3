using UnityEngine;

public class FitWorldCanvasToScreen : MonoBehaviour
{
	private void Start()
	{
		float num = (float)Screen.width * 1f / (float)Screen.height;
		RectTransform component = GetComponent<RectTransform>();
		RectTransform rectTransform = component;
		Vector2 sizeDelta = component.sizeDelta;
		float x = sizeDelta.y * num;
		Vector2 sizeDelta2 = component.sizeDelta;
		rectTransform.sizeDelta = new Vector2(x, sizeDelta2.y);
		RectTransform rectTransform2 = component;
		Vector3 position = component.position;
		rectTransform2.position = new Vector3(position.x, 0f, 0f);
		if ((bool)base.transform.parent)
		{
			RectTransform rectTransform3 = component;
			Vector2 sizeDelta3 = component.sizeDelta;
			rectTransform3.position = new Vector3(0f * sizeDelta3.y * num, 0f, 0f);
		}
	}

	private void LateUpdate()
	{
		RectTransform component = GetComponent<RectTransform>();
		RectTransform rectTransform = component;
		Vector3 position = Camera.main.transform.position;
		float x = position.x;
		Vector3 position2 = Camera.main.transform.position;
		rectTransform.position = new Vector3(x, position2.y, 0f);
	}
}
