using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[SingletonInitializeOnLoad]
[SingletonPrefab]
public class SequenceManager : Singleton<SequenceManager>
{
	public GameObject screenLock;

	private readonly List<SequenceElement> elements = new List<SequenceElement>();

	public bool playing
	{
		get;
		private set;
	}

	public static event Action BeginSequence;

	public static event Action EndSequence;

	public static void Enqueue(IEnumerator coroutine, int priority = 0)
	{
		Singleton<SequenceManager>.Instance.elements.AddSorted(new SequenceElement(coroutine, priority));
	}

	public static void Enqueue(Action callback, int priority = 0)
	{
		Singleton<SequenceManager>.Instance.elements.AddSorted(new SequenceElement(callback, priority));
	}

	public static void Play()
	{
		SequenceManager instance = Singleton<SequenceManager>.Instance;
		if (instance.elements.Count > 0)
		{
			instance.playing = true;
			SequenceManager.BeginSequence?.Invoke();
		}
	}

	public static void Skip()
	{
	}

	private void Update()
	{
		if (playing)
		{
			playing = UpdateElements();
			if (!playing)
			{
				SequenceManager.EndSequence?.Invoke();
			}
		}
		Helpers.SetActive(screenLock, playing);
	}

	private bool UpdateElements()
	{
		if (elements.Count == 0)
		{
			return false;
		}
		int num = 0;
		for (num = 0; num < elements.Count && !elements[num].UpdateElement(this); num++)
		{
		}
		elements.RemoveRange(0, num);
		return elements.Count > 0;
	}
}
