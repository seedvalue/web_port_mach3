using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public class M3SkillView : MetaSkillView, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IEventSystemHandler
{
	public delegate void OnSkillChosen(M3SkillView itemView);

	[Header("M3 Skill View")]
	public Material inactiveMaterial;

	public Text cooldownCounter;

	public Image inactiveFrame;

	public float inactiveDarkFactor = 0.5f;

	public Image activeFrame;

	public Image activeAdd;

	public float addAlphaBase = 0.8f;

	public float addAlphaFrequency = 1f;

	public float inactiveScale = 0.8f;

	public AnimationCurve scaleOnActivate;

	public AnimationCurve scaleOnCooldownChange;

	public float skillTapNHoldDelay = 0.4f;

	public float fadeDuration = 0.25f;

	public float cooldownChangeDuration = 0.4f;

	public GameObject fxOnActivatePrefab;

	public AudioSample sfxOnActivate;

	[HideInInspector]
	public MetaSkill skill;

	private Material baseMaterial;

	private int cooldown;

	private M3InterpolatedFloat slider;

	private IEnumerator toggleActive;

	private M3SkillPreviewWindow skillPreviewWindow;

	private bool pointerDown;

	private int ignoreClick;

	private float pointerDownTime;

	private int itemIndex = -1;

	private float addTimeStart;

	private M3TileManager tileManager;

	private OnSkillChosen onSkillChosen;

	public int Cooldown
	{
		get
		{
			return cooldown;
		}
		set
		{
			cooldown = value;
			SetSkillActive(value == 0 && (bool)skill);
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (tileManager.TutorialAllows(M3TutorialActivity.ItemUse, itemIndex) && tileManager.IsInteractionAllowed() && IsSkillActive() && ignoreClick == 0)
		{
			tileManager.OpenWindow<M3SkillWindow>(skill, OnM3SkillWindowClose);
			tileManager.TutorialActivityFinished(M3TutorialActivity.ItemUseShowOnly);
		}
		else if (ignoreClick == 0)
		{
			AudioManager.PlaySafe(M3Board.instance.sounds.skillInactive);
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (tileManager.IsInteractionAllowed())
		{
			pointerDown = true;
			pointerDownTime = 0f;
			ignoreClick = 0;
		}
	}

	private void CloseItemWindow()
	{
		if ((bool)skillPreviewWindow)
		{
			skillPreviewWindow.CloseWindow();
		}
		pointerDown = false;
		skillPreviewWindow = null;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if ((bool)skillPreviewWindow)
		{
			ignoreClick = 2;
		}
		CloseItemWindow();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		CloseItemWindow();
	}

	private void OnM3SkillWindowClose(Window window, object returnValue)
	{
		if (returnValue != null && onSkillChosen != null)
		{
			onSkillChosen(this);
		}
	}

	private void OnItemWindowClose(Window window, object returnValue)
	{
		tileManager.TutorialActivityFinished(M3TutorialActivity.ItemPreview);
	}

	private int FindSkillIndex()
	{
		M3Player m3Player = UnityEngine.Object.FindObjectOfType<M3Player>();
		for (int i = 0; i < m3Player.Skills.Count; i++)
		{
			if (m3Player.Skills[i] == this)
			{
				return i;
			}
		}
		return -1;
	}

	public void Init()
	{
		tileManager = M3TileManager.instance;
		if ((bool)iconImage)
		{
			baseMaterial = iconImage.material;
		}
		Transform transform = base.transform;
		Vector3 localPosition = base.transform.localPosition;
		float x = localPosition.x;
		Vector3 localPosition2 = base.transform.localPosition;
		transform.localPosition = new Vector3(x, localPosition2.y, 0f);
		slider = new M3InterpolatedFloat(this, M3InterpolationMode.Linear);
		skill = GetObject();
		itemIndex = FindSkillIndex();
		if ((bool)skill)
		{
			cooldown = Mathf.Max(1, skill.cooldown);
		}
		else
		{
			cooldown = 0;
		}
		SetSkillActive(active: false);
	}

	protected void Update()
	{
		if (pointerDown && !skillPreviewWindow)
		{
			if (pointerDownTime >= skillTapNHoldDelay && tileManager.TutorialAllows(M3TutorialActivity.ItemPreview, itemIndex))
			{
				skillPreviewWindow = tileManager.OpenWindow<M3SkillPreviewWindow>(this, OnItemWindowClose);
				ignoreClick = -1;
			}
			pointerDownTime += Time.deltaTime;
		}
		if (ignoreClick > 0)
		{
			ignoreClick--;
		}
		if ((bool)cooldownCounter)
		{
			cooldownCounter.text = Cooldown.ToString();
		}
		if (IsSkillActive() && (bool)activeAdd)
		{
			Helpers.SetImageAlpha(activeAdd, addAlphaBase + Mathf.Sin((float)Math.PI * 2f * (Time.time - addTimeStart) * addAlphaFrequency) * (1f - addAlphaBase));
		}
	}

	public IEnumerator ExecuteSkill(M3Battle battle, M3Board board, M3Player player)
	{
		cooldown = skill.cooldown;
		SetSkillActive(active: false);
		yield return StartCoroutine(skill.Execute(battle, board, player));
	}

	public void NewTurn()
	{
		if (cooldown > 0 && (bool)skill && base.gameObject.activeInHierarchy)
		{
			cooldown--;
			if (cooldown == 0)
			{
				SetSkillActive(active: true);
			}
			else
			{
				StartCoroutine(CoDecCooldown(cooldownChangeDuration));
			}
		}
	}

	private IEnumerator CoDecCooldown(float duration)
	{
		M3InterpolatedFloat sliderCooldown = new M3InterpolatedFloat(this, M3InterpolationMode.Linear);
		sliderCooldown.SlideUp(duration, adjustTime: false, forceFullCycle: true);
		do
		{
			yield return null;
			float time = sliderCooldown.Value;
			float scale = scaleOnCooldownChange.Evaluate(time);
			cooldownCounter.transform.localScale = new Vector3(scale, scale, 1f);
		}
		while (sliderCooldown.Value < 1f);
	}

	public void SetSkillActive(bool active)
	{
		if (toggleActive != null)
		{
			StopCoroutine(toggleActive);
		}
		if (active)
		{
			Helpers.SetImageMaterial(iconImage, baseMaterial);
			slider.SlideUp(fadeDuration, adjustTime: false, forceFullCycle: true);
			toggleActive = CoToggleActive(active: true, baseMaterial);
			StartCoroutine(toggleActive);
			if ((bool)fxOnActivatePrefab)
			{
				UnityEngine.Object.Instantiate(fxOnActivatePrefab, base.transform.position, base.transform.rotation);
			}
			AudioManager.PlaySafe(sfxOnActivate);
		}
		else
		{
			slider.SlideDown(fadeDuration, adjustTime: false, forceFullCycle: true);
			toggleActive = CoToggleActive(active: false, inactiveMaterial);
			StartCoroutine(toggleActive);
		}
	}

	private IEnumerator CoToggleActive(bool active, Material finalMaterial = null)
	{
		do
		{
			yield return null;
			float time = slider.Value;
			float darken = 1f - inactiveDarkFactor + inactiveDarkFactor * time;
			Helpers.SetImageAlpha(inactiveFrame, 1f - time);
			Helpers.SetImageAlpha(activeFrame, time);
			Helpers.SetImageColor(iconImage, new Color(darken, darken, darken));
			Helpers.SetTextAlpha(cooldownCounter, 1f - time);
			Helpers.SetImageAlpha(activeAdd, time * addAlphaBase);
			float scale = scaleOnActivate.Evaluate(time);
			base.transform.localScale = new Vector3(scale, scale, 1f);
		}
		while ((active && slider.Value < 1f) || (!active && slider.Value > float.Epsilon));
		if ((bool)finalMaterial)
		{
			Helpers.SetImageMaterial(iconImage, finalMaterial);
		}
		if (active)
		{
			addTimeStart = Time.time;
		}
		toggleActive = null;
	}

	public bool IsSkillActive()
	{
		return cooldown == 0 && (bool)skill;
	}

	public void SetOnSkillChosen(OnSkillChosen handler)
	{
		onSkillChosen = handler;
	}
}
