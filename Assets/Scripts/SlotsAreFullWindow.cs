using UnityEngine.UI;
using Utils;

public class SlotsAreFullWindow : MetaWindow
{
	public Button goToBattleButton;

	protected override void Start()
	{
		base.Start();
		Helpers.AddListenerOnClick(goToBattleButton, GoToBattle);
	}

	private void GoToBattle()
	{
		SlotsAreFullWindowContext slotsAreFullWindowContext = base.context as SlotsAreFullWindowContext;
		SingletonComponent<Meta, MetaFight>.Instance.Fight(slotsAreFullWindowContext.stage, slotsAreFullWindowContext.difficulty);
		CloseWindow(true);
	}
}
