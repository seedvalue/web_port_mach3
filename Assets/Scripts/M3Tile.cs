using System;
using System.Collections;
using UnityEngine;
using Utils;

public class M3Tile : MonoBehaviour
{
	public M3Orb attackType;

	public GameObject fxOnPowerCumulate;

	public GameObject fxOnPowerWaste;

	public ParticleFader fxOnSelectPrefab;

	public ParticleFader fxOnHighlightPrefab;

	public SpriteRenderer selectionAdd;

	public M3WoundText textOnPowerUpPrefab;

	public float addPowerOnSelect = 0.5f;

	public float addPowerOnHighlight = 0.8f;

	[HideInInspector]
	public int moveTimeStamp;

	[HideInInspector]
	public bool inStructure;

	private Vector3 source;

	private Vector3 destination;

	private Vector3 offset;

	private M3InterpolatedFloat moveTimer;

	private bool moving;

	private SpriteRenderer spriteRenderer;

	private Color colorDeselected;

	private M3InterpolatedFloat selectionTimer;

	private M3InterpolatedFloat highlightTimer;

	private M3InterpolatedFloat grayScaleTimer;

	private M3InterpolatedFloat deathTimer;

	private ParticleFader fxSelect;

	private ParticleFader fxHighlight;

	public Color ColorDeselected
	{
		get
		{
			return colorDeselected;
		}
		set
		{
			colorDeselected = value;
		}
	}

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		colorDeselected = spriteRenderer.color;
		selectionTimer = new M3InterpolatedFloat(this, M3InterpolationMode.Linear);
		highlightTimer = new M3InterpolatedFloat(this, M3InterpolationMode.Linear);
		grayScaleTimer = new M3InterpolatedFloat(this, M3InterpolationMode.Linear);
		deathTimer = new M3InterpolatedFloat(this, M3InterpolationMode.Linear);
		moveTimer = new M3InterpolatedFloat(this, M3InterpolationMode.Linear);
		selectionAdd.color = new Color(0f, 0f, 0f, 1f);
	}

	public void SetDestination(Vector3 dest, float time, float arcAmplitude = 0f)
	{
		if (moving)
		{
			base.transform.position = destination;
		}
		destination = dest;
		source = base.transform.position;
		Vector3 vector = destination - source;
		offset = Vector3.Normalize(new Vector3(vector.y, 0f - vector.x, 0f)) * arcAmplitude;
		moving = (time > float.Epsilon);
		if (moving)
		{
			moveTimer.SlideUp(time, adjustTime: false, forceFullCycle: true);
		}
		else
		{
			base.transform.position = dest;
		}
	}

	public void Select(float animTime)
	{
		selectionTimer.SlideUp(animTime, adjustTime: true);
		if (!fxSelect && (bool)fxOnSelectPrefab)
		{
			fxSelect = UnityEngine.Object.Instantiate(fxOnSelectPrefab, base.transform, worldPositionStays: false);
		}
		GrayOutOff(animTime);
	}

	public void Highlight(float animTime)
	{
		highlightTimer.SlideUp(animTime, adjustTime: true);
		if (!fxHighlight && (bool)fxOnHighlightPrefab)
		{
			fxHighlight = UnityEngine.Object.Instantiate(fxOnHighlightPrefab, base.transform, worldPositionStays: false);
		}
	}

	public void GrayOutOn(Color grayedOutColor, float grayOutTime)
	{
		StartCoroutine(FadeSprite(spriteRenderer, spriteRenderer.color, grayedOutColor, grayOutTime));
	}

	public void GrayOutOff(float grayOutTime)
	{
		StartCoroutine(FadeSprite(spriteRenderer, spriteRenderer.color, new Color(1f, 1f, 1f, 1f), grayOutTime));
	}

	private IEnumerator FadeSprite(SpriteRenderer sprite, Color colorFrom, Color colorTo, float fadeTime)
	{
		if (fadeTime > float.Epsilon)
		{
			float time = 0f;
			do
			{
				yield return null;
				time += Time.deltaTime;
				Color c = sprite.color = Helpers.SmoothStepColor(colorFrom, colorTo, time / fadeTime);
			}
			while (time < fadeTime);
		}
		else
		{
			sprite.color = colorTo;
		}
	}

	public void Deselect(float clearTime = 0f)
	{
		if ((bool)fxSelect)
		{
			fxSelect.FadeOut();
			fxSelect = null;
		}
		selectionTimer.SlideDown(clearTime, adjustTime: true);
		GrayOutOn(colorDeselected, clearTime);
	}

	public void ClearFx()
	{
		ParticleFader[] componentsInChildren = GetComponentsInChildren<ParticleFader>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
		}
		fxSelect = (fxHighlight = null);
	}

	public IEnumerator AnimateDeath(bool grayScaleFirst, float animTime, float grayScaleDelay, float grayScaleTime, float grayScaleFxDuration)
	{
		M3Board.instance.semaphores.orbsDying++;
		if ((bool)fxSelect)
		{
			fxSelect.FadeOut();
		}
		if ((bool)fxHighlight)
		{
			fxHighlight.FadeOut();
		}
		if (grayScaleFirst)
		{
			grayScaleTimer.SlideUp(grayScaleTime);
			yield return StartCoroutine(CoToGrayScale());
			if (grayScaleDelay > float.Epsilon)
			{
				yield return new WaitForSeconds(grayScaleDelay);
			}
		}
		deathTimer.SlideDown(animTime, adjustTime: false, forceFullCycle: true);
		yield return StartCoroutine(CoDeath());
		UnityEngine.Object.Destroy(base.gameObject);
		if (grayScaleFxDuration > 0f)
		{
			UnityEngine.Object.Instantiate(fxOnPowerWaste, base.transform.position, base.transform.rotation);
			yield return new WaitForSeconds(grayScaleFxDuration);
		}
		M3Board.instance.semaphores.orbsDying--;
	}

	private IEnumerator CoToGrayScale()
	{
		do
		{
			yield return null;
			spriteRenderer.material.SetFloat("_EffectAmount", grayScaleTimer.Value);
			Helpers.SetSpriteAlpha(selectionAdd, 1f - grayScaleTimer.Value);
		}
		while (grayScaleTimer.Value < 1f);
	}

	private IEnumerator CoDeath()
	{
		do
		{
			yield return null;
			Helpers.SetSpriteAlpha(spriteRenderer, deathTimer.Value);
			Helpers.SetSpriteAlpha(selectionAdd, deathTimer.Value);
		}
		while (deathTimer.Value > float.Epsilon);
	}

	private void UpdateSelection()
	{
		Color color = selectionAdd.color;
		color.r = (color.g = (color.b = Mathf.SmoothStep(0f, 1f, addPowerOnSelect * selectionTimer.Value + (addPowerOnHighlight - addPowerOnSelect) * highlightTimer.Value)));
		selectionAdd.color = color;
		selectionAdd.enabled = (selectionTimer.Value > float.Epsilon);
	}

	private void UpdateMovement()
	{
		if (moving)
		{
			Vector3 a = Vector3.Lerp(source, destination, moveTimer.Value);
			float d = Mathf.Sin(moveTimer.Value * (float)Math.PI);
			a += offset * d;
			base.transform.position = a;
			if (moveTimer.Value >= 1f)
			{
				moving = false;
			}
		}
	}

	private void Update()
	{
		UpdateMovement();
		UpdateSelection();
	}
}
