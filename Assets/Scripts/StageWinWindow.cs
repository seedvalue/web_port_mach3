using UnityEngine;
using UnityEngine.UI;
using Utils;

public class StageWinWindow : MetaWindow
{
	public Text locationNameText;

	public Text stageNameText;

	public Image star2;

	public Image star3;

	public MetaContainer rewardsContainer;

	public MetaRewardView rewardsPrefab;

	public AudioSample openingSound;

	public AudioSample closingSound;

	public AudioSample starSound;

	[WindowTestMethod]
	public static void TestWindow()
	{
		StageWinWindowContext stageWinWindowContext = new StageWinWindowContext();
		stageWinWindowContext.stage = Singleton<Meta>.Instance.FindRandomObject<MetaStage>();
		stageWinWindowContext.stars = 2;
		Singleton<WindowManager>.Instance.OpenWindow<StageWinWindow>(stageWinWindowContext);
	}

	public new MetaStage GetObject()
	{
		return base.GetObject() as MetaStage;
	}

	public override void OnWindowClosing()
	{
		base.OnWindowClosing();
		AudioManager.PlaySafe(closingSound);
	}

	public override void OnWindowOpening()
	{
		base.OnWindowOpening();
		AudioManager.PlaySafe(openingSound);
	}

	protected virtual void OnEnable()
	{
		StageWinWindowContext stageWinWindowContext = base.context as StageWinWindowContext;
		if (stageWinWindowContext != null)
		{
			InitWithContext(stageWinWindowContext);
		}
	}

	private void InitWithContext(StageWinWindowContext context)
	{
		SetObject(context.stage);
		if ((bool)context.stage)
		{
			Helpers.SetText(locationNameText, context.stage.location.displayName);
			Helpers.SetText(stageNameText, context.stage.displayName);
		}
		if (context.rewards != null)
		{
			rewardsContainer.Assign(context.rewards, rewardsPrefab);
		}
		if ((bool)star2)
		{
			star2.enabled = (context.stars >= 2);
		}
		if ((bool)star3)
		{
			star3.enabled = (context.stars >= 3);
		}
		Animator componentInChildren = GetComponentInChildren<Animator>();
		if ((bool)componentInChildren)
		{
			componentInChildren.SetTrigger("Stars");
		}
	}

	public void PlayStarSound(int star)
	{
		if ((bool)starSound && (star == 1 || (star == 2 && (bool)star2 && star2.enabled) || (star == 3 && (bool)star3 && star3.enabled)))
		{
			AudioManager.Play(starSound);
		}
	}
}
