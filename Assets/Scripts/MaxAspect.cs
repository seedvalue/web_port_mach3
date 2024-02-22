using UnityEngine;

public class MaxAspect : MonoBehaviour
{
	public float maxAspect = 0.72f;

	private RectTransform rect;

	private void Start()
	{
		rect = GetComponent<RectTransform>();
	}

	private void Update()
	{
		float b = (float)Screen.width * 1f / (float)Screen.height;
		b = Mathf.Min(maxAspect, b);
		rect.sizeDelta = new Vector2(rect.rect.height * b, 0f);
	}
}
