using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Utils;

public static class MetaReflection
{
	public enum Message
	{
		Awake,
		Start,
		Update,
		Reset,
		StaticAwake,
		StaticStart
	}

	public struct DataField
	{
		public FieldInfo Field;

		public object DefaultValue;

		public DataField(FieldInfo field, object defaultValue)
		{
			Field = field;
			DefaultValue = defaultValue;
		}
	}

	public class Type
	{
		public MethodInfo AwakeMessage
		{
			get;
			private set;
		}

		public MethodInfo StartMessage
		{
			get;
			private set;
		}

		public MethodInfo UpdateMessage
		{
			get;
			private set;
		}

		public MethodInfo ResetMessage
		{
			get;
			private set;
		}

		public MethodInfo StaticAwakeMessage
		{
			get;
			private set;
		}

		public MethodInfo StaticStartMessage
		{
			get;
			private set;
		}

		public DataField[] DataFields
		{
			get;
			private set;
		}

		public MethodInfo GetMessage(Message message)
		{
			switch (message)
			{
			case Message.Awake:
				return AwakeMessage;
			case Message.Start:
				return StartMessage;
			case Message.Update:
				return UpdateMessage;
			case Message.Reset:
				return ResetMessage;
			case Message.StaticAwake:
				return StaticAwakeMessage;
			case Message.StaticStart:
				return StaticStartMessage;
			default:
				return null;
			}
		}

