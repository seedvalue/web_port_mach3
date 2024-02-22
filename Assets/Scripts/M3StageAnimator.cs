using System.Collections.Generic;
using UnityEngine;

public class M3StageAnimator : MonoBehaviour
{
	[Header("Animated Objects")]
	public List<Animation> anims = new List<Animation>();

	public bool playAnim = true;

	private List<AnimationClip> clips = new List<AnimationClip>();

	private List<M3StageEvent> events = new List<M3StageEvent>();

	public void Init()
	{
		clips.Clear();
		foreach (Animation anim in anims)
		{
			if (anim != null)
			{
				clips.Add(anim.clip);
			}
		}
		events.Clear();
		GetComponentsInChildren(events);
	}

	public void SetDistance(float normalizedDistance)
	{
		if (clips.Count == 0)
		{
			Init();
		}
		bool flag = false;
		if (playAnim || !flag)
		{
			for (int i = 0; i < clips.Count; i++)
			{
				clips[i].SampleAnimation(anims[i].gameObject, normalizedDistance);
			}
			if (!flag)
			{
				foreach (M3StageEvent @event in events)
				{
					@event.UpdateEvents(normalizedDistance);
				}
			}
		}
	}
}
