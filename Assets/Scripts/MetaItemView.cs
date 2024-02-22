using UnityEngine;
using UnityEngine.UI;
using Utils;

public class MetaItemView : MetaView
{
	public Text nameText;

	public Text countText;

	public Text levelText;

	public Image iconImage;

	public Image bkgImage;

	public Image levelBkgImage;

	public Text rarityText;

	[Header("Item progress bar")]
	public ItemProgressBar itemLevelBar;

	public RectTransform itemLevelBarNode;

	public ItemProgressBar itemLevelBarPrefab;

	public bool levelBarAlwaysVisible;

	public Image slotTypeImage;

	public GameObject justFoundGroup;

	public Text justFoundText;

	[Header("Particle systems")]
	public ParticleSystem epicParticleSystem;

	public ParticleSystem legendaryParticleSystem;

	public new MetaItem GetObject()
	{
		return base.GetObject() as MetaItem;
	}

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
		MetaItem @object = GetObject();
		Helpers.SetText(nameText, @object.displayName);
		Helpers.SetActiveGameObject(levelBkgImage, @object.available);
		if (@object.available)
		{
			Helpers.SetImage(iconImage, @object.icon);
		}
		else
		{
			Helpers.SetImage(iconImage, SingletonComponent<Meta, MetaConsts>.Instance.questionMarkIcon);
		}
		if (!itemLevelBar && (bool)itemLevelBarPrefab)
		{
			itemLevelBar = Object.Instantiate(itemLevelBarPrefab, itemLevelBarNode).GetComponent<ItemProgressBar>();
			itemLevelBar.transform.localPosition = Vector3.zero;
			itemLevelBar.transform.localScale = Vector3.one;
		}
		Helpers.SetActiveGameObject(itemLevelBar, @object.found || levelBarAlwaysVisible);
		Helpers.SetImage(levelBkgImage, SingletonComponent<Meta, MetaConsts>.Instance.bkgLvlByRarity[(int)@object.rarity]);
		Helpers.SetImage(bkgImage, SingletonComponent<Meta, MetaConsts>.Instance.bkgByRarity[(int)@object.rarity]);
		Helpers.SetActiveGameObject(epicParticleSystem, @object.rarity == MetaItemRarity.Epic && @object.found);
		Helpers.SetActiveGameObject(legendaryParticleSystem, @object.rarity == MetaItemRarity.Legendary && @object.found);
		if ((bool)rarityText)
		{
			switch (@object.rarity)
			{
			case MetaItemRarity.Common:
				rarityText.text = "Common";
				break;
			case MetaItemRarity.Rare:
				rarityText.text = "Rare";
				break;
			case MetaItemRarity.Epic:
				rarityText.text = "Epic";
				break;
			case MetaItemRarity.Legendary:
				rarityText.text = "Legendary";
				break;
			}
			Helpers.SetRGB(rarityText, SingletonComponent<Meta, MetaConsts>.Instance.textColorByRarity[(int)@object.rarity]);
		}
		if ((bool)levelText)
		{
			levelText.color = SingletonComponent<Meta, MetaConsts>.Instance.textColorByRarity[(int)@object.rarity];
		}
		if ((bool)slotTypeImage && (int)@object.itemType < SingletonComponent<Meta, MetaConsts>.Instance.iconSlotIcon.Length)
		{
			Helpers.SetImage(slotTypeImage, SingletonComponent<Meta, MetaConsts>.Instance.iconSlotIcon[(int)@object.itemType]);
		}
	}

	protected override void OnChanged()
	{
		base.OnChanged();
		MetaItem @object = GetObject();
		Helpers.SetActiveGameObject(levelBkgImage, @object.available);
		Helpers.SetActive(justFoundGroup, GetPropertyOrDefault<bool>("justFound"));
		if ((bool)justFoundText)
		{
			justFoundText.enabled = GetPropertyOrDefault<bool>("justFound");
		}
		if (@object.available)
		{
			Helpers.SetImage(iconImage, @object.icon);
		}
		else
		{
			Helpers.SetImage(iconImage, SingletonComponent<Meta, MetaConsts>.Instance.questionMarkIcon);
		}
		Helpers.SetText(countText, "x" + GetPropertyOrDefault<int>("count").ToString());
		Helpers.SetText(levelText, "Level " + @object.level.ToString());
		if (@object.GetMaxItemLevel() == @object.level)
		{
			Helpers.SetActive(itemLevelBar.gameObject, false || levelBarAlwaysVisible);
		}
		else if ((bool)itemLevelBar)
		{
			if (@object.found)
			{
				int cardsNumRequiredToUpdate = @object.GetCardsNumRequiredToUpdate();
				itemLevelBar.SetProgress((float)@object.count / (float)cardsNumRequiredToUpdate, @object.count.ToString() + "/" + cardsNumRequiredToUpdate.ToString());
			}
			else
			{
				itemLevelBar.SetProgress(0f, "0/1");
			}
		}
	}
}
