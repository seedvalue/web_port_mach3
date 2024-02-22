using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

[SingletonPrefab]
[SingletonInitializeOnLoad]
public class SceneLoader : Singleton<SceneLoader>
{
	public enum Priority
	{
		Default,
		Tutorial,
		Fight,
		Forced
	}

	public string syncScene;

	public string metaScene;

	public string match3Scene;

	public string metaCreateHeroScene;

	private Canvas canvas;

	private Priority? targetPriority;

	private string targetScene;

	private object targetContext;

	private const string PlayerPrefsForcedSceneAfterLoad = "SceneLoader.UserPrefsForcedSceneAfterLoad";

	public bool loading
	{
		get;
		private set;
	}

	public object context
	{
		get;
		private set;
	}

	public bool inSync => SceneManager.GetActiveScene().name == syncScene;

	public bool inMeta => SceneManager.GetActiveScene().name == metaScene;

	public bool inMatch3 => SceneManager.GetActiveScene().name == match3Scene;

	public bool switchPending => !string.IsNullOrEmpty(targetScene);

	public static event Action BeginLoading;

	public static event Action EndLoading;

	public static void SetForcedSceneAfterLoad(string value)
	{
		PlayerPrefs.SetString("SceneLoader.UserPrefsForcedSceneAfterLoad", value);
	}

	public static string GetForcedSceneAfterLoad()
	{
		return PlayerPrefs.GetString("SceneLoader.UserPrefsForcedSceneAfterLoad", null);
	}

	public void SwitchToSync(Priority priority)
	{
		SwitchTo(syncScene, priority);
	}

	public void SwitchToMeta(Priority priority, Match3ResultContext context = null)
	{
		SwitchTo(metaScene, priority, context);
	}

	public void SwitchToMetaCreateHero(Priority priority)
	{
		SwitchTo(metaCreateHeroScene, priority);
	}

	public void SwitchToMatch3(Priority priority, Match3StartContext context)
	{
		SwitchTo(match3Scene, priority, context);
	}

	public void SwitchTo(string sceneName, Priority priority, object context = null)
	{
		if (targetPriority.HasValue)
		{
			if (targetPriority.Value == priority)
			{
				UnityEngine.Debug.LogWarning("SceneLoader: SwitchTo invoked with same priorities!");
			}
			else if (targetPriority.Value > priority)
			{
				return;
			}
		}
		targetPriority = priority;
		targetScene = sceneName;
		targetContext = context;
		if (!loading)
		{
			StartCoroutine(HandleSwitchScene());
		}
	}

	protected void ShowLoading()
	{
		canvas.enabled = true;
	}

	protected void HideLoading()
	{
		canvas.enabled = false;
	}

	protected virtual void Awake()
	{
		canvas = base.gameObject.GetComponent<Canvas>();
		StartCoroutine(HandleSwitchScene(initialLoad: true));
		string forcedSceneAfterLoad = GetForcedSceneAfterLoad();
		if (!string.IsNullOrEmpty(forcedSceneAfterLoad))
		{
			SetForcedSceneAfterLoad(null);
			SwitchTo(forcedSceneAfterLoad, Priority.Forced);
		}
	}

	private IEnumerator HandleSwitchScene(bool initialLoad = false)
	{
		SceneLoader.BeginLoading?.Invoke();
		loading = true;
		ShowLoading();
		yield return new WaitForEndOfFrame();
		while (switchPending || initialLoad)
		{
			if (!initialLoad)
			{
				NotifySceneUnloadingHandlers();
				yield return new WaitForEndOfFrame();
				while (AreSceneElementsUnloading())
				{
					yield return new WaitForEndOfFrame();
				}
				yield return SceneManager.LoadSceneAsync(targetScene);
				context = targetContext;
				targetPriority = null;
				targetContext = null;
				targetScene = null;
				yield return new WaitForEndOfFrame();
			}
			initialLoad = false;
			while (AreSceneElementsLoading())
			{
				yield return new WaitForEndOfFrame();
			}
			NotifySceneLoadedHandlers();
			yield return new WaitForEndOfFrame();
		}
		HideLoading();
		loading = false;
		SceneLoader.EndLoading?.Invoke();
	}

	private bool AreSceneElementsLoading()
	{
		bool any = false;
		ForEachObject(delegate(ISceneLoadingElement o)
		{
			any |= o.IsLoading();
		});
		return any;
	}

	private bool AreSceneElementsUnloading()
	{
		bool any = false;
		ForEachObject(delegate(ISceneUnloadingElement o)
		{
			any |= o.IsUnloading();
		});
		return any;
	}

	private void NotifySceneLoadedHandlers()
	{
		ForEachObject(delegate(ISceneLoadedHandler o)
		{
			o.OnSceneLoaded();
		});
	}

	private void NotifySceneUnloadingHandlers()
	{
		ForEachObject(delegate(ISceneUnloadingHandler o)
		{
			o.OnSceneUnloading();
		});
	}

	private bool ForEachObject<T>(Action<T> lambda) where T : class
	{
		Component[] array = UnityEngine.Object.FindObjectsOfType<Component>();
		for (int i = 0; i < array.Length; i++)
		{
			T val = array[i] as T;
			if (val != null)
			{
				lambda(val);
			}
		}
		return array.Length > 0;
	}
}
