using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class ItemWindow : MetaWindow
{
	public Button upgradeButton;

	public Text upgradeCost;

	public Button useButton;

	public Button swapButton;

	public Text titleText;

	public Text rarityText;

	public Text typeText;

	public Image typeImage;

	public Text descriptionText;

	public MetaItemView itemView;

	public MetaContainer perksContainer;

	public MetaPerkView perkViewPrefab;

	public MetaSkillView skillView;

	public SimpleItemStatsContainer statsContainer;

	public UIProgressBar levelBar;

	public Image rarityAndTypeBkg;

	public List<Sprite> rarityAndTypeBkgByRarity;

	private bool showUpgradeStats;

	public new MetaItem GetObject()
	{
		return base.GetObject() as MetaItem;
	}

	[WindowTestMethod]
	public static void TestWindow()
	{
		ItemWindowContext itemWindowContext = new ItemWindowContext();
		itemWindowContext.item = Singleton<Meta>.Instance.FindRandomObject<MetaItem>();
		Singleton<WindowManager>.Instance.OpenWindow<ItemWindow>(itemWindowContext);
	}

	protected virtual void OnEnable()
	{
		ItemWindowContext itemWindowContext = base.context as ItemWindowContext;
		if (itemWindowContext != null)
		{
			InitWithContext(itemWindowContext);
		}
	}

	protected override void Start()
	{
		base.Start();
		Helpers.AddListenerOnClick(upgradeButton, OnUpgradeClicked);
		Helpers.AddListenerOnClick(useButton, OnUseClicked);
		Helpers.AddListenerOnClick(swapButton, OnSwapClicked);
	}

	private void InitWithContext(ItemWindowContext context)
	{
		SetObject(context.item);
		showUpgradeStats = true;
	}

	protected override void OnChanged()
	{
		base.OnChanged();
		MetaItem @object = GetObject();
		if ((bool)itemView)
		{
			itemView.SetObject(@object);
		}
		Helpers.SetText(titleText, @object.displayName);
		if ((bool)@object.skill && (bool)skillView)
		{
			skillView.SetObject(@object.skill);
		}
		if ((bool)perksContainer)
		{
			perksContainer.Assign(@object.perks, perkViewPrefab);
		}
		if ((bool)statsContainer)
		{
			statsContainer.SetItem(@object, showUpgradeStats && @object.GetCardsNumRequiredToUpdate() <= @object.count);
		}
		Helpers.SetText(rarityText, Enum.GetName(typeof(MetaItemRarity), @object.rarity));
		if ((bool)rarityText)
		{
			rarityText.color = SingletonComponent<Meta, MetaConsts>.Instance.textColorByRarity[(int)@object.rarity];
		}
		if ((bool)typeText)
		{
			typeText.color = SingletonComponent<Meta, MetaConsts>.Instance.textColorByRarity[(int)@object.rarity];
		}
		if ((bool)rarityAndTypeBkg)
		{
			Helpers.SetImage(rarityAndTypeBkg, rarityAndTypeBkgByRarity[(int)@object.rarity]);
		}
		if ((bool)typeImage)
		{
			typeImage.color = SingletonComponent<Meta, MetaConsts>.Instance.textColorByRarity[(int)@object.rarity];
			Helpers.SetImage(typeImage, SingletonComponent<Meta, MetaConsts>.Instance.iconSlotIcon[(int)@object.itemType]);
		}
		Helpers.SetText(typeText, Enum.GetName(typeof(MetaItemType), @object.itemType));
		Helpers.SetText(descriptionText, @object.description);
		if (@object.GetMaxItemLevel() == @object.level)
		{
			Helpers.SetActive(levelBar.gameObject, value: false);
		}
		else if ((bool)levelBar)
		{
			int cardsNumRequiredToUpdate = @object.GetCardsNumRequiredToUpdate();
			levelBar.SetValue(@object.count, cardsNumRequiredToUpdate);
		}
		if ((bool)upgradeButton)
		{
			upgradeButton.interactable = (@object.GetCardsNumRequiredToUpdate() <= @object.count);
		}
		Helpers.SetTextWithQuads(upgradeCost, MetaResource.coins.quadText + @object.GetCoinsRequiredToUpdate());
		Helpers.SetActiveGameObject(useButton, @object.found);
		Helpers.SetActiveGameObject(upgradeButton, @object.found);
		Helpers.SetActiveGameObject(swapButton, @object.found);
		if (@object.found)
		{
			Helpers.SetActiveGameObject(useButton, !@object.IsWorn());
			Helpers.SetActiveGameObject(swapButton, @object.IsWorn());
		}
	}

	private void OnSwapClicked()
	{
		MetaItem @object = GetObject();
		Armory armory = (base.context as ItemWindowContext).armory;
		if ((bool)armory)
		{
			armory.ScrollToItems(@object.itemType);
		}
		CloseWindow();
	}

	private void OnSwap(Window window, object returnValue)
	{
		MetaItem metaItem = returnValue as MetaItem;
		if (metaItem != null)
		{
			metaItem.InsertToSlot();
			CloseWindow();
		}
	}

	private void OnUpgradeClicked()
	{
		MetaItem @object = GetObject();
		if (MetaResource.coins.count < @object.GetCoinsRequiredToUpdate())
		{
			NotEnoughCoinsWindowContext notEnoughCoinsWindowContext = new NotEnoughCoinsWindowContext();
			notEnoughCoinsWindowContext.coinsToBuy = @object.GetCoinsRequiredToUpdate() - MetaResource.coins.count;
			notEnoughCoinsWindowContext.requiredGems = MetaProduct.CalculateGemPriceForCoins(notEnoughCoinsWindowContext.coinsToBuy);
			Singleton<WindowManager>.Instance.OpenWindow<NotEnoughCoinsWindow>(notEnoughCoinsWindowContext, OnNotEnoughCoinsWindowClosed);
		}
		else
		{
			ItemLevelUpWindowContext itemLevelUpWindowContext = new ItemLevelUpWindowContext();
			itemLevelUpWindowContext.item = @object;
			itemLevelUpWindowContext.newLevel = @object.level + 1;
			Singleton<WindowManager>.Instance.OpenWindow<ItemLevelUpWindow>(itemLevelUpWindowContext);
			showUpgradeStats = true;
			@object.Upgrade();
		}
	}

	public void OnNotEnoughCoinsWindowClosed(Window window, object returnValue)
	{
		if (returnValue != null)
		{
			CloseWindow();
		}
	}

	private void OnUseClicked()
	{
		MetaItem @object = GetObject();
		MetaItemViewSelectable.TryDeselect();
		Armory armory = (base.context as ItemWindowContext).armory;
		armory.UseAndSelectItem(GetObject());
		CloseWindow(true);
	}
}
