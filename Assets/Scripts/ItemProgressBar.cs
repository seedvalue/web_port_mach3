using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class ItemProgressBar : MonoBehaviour
{
	public Image barImage;

	public Image barImageFull;

	private float width;

	public Text barLabel;

	[Header("Glow and Arrow")]
	public Image glow;

	public float glowFrequency = 1f;

	public Image arrowImage;

	public float arrowFrequency = 1f;

	public float arrowMoveFactor = 5f;

	private Vector3 arrowStarPos;

	public void SetProgress(float value, string text)
	{
		float num = Mathf.Max(Mathf.Min(value, 1f), 0f);
		Image image = (!(num < 1f)) ? barImageFull : barImage;
		Helpers.SetActiveGameObject(barImage, num < 1f);
		Helpers.SetActiveGameObject(barImageFull, num == 1f);
		Helpers.SetActiveGameObject(arrowImage, num == 1f);
		if ((bool)image)
		{
			image.fillAmount = num;
		}
		Helpers.SetText(barLabel, text);
	}

	private void Start()
	{
		if ((bool)barImage)
		{
			barImage.fillMethod = Image.FillMethod.Horizontal;
		}
		if ((bool)barImageFull)
		{
			Vector2 sizeDelta = barImageFull.rectTransform.sizeDelta;
			width = sizeDelta.x;
			barImage.fillMethod = Image.FillMethod.Horizontal;
		}
		if ((bool)arrowImage)
		{
			arrowStarPos = arrowImage.transform.localPosition;
		}
	}

	private void Update()
	{
		if ((bool)glow)
		{
			glow.transform.localPosition = glow.transform.localPosition + new Vector3(Time.deltaTime * width * glowFrequency, 0f, 0f);
			Vector3 localPosition = glow.transform.localPosition;
			float x = localPosition.x;
			Vector2 sizeDelta = (base.transform as RectTransform).sizeDelta;
			if (x > sizeDelta.x * 1.01f)
			{
				Transform transform = glow.transform;
				Vector2 sizeDelta2 = (glow.transform as RectTransform).sizeDelta;
				transform.localPosition = new Vector3(sizeDelta2.x * -1.01f, 0f, 0f);
			}
		}
		if ((bool)arrowImage)
		{
			arrowImage.transform.localPosition = arrowStarPos + new Vector3(0f, Mathf.Max(0f, Mathf.Sin(Time.time * (float)Math.PI * arrowFrequency)) * arrowMoveFactor + 0.1f, 0f);
		}
	}
}
