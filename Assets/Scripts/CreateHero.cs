using UnityEngine;
using Utils;

public class CreateHero : MonoBehaviour
{
	private void Start()
	{
		EnterNameWindowContext enterNameWindowContext = new EnterNameWindowContext();
		enterNameWindowContext.player = MetaPlayer.local;
		enterNameWindowContext.rename = false;
		Singleton<WindowManager>.Instance.OpenWindow<EnterNameWindow>(enterNameWindowContext, OnEnteredName);
	}

	public void OnEnteredName(Window window, object returnValue)
	{
		if (returnValue != null)
		{
			MetaPlayer.local.created = true;
			Singleton<SceneLoader>.Instance.SwitchToMeta(SceneLoader.Priority.Default);
		}
	}
}
