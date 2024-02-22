using System;
using System.Collections.Generic;

public class MetaLink
{
	public MetaObject metaObject;

	private readonly Dictionary<string, object> metaProperties = new Dictionary<string, object>();

	public MetaLink()
	{
	}

	public MetaLink(MetaObject metaObject)
	{
		this.metaObject = metaObject;
	}

	public static MetaLink Create(MetaObject metaObject)
	{
		return new MetaLink(metaObject);
	}

	public static MetaLink Create(MetaObject metaObject, string property, object value)
	{
		MetaLink metaLink = new MetaLink(metaObject);
		metaLink.SetProperty(property, value);
		return metaLink;
	}

	public bool HasProperty(string metaProperty)
	{
		return metaProperties.ContainsKey(metaProperty);
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
		object value = null;
		if (metaProperties.TryGetValue(metaProperty, out value))
		{
			return value;
		}
		return null;
	}

	public void SetProperty<T>(string metaProperty, T value)
	{
		metaProperties.Add(metaProperty, value);
	}

	public void RemProperty<T>(string metaProperty)
	{
		metaProperties.Remove(metaProperty);
	}

	public void Invoke(MetaActionType actionType)
	{
		foreach (KeyValuePair<string, object> metaProperty in metaProperties)
		{
			Invoke(actionType, metaProperty.Key, metaProperty.Value);
		}
	}

	private void Invoke(MetaActionType actionType, string metaProperty, object value)
	{
		object value2 = null;
		switch (actionType)
		{
		case MetaActionType.Inc:
			value2 = Inc(metaObject.GetPropertyValue(metaProperty), value);
			break;
		case MetaActionType.Dec:
			value2 = Dec(metaObject.GetPropertyValue(metaProperty), value);
			break;
		case MetaActionType.Set:
			value2 = value;
			break;
		}
		metaObject.SetProperty(metaProperty, value2);
	}

	private static object Inc(object a, object b)
	{
		Type type = a.GetType();
		Type type2 = b.GetType();
		if (type != type2)
		{
			throw new Exception("Cannot add '" + type + "' to '" + type2 + "'");
		}
		if (type == typeof(float))
		{
			float num = (float)a;
			float num2 = (float)b;
			return num + num2;
		}
		if (type == typeof(int))
		{
			int num3 = (int)a;
			int num4 = (int)b;
			return num3 + num4;
		}
		if (type == typeof(bool))
		{
			return (bool)a;
		}
		throw new Exception("Cannot add '" + type + "' to '" + type2 + "'");
	}

	private static object Dec(object a, object b)
	{
		Type type = a.GetType();
		Type type2 = b.GetType();
		if (type != type2)
		{
			throw new Exception("Cannot add '" + type + "' to '" + type2 + "'");
		}
		if (type == typeof(float))
		{
			float num = (float)a;
			float num2 = (float)b;
			return num - num2;
		}
		if (type == typeof(int))
		{
			int num3 = (int)a;
			int num4 = (int)b;
			return num3 - num4;
		}
		throw new Exception("Cannot add '" + type + "' to '" + type2 + "'");
	}
}
