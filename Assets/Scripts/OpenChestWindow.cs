using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public class OpenChestWindow : MetaWindow, IPointerClickHandler, IEventSystemHandler
{
	public Image chestImage;

	public Text chestName;

	public Text cardsCount;

	public RewardsContainer rewardsContainer;

	public GameObject chestRoot;

	public CanvasGroup cardsCounterGroup;

	private bool closeOnNextTap;

	public GameObject youGot;

	public GameObject reward;

	[Header("Sfx")]
	public AudioSample sfxStart;

	public AudioSample sfxCardCountIncrease;

	public AudioSample sfxCardSummary;

	private GameObject chest3D;

	private List<MetaLink> rewards;

	private List<MetaLink> rewardsLeft;

	public GameObject rewardBkg;

	[WindowTestMethod]
	public static void TestWindow()
	{
		OpenChestWindowContext openChestWindowContext = new OpenChestWindowContext();
		openChestWindowContext.chest = Singleton<Meta>.Instance.FindRandomObject<MetaChest>();
		Singleton<WindowManager>.Instance.OpenWindow<OpenChestWindow>(openChestWindowContext);
	}

	[WindowTestMethod]
	public static void TestSilver()
	{
		OpenChestWindowContext openChestWindowContext = new OpenChestWindowContext();
		openChestWindowContext.chest = (from c in Singleton<Meta>.Instance.FindObjects<MetaChest>()
			where c.metaID.Contains("Silver")
			select c).FirstOrDefault();
		Singleton<WindowManager>.Instance.OpenWindow<OpenChestWindow>(openChestWindowContext);
	}

	[WindowTestMethod]
	public static void TestGold()
	{
		OpenChestWindowContext openChestWindowContext = new OpenChestWindowContext();
		openChestWindowContext.chest = (from c in Singleton<Meta>.Instance.FindObjects<MetaChest>()
			where c.metaID.Contains("Gold")
			select c).FirstOrDefault();
		Singleton<WindowManager>.Instance.OpenWindow<OpenChestWindow>(openChestWindowContext);
	}

	[WindowTestMethod]
	public static void TesteTERNAL()
	{
		OpenChestWindowContext openChestWindowContext = new OpenChestWindowContext();
		openChestWindowContext.chest = (from c in Singleton<Meta>.Instance.FindObjects<MetaChest>()
			where c.metaID.Contains("Eternal")
			select c).FirstOrDefault();
		Singleton<WindowManager>.Instance.OpenWindow<OpenChestWindow>(openChestWindowContext);
	}

	public new MetaChest GetObject()
	{
		return base.GetObject() as MetaChest;
	}

	protected virtual void OnEnable()
	{
		OpenChestWindowContext openChestWindowContext = base.context as OpenChestWindowContext;
		if (openChestWindowContext != null)
		{
			InitWithContext(openChestWindowContext);
		}
	}

	private void InitWithContext(OpenChestWindowContext context)
	{
		List<GameObject> list = new List<GameObject>();
		IEnumerator enumerator = reward.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				list.Add(transform.gameObject);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		list.ForEach(delegate(GameObject child)
		{
			UnityEngine.Object.Destroy(child);
		});
		closeOnNextTap = false;
		SetObject(context.chest);
		if ((bool)context.content)
		{
			rewards = context.content.content;
		}
		else
		{
			int level = (context.chestLevel <= 0) ? MetaPlayer.local.level : context.chestLevel;
			rewards = context.chest.PickRewards(level, context.chanceItem);
		}
		foreach (MetaLink reward2 in rewards)
		{
			MetaItem metaItem = reward2.metaObject as MetaItem;
			if ((bool)metaItem)
			{
				UnityEngine.Debug.Log(metaItem.displayName + " " + metaItem.found);
				reward2.SetProperty("justFound", !metaItem.found);
			}
		}
		rewardsLeft = new List<MetaLink>();
		rewardsLeft.AddRange(rewards);
		Helpers.SetActive(rewardBkg, value: true);
		if (chest3D != null)
		{
			UnityEngine.Object.DestroyObject(chest3D);
		}
		chest3D = UnityEngine.Object.Instantiate(context.chest.vis3D, chestRoot.transform);
		chest3D.transform.localScale = Vector3.one;
		SetLayerRecursively(chest3D, chestRoot.layer);
		youGot.SetActive(value: false);
		reward.SetActive(value: true);
		Helpers.SetText(cardsCount, rewardsLeft.Count.ToString());
		cardsCounterGroup.alpha = 0f;
		UITweenerEx.fadeIn(cardsCounterGroup, 0.25f);
		StartCoroutine(DoShowNextReward(firstReward: true));
		Singleton<Meta>.Instance.Invoke(rewards, MetaActionType.Inc);
	}

	public IEnumerator DoShowNextReward(bool firstReward)
	{
		MetaRewardView rewardView = reward.GetComponent<MetaRewardView>();
		if (!firstReward)
		{
			Bounce();
		}
		else
		{
			AudioManager.PlaySafe(sfxStart);
		}
		MetaLink nextReward = rewardsLeft[0];
		rewardsLeft.RemoveAt(0);
		if (rewardsLeft.Count == 0)
		{
			UITweenerEx.fadeOut(cardsCounterGroup, 0.25f);
		}
		else
		{
			Helpers.SetText(cardsCount, rewardsLeft.Count.ToString());
		}
		rewardView.SetLink(nextReward);
		yield return new WaitForSeconds(0.1f);
	}

	private void ShowNextReward(bool withBouce = false)
	{
		if (rewardsLeft.Count == 0)
		{
			if (closeOnNextTap)
			{
				CloseWindow();
			}
			closeOnNextTap = true;
			Helpers.SetActive(rewardBkg, value: false);
			youGot.SetActive(value: true);
			rewardsContainer.Assign(rewards);
			reward.SetActive(value: false);
			AudioManager.PlaySafe(sfxCardSummary);
		}
		else
		{
			StartCoroutine(DoShowNextReward(firstReward: false));
		}
	}

	public static void SetLayerRecursively(GameObject go, int layerNumber)
	{
		Transform[] componentsInChildren = go.GetComponentsInChildren<Transform>(includeInactive: true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			transform.gameObject.layer = layerNumber;
		}
	}

	public void Bounce()
	{
		if ((bool)chest3D)
		{
			Animator component = chest3D.GetComponent<Animator>();
			if ((bool)component)
			{
				component.SetBool("Bounce", value: true);
			}
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		MetaRewardView component = reward.GetComponent<MetaRewardView>();
		if (component.allowSkip)
		{
			ShowNextReward();
		}
	}
}
