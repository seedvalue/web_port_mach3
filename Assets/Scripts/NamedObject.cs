using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class NamedObject : MonoBehaviour
{
	public string globalName;

	private static readonly Dictionary<string, WeakReference> namedObjects = new Dictionary<string, WeakReference>();

	public static GameObject Find(string globalName)
	{
		WeakReference value = null;
		if (!namedObjects.TryGetValue(globalName, out value))
		{
			return null;
		}
		GameObject gameObject = value.Target as GameObject;
		if (!gameObject)
		{
			namedObjects.Remove(globalName);
		}
		return gameObject;
	}

	private void Awake()
	{
		if (string.IsNullOrEmpty(globalName))
		{
			throw new ArgumentException("NamedObject has no globalName!");
		}
		if ((bool)Find(globalName))
		{
			throw new ArgumentException("NamedObject with globalName '" + globalName + "' already exists!");
		}
		namedObjects.Add(globalName, new WeakReference(base.gameObject));
	}

	private void OnDestroy()
	{
		namedObjects.Remove(globalName);
	}
}
