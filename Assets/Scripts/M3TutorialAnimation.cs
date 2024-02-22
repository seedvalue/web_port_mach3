using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M3TutorialAnimation : MonoBehaviour
{
	public GameObject animationPrefab;

	public float fadeInTime;

	public float fadeOutTime;

	private GameObject anim;

	private List<IM3TutorialAnimationClient> animClients = new List<IM3TutorialAnimationClient>();

	private List<SpriteRenderer> sprites = new List<SpriteRenderer>();

	private List<float> alpha = new List<float>();

	public void Init()
	{
		if ((bool)animationPrefab)
		{
			anim = UnityEngine.Object.Instantiate(animationPrefab, base.transform, worldPositionStays: false);
		}
		animClients.Clear();
		animClients.AddRange(GetComponentsInChildren<IM3TutorialAnimationClient>(includeInactive: true));
		sprites.Clear();
		sprites.AddRange(GetComponentsInChildren<SpriteRenderer>(includeInactive: true));
		for (int i = 0; i < sprites.Count; i++)
		{
			List<float> list = alpha;
			Color color = sprites[i].color;
			list.Add(color.a);
		}
	}

	public IEnumerator FadeIn()
	{
		Init();
		float time = 0f;
		if (fadeInTime > float.Epsilon)
		{
			do
			{
				time += Time.deltaTime;
				for (int i = 0; i < sprites.Count; i++)
				{
					Color color = sprites[i].color;
					color.a = alpha[i] * Mathf.Min(1f, time / fadeInTime);
					sprites[i].color = color;
				}
				yield return null;
			}
			while (time < fadeInTime);
		}
		else
		{
			for (int j = 0; j < sprites.Count; j++)
			{
				Color color2 = sprites[j].color;
				color2.a = alpha[j];
				sprites[j].color = color2;
			}
		}
		foreach (IM3TutorialAnimationClient animClient in animClients)
		{
			animClient.AfterFadeIn();
		}
	}

	public IEnumerator FadeOut()
	{
		foreach (IM3TutorialAnimationClient animClient in animClients)
		{
			animClient.BeforeFadeOut();
		}
		float time = 0f;
		if (fadeInTime > float.Epsilon)
		{
			do
			{
				time += Time.deltaTime;
				for (int i = 0; i < sprites.Count; i++)
				{
					Color color = sprites[i].color;
					color.a = alpha[i] * Mathf.Max(0f, 1f - time / fadeOutTime);
					sprites[i].color = color;
				}
				yield return null;
			}
			while (time < fadeInTime);
		}
		else
		{
			for (int j = 0; j < sprites.Count; j++)
			{
				Color color2 = sprites[j].color;
				color2.a = 0f;
				sprites[j].color = color2;
			}
		}
		if ((bool)anim)
		{
			anim.SetActive(value: false);
		}
	}
}
