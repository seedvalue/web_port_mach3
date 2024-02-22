using UnityEngine;
using UnityEngine.UI;

public class UIProgressBar : MonoBehaviour
{
	public enum LabelMode
	{
		None,
		CurrentValue,
		CurrentAndMaxValue,
		Custom
	}

	public Image fillImage;

	public int maxValue;

	[Header("Label")]
	public LabelMode mode = LabelMode.CurrentAndMaxValue;

	public Text label;

	public string separator = "/";

	public void SetProgress(float progress)
	{
		SetValue((int)(progress * (float)maxValue));
	}

	public void SetValue(int value)
	{
		if ((bool)fillImage)
		{
			fillImage.fillAmount = 1f * (float)value / (float)maxValue;
		}
		if ((bool)label)
		{
			switch (mode)
			{
			case LabelMode.None:
				label.text = string.Empty;
				break;
			case LabelMode.CurrentValue:
				label.text = value.ToString();
				break;
			case LabelMode.CurrentAndMaxValue:
				label.text = value.ToString() + separator + maxValue.ToString();
				break;
			}
		}
	}

	public void SetValue(int value, int _maxValue)
	{
		maxValue = _maxValue;
		SetValue(value);
	}
}
