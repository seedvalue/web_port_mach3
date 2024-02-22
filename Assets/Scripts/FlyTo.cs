using System.Collections;
using UnityEngine;

public class FlyTo : MonoBehaviour
{
	private bool useWorldPos;

	public bool isFlaying
	{
		get;
		private set;
	}

	public void flyTo(Vector3 targetPos, float value = 0.5f, bool time = true, bool local = true)
	{
		StopAllCoroutines();
		useWorldPos = !local;
		Vector3 source = (!useWorldPos) ? base.transform.localPosition : base.transform.position;
		if (time)
		{
			StartCoroutine(moveObject(base.gameObject, source, targetPos, value));
		}
		else
		{
			StartCoroutine(moveObjectBySpeed(base.gameObject, source, targetPos, value));
		}
	}

	public IEnumerator moveObjectBySpeed(GameObject obj, Vector3 source, Vector3 target, float speed)
	{
		float magnitude = (target - source).magnitude;
		float duration = magnitude / speed;
		return moveObject(obj, source, target, duration);
	}

	public IEnumerator moveObject(GameObject obj, Vector3 source, Vector3 target, float duration)
	{
		isFlaying = true;
		float startTime = Time.time;
		while (Time.time < startTime + duration)
		{
			if (useWorldPos)
			{
				obj.transform.position = Vector3.Lerp(source, target, (Time.time - startTime) / duration);
			}
			else
			{
				obj.transform.localPosition = Vector3.Lerp(source, target, (Time.time - startTime) / duration);
			}
			yield return null;
		}
		if (useWorldPos)
		{
			obj.transform.position = target;
		}
		else
		{
			obj.transform.localPosition = target;
		}
		isFlaying = false;
	}
}
