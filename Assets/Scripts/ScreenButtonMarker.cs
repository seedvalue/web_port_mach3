using UnityEngine;

public class ScreenButtonMarker : MonoBehaviour
{
	public float expandedWidth;

	public float collaspedWidth;

	public float space;

	private void Start()
	{
	}

	private void Update()
	{
		if ((bool)ScreenMgr.instance)
		{
			float screenWidth = ScreenMgr.instance.GetScreenWidth();
			Vector3 position = Camera.main.transform.position;
			float x = position.x;
			int num = Mathf.RoundToInt(x / screenWidth);
			Vector3 position2 = Camera.main.transform.position;
			float value = (position2.x - screenWidth * (float)num) / screenWidth;
			value = Mathf.Clamp(value, -1f, 1f);
			num -= 2;
			RectTransform component = GetComponent<RectTransform>();
			RectTransform rectTransform = component;
			float x2 = ((float)num + value) * collaspedWidth;
			Vector2 anchoredPosition = component.anchoredPosition;
			rectTransform.anchoredPosition = new Vector2(x2, anchoredPosition.y);
		}
	}
}
