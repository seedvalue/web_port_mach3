using Utils;

public abstract class MetaWindow : Window
{
	private enum LateUpdateDirty
	{
		None,
		Changed,
		ObjectChanged
	}

	private MetaObject _object;

	private LateUpdateDirty _lateUpdateDirty;

	public bool SetObject(MetaObject value)
	{
		if (_object == value)
		{
			return true;
		}
		if (value != null && !AcceptsObject(value))
		{
			return false;
		}
		OnObjectWillChange();
		_object = value;
		OnObjectDidChanged();
		return true;
	}

	public MetaObject GetObject()
	{
		return _object;
	}

	public bool HasObject()
	{
		return _object != null;
	}

	public virtual bool AcceptsObject(MetaObject value)
	{
		return true;
	}

	protected virtual void OnObjectChanged()
	{
	}

	protected virtual void OnObjectReset()
	{
	}

	protected virtual void OnChanged()
	{
	}

	protected virtual void Awake()
	{
	}

	protected override void Start()
	{
		base.Start();
	}

	protected virtual void OnDisable()
	{
		SetObject(null);
	}

	protected virtual void LateUpdate()
	{
		if (_lateUpdateDirty >= LateUpdateDirty.ObjectChanged)
		{
			OnObjectChanged();
		}
		if (_lateUpdateDirty >= LateUpdateDirty.Changed)
		{
			OnChanged();
		}
		_lateUpdateDirty = LateUpdateDirty.None;
	}

	private void OnObjectWillChange()
	{
		if (_object != null)
		{
			Singleton<Meta>.Instance.RemObjectListener(_object, OnObjectListener);
		}
	}

	private void OnObjectDidChanged()
	{
		if (HasObject())
		{
			Singleton<Meta>.Instance.AddObjectListener(_object, OnObjectListener);
			_lateUpdateDirty = LateUpdateDirty.ObjectChanged;
		}
		else
		{
			OnObjectReset();
			_lateUpdateDirty = LateUpdateDirty.None;
		}
		MetaView[] components = GetComponents<MetaView>();
		foreach (MetaView metaView in components)
		{
			if (!(metaView.initialMetaObject != null))
			{
				metaView.SetObject(_object);
			}
		}
	}

	private void OnObjectListener(MetaObject metaObject, string propertyName)
	{
		_lateUpdateDirty = LateUpdateDirty.Changed;
	}
}
