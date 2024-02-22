using UnityEngine;
using UnityEngine.UI;

public class M3InfoWindow : MetaWindow
{
	public Image infoBackground;

	public Text infoText;

	private M3TutorialStep item;

	private Vector2 anchorBase = Vector2.zero;

	protected override void Awake()
	{
		base.Awake();
		if ((bool)infoBackground)
		{
			RectTransform component = infoBackground.GetComponent<RectTransform>();
			anchorBase = component.anchoredPosition;
		}
	}

	protected virtual void OnEnable()
	{
		item = (base.context as M3TutorialStep);
		if ((bool)item)
		{
			Init();
		}
	}

	private void Init()
	{
		if ((bool)infoBackground)
		{
			RectTransform component = infoBackground.GetComponent<RectTransform>();
			component.anchoredPosition = anchorBase + new Vector2(0f, item.yPixelsFromCenter);
		}
		if ((bool)infoText)
		{
			infoText.text = item.stepDescription;
		}
	}
}
