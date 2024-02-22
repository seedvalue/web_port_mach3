using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class SimpleItemStatsContainer : MonoBehaviour
{
	public SimpleItemStatView statViewPrefab;

	public Sprite[] statsIcons = new Sprite[8];

	public Color[] statsColor = new Color[8];

	public bool waitForAnim;

	private bool showUpgrade;

	public void SetItem(MetaItem item, bool showUpgradeStats, int forcedLevel = 0)
	{
		List<GameObject> list = new List<GameObject>();
		IEnumerator enumerator = base.transform.GetEnumerator();
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
		showUpgrade = showUpgradeStats;
		int num = (forcedLevel == 0) ? item.level : forcedLevel;
		Stats metaItemStats = MetaItem.GetMetaItemStats(item, num);
		Stats stats = MetaItem.GetMetaItemStats(item, num + 1) - metaItemStats;
		Stats metaItemPercentStats = MetaItem.GetMetaItemPercentStats(item, num);
		Stats stats2 = MetaItem.GetMetaItemPercentStats(item, num + 1) - metaItemPercentStats;
		AddStatToContainer(metaItemStats.HP, stats.HP, "Health:", statsIcons[0], statsColor[0], string.Empty, string.Empty);
		AddStatToContainer(metaItemPercentStats.HP, stats2.HP, "Health:", statsIcons[0], statsColor[0], "%", "+");
		AddStatToContainer(metaItemStats.Def, stats.Def, "Defence: ", statsIcons[1], statsColor[1], string.Empty, string.Empty);
		AddStatToContainer(metaItemPercentStats.Def, stats2.Def, "Defence: ", statsIcons[1], statsColor[1], "%", "+");
		AddStatToContainer(metaItemStats.Rcv, stats.Rcv, "Recovery: ", statsIcons[2], statsColor[2], string.Empty, string.Empty);
		AddStatToContainer(metaItemPercentStats.Rcv, stats2.Rcv, "Recovery: ", statsIcons[2], statsColor[2], "%", "+");
		AddStatToContainer(metaItemStats.Mel, stats.Mel, "Melee: ", statsIcons[3], statsColor[3], string.Empty, string.Empty);
		AddStatToContainer(metaItemPercentStats.Mel, stats2.Mel, "Melee: ", statsIcons[3], statsColor[3], "%", "+");
		AddStatToContainer(metaItemStats.Rng, stats.Rng, "Ranged: ", statsIcons[4], statsColor[4], string.Empty, string.Empty);
		AddStatToContainer(metaItemPercentStats.Rng, stats2.Rng, "Ranged: ", statsIcons[4], statsColor[4], "%", "+");
		AddStatToContainer(metaItemStats.Hrm, stats.Hrm, "Harmony: ", statsIcons[5], statsColor[5], string.Empty, string.Empty);
		AddStatToContainer(metaItemPercentStats.Hrm, stats2.Hrm, "Harmony: ", statsIcons[5], statsColor[5], "%", "+");
		AddStatToContainer(metaItemStats.Prm, stats.Prm, "Pyromancy: ", statsIcons[6], statsColor[6], string.Empty, string.Empty);
		AddStatToContainer(metaItemPercentStats.Prm, stats2.Prm, "Pyromancy: ", statsIcons[6], statsColor[6], "%", "+");
		AddStatToContainer(metaItemStats.Vd, stats.Vd, "Void: ", statsIcons[7], statsColor[7], string.Empty, string.Empty);
		AddStatToContainer(metaItemPercentStats.Vd, stats2.Vd, "Void: ", statsIcons[7], statsColor[7], "%", "+");
	}

	public void SetStats(Stats stats, Stats newStats, bool showUpgradeStats)
	{
		List<GameObject> list = new List<GameObject>();
		IEnumerator enumerator = base.transform.GetEnumerator();
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
		showUpgrade = showUpgradeStats;
		Stats stats2 = newStats - stats;
		AddStatToContainer(stats.HP, stats2.HP, "Health:", statsIcons[0], statsColor[0], string.Empty, string.Empty);
		AddStatToContainer(stats.Rcv, stats2.Rcv, "Recovery: ", statsIcons[2], statsColor[2], string.Empty, string.Empty);
	}

	private void AddStatToContainer(int statValue, int upgValue, string statName, Sprite icon, Color color, string postFix = "", string prefix = "")
	{
		if (statValue > 0)
		{
			if (!showUpgrade)
			{
				upgValue = 0;
			}
			SimpleItemStatView simpleItemStatView = UnityEngine.Object.Instantiate(statViewPrefab, base.transform);
			simpleItemStatView.transform.localScale = Vector3.one;
			if ((float)upgValue < float.Epsilon)
			{
				simpleItemStatView.Set(statName, statValue, 0, prefix, postFix, icon);
			}
			else
			{
				simpleItemStatView.Set(statName, statValue, upgValue, prefix, postFix, icon);
			}
			Helpers.SetTextColor(simpleItemStatView.statText, color);
			Helpers.SetActive(simpleItemStatView.gameObject, !waitForAnim);
		}
	}

	public IEnumerator upgradeItemSequence(AudioSample sfxStatAppear)
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			SimpleItemStatView child = base.transform.GetChild(i).GetComponent<SimpleItemStatView>();
			if ((bool)child)
			{
				AudioManager.PlaySafe(sfxStatAppear);
				Helpers.SetActive(child.gameObject, value: true);
				StartCoroutine(child.upgradeItemSequence());
				yield return new WaitForSeconds(0.75f);
			}
		}
		yield return null;
	}
}
