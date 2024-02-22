using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ArmoryItemContainer : MetaContainer
{
	private List<Vector3> slotsLocPos;

	public void InitializeItemContainer()
	{
		if (base.isActiveAndEnabled)
		{
			StartCoroutine(DisableGrid());
		}
	}

	public void ResetItemContainer()
	{
		ContentSizeFitter component = GetComponent<ContentSizeFitter>();
		if ((bool)component)
		{
			component.enabled = true;
		}
		GridLayoutGroup component2 = GetComponent<GridLayoutGroup>();
		if ((bool)component2)
		{
			component2.enabled = true;
		}
		if (slotsLocPos != null)
		{
			slotsLocPos.Clear();
		}
		Clear();
	}

	public IEnumerator DisableGrid()
	{
		yield return new WaitForEndOfFrame();
		slotsLocPos = new List<Vector3>();
		ContentSizeFitter sizeFilter = GetComponent<ContentSizeFitter>();
		if ((bool)sizeFilter)
		{
			sizeFilter.enabled = false;
		}
		GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
		if ((bool)grid)
		{
			grid.enabled = false;
		}
		for (int i = 0; i < base.contents.Count; i++)
		{
			slotsLocPos.Add(base.contents[i].transform.localPosition);
		}
	}

	public void OrderItems(ArmoryOrder order)
	{
		List<MetaItemView> list = new List<MetaItemView>();
		switch (order)
		{
		case ArmoryOrder.Rarity:
			list = (from MetaItemView o in base.contents
				orderby o.GetObject().rarity
				select o into x
				orderby x.GetObject().found descending
				orderby x.GetObject().available descending
				select x).ToList();
			break;
		case ArmoryOrder.Level:
			list = (from MetaItemView o in base.contents
				orderby o.GetObject().level
				select o into x
				orderby x.GetObject().found descending
				orderby x.GetObject().available descending
				select x).ToList();
			break;
		}
		for (int i = 0; i < list.Count; i++)
		{
			FlyTo component = list[i].GetComponent<FlyTo>();
			if ((bool)component)
			{
				component.flyTo(slotsLocPos[i], 0.25f);
			}
		}
	}
}
