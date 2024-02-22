using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class RewardsContainer : MonoBehaviour
{
	public MetaContainer firstLine;

	public MetaContainer secondLine;

	public MetaItemView prefabItemReward;

	public MetaResourceView prefabResourceReward;

	[Header("Animation")]
	public bool useAnim;

	public float animTime = 0.5f;

	public float animInterval = 0.25f;

	public float animScale = 1.5f;

	private List<MetaLink> rewards;

	public void Assign(List<MetaLink> rewardsList)
	{
		Clear();
		rewards.AddRange(rewardsList);
		Helpers.SetActiveGameObject(secondLine, rewards.Count > 4);
		int num = (rewards.Count <= 4) ? rewards.Count : (rewards.Count / 2);
		for (int i = 0; i < rewards.Count; i++)
		{
			MetaContainer metaContainer = (i >= num) ? secondLine : firstLine;
			if ((bool)(rewards[i].metaObject as MetaItem))
			{
				metaContainer.Add(rewards[i], prefabItemReward);
			}
			else if ((bool)(rewards[i].metaObject as MetaResource))
			{
				metaContainer.Add(rewards[i], prefabResourceReward);
			}
		}
		if (useAnim)
		{
			StartCoroutine(AssignSequence());
		}
	}

	public IEnumerator AssignSequence()
	{
		foreach (MetaView content in firstLine.contents)
		{
			content.GetComponent<CanvasGroup>().alpha = 0f;
			content.transform.localScale = Vector3.one * animScale;
		}
		foreach (MetaView content2 in secondLine.contents)
		{
			content2.GetComponent<CanvasGroup>().alpha = 0f;
			content2.transform.localScale = Vector3.one * animScale;
		}
		foreach (MetaView child2 in firstLine.contents)
		{
			UITweenerEx.fadeIn(child2, animTime);
			UITweenerEx.scaleTo(child2, 1f, animTime);
			yield return new WaitForSeconds(animInterval);
		}
		foreach (MetaView child in secondLine.contents)
		{
			UITweenerEx.fadeIn(child, animTime);
			UITweenerEx.scaleTo(child, 1f, animTime);
			yield return new WaitForSeconds(animInterval);
		}
	}

	public void Clear()
	{
		if (rewards != null)
		{
			rewards.Clear();
		}
		else
		{
			rewards = new List<MetaLink>();
		}
		if ((bool)firstLine)
		{
			firstLine.Clear();
		}
		if ((bool)secondLine)
		{
			secondLine.Clear();
		}
	}
}
