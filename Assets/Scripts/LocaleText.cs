using UnityEngine;
using UnityEngine.UI;
using Utils;

public class LocaleText : MonoBehaviour
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

	private Text affectedObject;

	private static readonly WeakReferenceSet<LocaleText> _instances = new WeakReferenceSet<LocaleText>();

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

	protected virtual void Awake()
	{
		affectedObject = GetComponent<Text>();
	}

	protected virtual void Start()
	{
		affectedObject.text = text;
	}

	protected virtual void OnEnable()
	{
		_instances.Add(this);
	}

	protected virtual void OnDisable()
	{
		_instances.Remove(this);
	}

	protected virtual void Refresh()
	{
		if (_state == State.Resolved)
		{
			_text = Singleton<LocaleSystem>.Instance.ResolveKey(_key);
			_state = State.Resolved;
		}
	}

	protected virtual void OnLocaleChanged()
	{
		_state = State.Dirty;
		affectedObject.text = text;
	}

	public static void NotifyOnLocaleChanged()
	{
		LocaleText[] objectsAndCleanup = _instances.GetObjectsAndCleanup();
		foreach (LocaleText localeText in objectsAndCleanup)
		{
			localeText.OnLocaleChanged();
		}
	}
}
