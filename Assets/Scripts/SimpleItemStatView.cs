using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class SimpleItemStatView : MonoBehaviour
{
	public Image iconImage;

	public Text statText;

	public Text statValueText;

	public Image backgroundImage;

	public Sprite bkg;

	public Sprite upgradeAvailableBkg;

	private int value;

	private int upgValue;

	private string prefix;

	private string postfix;

	[Header("Glow and Arrow")]
	public Image glow;

	public float glowFrequency = 2f;

	private bool upgradeAvailable;

	private float width;

	private void DoSet(int value, int upgValue, bool forceGlow)
	{
		string str = prefix + value.ToString() + postfix;
		string str2 = upgValue.ToString() + postfix;
		if (upgValue == 0)
		{
			upgradeAvailable = false;
			Helpers.SetText(statValueText, str);
			Helpers.SetActiveGameObject(glow, false || forceGlow);
		}
		else
		{
			upgradeAvailable = true;
			Helpers.SetText(statValueText, str + " <color=#3cee12ff>+" + str2 + "</color>");
			Helpers.SetActiveGameObject(glow, value: true);
		}
	}

	public void Set(string statsName, int _value, int _upgValue, string _prefix, string _postfix, Sprite texture)
	{
		value = _value;
		upgValue = _upgValue;
		prefix = _prefix;
		postfix = _postfix;
		string str = prefix + value.ToString() + postfix;
		string str2 = upgValue.ToString() + postfix;
		Helpers.SetImage(iconImage, texture);
		if (upgValue == 0)
		{
			upgradeAvailable = false;
			Helpers.SetText(statText, statsName);
			Helpers.SetText(statValueText, str);
			Helpers.SetImage(backgroundImage, bkg);
			Helpers.SetActiveGameObject(glow, value: false);
		}
		else
		{
			upgradeAvailable = true;
			Helpers.SetText(statText, statsName);
			Helpers.SetText(statValueText, str + " <color=#3cee12ff>+" + str2 + "</color>");
			Helpers.SetImage(backgroundImage, upgradeAvailableBkg);
			Helpers.SetActiveGameObject(glow, value: true);
		}
	}

	public IEnumerator upgradeItemSequence()
	{
		DoSet(value, 0, forceGlow: false);
		Helpers.SetImage(backgroundImage, bkg);
		Image image = backgroundImage;
		Color color = backgroundImage.color;
		float r = color.r;
		Color color2 = backgroundImage.color;
		float g = color2.g;
		Color color3 = backgroundImage.color;
		float b = color3.b;
		Color color4 = backgroundImage.color;
		image.color = new Color(r, g, b, color4.a * 0.6f);
		float scaleTime = 0.2f;
		iconImage.transform.localScale = Vector3.zero;
		statText.transform.localScale = Vector3.zero;
		statValueText.transform.localScale = Vector3.zero;
		Transform transform = glow.transform;
		Vector2 sizeDelta = (glow.transform as RectTransform).sizeDelta;
		transform.localPosition = new Vector3(sizeDelta.x * -1.01f, 0f, 0f);
		glowFrequency = 0f;
		if ((bool)iconImage)
		{
			UITweenerEx.scaleIn(iconImage, scaleTime);
			yield return new WaitForSeconds(scaleTime / 2f);
		}
		if ((bool)statText)
		{
			UITweenerEx.scaleIn(statText, scaleTime);
			yield return new WaitForSeconds(scaleTime / 2f);
		}
		if ((bool)statValueText)
		{
			UITweenerEx.scaleIn(statValueText, scaleTime);
			yield return new WaitForSeconds(scaleTime / 2f);
		}
		float countingTimeMin = 0.25f;
		float countingTimeMax = 0.75f;
		float countingTime = Mathf.Lerp(countingTimeMin, countingTimeMax, Mathf.Clamp01(Mathf.Sqrt((float)upgValue * 1f - 1f) / 10f));
		float startTime = Time.time;
		bool glowOnce = true;
		while (Time.time < startTime + countingTime)
		{
			float t = (Time.time - startTime) / countingTime;
			int newValue = (int)Mathf.Lerp(value, value + upgValue, t);
			if (glowOnce && t > 0.75f)
			{
				glowOnce = false;
				GlowOnce(2f);
			}
			DoSet(newValue, 0, t > 0.75f);
			yield return null;
		}
		if (glowOnce)
		{
			GlowOnce(2f);
		}
		DoSet(value + upgValue, 0, forceGlow: true);
		yield return new WaitForSeconds(0.25f);
		DoSet(value + upgValue, upgValue, forceGlow: false);
	}

	private void GlowOnce(float _glowFrequency)
	{
		if ((bool)glow)
		{
			glow.gameObject.SetActive(value: true);
			Transform transform = glow.transform;
			Vector2 sizeDelta = (glow.transform as RectTransform).sizeDelta;
			transform.localPosition = new Vector3(sizeDelta.x * -1.01f, 0f, 0f);
			Image obj = glow;
			Vector2 sizeDelta2 = (glow.transform as RectTransform).sizeDelta;
			UITweenerEx.flyTo(obj, new Vector3(sizeDelta2.x * 1.01f, 0f, 0f), 1f / _glowFrequency);
		}
	}

	private void Start()
	{
		if ((bool)glow)
		{
			Vector2 sizeDelta = glow.rectTransform.sizeDelta;
			width = sizeDelta.x;
		}
	}

	private void Update()
	{
		if (!(glowFrequency < 0.01f) && (bool)glow && upgradeAvailable)
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
	}
}
