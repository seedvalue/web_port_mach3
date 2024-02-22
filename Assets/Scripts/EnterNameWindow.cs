using UnityEngine;
using UnityEngine.UI;
using Utils;

public class EnterNameWindow : MetaWindow
{
	public Button confirmButton;

	public InputField inputField;

	public KeyCode confirmKey;

	[WindowTestMethod]
	public static void TestWindow()
	{
		EnterNameWindowContext enterNameWindowContext = new EnterNameWindowContext();
		enterNameWindowContext.player = MetaPlayer.local;
		enterNameWindowContext.rename = false;
		Singleton<WindowManager>.Instance.OpenWindow<EnterNameWindow>(enterNameWindowContext);
	}

	public new MetaPlayer GetObject()
	{
		return base.GetObject() as MetaPlayer;
	}

	protected virtual void OnEnable()
	{
		EnterNameWindowContext enterNameWindowContext = base.context as EnterNameWindowContext;
		if (enterNameWindowContext != null)
		{
			InitWithContext(enterNameWindowContext);
		}
	}

	protected override void Start()
	{
		base.Start();
		if (confirmButton != null)
		{
			confirmButton.onClick.AddListener(OnConfirmClicked);
		}
	}

	private void InitWithContext(EnterNameWindowContext context)
	{
		SetObject(context.player);
		foreach (Button closeButton in closeButtons)
		{
			Helpers.SetActiveGameObject(closeButton, context.rename);
		}
	}

	private void OnConfirmClicked()
	{
		MetaPlayer player = (base.context as EnterNameWindowContext).player;
		player.playerName = inputField.text;
		CloseWindow(true);
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(confirmKey))
		{
			OnConfirmClicked();
		}
	}
}
