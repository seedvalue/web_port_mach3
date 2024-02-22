using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Utils;

public abstract class MetaObject : ScriptableObject, IMetaObject, IComparable, IComparable<MetaObject>
{
	private readonly List<MetaObject> dependencies = new List<MetaObject>();

	public string metaID => base.name;

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
		Type type = GetType();
		PropertyInfo property = type.GetProperty(metaProperty);
		if (property != null)
		{
			return property.GetValue(this, null);
		}
		return type.GetField(metaProperty)?.GetValue(this);
	}

	public bool SetProperty(string metaProperty, object value)
	{
		Type type = GetType();
		PropertyInfo property = type.GetProperty(metaProperty);
		if (property != null)
		{
			property.SetValue(this, value, null);
			return true;
		}
		FieldInfo field = type.GetField(metaProperty);
		if (field != null)
		{
			field.SetValue(this, value);
			return true;
		}
		return false;
	}

	protected virtual bool OnPropertyWillChange(string propertyName, object before, object after)
	{
		return true;
	}

	protected virtual void OnPropertyChanged(string propertyName, object before, object after)
	{
	}

	protected virtual void OnDependencyChanged(MetaObject metaObject, string propertyName, object before, object after)
	{
	}

	protected void PropertySetter<T>(ref T field, T value, string propertyName)
	{
		if (!EqualityComparer<T>.Default.Equals(field, value) && OnPropertyWillChange(propertyName, field, value))
		{
			T val = field;
			field = value;
			OnPropertyChanged(propertyName, val, value);
			for (int i = 0; i < dependencies.Count; i++)
			{
				dependencies[i].OnDependencyChanged(this, propertyName, val, value);
			}
			Singleton<Meta>.Instance.OnPropertyChanged(this, propertyName);
		}
	}

	protected void DependsOn(MetaObject metaObject)
	{
		metaObject.dependencies.Add(this);
	}

	public int CompareTo(object obj)
	{
		MetaObject metaObject = obj as MetaObject;
		if (metaObject == null)
		{
			throw new ArgumentException("Argument is not MetaObject");
		}
		return CompareTo(metaObject);
	}

	public int CompareTo(MetaObject other)
	{
		return metaID.CompareTo(other.metaID);
	}
}