		public static Type Build(System.Type type)
		{
			Type type2 = new Type();
			type2.AwakeMessage = type.GetMethod("MetaAwake", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			type2.StartMessage = type.GetMethod("MetaStart", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			type2.UpdateMessage = type.GetMethod("MetaUpdate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			type2.ResetMessage = type.GetMethod("MetaReset", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			type2.StaticAwakeMessage = type.GetMethod("MetaStaticAwake", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			type2.StaticStartMessage = type.GetMethod("MetaStaticStart", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			type2.DataFields = BuildDataFields(type).ToArray();
			return type2;
		}

		private static IEnumerable<DataField> BuildDataFields(System.Type type)
		{
			IEnumerable<DataField> enumerable = from data in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).Select(delegate(FieldInfo field)
				{
					MetaDataAttribute metaDataAttribute = field.GetCustomAttributes(typeof(MetaDataAttribute), inherit: true).Cast<MetaDataAttribute>().FirstOrDefault();
					if (metaDataAttribute == null)
					{
						return new DataField(null, null);
					}
					object obj = metaDataAttribute.DefaultValue;
					if (field.FieldType.IsArray)
					{
						obj = Activator.CreateInstance(field.FieldType, metaDataAttribute.Count);
						Array array = obj as Array;
						for (int i = 0; i < array.Length; i++)
						{
							array.SetValue(metaDataAttribute.DefaultValue, i);
						}
						if (!field.FieldType.IsAssignableFrom(obj.GetType()))
						{
							UnityEngine.Debug.LogErrorFormat("Field '{0}' of type '{1}' has bad default value! Type '{2}' is not assignable from '{3}'!", field.Name, type, field.FieldType, obj.GetType());
							obj = Activator.CreateInstance(field.FieldType);
						}
					}
					else if (Reflection.IsTypeNullable(field.FieldType))
					{
						if (obj != null && !field.FieldType.IsAssignableFrom(obj.GetType()))
						{
							UnityEngine.Debug.LogErrorFormat("Field '{0}' of type '{1}' has bad default value! Type '{2}' is not assignable from '{3}'!", field.Name, type, field.FieldType, obj.GetType());
						}
					}
					else
					{
						if (obj == null)
						{
							obj = Activator.CreateInstance(field.FieldType);
						}
						if (!field.FieldType.IsAssignableFrom(obj.GetType()))
						{
							UnityEngine.Debug.LogErrorFormat("Field '{0}' of type '{1}' has bad default value! Type '{2}' is not assignable from '{3}'!", field.Name, type, field.FieldType, obj.GetType());
							obj = Activator.CreateInstance(field.FieldType);
						}
					}
					return new DataField(field, obj);
				})
				where data.Field != null
				select data;
			if (type.BaseType != null)
			{
				enumerable = enumerable.Concat(BuildDataFields(type.BaseType));
			}
			return enumerable;
		}
	}

	private static readonly Dictionary<System.Type, Type> types;

	private const BindingFlags BaseFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

	private const BindingFlags InstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

	private const BindingFlags StaticFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

	static MetaReflection()
	{
		types = new Dictionary<System.Type, Type>();
		Rebuild();
	}

	public static void Rebuild()
	{
		types.Clear();
		foreach (System.Type item in Reflection.FindAllGenericTypes(typeof(MetaComponent<>)))
		{
			Type value = Type.Build(item);
			types.Add(item, value);
		}
		foreach (System.Type item2 in Reflection.FindAllDerivedTypes<MetaObject>())
		{
			Type value2 = Type.Build(item2);
			types.Add(item2, value2);
		}
	}

	public static Type FindType(object obj)
	{
		return FindType(obj.GetType());
	}

	public static Type FindType(System.Type type)
	{
		Type value = null;
		if (!types.TryGetValue(type, out value))
		{
			UnityEngine.Debug.LogErrorFormat("MetaReflection: Unsupported type '{0}'", type);
		}
		return value;
	}

	public static Type TryFindType(object obj)
	{
		return FindType(obj.GetType());
	}

	public static Type TryFindType(System.Type type)
	{
		Type value = null;
		types.TryGetValue(type, out value);
		return value;
	}

	public static void InvokeMessage<T>(Message message, T target)
	{
		InvokeMessage(message, Enumerable.Repeat(target, 1));
	}

	public static void InvokeMessage<T>(Message message, List<T> targets)
	{
		InvokeMessage(message, targets.AsEnumerable());
	}

	public static void InvokeMessage<T>(Message message, T[] targets)
	{
		InvokeMessage(message, targets.AsEnumerable());
	}

	public static void InvokeMessage<T>(Message message, IEnumerable<T> targets)
	{
		ForeachType(targets, delegate(Type type, IEnumerable<T> objects)
		{
			MethodInfo message2 = type.GetMessage(message);
			if (message2 != null)
			{
				if (message2.IsStatic)
				{
					message2.Invoke(null, null);
				}
				else
				{
					foreach (T @object in objects)
					{
						message2.Invoke(@object, null);
					}
				}
			}
		});
	}

	public static void ResetData<T>(T target)
	{
		ResetData(Enumerable.Repeat(target, 1));
	}

	public static void ResetData<T>(List<T> targets)
	{
		ResetData(targets.AsEnumerable());
	}

	public static void ResetData<T>(T[] targets)
	{
		ResetData(targets.AsEnumerable());
	}

	public static void ResetData<T>(IEnumerable<T> targets)
	{
		ForeachType(targets, delegate(Type type, IEnumerable<T> objects)
		{
			foreach (T @object in objects)
			{
				DataField[] dataFields = type.DataFields;
				for (int i = 0; i < dataFields.Length; i++)
				{
					DataField dataField = dataFields[i];
					if (dataField.DefaultValue == null || dataField.DefaultValue.GetType().IsValueType)
					{
						dataField.Field.SetValue(@object, dataField.DefaultValue);
					}
					else if (dataField.DefaultValue.GetType().IsArray)
					{
						dataField.Field.SetValue(@object, (dataField.DefaultValue as Array).Clone());
					}
				}
			}
		});
	}

	public static void ForeachType<T>(IEnumerable<T> targets, Action<Type, IEnumerable<T>> action)
	{
		foreach (IGrouping<System.Type, T> item in from t in targets
			group t by t.GetType())
		{
			System.Type key = item.Key;
			Type type = FindType(key);
			if (type != null)
			{
				action(type, item);
			}
		}
	}
}
