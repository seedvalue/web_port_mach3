using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class LevelUpWindow : MetaWindow
{
	public UIProgressBar expBar;

	public Text levelText;

	public Text levelTextAnim;

	public SimpleItemStatsContainer statsContainer;

	public CanvasGroup titleGroup;

	public CanvasGroup avatarGroup;

	public CanvasGroup closeButton;

	[Header("Unlocked")]
	public CanvasGroup unlockedGroup;

	public Image unlockedIcon;

	public Text unlockedText;

	[Header("ChestLevel")]
	public CanvasGroup chestGroup;

	public Text chestText;

	[Header("Rewards")]
	public CanvasGroup rewardsGroup;

	public Text rewardText;

	[Header("Additive Group One")]
	public CanvasGroup addLayerOne;

	[Header("Additive Group Two")]
	public CanvasGroup addLayerTwo;

	[Header("SFX")]
	public AudioSample sfxStart;

	public AudioSample sfxExpBarReset;

	public AudioSample sfxExpToLevelIcon;

	public AudioSample sfxStatAppear;

	public AudioSample sfxChestsInfo;

	public AudioSample sfxRewardsInfo;

	public new MetaPlayer GetObject()
	{
		return base.GetObject() as MetaPlayer;
	}

	[WindowTestMethod]
	public static void TestWindow()
	{
		LevelUpWindowContext levelUpWindowContext = new LevelUpWindowContext();
		levelUpWindowContext.player = MetaPlayer.local;
		levelUpWindowContext.newLevel = 4;
		levelUpWindowContext.exp = 1000;
		levelUpWindowContext.stats = Stats.RandomizeHPandRCV() * 2f;
		levelUpWindowContext.newStats = levelUpWindowContext.stats * 2f;
		levelUpWindowContext.unlockedText = "Dupa unlocked";
		levelUpWindowContext.rewardCoins = 1000;
		levelUpWindowContext.rewardGems = 100;
		Singleton<WindowManager>.Instance.OpenWindow<LevelUpWindow>(levelUpWindowContext);
	}

	protected virtual void OnEnable()
	{
		LevelUpWindowContext levelUpWindowContext = base.context as LevelUpWindowContext;
		if (levelUpWindowContext != null)
		{
			InitWithContext(levelUpWindowContext);
		}
	}

	private void InitWithContext(LevelUpWindowContext context)
	{
		SetObject(context.player);
		StartCoroutine(upgradeItemSequence());
	}

	protected override void OnChanged()
	{
		base.OnChanged();
		LevelUpWindowContext levelUpWindowContext = base.context as LevelUpWindowContext;
		MetaPlayer @object = GetObject();
		if ((bool)statsContainer)
		{
			statsContainer.SetStats(levelUpWindowContext.stats, levelUpWindowContext.newStats, showUpgradeStats: true);
		}
		if ((bool)expBar)
		{
			expBar.SetValue(levelUpWindowContext.exp, levelUpWindowContext.exp);
		}
		if (levelUpWindowContext.unlockedText != string.Empty)
		{
			Helpers.SetActiveGameObject(unlockedGroup, value: true);
			Helpers.SetImage(unlockedIcon, levelUpWindowContext.unlockedIcon);
			Helpers.SetText(unlockedText, levelUpWindowContext.unlockedText);
		}
		else
		{
			Helpers.SetActiveGameObject(unlockedGroup, value: false);
		}
		string text = string.Empty;
		if (levelUpWindowContext.rewardGems > 0)
		{
			string text2 = text;
			text = text2 + MetaResource.gems.quadText + "x" + levelUpWindowContext.rewardGems + "\n";
		}
		if (levelUpWindowContext.rewardCoins > 0)
		{
			string text2 = text;
			text = text2 + MetaResource.coins.quadText + "x" + levelUpWindowContext.rewardCoins + "\n";
		}
		Helpers.SetText(rewardText, text);
		Helpers.SetText(chestText, levelUpWindowContext.newLevel.ToString());
		titleGroup.alpha = 0f;
		avatarGroup.alpha = 0f;
		rewardsGroup.alpha = 0f;
		chestGroup.alpha = 0f;
		unlockedGroup.alpha = 0f;
		closeButton.alpha = 0f;
	}

	public IEnumerator upgradeItemSequence()
	{
		yield return new WaitForEndOfFrame();
		LevelUpWindowContext lvlUpContext = base.context as LevelUpWindowContext;
		Helpers.SetText(levelText, (lvlUpContext.newLevel - 1).ToString());
		Helpers.SetText(levelTextAnim, (lvlUpContext.newLevel - 1).ToString());
		AudioManager.PlaySafe(sfxStart);
		titleGroup.alpha = 0f;
		avatarGroup.alpha = 0f;
		UITweenerEx.fadeIn(titleGroup, 0.3f);
		yield return new WaitForSeconds(0.2f);
		UITweenerEx.fadeIn(avatarGroup, 0.3f);
		yield return new WaitForSeconds(0.2f);
		if ((bool)levelTextAnim)
		{
			levelTextAnim.transform.localScale = Vector3.one;
			levelTextAnim.color = Color.white;
		}
		if ((bool)unlockedGroup)
		{
			unlockedGroup.alpha = 0f;
		}
		if ((bool)rewardsGroup)
		{
			rewardsGroup.alpha = 0f;
		}
		if ((bool)chestGroup)
		{
			chestGroup.alpha = 0f;
		}
		if ((bool)rewardText)
		{
			rewardText.GetComponent<CanvasGroup>().alpha = 0f;
		}
		yield return new WaitForSeconds(0.25f);
		float levelBarCountingDownTimeMin = 0.5f;
		float levelBarCountingDownTimeMax = 1.25f;
		float itemBumpTime = 0.5f;
		float scaleBump = 0.2f;
		float targetScale = 1.2f;
		float levelBarCountingDownTime = Mathf.Lerp(levelBarCountingDownTimeMin, levelBarCountingDownTimeMax, Mathf.Clamp01(Mathf.Sqrt((float)lvlUpContext.exp * 1f - 1f) / 10f));
		float startTime = Time.time;
		float cardStartTime = Time.time + levelBarCountingDownTime - itemBumpTime;
		AudioManager.PlaySafe(sfxExpBarReset);
		while (Time.time < startTime + levelBarCountingDownTime)
		{
			float t = (Time.time - startTime) / levelBarCountingDownTime;
			expBar.SetProgress(1f - t);
			addLayerOne.alpha = t;
			Vector3 target_scale = Vector3.Lerp(Vector3.one, Vector3.one * targetScale, t);
			target_scale.z = 1f;
			if (Time.time >= cardStartTime)
			{
				float num = (Time.time - cardStartTime) / itemBumpTime;
				float d = Helpers.simple_bounce3(num);
				Vector3 vector = Vector3.one * d * scaleBump;
				addLayerTwo.alpha = num * 0.8f;
			}
			yield return null;
		}
		Helpers.SetText(levelText, lvlUpContext.newLevel.ToString());
		Helpers.SetText(levelTextAnim, lvlUpContext.newLevel.ToString());
		UITweenerEx.scaleTo(levelTextAnim, 4f);
		UITweenerEx.fadeOut(levelTextAnim);
		UITweenerEx.fadeOut(addLayerOne, 0.1f);
		UITweenerEx.fadeOut(addLayerTwo, 0.1f);
		float scaleTime = 0.25f;
		AudioManager.PlaySafe(sfxExpToLevelIcon);
		if ((bool)unlockedGroup)
		{
			UITweenerEx.fadeIn(unlockedGroup, scaleTime);
			yield return new WaitForSeconds(scaleTime);
		}
		expBar.SetProgress(0f);
		Vector3 scale = Vector3.one * targetScale;
		scale.z = 1f;
		if ((bool)statsContainer)
		{
			yield return StartCoroutine(statsContainer.upgradeItemSequence(sfxStatAppear));
		}
		yield return new WaitForSeconds(scaleTime / 2f);
		if ((bool)chestGroup)
		{
			AudioManager.PlaySafe(sfxChestsInfo);
			UITweenerEx.fadeIn(chestGroup, scaleTime);
			yield return new WaitForSeconds(scaleTime * 1.5f);
		}
		AudioManager.PlaySafe(sfxRewardsInfo);
		if ((bool)rewardsGroup)
		{
			UITweenerEx.fadeIn(rewardsGroup, scaleTime);
			yield return new WaitForSeconds(scaleTime / 2f);
			UITweenerEx.fadeIn(rewardText, scaleTime);
		}
		yield return new WaitForSeconds(scaleTime / 2f);
		UITweenerEx.fadeIn(closeButton, scaleTime / 2f);
	}
}
