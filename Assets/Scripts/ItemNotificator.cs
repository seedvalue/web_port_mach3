using UnityEngine;
using UnityEngine.UI;
using Utils;

public class ItemNotificator : MonoBehaviour
{
	public GameObject upgradeGroup;

	public GameObject justFoundGroup;

	public Text countText;

	private bool dirty;

	protected void OnEnable()
	{
		Singleton<Meta>.Instance.AddTypePropertyListener(typeof(MetaItem), "count", OnChanged);
		Singleton<Meta>.Instance.AddTypePropertyListener(typeof(MetaItem), "justFound", OnChanged);
	}

	protected void OnDisable()
	{
		if (Singleton<Meta>.HasInstance)
		{
			Singleton<Meta>.Instance.RemTypePropertyListener(typeof(MetaItem), "count", OnChanged);
			Singleton<Meta>.Instance.RemTypePropertyListener(typeof(MetaItem), "justFound", OnChanged);
		}
	}

	protected void Start()
	{
		RefreshState();
	}

	protected void LateUpdate()
	{
		if (dirty)
		{
			RefreshState();
		}
	}

	private void OnChanged(MetaObject metaObject, string propertyName)
	{
		dirty = true;
	}

	private void RefreshState()
	{
		int num = 0;
		int num2 = 0;
		MetaItem[] array = Singleton<Meta>.Instance.FindObjects<MetaItem>();
		foreach (MetaItem metaItem in array)
		{
			if (metaItem.found)
			{
				if (metaItem.justFound)
				{
					num++;
				}
				if (metaItem.canUpgrade)
				{
					num2++;
				}
			}
		}
		if (num > 0)
		{
			Helpers.SetActive(justFoundGroup, value: true);
			Helpers.SetActive(upgradeGroup, value: false);
			Helpers.SetActiveGameObject(countText, value: true);
			Helpers.SetText(countText, num.ToString());
		}
		else if (num2 > 0)
		{
			Helpers.SetActive(justFoundGroup, value: false);
			Helpers.SetActive(upgradeGroup, value: true);
			Helpers.SetActiveGameObject(countText, value: true);
			Helpers.SetText(countText, num2.ToString());
		}
		else
		{
			Helpers.SetActive(justFoundGroup, value: false);
			Helpers.SetActive(upgradeGroup, value: false);
			Helpers.SetActiveGameObject(countText, value: false);
		}
		dirty = false;
	}
}
