using System;
using UnityEngine;
using Utils;

namespace Tutorial
{
	[SingletonPrefab]
	[SingletonInitializeOnLoad]
	public class TutorialManager : Singleton<TutorialManager>
	{
		public Tutorial CreateTutorial(Tutorial prefab, Action completeCallback = null)
		{
			Tutorial tutorial = UnityEngine.Object.Instantiate(prefab, base.transform);
			if (completeCallback != null)
			{
				Tutorial tutorial2 = tutorial;
				tutorial2.completeCallback = (Action)Delegate.Combine(tutorial2.completeCallback, completeCallback);
			}
			return tutorial;
		}
	}
}
