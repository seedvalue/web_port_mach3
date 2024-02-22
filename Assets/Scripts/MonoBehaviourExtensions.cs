using System.Collections;
using UnityEngine;

public static class MonoBehaviourExtensions
{
	public static CoroutineWithResult StartCoroutineWithResult(this MonoBehaviour self, IEnumerator coroutine)
	{
		return new CoroutineWithResult(self, coroutine);
	}
}
