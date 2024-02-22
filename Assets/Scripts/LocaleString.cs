using System;
using UnityEngine;
using Utils;

[Serializable]
public class LocaleString
{
	private enum State : byte
	{
		Dirty,
		Resolved
	}

	[SerializeField]
	private string _key;

	private string _text;

	private State _state;

	private static readonly WeakReferenceSet<LocaleString> _instances = new WeakReferenceSet<LocaleString>();

	public string text
	{
		get
		{
			Refresh();
			return _text;
		}
	}

	public string key
	{
		get
		{
			return _key;
		}
		set
		{
			_key = value;
			_state = State.Dirty;
		}
	}

	public LocaleString()
	{
		_instances.Add(this);
	}

	public LocaleString(string key)
	{
		_instances.Add(this);
		_key = key;
	}

	~LocaleString()
	{
		_instances.Remove(this);
	}

	public override string ToString()
	{
		return text;
	}

	public static implicit operator string(LocaleString ls)
	{
		return ls.text;
	}

	protected virtual void Refresh()
	{
		if (_state != State.Resolved)
		{
			_text = Singleton<LocaleSystem>.Instance.ResolveKey(_key);
			_state = State.Resolved;
		}
	}

	protected virtual void OnLocaleChanged()
	{
		_state = State.Dirty;
	}

	public static void NotifyOnLocaleChanged()
	{
		LocaleString[] objectsAndCleanup = _instances.GetObjectsAndCleanup();
		foreach (LocaleString localeString in objectsAndCleanup)
		{
			localeString.OnLocaleChanged();
		}
	}
}
