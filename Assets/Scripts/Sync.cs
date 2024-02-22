using System.Collections;
using UnityEngine;
using Utils;

public class Sync : MonoBehaviour, ISceneLoadedHandler, ISceneLoadingElement
{
	private bool sequenceDone;

	private void Start()
	{
		StartCoroutine(Sequence());
	}

	public bool IsLoading()
	{
		return !sequenceDone;
	}

	public void OnSceneLoaded()
	{
		Singleton<SceneLoader>.Instance.SwitchToMeta(SceneLoader.Priority.Default);
	}

	private IEnumerator Sequence()
	{
		Singleton<SceneLoader>.GetInstance();
		yield return new WaitForEndOfFrame();
		yield return SingletonLoader.InitAllSequence();
		sequenceDone = true;
	}
}
