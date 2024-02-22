using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class MetaRewardChestSlotView : MetaView
{
	[Header("Meta Chest Slot")]
	public GameObject emptyGroup;

	public GameObject withChestGroup;

	public GameObject openingGroup;

	public GameObject openGroup;

	public GameObject ableToUnlockGroup;

	public GameObject unableToUnlockGroup;

	public List<Image> chestIcons;

	public List<Text> chestLevels;

	public TimeText baseOpenTime;

	public TimeText currentOpenTime;

	public TextWithQuads openingCost;

	[Header("InsertAnim")]
	public CanvasGroup insertAdd;

	public AudioSample insertSFX;

	public GameObject chanceItemGroup;

	public MetaView chanceItemView;

	public new MetaRewardChestSlot GetObject()
	{
		return base.GetObject() as MetaRewardChestSlot;
	}

	protected override void OnInteract()
	{
		MetaRewardChestSlot obj = GetObject();
		if (obj.state == MetaRewardChestSlotState.WithChest)
		{
			if (obj.ableToUnlock)
			{
				ChestWindowContext chestWindowContext = new ChestWindowContext();
				chestWindowContext.chest = obj.chest;
				chestWindowContext.chestLevel = obj.contentLevel;
				chestWindowContext.content = obj.content;
				chestWindowContext.chanceItem = obj.chanceItem;
				chestWindowContext.slot = obj;
				chestWindowContext.state = ChestWindowState.Open;
				Singleton<WindowManager>.Instance.OpenWindow<ChestWindow>(chestWindowContext, delegate(Window w, object r)
				{
					if (r != null)
					{
						obj.state = MetaRewardChestSlotState.OpeningChest;
					}
				});
			}
			else
			{
				ChestWindowContext chestWindowContext2 = new ChestWindowContext();
				chestWindowContext2.chest = obj.chest;
				chestWindowContext2.chestLevel = obj.contentLevel;
				chestWindowContext2.content = obj.content;
				chestWindowContext2.chanceItem = obj.chanceItem;
				chestWindowContext2.slot = obj;
				chestWindowContext2.state = ChestWindowState.OpenNowForced;
				Singleton<WindowManager>.Instance.OpenWindow<ChestWindow>(chestWindowContext2, delegate(Window w, object r)
				{
					if (r != null)
					{
						obj.state = MetaRewardChestSlotState.WithOpenChest;
					}
				});
			}
		}
		else if (obj.state == MetaRewardChestSlotState.OpeningChest)
		{
			ChestWindowContext chestWindowContext3 = new ChestWindowContext();
			chestWindowContext3.chest = obj.chest;
			chestWindowContext3.chestLevel = obj.contentLevel;
			chestWindowContext3.content = obj.content;
			chestWindowContext3.chanceItem = obj.chanceItem;
			chestWindowContext3.openTime = obj.openTime;
			chestWindowContext3.slot = obj;
			chestWindowContext3.state = ChestWindowState.OpenNow;
			Singleton<WindowManager>.Instance.OpenWindow<ChestWindow>(chestWindowContext3, delegate(Window w, object r)
			{
				if (r != null)
				{
					obj.state = MetaRewardChestSlotState.WithOpenChest;
				}
			});
		}
		else if (obj.state == MetaRewardChestSlotState.WithOpenChest)
		{
			OpenChestWindowContext openChestWindowContext = new OpenChestWindowContext();
			openChestWindowContext.chest = obj.chest;
			openChestWindowContext.chestLevel = obj.contentLevel;
			openChestWindowContext.content = obj.content;
			openChestWindowContext.chanceItem = obj.chanceItem;
			Singleton<WindowManager>.Instance.OpenWindow<OpenChestWindow>(openChestWindowContext);
			obj.state = MetaRewardChestSlotState.Empty;
		}
	}

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
		Helpers.SetActiveGameObject(insertAdd, value: false);
	}

	protected override void OnChanged()
	{
		base.OnChanged();
		MetaRewardChestSlot @object = GetObject();
		Helpers.SetActive(emptyGroup, @object.state == MetaRewardChestSlotState.Empty);
		Helpers.SetActive(withChestGroup, @object.state == MetaRewardChestSlotState.WithChest);
		Helpers.SetActive(openingGroup, @object.state == MetaRewardChestSlotState.OpeningChest);
		Helpers.SetActive(openGroup, @object.state == MetaRewardChestSlotState.WithOpenChest);
		for (int i = 0; i < chestIcons.Count; i++)
		{
			Helpers.SetImage(chestIcons[i], (!@object.chest) ? null : @object.chest.icon);
		}
		for (int j = 0; j < chestLevels.Count; j++)
		{
			Helpers.SetText(chestLevels[j], "Level " + @object.contentLevel);
		}
		if (@object.chest != null)
		{
			Helpers.SetTime(baseOpenTime, DateTime.MinValue + TimeSpan.FromHours(@object.chest.openTimeHours));
			Helpers.SetTextWithQuads(openingCost, MetaResource.gems.quadText + " " + @object.openNowPrice);
		}
		Helpers.SetTime(currentOpenTime, @object.openTime, countdown: true);
		Helpers.SetActive(chanceItemGroup, @object.chanceItem);
		if ((bool)chanceItemView)
		{
			chanceItemView.SetObject(@object.chanceItem);
		}
		Helpers.SetActive(ableToUnlockGroup, @object.ableToUnlock);
		Helpers.SetActive(unableToUnlockGroup, !@object.ableToUnlock);
	}

	public IEnumerator InsertSequencePre()
	{
		yield return new WaitForEndOfFrame();
		Helpers.SetActiveGameObject(insertAdd, value: true);
		insertAdd.alpha = 0f;
		insertAdd.transform.localScale = Vector3.zero;
		UITweenerEx.fadeIn(insertAdd, 0.125f);
		UITweenerEx.scaleTo(insertAdd, 1f, 0.125f);
		AudioManager.PlaySafe(insertSFX);
		yield return new WaitForSeconds(0.125f);
	}

	public IEnumerator InsertSequencePost()
	{
		UITweenerEx.scaleTo(insertAdd, 3f, 1f);
		UITweenerEx.fadeOut(insertAdd, 1f);
		yield return new WaitForSeconds(1f);
		insertAdd.alpha = 0f;
		insertAdd.transform.localScale = Vector3.zero;
		Helpers.SetActiveGameObject(insertAdd, value: false);
	}
}
