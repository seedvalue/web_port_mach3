using System;
using System.Collections;

public class SequenceElement : IComparable<SequenceElement>
{
	private int priority;

	private IEnumerator coroutine;

	private Action callback;

	private bool coroutinePlaying;

	public SequenceElement(IEnumerator coroutine, int priority = 0)
	{
		this.priority = priority;
		this.coroutine = coroutine;
	}

	public SequenceElement(Action callback, int priority = 0)
	{
		this.priority = priority;
		this.callback = callback;
	}

	public bool UpdateElement(SequenceManager manager)
	{
		if (coroutine != null && !coroutinePlaying)
		{
			manager.StartCoroutine(RunCoroutine());
		}
		if (coroutinePlaying)
		{
			return true;
		}
		if (callback != null)
		{
			callback();
		}
		return false;
	}

	public int CompareTo(SequenceElement other)
	{
		return priority.CompareTo(other.priority);
	}

	private IEnumerator RunCoroutine()
	{
		coroutinePlaying = true;
		yield return coroutine;
		coroutine = null;
		coroutinePlaying = false;
	}
}
