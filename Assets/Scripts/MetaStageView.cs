using UnityEngine;
using UnityEngine.UI;
using Utils;

public class MetaStageView : MetaView
{
	[Header("Meta Stage")]
	public Text nameText;

	public Image iconImage;

	public Text stageNumText;

	public Button fightButton;

	public MetaItemView chanceItemView;

	public GameObject chanceItemRoot;

	public MetaStageDifficulty difficulty;

	public static MetaStageView expandedStageView;

	public Image[] stars = new Image[3];

	[Header("Rewards")]
	public Text rewardExpText;

	public Text rewardCoinsText;

	public new MetaStage GetObject()
	{
		return base.GetObject() as MetaStage;
	}

	protected override void OnInteract()
	{
		base.OnInteract();
		Toggle(this);
	}

	protected override void OnObjectReset()
	{
		base.OnObjectReset();
		Collapse(this, instant: true);
	}

	protected bool SetMaterial()
	{
		LocationWindow componentInParent = GetComponentInParent<LocationWindow>();
		Material material = (!componentInParent) ? null : componentInParent.lockedStageMaterial;
		MetaStage @object = GetObject();
		Image[] componentsInChildren = GetComponentsInChildren<Image>();
		Image[] array = componentsInChildren;
		foreach (Image image in array)
		{
			image.material = ((!@object.GetUnlocked(difficulty)) ? material : null);
		}
		return componentInParent != null;
	}

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
		MetaStage @object = GetObject();
		Helpers.SetText(nameText, @object.displayName);
		Helpers.SetImage(iconImage, @object.icon);
		MetaItem chanceItem = @object.GetChanceItem(difficulty);
		Helpers.SetActive(chanceItemRoot, chanceItem != null);
		if ((bool)chanceItemView)
		{
			chanceItemView.SetObject(chanceItem);
		}
		if ((bool)interactButton)
		{
			interactButton.enabled = @object.GetUnlocked(difficulty);
		}
		if ((bool)fightButton)
		{
			fightButton.enabled = @object.GetUnlocked(difficulty);
		}
		Helpers.SetText(rewardExpText, @object.GetExp(difficulty).ToString());
		Helpers.SetText(rewardCoinsText, @object.GetCoinsMin(difficulty) + "-" + @object.GetCoinsMax(difficulty));
		int starsShown = @object.GetStarsShown(difficulty);
		int num = @object.GetStars(difficulty);
		Helpers.SetActiveGameObject(stars[0], num > 0);
		Helpers.SetActiveGameObject(stars[1], num > 1);
		Helpers.SetActiveGameObject(stars[2], num > 2);
		if (num > 0 && starsShown == 0)
		{
			Image obj = stars[0];
			Color color = stars[0].color;
			float r = color.r;
			Color color2 = stars[0].color;
			float g = color2.g;
			Color color3 = stars[0].color;
			obj.color = new Color(r, g, color3.b, 0f);
			UITweenerEx.fadeIn(stars[0]);
		}
		if (num > 1 && starsShown <= 1)
		{
			Image obj2 = stars[1];
			Color color4 = stars[1].color;
			float r2 = color4.r;
			Color color5 = stars[1].color;
			float g2 = color5.g;
			Color color6 = stars[1].color;
			obj2.color = new Color(r2, g2, color6.b, 0f);
			UITweenerEx.fadeIn(stars[1]);
		}
		if (num > 2 && starsShown <= 2)
		{
			Image obj3 = stars[2];
			Color color7 = stars[2].color;
			float r3 = color7.r;
			Color color8 = stars[2].color;
			float g3 = color8.g;
			Color color9 = stars[2].color;
			obj3.color = new Color(r3, g3, color9.b, 0f);
			UITweenerEx.fadeIn(stars[2]);
		}
		@object.StarsShown(difficulty);
		if (!SetMaterial())
		{
			CanvasGroup component = GetComponent<CanvasGroup>();
			if ((bool)component)
			{
				component.alpha = ((!@object.GetUnlocked(difficulty)) ? 0.4f : 1f);
			}
		}
		Helpers.SetText(value: (base.transform.GetSiblingIndex() + 1).ToString(), target: stageNumText);
	}

	public static void CollapseAny(bool instant = false)
	{
		if (!(expandedStageView == null))
		{
			Collapse(expandedStageView, instant);
		}
	}

	public static void Collapse(MetaStageView view, bool instant = false)
	{
		if (expandedStageView != view)
		{
			return;
		}
		expandedStageView = null;
		Animator component = view.GetComponent<Animator>();
		if ((bool)component)
		{
			component.SetBool("Expanded", value: false);
			if (instant)
			{
				component.Update(10f);
			}
		}
	}

	public static void Expand(MetaStageView view, bool instant = false)
	{
		if (expandedStageView == view)
		{
			return;
		}
		CollapseAny(instant);
		expandedStageView = view;
		Animator component = view.GetComponent<Animator>();
		if ((bool)component)
		{
			component.SetBool("Expanded", value: true);
			if (instant)
			{
				component.Update(10f);
			}
		}
	}

	public static void Toggle(MetaStageView view, bool instant = false)
	{
		if (expandedStageView == view)
		{
			Collapse(view, instant);
		}
		else
		{
			Expand(view, instant);
		}
	}
}
