using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Unity.IO.Compression;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

[SingletonPrefab]
[SingletonInitializeOnLoad]
public class Meta : Singleton<Meta>, ISceneLoadingElement
{
	private class ObjectListener : IComparable<ObjectListener>
	{
		private MetaObject metaObject;

		private string metaProperty;

		public ObjectListener(MetaObject metaObject)
		{
			this.metaObject = metaObject;
			metaProperty = null;
		}

		public ObjectListener(MetaObject metaObject, string metaProperty)
		{
			this.metaObject = metaObject;
			this.metaProperty = metaProperty;
		}

		public int CompareTo(ObjectListener rhs)
		{
			int num = Comparer<int>.Default.Compare(metaObject.GetInstanceID(), rhs.metaObject.GetInstanceID());
			if (num == 0)
			{
				num = Comparer<string>.Default.Compare(metaProperty, rhs.metaProperty);
			}
			return num;
		}
	}

	private class TypeListener : IComparable<TypeListener>
	{
		private Type metaType;

		private string metaProperty;

		public TypeListener(Type metaType)
		{
			this.metaType = metaType;
			metaProperty = null;
		}

		public TypeListener(Type metaType, string metaProperty)
		{
			this.metaType = metaType;
			this.metaProperty = metaProperty;
		}

		public int CompareTo(TypeListener rhs)
		{
			int num = Comparer<int>.Default.Compare(metaType.GetHashCode(), rhs.metaType.GetHashCode());
			if (num == 0)
			{
				num = Comparer<string>.Default.Compare(metaProperty, rhs.metaProperty);
			}
			return num;
		}
	}

	public string workbook;

	public string assetsPath;

	public string resourcesPath;

	private MetaObject[] metaObjects;

	private IMetaComponent[] metaComponents;

	private bool loaded;

	private bool saveNeeded;

	private bool saveEnabled = true;

	private readonly SortedDictionary<ObjectListener, List<MetaAction>> objectListeners = new SortedDictionary<ObjectListener, List<MetaAction>>();

	private readonly SortedDictionary<TypeListener, List<MetaAction>> typeListeners = new SortedDictionary<TypeListener, List<MetaAction>>();

	private const string SaveElementMeta = "meta";

	private const string SaveElementComponent = "component";

	private const string SaveElementObject = "object";

	private const string SaveElementField = "field";

	private const string SaveElementValue = "value";

	private const string SaveElementNull = "null";

	private const string SaveAttributeID = "id";

	private const int SaveDepthElement = 1;

	private const int SaveDepthField = 2;

	private const int SaveDepthValue = 3;

	private const string EditorPrefsDisableSave = "Meta.DisableSave";

	private const string SaveFile = "/meta.save";

	private static string saveFilePath => Application.persistentDataPath + "/meta.save";

	public static bool editorDisableSave
	{
		get
		{
			return EditorPrefsHelper.GetBool("Meta.DisableSave");
		}
		set
		{
			EditorPrefsHelper.SetBool("Meta.DisableSave", value);
		}
	}

	protected virtual void Awake()
	{
		if (editorDisableSave)
		{
			saveEnabled = false;
		}
		StartCoroutine(LoadCoroutine());
	}

	protected virtual void Update()
	{
		if (loaded)
		{
			MetaReflection.InvokeMessage(MetaReflection.Message.Update, metaComponents);
			MetaReflection.InvokeMessage(MetaReflection.Message.Update, metaObjects);
			if (saveEnabled && saveNeeded)
			{
				saveNeeded = !Save();
			}
		}
	}

	public MetaObject FindObject(string metaID)
	{
		return metaObjects.FirstOrDefault((MetaObject o) => o.metaID == metaID);
	}

	public T[] FindObjects<T>() where T : MetaObject
	{
		return metaObjects.OfType<T>().ToArray();
	}

	public T FindRandomObject<T>() where T : MetaObject
	{
		T[] array = FindObjects<T>();
		if (array.Length == 0)
		{
			return (T)null;
		}
		return array[UnityEngine.Random.Range(0, array.Length)];
	}

	public IMetaComponent FindComponent(string metaID)
	{
		return metaComponents.FirstOrDefault((IMetaComponent c) => c.metaID == metaID);
	}

	public void OnPropertyChanged(MetaObject metaObject, string propertyName)
	{
		InvokeListeners(metaObject, propertyName);
		saveNeeded = true;
	}

	public void Invoke(MetaLink link, MetaActionType actionType)
	{
		link.Invoke(actionType);
	}

	public void Invoke(MetaLink[] links, MetaActionType actionType)
	{
		for (int i = 0; i < links.Length; i++)
		{
			Invoke(links[i], actionType);
		}
	}

	public void Invoke(List<MetaLink> links, MetaActionType actionType)
	{
		for (int i = 0; i < links.Count; i++)
		{
			Invoke(links[i], actionType);
		}
	}

