using UnityEngine;

public class SimpleClipping : MonoBehaviour
{
	public Canvas canvas;

	private void Update()
	{
		RectTransform component = canvas.GetComponent<RectTransform>();
		Vector2 sizeDelta = component.sizeDelta;
		float num = sizeDelta.x * 0.95f;
		Camera component2 = GetComponent<Camera>();
		if ((Camera.main.transform.position - component.transform.position).sqrMagnitude > num * num)
		{
			component2.enabled = false;
		}
		else
		{
			component2.enabled = true;
		}
	}
}
