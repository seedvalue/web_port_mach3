using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utils;

public abstract class MetaView : MonoBehaviour
{
	private enum LateUpdateDirty
	{
		None,
		Changed,
		ObjectChanged
	}

	[Header("Meta View")]
	[SerializeField]
	[FormerlySerializedAs("interactable")]
	private bool _interactable = true;

	public bool masterView;

	public MetaObject initialMetaObject;

	public Button interactButton;

	public UnityEvent onClick;

	private MetaLink _link;

	private LateUpdateDirty _lateUpdateDirty;

	public bool interactable
	{
		get
		{
			return _interactable;
		}
		set
		{
			_interactable = value;
			if ((bool)interactButton)
			{
				interactButton.interactable = value;
			}
		}
	}

	public bool SetLink(MetaLink value)
	{
		if (_link == value)
		{
			return true;
		}
		if (value != null && !AcceptsObject(value.metaObject))
		{
			return false;
		}
		OnObjectWillChange();
		_link = value;
		OnObjectDidChanged();
		return true;
	}

	public bool SetObject(MetaObject value)
	{
		return SetLink(new MetaLink(value));
	}

	public MetaLink GetLink()
	{
		return _link;
	}

	public MetaObject GetObject()
	{
		if (_link == null)
		{
			return null;
		}
		return _link.metaObject;
	}

	public bool HasObject()
	{
		return GetObject() != null;
	}

	public virtual bool AcceptsObject(MetaObject value)
	{
		return true;
	}

	public bool HasProperty(string metaProperty)
	{
		return GetPropertyValue(metaProperty) != null;
	}

	public T GetProperty<T>(string metaProperty) where T : class
	{
		return GetPropertyValue(metaProperty) as T;
	}

	public T GetPropertyOrDefault<T>(string metaProperty) where T : struct
	{
		return (GetPropertyValue(metaProperty) as T?).GetValueOrDefault();
	}

	public object GetPropertyValue(string metaProperty)
	{
		if (_link == null)
		{
			return null;
		}
		if (_link.HasProperty(metaProperty))
		{
			return _link.GetPropertyValue(metaProperty);
		}
		if (_link.metaObject != null)
		{
			return _link.metaObject.GetPropertyValue(metaProperty);
		}
		return null;
	}

	protected virtual void OnObjectChanged()
	{
	}

	protected virtual void OnObjectReset()
	{
		onClick.RemoveAllListeners();
	}

	protected virtual void OnChanged()
	{
	}

	protected virtual void OnInteract()
	{
	}

	protected virtual void Awake()
	{
		SetObject(initialMetaObject);
		if ((bool)interactButton)
		{
			interactButton.onClick.AddListener(OnInteractButtonClicked);
			interactButton.interactable = interactable;
		}
	}

	protected virtual void Start()
	{
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
		MetaObject @object = GetObject();
		if (@object != null)
		{
			Singleton<Meta>.Instance.RemObjectListener(@object, OnObjectListener);
		}
	}

	private void OnObjectDidChanged()
	{
		MetaObject @object = GetObject();
		if (@object != null)
		{
			Singleton<Meta>.Instance.AddObjectListener(@object, OnObjectListener);
			_lateUpdateDirty = LateUpdateDirty.ObjectChanged;
		}
		else
		{
			OnObjectReset();
			_lateUpdateDirty = LateUpdateDirty.None;
		}
	}

	private void OnInteractButtonClicked()
	{
		OnInteract();
		onClick.Invoke();
	}

	private void OnObjectListener(MetaObject metaObject, string propertyName)
	{
		if (_lateUpdateDirty < LateUpdateDirty.Changed)
		{
			_lateUpdateDirty = LateUpdateDirty.Changed;
		}
	}

	public static T FindMasterView<T>(MetaObject metaObject) where T : MetaView
	{
		return (from r in UnityEngine.Object.FindObjectsOfType<T>()
			where r.masterView && r.GetObject() == metaObject
			select r).FirstOrDefault();
	}
}