	public bool IsLoading()
	{
		return !loaded;
	}

	protected IEnumerator LoadCoroutine()
	{
		metaComponents = this.GetComponentsAs<IMetaComponent>();
		yield return null;
		UnityEngine.Debug.Log("Meta: Before load");
		metaObjects = (from MetaObject o in Resources.LoadAll(resourcesPath, typeof(MetaObject))
			orderby o.name
			orderby o.GetType().GetHashCode()
			select o).ToArray();
		UnityEngine.Debug.Log("Meta: After load");
		yield return null;
		MetaReflection.ResetData(metaComponents);
		MetaReflection.ResetData(metaObjects);
		yield return null;
		MetaReflection.InvokeMessage(MetaReflection.Message.StaticAwake, metaComponents);
		MetaReflection.InvokeMessage(MetaReflection.Message.StaticAwake, metaObjects);
		yield return null;
		MetaReflection.InvokeMessage(MetaReflection.Message.Awake, metaComponents);
		MetaReflection.InvokeMessage(MetaReflection.Message.Awake, metaObjects);
		yield return null;
		if (saveEnabled)
		{
			Load();
		}
		yield return null;
		MetaReflection.InvokeMessage(MetaReflection.Message.StaticStart, metaComponents);
		MetaReflection.InvokeMessage(MetaReflection.Message.StaticStart, metaObjects);
		yield return null;
		MetaReflection.InvokeMessage(MetaReflection.Message.Start, metaComponents);
		MetaReflection.InvokeMessage(MetaReflection.Message.Start, metaObjects);
		loaded = true;
	}

	public void Reset()
	{
		SceneManager.activeSceneChanged += OnReset;
		Singleton<SceneLoader>.Instance.SwitchToSync(SceneLoader.Priority.Default);
	}

	private void OnReset(Scene from, Scene to)
	{
		SceneManager.activeSceneChanged -= OnReset;
		loaded = false;
		if (saveEnabled)
		{
			RemoveSave();
		}
		MetaReflection.InvokeMessage(MetaReflection.Message.Reset, metaComponents);
		MetaReflection.InvokeMessage(MetaReflection.Message.Reset, metaObjects);
		MetaReflection.ResetData(metaComponents);
		MetaReflection.ResetData(metaObjects);
		objectListeners.Clear();
		typeListeners.Clear();
		MetaObject[] array = metaObjects;
		foreach (MetaObject assetToUnload in array)
		{
			Resources.UnloadAsset(assetToUnload);
		}
		metaObjects = null;
		metaComponents = null;
		saveNeeded = false;
		Application.Quit();
	}

	public void RequestSave()
	{
		saveNeeded = true;
	}

	public void ForceSave()
	{
		if (saveEnabled)
		{
			saveNeeded = !Save();
		}
	}

	public void AddObjectListener(MetaObject metaObject, MetaAction action)
	{
		AddListener(new ObjectListener(metaObject), action);
	}

	public void RemObjectListener(MetaObject metaObject, MetaAction action)
	{
		RemListener(new ObjectListener(metaObject), action);
	}

	public void AddObjectPropertyListener(MetaObject metaObject, string metaProperty, MetaAction action)
	{
		AddListener(new ObjectListener(metaObject, metaProperty), action);
	}

	public void RemObjectPropertyListener(MetaObject metaObject, string metaProperty, MetaAction action)
	{
		RemListener(new ObjectListener(metaObject, metaProperty), action);
	}

	public void AddTypeListener(Type metaType, MetaAction action)
	{
		AddListener(new TypeListener(metaType), action);
	}

	public void RemTypeListener(Type metaType, MetaAction action)
	{
		RemListener(new TypeListener(metaType), action);
	}

	public void AddTypePropertyListener(Type metaType, string metaProperty, MetaAction action)
	{
		AddListener(new TypeListener(metaType, metaProperty), action);
	}

	public void RemTypePropertyListener(Type metaType, string metaProperty, MetaAction action)
	{
		RemListener(new TypeListener(metaType, metaProperty), action);
	}

	private void AddListener(ObjectListener listener, MetaAction action)
	{
		if (!objectListeners.TryGetValue(listener, out List<MetaAction> value))
		{
			value = new List<MetaAction>();
			objectListeners.Add(listener, value);
		}
		value.Add(action);
	}

	private void RemListener(ObjectListener listener, MetaAction action)
	{
		if (objectListeners.TryGetValue(listener, out List<MetaAction> value))
		{
			value.Remove(action);
		}
	}

	private void AddListener(TypeListener listener, MetaAction action)
	{
		if (!typeListeners.TryGetValue(listener, out List<MetaAction> value))
		{
			value = new List<MetaAction>();
			typeListeners.Add(listener, value);
		}
		value.Add(action);
	}

