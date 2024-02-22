using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Utils
{
	public class SingletonLoader
	{
		public static void InitAll()
		{
			UnityEngine.Debug.Log("Singleton: Before load");
			List<Type> list = Reflection.FindAllGenericTypes(typeof(Singleton<>));
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].GetCustomAttributes(typeof(SingletonInitializeOnLoadAttribute), inherit: false).Length > 0)
				{
					list[i].BaseType.GetMethod("GetInstance")?.Invoke(null, null);
				}
			}
			UnityEngine.Debug.Log("Singleton: After load");
		}

		public static IEnumerator InitAllSequence()
		{
			List<Type> singletonTypes = Reflection.FindAllGenericTypes(typeof(Singleton<>));
			for (int i = 0; i < singletonTypes.Count; i++)
			{
				if (singletonTypes[i].GetCustomAttributes(typeof(SingletonInitializeOnLoadAttribute), inherit: false).Length > 0)
				{
					MethodInfo method = singletonTypes[i].BaseType.GetMethod("GetInstance");
					if (method != null)
					{
						method.Invoke(null, null);
						yield return new WaitForEndOfFrame();
					}
				}
			}
		}
	}
}
