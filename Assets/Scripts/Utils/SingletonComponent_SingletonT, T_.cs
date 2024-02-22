using System;
using UnityEngine;

namespace Utils
{
	public abstract class SingletonComponent<SingletonT, T> : MonoBehaviour where SingletonT : Singleton<SingletonT> where T : MonoBehaviour
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
					UnityEngine.Debug.LogWarning("SingletonComponent (" + typeof(T) + "): Instance already destroyed on application quit. Won't create again - returning null.");
					return (T)null;
				}
				lock (_lock)
				{
					if ((UnityEngine.Object)_instance == (UnityEngine.Object)null)
					{
						SingletonT instance = Singleton<SingletonT>.Instance;
						if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
						{
							throw new Exception("SingletonComponent (" + typeof(T) + ") couldn't create Singleton (" + typeof(SingletonT) + ")!");
						}
						_instance = instance.GetComponent<T>();
						if ((UnityEngine.Object)_instance == (UnityEngine.Object)null)
						{
							throw new Exception("SingletonComponent (" + typeof(T) + ") not found in Singleton (" + typeof(SingletonT) + ")!");
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

		public virtual void OnApplicationQuit()
		{
			_applicationIsQuitting = true;
			_instance = (T)null;
		}

		public virtual void OnDestroy()
		{
			_applicationIsQuitting = true;
		}
	}
}