	private void RemListener(TypeListener listener, MetaAction action)
	{
		if (typeListeners.TryGetValue(listener, out List<MetaAction> value))
		{
			value.Remove(action);
		}
	}

	private void InvokeListeners(MetaObject metaObject, string propertyName)
	{
		if (objectListeners.TryGetValue(new ObjectListener(metaObject, propertyName), out List<MetaAction> value))
		{
			foreach (MetaAction item in value)
			{
				item(metaObject, propertyName);
			}
		}
		if (objectListeners.TryGetValue(new ObjectListener(metaObject), out value))
		{
			foreach (MetaAction item2 in value)
			{
				item2(metaObject, propertyName);
			}
		}
		if (typeListeners.TryGetValue(new TypeListener(metaObject.GetType(), propertyName), out value))
		{
			foreach (MetaAction item3 in value)
			{
				item3(metaObject, propertyName);
			}
		}
		if (typeListeners.TryGetValue(new TypeListener(metaObject.GetType()), out value))
		{
			foreach (MetaAction item4 in value)
			{
				item4(metaObject, propertyName);
			}
		}
	}

	public static bool RemoveSave()
	{
		try
		{
			File.Delete(saveFilePath);
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	protected bool Save()
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.OmitXmlDeclaration = true;
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.IndentChars = "  ";
			XmlWriterSettings settings = xmlWriterSettings;
			using (XmlWriter xml = XmlWriter.Create(stringBuilder, settings))
			{
				Save(xml);
			}
			using (FileStream fileStream = File.Open(saveFilePath, FileMode.Create, FileAccess.Write))
			{
				using (GZipStream gZipStream = new GZipStream(fileStream, CompressionMode.Compress))
				{
					using (StreamWriter streamWriter = new StreamWriter(gZipStream))
					{
						streamWriter.Write(stringBuilder.ToString());
						streamWriter.Close();
					}
					gZipStream.Close();
				}
				fileStream.Close();
			}
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	protected bool Load()
	{
		try
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.IgnoreProcessingInstructions = true;
			xmlReaderSettings.IgnoreWhitespace = true;
			XmlReaderSettings settings = xmlReaderSettings;
			using (FileStream fileStream = File.Open(saveFilePath, FileMode.Open, FileAccess.Read))
			{
				using (GZipStream gZipStream = new GZipStream(fileStream, CompressionMode.Decompress))
				{
					using (XmlReader xml = XmlReader.Create(gZipStream, settings))
					{
						Load(xml);
					}
					gZipStream.Close();
				}
				fileStream.Close();
			}
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	protected void Save(XmlWriter xml)
	{
		xml.WriteStartDocument();
		xml.WriteStartElement("meta");
		MetaReflection.ForeachType(metaComponents, delegate(MetaReflection.Type type, IEnumerable<IMetaComponent> objects)
		{
			MetaReflection.DataField[] dataFields2 = type.DataFields;
			foreach (IMetaComponent item in objects.Cast<IMetaComponent>())
			{
				if (item != null)
				{
					xml.WriteStartElement("component");
					xml.WriteAttributeString("id", item.metaID);
					for (int j = 0; j < dataFields2.Length; j++)
					{
						FieldInfo field2 = dataFields2[j].Field;
						object value2 = field2.GetValue(item);
						if (value2 == null)
						{
							xml.WriteStartElement("null");
							xml.WriteAttributeString("id", field2.Name);
							xml.WriteEndElement();
						}
						else
						{
							xml.WriteStartElement("field");
							xml.WriteAttributeString("id", field2.Name);
							Save(xml, value2, field2.FieldType);
							xml.WriteEndElement();
						}
					}
					xml.WriteEndElement();
				}
			}
		});
		MetaReflection.ForeachType(metaObjects, delegate(MetaReflection.Type type, IEnumerable<MetaObject> objects)
		{
			MetaReflection.DataField[] dataFields = type.DataFields;
			foreach (MetaObject item2 in objects.Cast<MetaObject>())
			{
				if (!(item2 == null))
				{
					xml.WriteStartElement("object");
					xml.WriteAttributeString("id", item2.metaID);
					for (int i = 0; i < dataFields.Length; i++)
					{
						FieldInfo field = dataFields[i].Field;
						object value = field.GetValue(item2);
						if (value == null)
						{
							xml.WriteStartElement("null");
							xml.WriteAttributeString("id", field.Name);
							xml.WriteEndElement();
						}
						else
						{
							xml.WriteStartElement("field");
							xml.WriteAttributeString("id", field.Name);
							Save(xml, value, field.FieldType);
							xml.WriteEndElement();
						}
					}
					xml.WriteEndElement();
				}
			}
		});
		xml.WriteEndElement();
		xml.WriteEndDocument();
	}

	protected void Load(XmlReader xml)
	{
		MetaReflection.DataField[] array = null;
		object obj = null;
		string id;
		while (xml.Read())
		{
			switch (xml.NodeType)
			{
			case XmlNodeType.Element:
				if (xml.Depth == 2)
				{
					if (array == null)
					{
						break;
					}
					id = xml.GetAttribute("id");
					if (!string.IsNullOrEmpty(id))
					{
						FieldInfo fieldInfo = (from f in array
							select f.Field).FirstOrDefault((FieldInfo f) => f.Name == id);
						if (fieldInfo != null)
						{
							try
							{
								object value = Load(xml, fieldInfo.FieldType);
								fieldInfo.SetValue(obj, value);
							}
							catch (Exception)
							{
							}
						}
					}
				}
				else
				{
					if (xml.Depth != 1)
					{
						break;
					}
					array = null;
					obj = null;
					if (xml.Name == "object")
					{
						string attribute = xml.GetAttribute("id");
						if (!string.IsNullOrEmpty(attribute))
						{
							MetaObject metaObject = FindObject(attribute);
							if (!(metaObject == null))
							{
								array = MetaReflection.FindType(metaObject).DataFields;
								obj = metaObject;
							}
						}
					}
					else
					{
						if (!(xml.Name == "component"))
						{
							break;
						}
						string attribute2 = xml.GetAttribute("id");
						if (!string.IsNullOrEmpty(attribute2))
						{
							IMetaComponent metaComponent = FindComponent(attribute2);
							if (metaComponent != null)
							{
								array = MetaReflection.FindType(metaComponent).DataFields;
								obj = metaComponent;
							}
						}
					}
				}
				break;
			case XmlNodeType.EndElement:
				if (xml.Depth == 1)
				{
					array = null;
					obj = null;
				}
				break;
			}
		}
	}

	protected void Save(XmlWriter xml, object value, Type type)
	{
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
		{
			IList list = value as IList;
			Type type2 = type.GetGenericArguments()[0];
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] == null)
				{
					xml.WriteStartElement("null");
					xml.WriteEndElement();
				}
				else
				{
					Save(xml, list[i], type2);
				}
			}
		}
		else if (type.IsArray)
		{
			IList list2 = value as IList;
			Type elementType = type.GetElementType();
			for (int j = 0; j < list2.Count; j++)
			{
				if (list2[j] == null)
				{
					xml.WriteStartElement("null");
					xml.WriteEndElement();
				}
				else
				{
					Save(xml, list2[j], elementType);
				}
			}
		}
		else
		{
			xml.WriteElementString("value", SaveString(value, type));
		}
	}

