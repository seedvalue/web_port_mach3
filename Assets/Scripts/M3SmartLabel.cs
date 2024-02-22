using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class M3SmartLabel : MonoBehaviour
{
	public AnimationCurve scaleOnDamageCurve;

	public AnimationCurve scaleLabelOnDamageCurve;

	public Text label;

	public Image icon;

	public string prefix;

	public float shadeIntensity = 0.75f;

	public float alphaIntensity = 0.6f;

	public float yDownOnShade = 10f;

	public float yUp = 10f;

	public float yUpDownTime = 0.1f;

	public bool playSound = true;

	public GameObject fxGlowPrefab;

	public GameObject fxPulsePrefab;

	private Canvas localCanvas;

	private Image background;

	private Vector3 basePos;

	private List<ParticleSystemRenderer> glow = new List<ParticleSystemRenderer>();

	private List<ParticleSystemRenderer> pulse = new List<ParticleSystemRenderer>();

	private ParticleFader glowFader;

	private Vector3 shadedPos;

	private int src;

	private int dest;

	private int value;

	private M3InterpolatedFloat pos;

	private float animTime;

	private float animTimeCurrent;

	public void BackToBase(int valueBase)
	{
		src = (dest = (value = 0));
		if (yUp > float.Epsilon)
		{
			pos.SlideDown(yUpDownTime);
		}
		if ((bool)glowFader)
		{
			glowFader.FadeOut();
		}
	}

	public void AddValue(int addedValue, float addingTime)
	{
		if (addedValue <= 0)
		{
			return;
		}
		if (dest == 0)
		{
			pos.SlideUp(yUpDownTime);
			if ((bool)fxGlowPrefab)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(fxGlowPrefab, base.transform, worldPositionStays: false);
				gameObject.GetComponentsInChildren(glow);
				glowFader = gameObject.GetComponent<ParticleFader>();
			}
		}
		dest += addedValue;
		src = value;
		animTime = addingTime;
		animTimeCurrent = 0f;
		if ((bool)fxPulsePrefab)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(fxPulsePrefab, base.transform, worldPositionStays: false);
			gameObject2.GetComponentsInChildren(pulse);
		}
		if (playSound)
		{
			AudioManager.PlaySafe(M3Board.instance.sounds.damageAdd);
		}
	}

	public void ToggleShade(bool shading, float animDuration)
	{
		if (shading)
		{
			shadedPos = new Vector3(0f, 0f - yDownOnShade, 0f);
			Color color = new Color(1f - shadeIntensity, 1f - shadeIntensity, 1f - shadeIntensity, 1f - alphaIntensity);
			if ((bool)background)
			{
				Helpers.SetImageColor(background, color);
			}
			if ((bool)icon)
			{
				icon.enabled = false;
			}
		}
		else
		{
			shadedPos = Vector3.zero;
			Color color2 = new Color(1f, 1f, 1f);
			if ((bool)background)
			{
				Helpers.SetImageColor(background, color2);
			}
			if ((bool)icon)
			{
				icon.enabled = true;
			}
		}
	}

	public void SetLabelColor(Color color)
	{
		if ((bool)label)
		{
			label.color = color;
		}
	}

	public bool IsLocked()
	{
		return dest != value;
	}

	private void Awake()
	{
		localCanvas = GetComponent<Canvas>();
		background = GetComponent<Image>();
		basePos = base.transform.localPosition;
		pos = new M3InterpolatedFloat(this, M3InterpolationMode.Smooth, yUp);
	}

	private void Update()
	{
		if ((bool)label)
		{
			label.text = ((value != 0) ? (prefix + value.ToString()) : string.Empty);
		}
		float num = 1f;
		float num2 = 1f;
		if (animTimeCurrent < animTime)
		{
			animTimeCurrent += Time.deltaTime;
			value = Mathf.RoundToInt(Mathf.Lerp(src, dest, animTimeCurrent / animTime));
			if (value > dest)
			{
				value = dest;
			}
			num = scaleOnDamageCurve.Evaluate(animTimeCurrent / animTime);
			num2 = scaleLabelOnDamageCurve.Evaluate(animTimeCurrent / animTime);
		}
		else
		{
			value = dest;
		}
		base.transform.localScale = new Vector3(num, num, 1f);
		if ((bool)label && label.gameObject != base.gameObject)
		{
			label.transform.localScale = new Vector3(num2, num2, 1f);
		}
		base.transform.localPosition = basePos + new Vector3(0f, pos.Value, 0f) + shadedPos;
	}

	private void UpdateSortingOrder(int sortingOrder)
	{
		if ((bool)localCanvas)
		{
			localCanvas.sortingOrder = sortingOrder;
		}
		for (int i = 0; i < glow.Count; i++)
		{
			if (!glow[i].Equals(null))
			{
				glow[i].sortingOrder = sortingOrder;
			}
		}
		for (int j = 0; j < pulse.Count; j++)
		{
			if (!pulse[j].Equals(null))
			{
				pulse[j].sortingOrder = sortingOrder;
			}
		}
	}
}
