using UnityEngine;
using Utils;

public class ScreenLock : MonoBehaviour
{
	public ScreenID screen;

	public GameObject lockObject;

	private void OnEnable()
	{
		Refresh();
		Singleton<Meta>.Instance.AddObjectPropertyListener(MetaPlayer.local, "level", OnPlayerLevelChanged);
	}

	private void OnDisable()
	{
		if (Singleton<Meta>.HasInstance)
		{
			Singleton<Meta>.Instance.RemObjectPropertyListener(MetaPlayer.local, "level", OnPlayerLevelChanged);
		}
	}

	private void OnPlayerLevelChanged(MetaObject metaObject, string propertyName)
	{
		Refresh();
	}

	private void Refresh()
	{
		MetaConsts component = Singleton<Meta>.Instance.GetComponent<MetaConsts>();
		int level = MetaPlayer.local.level;
		int num = 0;
		switch (screen)
		{
		case ScreenID.PVP:
			num = component.pvpUnlockLevel;
			break;
		case ScreenID.Shop:
			num = component.shopUnlockLevel;
			break;
		case ScreenID.Guilds:
			num = component.guildsUnlockLevel;
			break;
		}
		Helpers.SetActive(lockObject, num > level);
	}
}
