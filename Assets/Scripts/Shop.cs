using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class Shop : MonoBehaviour
{
	public MetaContainer itemsContainer;

	public MetaContainer chestsContainer;

	public MetaContainer gemsContainer;

	public MetaContainer coinsContainer;

	public ShopCoinsView prefabCoinsView;

	public ShopGemsView prefabGemsView;

	public ShopChestView prefabChestView;

	public ShopItemView prefabItemView;

	public ScrollRectTweener scrollRect;

	public TimeText offerTimer;

	private float checkItemsTimer;

	protected virtual void Start()
	{
		List<MetaProduct> list = Singleton<Meta>.Instance.FindObjects<MetaProduct>().ToList();
		foreach (MetaProduct item in list)
		{
			if (item.productType == MetaProductType.Coins)
			{
				coinsContainer.Add(item, prefabCoinsView);
			}
			else if (item.productType == MetaProductType.Gems)
			{
				gemsContainer.Add(item, prefabGemsView);
			}
		}
		List<MetaChest> source = Singleton<Meta>.Instance.FindObjects<MetaChest>().ToList();
		string[] chestsNames = new string[3]
		{
			"Huge",
			"Relic",
			"Eternal"
		};
		int i;
		for (i = 0; i < chestsNames.Length; i++)
		{
			MetaChest metaChest = (from x in source
				where x.displayName.Contains(chestsNames[i])
				select x).FirstOrDefault();
			if ((bool)metaChest && metaChest.levelData.PriceGems > 0)
			{
				chestsContainer.Add(metaChest, prefabChestView);
			}
		}
		DateTime utcNow = SingletonComponent<Meta, MetaTimeManager>.Instance.utcNow;
		MetaItem[] array = (from x in Singleton<Meta>.Instance.FindObjects<MetaItem>()
			where x.expireTime > utcNow
			orderby x.rarity
			select x).ToArray();
		if (array.Length == 3)
		{
			MetaItem[] array2 = array;
			foreach (MetaItem metaObject in array2)
			{
				itemsContainer.Add(metaObject, prefabItemView);
			}
			checkItemsTimer = 5f;
			Helpers.SetTime(offerTimer, array[0].expireTime, countdown: true);
		}
		else
		{
			checkItemsTimer = 0f;
		}
		if ((bool)offerTimer)
		{
			offerTimer.skipZeroValues = false;
		}
	}

	private void Update()
	{
		if (checkItemsTimer > 0f)
		{
			checkItemsTimer -= Time.deltaTime;
			return;
		}
		DateTime utcNow = SingletonComponent<Meta, MetaTimeManager>.Instance.utcNow;
		checkItemsTimer = 5f;
		MetaItem[] array = (from x in Singleton<Meta>.Instance.FindObjects<MetaItem>()
			where x.expireTime > utcNow
			select x).ToArray();
		if (array.Length < 3)
		{
			itemsContainer.Clear();
			int i;
			for (i = 0; i < 3; i++)
			{
				MetaItem metaItem = (from x in Singleton<Meta>.Instance.FindObjects<MetaItem>()
					where x.available
					where x.rarity == (MetaItemRarity)i
					orderby Guid.NewGuid()
					select x).First();
				metaItem.expireTime = utcNow + new TimeSpan(1, 0, 0);
				metaItem.purchasedCount = 0;
				itemsContainer.Add(metaItem, prefabItemView);
			}
			Helpers.SetTime(offerTimer, utcNow + new TimeSpan(1, 0, 0), countdown: true);
		}
	}

	public void ShowGems()
	{
		if ((bool)scrollRect)
		{
			scrollRect.Scroll(gemsContainer.GetComponent<RectTransform>(), fast: true);
		}
	}

	public void ShowCoins()
	{
		if ((bool)scrollRect)
		{
			scrollRect.Scroll(coinsContainer.GetComponent<RectTransform>(), fast: true);
		}
	}
}
