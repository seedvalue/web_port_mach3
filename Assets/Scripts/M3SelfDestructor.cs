using System.Collections;
using UnityEngine;

public class M3SelfDestructor : MonoBehaviour
{
	public float lifeTime = 1f;

	public bool autoStart = true;

	private void Start()
	{
		if (autoStart)
		{
			StartCoroutine(Detonate(lifeTime));
		}
	}

	public void DetonateIn(float time)
	{
		StartCoroutine(Detonate(time));
	}

	public void Detonate()
	{
		StartCoroutine(Detonate(lifeTime));
	}

	private IEnumerator Detonate(float detonationTime)
	{
		yield return new WaitForSeconds(detonationTime);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
