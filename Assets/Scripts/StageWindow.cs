using UnityEngine.UI;
using Utils;

public class StageWindow : MetaWindow
{
	public Text locationNameText;

	public Button fightButton;

	private MetaStageDifficulty difficulty;

	[WindowTestMethod]
	public static void TestWindow()
	{
		StageWindowContext stageWindowContext = new StageWindowContext();
		stageWindowContext.stage = Singleton<Meta>.Instance.FindRandomObject<MetaStage>();
		Singleton<WindowManager>.Instance.OpenWindow<StageWindow>(stageWindowContext);
	}

	public new MetaStage GetObject()
	{
		return base.GetObject() as MetaStage;
	}

	protected virtual void OnEnable()
	{
		StageWindowContext stageWindowContext = base.context as StageWindowContext;
		if (stageWindowContext != null)
		{
			InitWithContext(stageWindowContext);
		}
	}

	protected override void Start()
	{
		base.Start();
		if (fightButton != null)
		{
			fightButton.onClick.AddListener(OnFightClicked);
		}
	}

	private void InitWithContext(StageWindowContext context)
	{
		SetObject(context.stage);
		difficulty = context.difficulty;
		if (context.stage != null)
		{
			Helpers.SetText(locationNameText, context.location.displayName);
		}
	}

	private void OnFightClicked()
	{
		CloseWindow(true);
		SingletonComponent<Meta, MetaFight>.Instance.Fight(GetObject(), difficulty);
	}
}
