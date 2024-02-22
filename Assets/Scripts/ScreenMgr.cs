using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScreenMgr : ExternalSceneLoader
{
	[HideInInspector]
	public static ScreenMgr instance;

	public float minAspect;

	protected override void Start()
	{
		base.Start();
		instance = this;
	}

	protected override void OnLoaded()
	{
		ScreenSeparators component = GetComponent<ScreenSeparators>();
		if ((bool)component)
		{
			component.Initialze();
		}
		IEnumerable<Graphic> enumerable = from x in Object.FindObjectsOfType<Graphic>()
			where x.raycastTarget
			where x.GetComponentInParent<MetaView>() == null
			where x.GetComponentInParent<Button>() == null || x.GetComponent<Button>() != null
			select x;
		foreach (Graphic item in enumerable)
		{
			item.gameObject.AddComponent<Deselect>();
		}
	}

	public void ShowScreenHUD(int screenNum)
	{
		Transform child = base.transform.GetChild(screenNum);
		FlyTo component = Camera.main.GetComponent<FlyTo>();
		if ((bool)component)
		{
			FlyTo flyTo = component;
			Vector3 position = child.transform.position;
			float x = position.x;
			Vector3 position2 = Camera.main.transform.position;
			float y = position2.y;
			Vector3 position3 = Camera.main.transform.position;
			flyTo.flyTo(new Vector3(x, y, position3.z), 0.25f, time: true, local: false);
		}
	}

	public static void ShowScreen(ScreenID screenNum)
	{
		ScreenMgr screenMgr = instance;
		if (!(screenMgr == null))
		{
			Transform child = screenMgr.transform.GetChild((int)screenNum);
			FlyTo component = Camera.main.GetComponent<FlyTo>();
			if ((bool)component && (bool)child)
			{
				FlyTo flyTo = component;
				Vector3 position = child.transform.position;
				float x = position.x;
				Vector3 position2 = Camera.main.transform.position;
				float y = position2.y;
				Vector3 position3 = Camera.main.transform.position;
				flyTo.flyTo(new Vector3(x, y, position3.z), 0.25f, time: true, local: false);
			}
		}
	}

	public static Canvas GetScreen(ScreenID screenNum)
	{
		ScreenMgr screenMgr = instance;
		if (screenMgr == null)
		{
			return null;
		}
		Transform child = screenMgr.transform.GetChild((int)screenNum);
		if ((bool)child)
		{
			return child.GetComponent<Canvas>();
		}
		return null;
	}

	public void GoToShopAndShowGems()
	{
		ShowScreen(ScreenID.Shop);
		GetScreen(ScreenID.Shop).GetComponent<Shop>().ShowGems();
	}

	public void GoToShopAndShowCoins()
	{
		ShowScreen(ScreenID.Shop);
		GetScreen(ScreenID.Shop).GetComponent<Shop>().ShowCoins();
	}

	public float GetScreenWidth()
	{
		if (base.transform.childCount == 0)
		{
			return 0f;
		}
		Vector2 sizeDelta = base.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
		return sizeDelta.x;
	}

	public float GetScreenHeight()
	{
		if (base.transform.childCount == 0)
		{
			return 0f;
		}
		Vector2 sizeDelta = base.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
		return sizeDelta.y;
	}

	public int GetScreenCount()
	{
		return base.transform.childCount;
	}

	public int GetCurrentScreen()
	{
		if (base.transform.childCount == 0)
		{
			return 0;
		}
		Vector2 sizeDelta = base.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
		float x = sizeDelta.x;
		Vector3 position = Camera.main.transform.position;
		return (int)(position.x / x + 0.5f);
	}

	public ScreenID GetCurrentScreenID()
	{
		return (ScreenID)GetCurrentScreen();
	}
}
