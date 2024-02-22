using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class ItemLevelUpWindow : MetaWindow
{
	public Text titleText;

	public float timeOffset;

	public float timeScale;

	public float highlightTime = 0.5f;

	public MetaItemView itemView;

	public SimpleItemStatsContainer statsContainer;

	public ItemProgressBar itemLevelBar;

	public Text levelText;

	public List<CanvasGroup> fadeInGroups;

	public CanvasGroup closeButton;

	[Header("Additive Group One")]
	public CanvasGroup addLayerOne;

	public Image addLevelBkgImage;

	public Image addBkgImage;

	public Image addItemImage;

	[Header("Additive Group Two")]
	public CanvasGroup addLayerTwo;

	[Header("SFX")]
	public AudioSample sfxStart;

	public AudioSample sfxExpBarReset;

	public AudioSample sfxStatAppear;

	public AudioSample sfxEnd;

	private bool showUpgradeStats;

	public new MetaItem GetObject()
	{
		return base.GetObject() as MetaItem;
	}

	[WindowTestMethod]
	public static void TestWindow()
	{
		ItemLevelUpWindowContext itemLevelUpWindowContext = new ItemLevelUpWindowContext();
		itemLevelUpWindowContext.item = Singleton<Meta>.Instance.FindRandomObject<MetaItem>();
		while (!itemLevelUpWindowContext.item.available)
		{
			itemLevelUpWindowContext.item = Singleton<Meta>.Instance.FindRandomObject<MetaItem>();
		}
		itemLevelUpWindowContext.newLevel = 4;
		Singleton<WindowManager>.Instance.OpenWindow<ItemLevelUpWindow>(itemLevelUpWindowContext);
	}

	protected virtual void OnEnable()
	{
		ItemLevelUpWindowContext itemLevelUpWindowContext = base.context as ItemLevelUpWindowContext;
		if (itemLevelUpWindowContext != null)
		{
			InitWithContext(itemLevelUpWindowContext);
		}
	}

	private void InitWithContext(ItemLevelUpWindowContext context)
	{
		SetObject(context.item);
		showUpgradeStats = true;
		StartCoroutine(upgradeItemSequence());
	}

	protected override void OnChanged()
	{
		base.OnChanged();
		MetaItem @object = GetObject();
		Helpers.SetImage(addLevelBkgImage, SingletonComponent<Meta, MetaConsts>.Instance.bkgLvlByRarity[(int)@object.rarity]);
		Helpers.SetImage(addBkgImage, SingletonComponent<Meta, MetaConsts>.Instance.bkgByRarity[(int)@object.rarity]);
		Helpers.SetImage(addItemImage, @object.icon);
		int newLevel = (base.context as ItemLevelUpWindowContext).newLevel;
		Helpers.SetText(levelText, "Level " + (newLevel - 1).ToString());
		if ((bool)itemView)
		{
			itemView.SetObject(@object);
			itemView.transform.localScale = Vector3.one;
		}
		Helpers.SetText(titleText, @object.displayName);
		if ((bool)statsContainer)
		{
			statsContainer.SetItem(@object, showUpgradeStats: true, newLevel - 1);
		}
		int cardsNumRequiredToUpdate = MetaItem.GetCardsNumRequiredToUpdate(newLevel, @object.rarity);
		if ((bool)itemLevelBar)
		{
			itemLevelBar.SetProgress(1f, cardsNumRequiredToUpdate.ToString() + "/" + cardsNumRequiredToUpdate.ToString());
		}
		foreach (CanvasGroup fadeInGroup in fadeInGroups)
		{
			if ((bool)fadeInGroup)
			{
				fadeInGroup.alpha = 0f;
			}
		}
		if ((bool)closeButton)
		{
			closeButton.alpha = 0f;
		}
	}

	public IEnumerator upgradeItemSequence()
	{
		yield return new WaitForEndOfFrame();
		int newLevel = (base.context as ItemLevelUpWindowContext).newLevel;
		Helpers.SetText(levelText, "Level " + (newLevel - 1).ToString());
		AudioManager.PlaySafe(sfxStart);
		foreach (CanvasGroup fadeInGroup in fadeInGroups)
		{
			if ((bool)fadeInGroup)
			{
				fadeInGroup.alpha = 0f;
			}
		}
		foreach (CanvasGroup group in fadeInGroups)
		{
			if ((bool)group)
			{
				UITweenerEx.fadeIn(group, 0.3f);
				yield return new WaitForSeconds(0.15f);
			}
		}
		float levelBarCountingDownTimeMin = 0.5f + timeScale;
		float levelBarCountingDownTimeMax = 1.25f + timeScale;
		float itemBumpTime = highlightTime;
		float scaleBump = 0.2f;
		float targetScale = 1.2f;
		int cardsRequired = MetaItem.GetCardsNumRequiredToUpdate(rarity: GetObject().rarity, newLevel: newLevel);
		float levelBarCountingDownTime = Mathf.Lerp(levelBarCountingDownTimeMin, levelBarCountingDownTimeMax, Mathf.Clamp01(Mathf.Sqrt((float)cardsRequired * 1f - 1f) / 10f));
		AudioManager.PlaySafe(sfxExpBarReset);
		float startTime = Time.time;
		float cardStartTime = Time.time + levelBarCountingDownTime - itemBumpTime + timeOffset;
		int cardsRequiredTotal = cardsRequired;
		string cardsRequiredTotalText = "/" + cardsRequired.ToString();
		while (Time.time < startTime + levelBarCountingDownTime + timeOffset)
		{
			float t = Mathf.Min(1f, (Time.time - startTime) / levelBarCountingDownTime);
			itemLevelBar.SetProgress(1f - t, ((int)((float)cardsRequiredTotal * (1f - t))).ToString() + cardsRequiredTotalText);
			addLayerOne.alpha = t;
			Vector3 target_scale = Vector3.Lerp(Vector3.one, Vector3.one * targetScale, t);
			target_scale.z = 1f;
			itemView.transform.localScale = target_scale;
			if (Time.time >= cardStartTime && (bool)itemView)
			{
				float num = (Time.time - cardStartTime) / itemBumpTime;
				float d = Helpers.simple_bounce3(num);
				Vector3 vector = Vector3.one * d * scaleBump;
				addLayerTwo.alpha = num * 0.8f;
			}
			yield return null;
		}
		UITweenerEx.fadeOut(addLayerOne, 0.1f);
		UITweenerEx.fadeOut(addLayerTwo, 0.1f);
		if ((bool)itemView && (bool)itemView.levelText)
		{
			itemView.levelText.GetComponent<CanvasGroup>().alpha = 0f;
			itemView.levelText.transform.localScale = Vector3.one * 2.5f;
			Helpers.SetText(levelText, "Level " + newLevel.ToString());
			UITweenerEx.fadeIn(itemView.levelText, 0.3f);
			UITweenerEx.scaleTo(itemView.levelText, 1f, 0.3f);
			yield return new WaitForSeconds(0.2f);
		}
		itemLevelBar.SetProgress(0f, "0" + cardsRequiredTotalText);
		Vector3 scale = Vector3.one * targetScale;
		scale.z = 1f;
		itemView.transform.localScale = scale;
		if ((bool)statsContainer)
		{
			yield return StartCoroutine(statsContainer.upgradeItemSequence(sfxStatAppear));
		}
		UITweenerEx.fadeIn(closeButton, 0.25f);
		yield return new WaitForSeconds(0.25f);
		AudioManager.PlaySafe(sfxEnd);
	}
}
