using UnityEngine.UI;
using Utils;

public class StageLoseWindow : MetaWindow
{
	public Text locationNameText;

	public Text stageNameText;

	public AudioSample openingSound;

	public AudioSample closingSound;

	[WindowTestMethod]
	public static void TestWindow()
	{
		StageLoseWindowContext stageLoseWindowContext = new StageLoseWindowContext();
		stageLoseWindowContext.stage = Singleton<Meta>.Instance.FindRandomObject<MetaStage>();
		Singleton<WindowManager>.Instance.OpenWindow<StageLoseWindow>(stageLoseWindowContext);
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
		StageLoseWindowContext stageLoseWindowContext = base.context as StageLoseWindowContext;
		if (stageLoseWindowContext != null)
		{
			InitWithContext(stageLoseWindowContext);
		}
	}

	private void InitWithContext(StageLoseWindowContext context)
	{
		SetObject(context.stage);
		if ((bool)context.stage)
		{
			Helpers.SetText(locationNameText, context.stage.location.displayName);
			Helpers.SetText(stageNameText, context.stage.displayName);
		}
	}
}
