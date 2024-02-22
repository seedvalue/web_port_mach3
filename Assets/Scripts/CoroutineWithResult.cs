using System;
using System.Collections;
using UnityEngine;

public class CoroutineWithResult
{
	public object result;

	private IEnumerator target;

	public Coroutine coroutine
	{
		get;
		private set;
	}

	public CoroutineWithResult(MonoBehaviour owner, IEnumerator target)
	{
		this.target = target;
		coroutine = owner.StartCoroutine(Run());
	}

	private IEnumerator Run()
	{
		while (target.MoveNext())
		{
			result = target.Current;
			if (result is Exception)
			{
				break;
			}
			yield return result;
		}
	}
}
