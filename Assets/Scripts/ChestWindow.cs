using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class ChestWindow : MetaWindow
{
	public Image chestImage;

	public Text chestLevel;

	public Text chestName;

	public Text coinsCount;

	public Text cardsCount;

	public GameObject guaranteedGroup;

	public GameObject guaranteedRareGroup;

	public GameObject guaranteedEpicGroup;

	public GameObject guaranteedLegendaryGroup;

	public Text guaranteedRareCount;

	public Text guaranteedEpicCount;

	public Text guaranteedLegendaryCount;

	public GameObject openGroup;

	public Button openButton;

	public GameObject openNowGroup;

	public GameObject openNowNormalGroup;

	public GameObject openNowForcedGroup;

	public Button openNowButton;

	public TextWithQuads openNowButtonText;

	public TimeText openNowTimeText;

	public GameObject buyGroup;

	public Button buyButton;

	public TextWithQuads buyButtonText;

	public GameObject chanceItemGroup;

	public MetaView chanceItemView;

	[WindowTestMethod]
	public static void TestOpen()
	{
		ChestWindowContext chestWindowContext = new ChestWindowContext();
		chestWindowContext.chest = Singleton<Meta>.Instance.FindRandomObject<MetaChest>();
		chestWindowContext.state = ChestWindowState.Open;
		Singleton<WindowManager>.Instance.OpenWindow<ChestWindow>(chestWindowContext);
	}

	[WindowTestMethod]
	public static void TestOpenNow()
	{
		ChestWindowContext chestWindowContext = new ChestWindowContext();
		chestWindowContext.chest = Singleton<Meta>.Instance.FindRandomObject<MetaChest>();
		chestWindowContext.state = ChestWindowState.OpenNow;
		chestWindowContext.openTime = DateTime.UtcNow.AddMinutes(UnityEngine.Random.Range(0.1f, 140f));
		Singleton<WindowManager>.Instance.OpenWindow<ChestWindow>(chestWindowContext);
	}

	[WindowTestMethod]
	public static void TestBuyFromShop()
	{
		ChestWindowContext chestWindowContext = new ChestWindowContext();
		chestWindowContext.chest = Singleton<Meta>.Instance.FindRandomObject<MetaChest>();
		chestWindowContext.state = ChestWindowState.BuyFromShop;
		Singleton<WindowManager>.Instance.OpenWindow<ChestWindow>(chestWindowContext);
	}

	public new MetaChest GetObject()
	{
		return base.GetObject() as MetaChest;
	}

	protected virtual void OnEnable()
	{
		ChestWindowContext chestWindowContext = base.context as ChestWindowContext;
		if (chestWindowContext != null)
		{
			InitWithContext(chestWindowContext);
		}
	}

	protected override void Start()
	{
		base.Start();
		openButton.onClick.AddListener(OnOpenClick);
		openNowButton.onClick.AddListener(OnOpenNowClick);
		buyButton.onClick.AddListener(OnBuyClick);
	}

	public override void OnWindowClosed()
	{
		base.OnWindowClosed();
		ChestWindowContext chestWindowContext = base.context as ChestWindowContext;
		if (chestWindowContext != null && (bool)chestWindowContext.slot)
		{
			Singleton<Meta>.Instance.RemObjectPropertyListener(chestWindowContext.slot, "openNowPrice", OnSlotOpenNowChanged);
		}
	}

	private void InitWithContext(ChestWindowContext context)
	{
		SetObject(context.chest);
		int num = (context.chestLevel <= 0) ? MetaPlayer.local.level : context.chestLevel;
		Helpers.SetImage(chestImage, context.chest.icon);
		Helpers.SetText(chestName, context.chest.displayName);
		Helpers.SetText(chestLevel, "Level " + num);
		MetaChest.LevelData levelData = context.chest.CalcLevelData(num);
		int num2 = Mathf.FloorToInt(levelData.RareMin);
		int num3 = Mathf.FloorToInt(levelData.EpicMin);
		int num4 = Mathf.FloorToInt(levelData.LegendMin);
		int num5 = levelData.GoldMin;
		int num6 = levelData.GoldMax;
		int num7 = levelData.CardCount;
		if ((bool)context.content)
		{
			num7 = 0;
			List<MetaLink> content = context.content.content;
			for (int i = 0; i < content.Count; i++)
			{
				MetaLink metaLink = content[i];
				int propertyOrDefault = metaLink.GetPropertyOrDefault<int>("count");
				if (metaLink.metaObject == MetaResource.coins)
				{
					num5 = Mathf.Min(num5, propertyOrDefault);
					num6 = Mathf.Max(num6, propertyOrDefault);
				}
				else if (metaLink.metaObject is MetaItem)
				{
					num7 += propertyOrDefault;
				}
			}
		}
		bool active = num2 > 0 || num3 > 0 || num4 > 0;
		guaranteedGroup.SetActive(active);
		guaranteedRareGroup.SetActive(num2 > 0);
		guaranteedEpicGroup.SetActive(num3 > 0);
		guaranteedLegendaryGroup.SetActive(num4 > 0);
		Helpers.SetText(coinsCount, num5 + "-" + num6);
		Helpers.SetText(cardsCount, "x" + num7);
		Helpers.SetText(guaranteedRareCount, "x" + num2 + " Rare");
		Helpers.SetText(guaranteedEpicCount, "x" + num3 + " Epic");
		Helpers.SetText(guaranteedLegendaryCount, "x" + num4 + " Legendary");
		openGroup.SetActive(context.state == ChestWindowState.Open);
		openNowGroup.SetActive(context.state == ChestWindowState.OpenNow || context.state == ChestWindowState.OpenNowForced);
		openNowNormalGroup.SetActive(context.state == ChestWindowState.OpenNow);
		openNowForcedGroup.SetActive(context.state == ChestWindowState.OpenNowForced);
		buyGroup.SetActive(context.state == ChestWindowState.BuyFromShop);
		if (context.state == ChestWindowState.OpenNow)
		{
			Helpers.SetTextWithQuads(openNowButtonText, MetaResource.gems.quadText + " " + context.slot.openNowPrice);
			Helpers.SetTime(openNowTimeText, context.openTime, countdown: true);
		}
		else if (context.state == ChestWindowState.OpenNowForced)
		{
			Helpers.SetTextWithQuads(openNowButtonText, MetaResource.gems.quadText + " " + context.chest.openGems);
		}
		else if (context.state == ChestWindowState.BuyFromShop)
		{
			Helpers.SetTextWithQuads(buyButtonText, MetaResource.gems.quadText + " " + levelData.PriceGems.ToString());
		}
		Helpers.SetActive(chanceItemGroup, context.chanceItem);
		if ((bool)chanceItemView)
		{
			chanceItemView.SetObject(context.chanceItem);
		}
		if ((bool)context.slot)
		{
			Singleton<Meta>.Instance.AddObjectPropertyListener(context.slot, "openNowPrice", OnSlotOpenNowChanged);
		}
	}

	private void OnSlotOpenNowChanged(MetaObject metaObject, string propertyName)
	{
		Helpers.SetTextWithQuads(openNowButtonText, MetaResource.gems.quadText + " " + (metaObject as MetaRewardChestSlot).openNowPrice);
	}

	private void OnOpenClick()
	{
		CloseWindow(true);
	}

	private void OnOpenNowClick()
	{
		ChestWindowContext chestWindowContext = base.context as ChestWindowContext;
		int num = (chestWindowContext.chestLevel <= 0) ? MetaPlayer.local.level : chestWindowContext.chestLevel;
		int num2 = chestWindowContext.slot.openNowPrice;
		if (chestWindowContext.state == ChestWindowState.OpenNowForced)
		{
			num2 = chestWindowContext.chest.openGems;
		}
		if (MetaResource.gems.count < num2)
		{
			Singleton<WindowManager>.Instance.OpenWindow<GoToShopWindow>(null, OnGoToShopWindowClosed);
			return;
		}
		MetaResource.gems.count -= num2;
		AnalyticsManager.ResourceSink(MetaResource.gems.analyticsID, num2, "ChestOpen", chestWindowContext.chest.analyticsID);
		CloseWindow(true);
	}

	private void OnBuyClick()
	{
		ChestWindowContext chestWindowContext = base.context as ChestWindowContext;
		if (chestWindowContext != null)
		{
			if (MetaResource.gems.count < chestWindowContext.chest.levelData.PriceGems)
			{
				Singleton<WindowManager>.Instance.OpenWindow<GoToShopWindow>(null, OnGoToShopWindowClosed);
				return;
			}
			CloseWindow();
			int priceGems = chestWindowContext.chest.levelData.PriceGems;
			MetaResource.gems.count -= priceGems;
			AnalyticsManager.ResourceSink(MetaResource.gems.analyticsID, priceGems, "ChestBuy", chestWindowContext.chest.analyticsID);
			OpenChestWindowContext openChestWindowContext = new OpenChestWindowContext();
			openChestWindowContext.chest = chestWindowContext.chest;
			Singleton<WindowManager>.Instance.OpenWindow<OpenChestWindow>(openChestWindowContext);
		}
	}

	private void OnGoToShopWindowClosed(Window window, object returnValue)
	{
		if (returnValue != null)
		{
			CloseWindow();
		}
	}
}
