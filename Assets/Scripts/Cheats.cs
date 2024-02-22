using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public class Cheats : MonoBehaviour
{
	public Button cheatButtonPrefab;

	public RectTransform cheatNewLinePrefab;

	public RectTransform cheatEmptyPrefab;

	public RectTransform container;

	private void Start()
	{
		AddSection("RESOURCES");
		AddCheat("+100" + MetaResource.coins.quadText, Gold100);
		AddCheat("+1000" + MetaResource.coins.quadText, Gold1000);
		AddCheat("+10000" + MetaResource.coins.quadText, Gold10000);
		AddCheat("+100000" + MetaResource.coins.quadText, Gold100000);
		AddCheat("+10" + MetaResource.gems.quadText, Gems10);
		AddCheat("+100" + MetaResource.gems.quadText, Gems100);
		AddCheat("+1000" + MetaResource.gems.quadText, Gems1000);
		AddCheat("+10000" + MetaResource.gems.quadText, Gems10000);
		AddCheat("+5" + MetaResource.exp.quadText, delegate
		{
			AddExp(5);
		});
		AddCheat("+20" + MetaResource.exp.quadText, delegate
		{
			AddExp(20);
		});
		AddCheat("+100" + MetaResource.exp.quadText, delegate
		{
			AddExp(100);
		});
		AddCheat("+1000" + MetaResource.exp.quadText, delegate
		{
			AddExp(1000);
		});
		AddSection("TIME");
		AddCheat("+15m", FastForward15m);
		AddCheat("+1h", FastForward1h);
		AddCheat("+3h", FastForward3h);
		AddCheat("+12h", FastForward12h);
		AddSection("OTHERS");
		AddCheat("Unlock All Items", UnlockAllItems);
		AddCheat("Unlock All Levels", UnlockAllLevels);
		AddCheat("Random Chest", RandomChest);
		AddCheat("Random Chest With Item", RandomChestWithItem);
		AddCheat("Open Chests", OpenChests);
		AddCheat("Add Star", AddStar);
		AddCheat("Remove Save", RemoveSave);
		AddCheat("Unequip All", UnequipAll);
		AddCheat("+1000 Items", Add1000Items);
		AddSection("DRAG TRESHOLD");
		AddCheat("0", delegate
		{
			EventSystemTreshold(0);
		});
		AddCheat("5", delegate
		{
			EventSystemTreshold(5);
		});
		AddCheat("10", delegate
		{
			EventSystemTreshold(10);
		});
		AddCheat("15", delegate
		{
			EventSystemTreshold(15);
		});
		AddCheat("20", delegate
		{
			EventSystemTreshold(20);
		});
		AddCheat("25", delegate
		{
			EventSystemTreshold(25);
		});
		AddCheat("30", delegate
		{
			EventSystemTreshold(30);
		});
		AddCheat("40", delegate
		{
			EventSystemTreshold(40);
		});
	}

	private void EndLine()
	{
		int num = 4 - container.childCount % 4;
		for (int i = 0; i < num; i++)
		{
			AddEpmty();
		}
	}

	private void AddSection(string sectionName)
	{
		EndLine();
		RectTransform rectTransform = UnityEngine.Object.Instantiate(cheatNewLinePrefab, container);
		rectTransform.GetComponentInChildren<Text>().text = sectionName;
		EndLine();
	}

	private void AddEpmty()
	{
		UnityEngine.Object.Instantiate(cheatEmptyPrefab, container);
	}

	private void AddCheat(string name, UnityAction action)
	{
		Button button = UnityEngine.Object.Instantiate(cheatButtonPrefab, container);
		button.GetComponentInChildren<Text>().text = name;
		Helpers.AddListenerOnClick(button, action);
	}

	private void Gold100()
	{
		MetaResource.coins.count += 100;
	}

	private void Gold1000()
	{
		MetaResource.coins.count += 1000;
	}

	private void Gold10000()
	{
		MetaResource.coins.count += 10000;
	}

	private void Gold100000()
	{
		MetaResource.coins.count += 100000;
	}

	private void Gems10()
	{
		MetaResource.gems.count += 10;
	}

	private void Gems100()
	{
		MetaResource.gems.count += 100;
	}

	private void Gems1000()
	{
		MetaResource.gems.count += 1000;
	}

	private void Gems10000()
	{
		MetaResource.gems.count += 10000;
	}

	private void FastForward15m()
	{
		SingletonComponent<Meta, MetaTimeManager>.Instance.cheatOffset += new TimeSpan(0, 0, 15, 0);
	}

	private void FastForward1h()
	{
		SingletonComponent<Meta, MetaTimeManager>.Instance.cheatOffset += new TimeSpan(0, 1, 0, 0);
	}

	private void FastForward3h()
	{
		SingletonComponent<Meta, MetaTimeManager>.Instance.cheatOffset += new TimeSpan(0, 3, 0, 0);
	}

	private void FastForward12h()
	{
		SingletonComponent<Meta, MetaTimeManager>.Instance.cheatOffset += new TimeSpan(0, 12, 0, 0);
	}

	private void UnlockAllItems()
	{
		IEnumerable<MetaItem> enumerable = from i in Singleton<Meta>.Instance.FindObjects<MetaItem>()
			where i.available
			select i;
		foreach (MetaItem item in enumerable)
		{
			item.found = true;
		}
	}

	private void Add1000Items()
	{
		IEnumerable<MetaItem> enumerable = from i in Singleton<Meta>.Instance.FindObjects<MetaItem>()
			where i.available
			where i.found
			select i;
		foreach (MetaItem item in enumerable)
		{
			item.count += 1000;
		}
	}

	private void UnlockAllLevels()
	{
		MetaStage[] array = Singleton<Meta>.Instance.FindObjects<MetaStage>();
		foreach (MetaStage metaStage in array)
		{
			metaStage.easyUnlocked = true;
			metaStage.mediumUnlocked = true;
			metaStage.hardUnlocked = true;
		}
	}

	private void RandomChest()
	{
		MetaRewardChestSlot.AddChest(SingletonComponent<Meta, MetaChestSequence>.Instance.GetReward());
	}

	private void RandomChestWithItem()
	{
		MetaItem chanceItem = (from i in Singleton<Meta>.Instance.FindObjects<MetaItem>()
			where i.available && i.found
			orderby Rand.uniform
			select i).FirstOrDefault();
		MetaRewardChestSlot.AddChest(SingletonComponent<Meta, MetaChestSequence>.Instance.GetReward(), chanceItem);
	}

	private void OpenChests()
	{
		MetaRewardChestSlot[] array = Singleton<Meta>.Instance.FindObjects<MetaRewardChestSlot>();
		foreach (MetaRewardChestSlot metaRewardChestSlot in array)
		{
			if ((bool)metaRewardChestSlot.chest)
			{
				metaRewardChestSlot.state = MetaRewardChestSlotState.WithOpenChest;
			}
		}
	}

	private void RemoveSave()
	{
		Singleton<Meta>.Instance.Reset();
	}

	private void AddStar()
	{
		MetaVictoryChestSlot.AddStars(1);
	}

	private void UnequipAll()
	{
		MetaItemSlot[] array = Singleton<Meta>.Instance.FindObjects<MetaItemSlot>();
		foreach (MetaItemSlot metaItemSlot in array)
		{
			metaItemSlot.item = null;
		}
	}

	private void EventSystemTreshold(int value)
	{
		if ((bool)EventSystem.current)
		{
			EventSystem.current.pixelDragThreshold = value;
		}
	}

	private void AddExp(int value)
	{
		MetaResource.exp.count += value;
	}
}