	protected object Load(XmlReader xml, Type type)
	{
		if (xml.Name == "null")
		{
			return null;
		}
		List<string> list = new List<string>();
		if (!xml.IsEmptyElement)
		{
			xml.Read();
			while (xml.Depth == 3)
			{
				if (xml.Name == "null")
				{
					list.Add(null);
					xml.Read();
					continue;
				}
				if (xml.Name == "value")
				{
					list.Add(xml.ReadElementContentAsString());
					continue;
				}
				throw new Exception("Unknown value element");
			}
		}
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
		{
			IList list2 = (IList)Activator.CreateInstance(type, list.Count);
			Type type2 = type.GetGenericArguments()[0];
			for (int i = 0; i < list.Count; i++)
			{
				list2.Add(LoadString(list[i], type2));
			}
			return list2;
		}
		if (type.IsArray)
		{
			IList list3 = (IList)Activator.CreateInstance(type, list.Count);
			Type elementType = type.GetElementType();
			for (int j = 0; j < list.Count; j++)
			{
				list3[j] = LoadString(list[j], elementType);
			}
			return list3;
		}
		if (list.Count == 0)
		{
			throw new Exception("No value");
		}
		return LoadString(list[0], type);
	}

	protected string SaveString(object value, Type type)
	{
		if (type == typeof(MetaObject) || type.IsSubclassOf(typeof(MetaObject)))
		{
			MetaObject metaObject = value as MetaObject;
			return metaObject.metaID;
		}
		return value.ToString();
	}

	protected object LoadString(string str, Type type)
	{
		if (type == typeof(MetaObject) || type.IsSubclassOf(typeof(MetaObject)))
		{
			MetaObject metaObject = FindObject(str);
			if (metaObject == null || !type.IsAssignableFrom(metaObject.GetType()))
			{
				return null;
			}
			return metaObject;
		}
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
		{
			type = Nullable.GetUnderlyingType(type);
		}
		if (type.IsEnum)
		{
			return Enum.Parse(type, str);
		}
		if (type == typeof(DateTime))
		{
			return DateTime.Parse(str);
		}
		if (type == typeof(TimeSpan))
		{
			return TimeSpan.Parse(str);
		}
		return Convert.ChangeType(str, type);
	}
}
