using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class LocationWindow : MetaWindow
{
	public Text locationNameText;

	public MetaStageView stagePrefab;

	public MetaContainer stageContainer;

	public ScrollRect stageScroll;

	public Button easyButton;

	public Button mediumButton;

	public Button hardButton;

	public Material lockedStageMaterial;

	public Image locationBkgImage;

	[Header("ProgressBar")]
	public Image progressBar;

	[Header("Stars")]
	public Text starsTotalCount;

	public Text starsBronzeCount;

	public Text starsSilverCount;

	public Text starsGoldCount;

	[Header("Reward")]
	public Text rewardBronzeCount;

	public Text rewardSilverCount;

	public Text rewardGoldCount;

	public Button rewardBronzeButton;

	public Button rewardSilverButton;

	public Button rewardGoldButton;

	public Image rewardBronzeMarker;

	public Image rewardSilverMarker;

	public Image rewardGoldMarker;

	public GameObject rewardBronzeGlow;

	public GameObject rewardSilverGlow;

	public GameObject rewardGoldGlow;

	[Header("Medal")]
	public GameObject medalBronzeEasy;

	public GameObject medalSilverEasy;

	public GameObject medalGoldEasy;

	public GameObject medalBronzeMedium;

	public GameObject medalSilverMedium;

	public GameObject medalGoldMedium;

	public GameObject medalBronzeHard;

	public GameObject medalSilverHard;

	public GameObject medalGoldHard;

	protected bool slotsFullWarning = true;

	public MetaStageDifficulty currentDifficulty
	{
		get;
		private set;
	}

	[WindowTestMethod]
	public static void TestWindow()
	{
		LocationWindowContext locationWindowContext = new LocationWindowContext();
		locationWindowContext.location = Singleton<Meta>.Instance.FindRandomObject<MetaLocation>();
		Singleton<WindowManager>.Instance.OpenWindow<LocationWindow>(locationWindowContext);
	}

	public new MetaLocation GetObject()
	{
		return base.GetObject() as MetaLocation;
	}

	protected virtual void OnEnable()
	{
		LocationWindowContext locationWindowContext = base.context as LocationWindowContext;
		if (locationWindowContext != null)
		{
			InitWithContext(locationWindowContext);
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		stageContainer.Clear();
	}

	protected override void Start()
	{
		base.Start();
		Helpers.AddListenerOnClick(easyButton, delegate
		{
			OnDifficultyClicked(MetaStageDifficulty.Easy);
		});
		Helpers.AddListenerOnClick(mediumButton, delegate
		{
			OnDifficultyClicked(MetaStageDifficulty.Medium);
		});
		Helpers.AddListenerOnClick(hardButton, delegate
		{
			OnDifficultyClicked(MetaStageDifficulty.Hard);
		});
		Helpers.AddListenerOnClick(rewardBronzeButton, OnCollectBronzeRewardClicked);
		Helpers.AddListenerOnClick(rewardSilverButton, OnCollectSilverRewardClicked);
		Helpers.AddListenerOnClick(rewardGoldButton, OnCollectGoldRewardClicked);
	}

	private void InitWithContext(LocationWindowContext context)
	{
		SetObject(context.location);
		if (context.location != null)
		{
			Helpers.SetText(locationNameText, context.location.displayName);
			OnDifficultyClicked(context.location.currentDifficultyLevel);
			Helpers.SetImage(locationBkgImage, context.location.bkgImage);
		}
	}

	private void PrepareRewardsButtons(int collectedStars)
	{
		MetaLocation @object = GetObject();
		Helpers.SetActiveGameObject(rewardBronzeButton, @object.IsRewardAvailable(currentDifficulty, collectedStars, MetaLocationState.Bronze));
		Helpers.SetActiveGameObject(rewardSilverButton, @object.IsRewardAvailable(currentDifficulty, collectedStars, MetaLocationState.Silver));
		Helpers.SetActiveGameObject(rewardGoldButton, @object.IsRewardAvailable(currentDifficulty, collectedStars, MetaLocationState.Gold));
		Helpers.SetActive(rewardBronzeGlow, @object.IsRewardAvailable(currentDifficulty, collectedStars, MetaLocationState.Bronze));
		Helpers.SetActive(rewardSilverGlow, @object.IsRewardAvailable(currentDifficulty, collectedStars, MetaLocationState.Silver));
		Helpers.SetActive(rewardGoldGlow, @object.IsRewardAvailable(currentDifficulty, collectedStars, MetaLocationState.Gold));
		Helpers.SetActiveGameObject(rewardBronzeMarker, @object.IsRewardCollected(currentDifficulty, MetaLocationState.Bronze));
		Helpers.SetActiveGameObject(rewardSilverMarker, @object.IsRewardCollected(currentDifficulty, MetaLocationState.Silver));
		Helpers.SetActiveGameObject(rewardGoldMarker, @object.IsRewardCollected(currentDifficulty, MetaLocationState.Gold));
	}

	private void OnDifficultyClicked(MetaStageDifficulty difficulty)
	{
		MetaLocation @object = GetObject();
		currentDifficulty = difficulty;
		Helpers.SetText(starsTotalCount, @object.GetCollectedStars(currentDifficulty, onlyShown: true).ToString() + "/" + @object.totalStars.ToString());
		Helpers.SetText(starsBronzeCount, @object.GetRequiredStars(MetaLocationState.Bronze).ToString());
		Helpers.SetText(starsSilverCount, @object.GetRequiredStars(MetaLocationState.Silver).ToString());
		Helpers.SetText(starsGoldCount, @object.GetRequiredStars(MetaLocationState.Gold).ToString());
		Helpers.SetActive(medalBronzeEasy, difficulty == MetaStageDifficulty.Easy);
		Helpers.SetActive(medalSilverEasy, difficulty == MetaStageDifficulty.Easy);
		Helpers.SetActive(medalGoldEasy, difficulty == MetaStageDifficulty.Easy);
		Helpers.SetActive(medalBronzeMedium, difficulty == MetaStageDifficulty.Medium);
		Helpers.SetActive(medalSilverMedium, difficulty == MetaStageDifficulty.Medium);
		Helpers.SetActive(medalGoldMedium, difficulty == MetaStageDifficulty.Medium);
		Helpers.SetActive(medalBronzeHard, difficulty == MetaStageDifficulty.Hard);
		Helpers.SetActive(medalSilverHard, difficulty == MetaStageDifficulty.Hard);
		Helpers.SetActive(medalGoldHard, difficulty == MetaStageDifficulty.Hard);
		Helpers.SetActiveGameObject(rewardBronzeMarker, @object.IsRewardCollected(currentDifficulty, MetaLocationState.Bronze));
		Helpers.SetActiveGameObject(rewardSilverMarker, @object.IsRewardCollected(currentDifficulty, MetaLocationState.Silver));
		Helpers.SetActiveGameObject(rewardGoldMarker, @object.IsRewardCollected(currentDifficulty, MetaLocationState.Gold));
		Helpers.SetText(rewardBronzeCount, "x" + @object.GetRewardValue(currentDifficulty, MetaLocationState.Bronze));
		Helpers.SetText(rewardSilverCount, "x" + @object.GetRewardValue(currentDifficulty, MetaLocationState.Silver));
		Helpers.SetText(rewardGoldCount, "x" + @object.GetRewardValue(currentDifficulty, MetaLocationState.Gold));
		if ((bool)progressBar)
		{
			progressBar.fillAmount = @object.GetProgress(currentDifficulty, onlyShown: true);
		}
		int collectedStars = @object.GetCollectedStars(currentDifficulty, onlyShown: true);
		int collectedStars2 = @object.GetCollectedStars(currentDifficulty);
		if (collectedStars != collectedStars2)
		{
			StartCoroutine(CollectProgress());
		}
		else
		{
			PrepareRewardsButtons(collectedStars2);
		}
		List<MetaStage> stages = GetObject().stages;
		stageContainer.Assign(stages, stagePrefab);
		MetaStageView.expandedStageView = null;
		foreach (MetaView content in stageContainer.contents)
		{
			MetaStageView stageView = content as MetaStageView;
			stageView.difficulty = difficulty;
			if ((bool)stageView.fightButton)
			{
				stageView.fightButton.onClick.AddListener(delegate
				{
					OnStageClicked(stageView);
				});
			}
		}
		if ((bool)stageScroll)
		{
			stageScroll.verticalNormalizedPosition = 0f;
		}
		Image component = easyButton.GetComponent<Image>();
		Image component2 = mediumButton.GetComponent<Image>();
		Image component3 = hardButton.GetComponent<Image>();
		Helpers.SetActiveMonoBehaviour(component, currentDifficulty == MetaStageDifficulty.Easy);
		Helpers.SetActiveMonoBehaviour(component2, currentDifficulty == MetaStageDifficulty.Medium);
		Helpers.SetActiveMonoBehaviour(component3, currentDifficulty == MetaStageDifficulty.Hard);
		GetObject().currentDifficultyLevel = difficulty;
	}

	private IEnumerator CollectProgress()
	{
		MetaLocation location = GetObject();
		int shownStars = location.GetCollectedStars(currentDifficulty, onlyShown: true);
		int stars = location.GetCollectedStars(currentDifficulty);
		float shownProgress = location.GetProgress(currentDifficulty, onlyShown: true);
		float progress = location.GetProgress(currentDifficulty);
		Helpers.SetText(starsTotalCount, shownStars.ToString() + "/" + location.totalStars.ToString());
		PrepareRewardsButtons(shownStars);
		yield return new WaitForSeconds(0.5f);
		for (int i = shownStars; i < stars; i++)
		{
			float progressDelta = (progress - shownProgress) / (float)(stars - shownStars);
			float startTime = Time.time;
			while (Time.time < startTime + 0.5f)
			{
				progressBar.fillAmount = shownProgress + (float)(i - shownStars) * progressDelta + progressDelta * (Time.time - startTime) / 0.5f;
				yield return null;
			}
			progressBar.fillAmount = shownProgress + (float)(i - shownStars) * progressDelta + progressDelta;
			PrepareRewardsButtons(i + 1);
			Helpers.SetText(starsTotalCount, (i + 1).ToString() + "/" + location.totalStars.ToString());
		}
	}

	protected void SetButtonAlpha(Button button, float alpha, float scale = 1f)
	{
		if (!(button == null))
		{
			CanvasGroup component = button.GetComponent<CanvasGroup>();
			if ((bool)component)
			{
				component.alpha = alpha;
			}
			button.transform.localScale = new Vector3(scale, scale, 1f);
		}
	}

	private void OnStageClicked(MetaStageView stageView)
	{
		int num = Singleton<Meta>.Instance.FindObjects<MetaRewardChestSlot>().Count((MetaRewardChestSlot x) => x.state == MetaRewardChestSlotState.Empty);
		if (num == 0 && slotsFullWarning)
		{
			SlotsAreFullWindowContext slotsAreFullWindowContext = new SlotsAreFullWindowContext();
			slotsAreFullWindowContext.stage = stageView.GetObject();
			slotsAreFullWindowContext.difficulty = stageView.difficulty;
			Singleton<WindowManager>.Instance.OpenWindow<SlotsAreFullWindow>(slotsAreFullWindowContext, OnSlotsAreFullWindowClosed);
			return;
		}
		if (num > 0)
		{
			slotsFullWarning = true;
		}
		CloseWindow(true);
		SingletonComponent<Meta, MetaFight>.Instance.Fight(stageView.GetObject(), stageView.difficulty);
	}

	private void OnSlotsAreFullWindowClosed(Window window, object returnValue)
	{
		if (returnValue != null)
		{
			slotsFullWarning = false;
			CloseWindow(true);
		}
	}

	private void OnStageWindowClosed(Window window, object returnValue)
	{
		if (returnValue != null)
		{
			CloseWindow();
		}
	}

	private void OnCollectRewardClicked(MetaLocationState reward)
	{
		MetaLocation @object = GetObject();
		if (@object.CollectReward(currentDifficulty, reward))
		{
			Helpers.SetActiveGameObject(rewardBronzeMarker, @object.IsRewardCollected(currentDifficulty, MetaLocationState.Bronze));
			Helpers.SetActiveGameObject(rewardSilverMarker, @object.IsRewardCollected(currentDifficulty, MetaLocationState.Silver));
			Helpers.SetActiveGameObject(rewardGoldMarker, @object.IsRewardCollected(currentDifficulty, MetaLocationState.Gold));
			Helpers.SetActiveGameObject(rewardBronzeButton, @object.IsRewardAvailable(currentDifficulty, MetaLocationState.Bronze));
			Helpers.SetActiveGameObject(rewardSilverButton, @object.IsRewardAvailable(currentDifficulty, MetaLocationState.Silver));
			Helpers.SetActiveGameObject(rewardGoldButton, @object.IsRewardAvailable(currentDifficulty, MetaLocationState.Gold));
			Helpers.SetActive(rewardBronzeGlow, @object.IsRewardAvailable(currentDifficulty, MetaLocationState.Bronze));
			Helpers.SetActive(rewardSilverGlow, @object.IsRewardAvailable(currentDifficulty, MetaLocationState.Silver));
			Helpers.SetActive(rewardGoldGlow, @object.IsRewardAvailable(currentDifficulty, MetaLocationState.Gold));
		}
	}

	private void OnCollectBronzeRewardClicked()
	{
		OnCollectRewardClicked(MetaLocationState.Bronze);
	}

	private void OnCollectSilverRewardClicked()
	{
		OnCollectRewardClicked(MetaLocationState.Silver);
	}

	private void OnCollectGoldRewardClicked()
	{
		OnCollectRewardClicked(MetaLocationState.Gold);
	}
}
