using System;
using UnityEngine;

namespace Utils
{
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static bool _applicationIsQuitting = false;

		private static object _lock = new object();

		private static T _instance = (T)null;

		public static bool HasInstance => (UnityEngine.Object)_instance != (UnityEngine.Object)null;

		public static T Instance
		{
			get
			{
				if (_applicationIsQuitting)
				{
					UnityEngine.Debug.LogWarning("Singleton (" + typeof(T) + "): Instance already destroyed on application quit. Won't create again - returning null.");
					return (T)null;
				}
				lock (_lock)
				{
					if ((UnityEngine.Object)_instance == (UnityEngine.Object)null)
					{
						Type typeFromHandle = typeof(T);
						_instance = (UnityEngine.Object.FindObjectOfType(typeFromHandle) as T);
						if ((UnityEngine.Object)_instance == (UnityEngine.Object)null)
						{
							string name = "(Singleton)" + typeFromHandle;
							GameObject gameObject = null;
							if (gameObject == null)
							{
								string singletonPrefabName = GetSingletonPrefabName();
								if (singletonPrefabName != null)
								{
									try
									{
										UnityEngine.Object original = Resources.Load(singletonPrefabName, typeof(GameObject));
										gameObject = (UnityEngine.Object.Instantiate(original) as GameObject);
										gameObject.name = name;
									}
									catch (Exception ex)
									{
										UnityEngine.Debug.LogError("Singleton (" + typeof(T) + "): Could not instantiate prefab even though prefab attribute was set: " + ex.Message + "\n" + ex.StackTrace);
									}
								}
							}
							if (gameObject == null)
							{
								gameObject = new GameObject();
								gameObject.name = name;
							}
							_instance = gameObject.GetComponent<T>();
							if ((UnityEngine.Object)_instance == (UnityEngine.Object)null)
							{
								_instance = gameObject.AddComponent<T>();
							}
							UnityEngine.Object.DontDestroyOnLoad(gameObject);
						}
						else
						{
							int num = UnityEngine.Object.FindObjectsOfType(typeFromHandle).Length;
							if (num > 1)
							{
								UnityEngine.Debug.LogError("Singleton (" + typeof(T) + "): There are " + num + " objects");
								throw new Exception("Too many (" + num + ") singletons of type: " + typeof(T));
							}
						}
					}
					return _instance;
				}
			}
		}

		public static T GetInstance()
		{
			return Instance;
		}

		public static string GetSingletonPrefabName()
		{
			Type typeFromHandle = typeof(T);
			if (Attribute.IsDefined(typeFromHandle, typeof(SingletonPrefabAttribute)))
			{
				SingletonPrefabAttribute singletonPrefabAttribute = Attribute.GetCustomAttribute(typeFromHandle, typeof(SingletonPrefabAttribute)) as SingletonPrefabAttribute;
				if (!string.IsNullOrEmpty(singletonPrefabAttribute.Name))
				{
					return singletonPrefabAttribute.Name;
				}
				return typeFromHandle.ToString();
			}
			return null;
		}

		public static string GetSingletonPrefabPath()
		{
			string singletonPrefabName = GetSingletonPrefabName();
			if (singletonPrefabName == null)
			{
				return null;
			}
			return "Assets/Resources/" + singletonPrefabName + ".prefab";
		}

		protected virtual void OnApplicationQuit()
		{
			_applicationIsQuitting = true;
			_instance = (T)null;
		}

		protected virtual void OnDestroy()
		{
			_applicationIsQuitting = true;
		}
	}
}
