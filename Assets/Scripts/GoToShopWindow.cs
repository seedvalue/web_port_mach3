using UnityEngine.UI;
using Utils;

public class GoToShopWindow : MetaWindow
{
	public Button goToShopButton;

	protected override void Start()
	{
		base.Start();
		Helpers.AddListenerOnClick(goToShopButton, GoToShop);
	}

	private void GoToShop()
	{
		CloseWindow(true);
		ScreenMgr.ShowScreen(ScreenID.Shop);
		ScreenMgr.GetScreen(ScreenID.Shop).GetComponent<Shop>().ShowGems();
	}
}
