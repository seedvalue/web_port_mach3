using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class MetaRewardView : MetaView
{
	[Header("Reward View")]
	public Text labelText;

	public Image iconImage;

	public MetaItemView prefabItemView;

	public MetaResourceView prefabResourceView;

	public AudioSample sfxCardOut;

	public AudioSample[] sfxCardFrontByRarity = new AudioSample[4];

	public AudioSample sfxCardCountIncrease;

	[HideInInspector]
	public bool allowSkip;

	private MetaView createdView;

	[Header("Animation")]
	public float animationDelay = 0.25f;

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
		MetaObject @object = GetObject();
		Helpers.SetImage(iconImage, GetProperty<Sprite>("icon"));
		int propertyOrDefault = GetPropertyOrDefault<int>("count");
		if (propertyOrDefault > 1 && (bool)(GetObject() as MetaResource))
		{
			Helpers.SetText(labelText, "+" + propertyOrDefault);
		}
		else
		{
			Helpers.SetText(labelText, GetProperty<string>("displayName"));
		}
		if ((bool)createdView)
		{
			Hide();
		}
		if ((bool)(@object as MetaItem) && (bool)prefabItemView)
		{
			createdView = Object.Instantiate(prefabItemView, base.transform);
		}
		else if ((bool)(@object as MetaResource) && (bool)prefabResourceView)
		{
			createdView = Object.Instantiate(prefabResourceView, base.transform);
		}
		if ((bool)createdView)
		{
			createdView.SetLink(GetLink());
			createdView.transform.localScale = Vector3.one;
			createdView.transform.localPosition = new Vector3(0f, -250f, 0f);
			StartCoroutine(Show());
		}
	}

	private IEnumerator Show()
	{
		allowSkip = true;
		MetaObject obj = GetObject();
		if ((bool)(obj as MetaItem))
		{
			MetaItem item = obj as MetaItem;
			MetaItemView view2 = createdView as MetaItemView;
			Helpers.SetTextAlpha(view2.nameText, 0f);
			Helpers.SetTextAlpha(view2.countText, 0f);
			Helpers.SetTextAlpha(view2.rarityText, 0f);
			view2.GetComponent<CanvasGroup>().alpha = 0f;
			view2.transform.localScale = Vector3.one * 0.5f;
			bool justFound = GetPropertyOrDefault<bool>("justFound");
			if (justFound)
			{
				CanvasGroup component = view2.justFoundGroup.GetComponent<CanvasGroup>();
				if ((bool)component)
				{
					component.alpha = 0f;
				}
			}
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			while (view2.itemLevelBar == null)
			{
				yield return null;
			}
			CanvasGroup canvas = view2.itemLevelBar.GetComponent<CanvasGroup>();
			if ((bool)canvas)
			{
				canvas.alpha = 0f;
			}
			AudioManager.PlaySafe(sfxCardOut);
			UITweenerEx.flyTo(view2, Vector3.zero, 0.5f, destroyOnEnd: true, time: true, local: true, smooth_step: true);
			UITweenerEx.fadeIn(view2, 0.4f);
			UITweenerEx.scaleIn(view2);
			UITweenerEx.flip(view2.bkgImage, 1f);
			if ((bool)view2.epicParticleSystem && view2.epicParticleSystem.gameObject.activeSelf)
			{
				UITweenerEx.flip(view2.epicParticleSystem, 1f);
			}
			if ((bool)view2.legendaryParticleSystem && view2.legendaryParticleSystem.gameObject.activeSelf)
			{
				UITweenerEx.flip(view2.legendaryParticleSystem, 1f);
			}
			yield return new WaitForSeconds(0.5f);
			AudioManager.PlaySafe(sfxCardFrontByRarity[(int)item.rarity]);
			UITweenerEx.fadeIn(view2.rarityText, 0.25f);
			yield return new WaitForSeconds(0.15f);
			UITweenerEx.fadeIn(view2.nameText, 0.25f);
			yield return new WaitForSeconds(0.15f);
			if (justFound)
			{
				Helpers.SetText(view2.countText, "Unlocked!");
				UITweenerEx.fadeIn(view2.justFoundGroup, 0.25f);
			}
			yield return new WaitForSeconds(0.15f);
			AudioManager.PlaySafe(sfxCardCountIncrease);
			yield return new WaitForSeconds(0.1f);
			float levelBarCountingDownTimeMin = 0.5f;
			float levelBarCountingDownTimeMax = 1.25f;
			int count = GetPropertyOrDefault<int>("count");
			int totalCount = view2.GetObject().count;
			int startCount = totalCount - count;
			int requiredCardsNum = view2.GetObject().GetCardsNumRequiredToUpdate();
			float levelBarCountingDownTime = Mathf.Lerp(levelBarCountingDownTimeMin, levelBarCountingDownTimeMax, Mathf.Clamp01(Mathf.Sqrt((float)count * 1f - 1f) / 10f));
			if (totalCount > 0)
			{
				UITweenerEx.fadeIn(view2.itemLevelBar, 0.25f);
				view2.itemLevelBar.SetProgress((float)startCount / (float)requiredCardsNum, startCount + "/" + requiredCardsNum.ToString());
				yield return CoroutineHelper.AnimateInTime(levelBarCountingDownTime, delegate(float t)
				{
					int num = (int)((float)startCount + t * (float)count);
					view2.itemLevelBar.SetProgress((float)num / (float)requiredCardsNum, num + "/" + requiredCardsNum.ToString());
				});
			}
			UITweenerEx.fadeIn(view2.countText, 0.25f);
		}
		else if (obj is MetaResource)
		{
			allowSkip = false;
			MetaResourceView view = createdView as MetaResourceView;
			Helpers.SetTextAlpha(view.nameText, 0f);
			Helpers.SetTextAlpha(view.countText, 0f);
			view.GetComponent<CanvasGroup>().alpha = 0f;
			view.transform.localScale = Vector3.one * 0.5f;
			OpenChestWindow window = GetComponentInParent<OpenChestWindow>();
			MetaChest chest = window.GetObject();
			float delay = (!chest.metaID.Contains("Eternal")) ? 0f : 1.6f;
			yield return new WaitForSeconds(animationDelay + delay);
			AudioManager.PlaySafe(sfxCardOut);
			UITweenerEx.flyTo(createdView, Vector3.zero, 0.5f, destroyOnEnd: true, time: true, local: true, smooth_step: true);
			UITweenerEx.fadeIn(view, 0.4f);
			UITweenerEx.scaleIn(view);
			UITweenerEx.flip(view.cardImage.transform.parent, 1f);
			yield return new WaitForSeconds(0.6f);
			UITweenerEx.fadeIn(view.nameText, 0.25f);
			yield return new WaitForSeconds(0.2f);
			UITweenerEx.fadeIn(view.countText, 0.25f);
		}
		allowSkip = true;
	}

	private float Hide()
	{
		UITweenerEx.fadeOut(createdView);
		return 0.15f;
	}
}
