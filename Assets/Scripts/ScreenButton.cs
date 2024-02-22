using UnityEngine;
using UnityEngine.UI;
using Utils;

public class ScreenButton : MonoBehaviour
{
	public ScreenID targetScreen;

	public CanvasGroup expandedGroup;

	public float expandedWidth;

	public float expandedHeight;

	public Text label;

	public CanvasGroup collapsedGroup;

	public float collapsedWidth;

	public float collapsedHeight;

	public RectTransform iconGroup;

	public Image leftBkg;

	public Image rightBkg;

	private void Start()
	{
		Button component = GetComponent<Button>();
		component.onClick.AddListener(ShowScreen);
	}

	private void ShowScreen()
	{
		ScreenMgr.instance.ShowScreenHUD((int)targetScreen);
	}

	private void Update()
	{
		if ((bool)ScreenMgr.instance)
		{
			float screenWidth = ScreenMgr.instance.GetScreenWidth();
			Vector3 position = Camera.main.transform.position;
			float x = position.x;
			int num = Mathf.RoundToInt(x / screenWidth);
			float num2 = x / screenWidth - (float)num;
			Vector3 position2 = Camera.main.transform.position;
			num2 = (position2.x - screenWidth * (float)targetScreen) / screenWidth;
			num2 = Mathf.Clamp(num2, -1f, 1f);
			float num3 = 1f - Mathf.Abs(num2);
			int num4 = ScreenMgr.instance ? ScreenMgr.instance.GetCurrentScreen() : 0;
			RectTransform component = GetComponent<RectTransform>();
			RectTransform rectTransform = component;
			float x2 = Mathf.Lerp(collapsedWidth, expandedWidth, num3);
			Vector2 sizeDelta = component.sizeDelta;
			rectTransform.sizeDelta = new Vector2(x2, sizeDelta.y);
			expandedGroup.alpha = num3;
			collapsedGroup.alpha = 1f - num3;
			if ((bool)label)
			{
				Color color = label.color;
				color.a = Mathf.Lerp(0f, 1f, num3 * num3);
				label.color = color;
			}
			RectTransform rectTransform2 = iconGroup;
			Vector2 anchoredPosition = iconGroup.anchoredPosition;
			rectTransform2.anchoredPosition = new Vector2(anchoredPosition.x, Mathf.Lerp(collapsedHeight, expandedHeight, num3));
			Helpers.SetActiveGameObject(leftBkg, num4 > (int)targetScreen);
			Helpers.SetActiveGameObject(rightBkg, num4 < (int)targetScreen);
			LayoutRebuilder.MarkLayoutForRebuild(component);
		}
	}
}
