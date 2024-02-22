using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExternalSceneLoader : MonoBehaviour, ISceneLoadingElement
{
	public GameObject root;

	public List<string> scenesToLoad;

	private bool loading;

	public bool IsLoading()
	{
		return loading;
	}

	protected virtual void Awake()
	{
		if (root == null)
		{
			root = base.gameObject;
		}
	}

	protected virtual void Start()
	{
		StartCoroutine(DoLoadScenes());
	}

	protected virtual void OnElementLoaded(ExternalSceneElement element)
	{
	}

	protected virtual void OnSceneLoaded(int sceneIndex)
	{
	}

	protected virtual void OnLoaded()
	{
	}

	private void LoadFromScene(Scene scene)
	{
		GameObject[] rootGameObjects = scene.GetRootGameObjects();
		foreach (GameObject gameObject in rootGameObjects)
		{
			ExternalSceneElement component = gameObject.GetComponent<ExternalSceneElement>();
			if (component == null)
			{
				UnityEngine.Object.DestroyImmediate(gameObject);
				continue;
			}
			SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
			if (!component.loadAsRoot)
			{
				gameObject.transform.SetParent(root.transform, component.worldPositionStays);
			}
			component.OnLoaded();
			OnElementLoaded(component);
		}
	}

	private IEnumerator DoLoadScenes()
	{
		loading = true;
		for (int i = 0; i < scenesToLoad.Count; i++)
		{
			yield return DoLoadScene(scenesToLoad[i]);
			OnSceneLoaded(i);
		}
		OnLoaded();
		loading = false;
	}

	private IEnumerator DoLoadScene(string sceneName)
	{
		yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		Scene scene = SceneManager.GetSceneByName(sceneName);
		if (!scene.IsValid())
		{
			UnityEngine.Debug.LogWarningFormat("Couldn't open scene '{0}' because it doesn't exist in build.", sceneName);
			yield break;
		}
		LoadFromScene(scene);
		yield return DoUnloadScene(scene.buildIndex);
	}

	private IEnumerator DoUnloadScene(int buildIndex)
	{
		yield return new WaitForSeconds(0.1f);
		yield return SceneManager.UnloadSceneAsync(buildIndex);
	}
}
