using System.Collections;
using UnityEngine;
using Utils;

public class SlideTween : MonoBehaviour
{
	[Header("Effect")]
	public Vector3 movement = new Vector3(0f, 10f, 0f);

	[Header("Timing")]
	[Range(0.1f, 10f)]
	public float slideTime = 1f;

	[Range(0f, 5f)]
	public float delayTime = 0.25f;

	[Range(0f, 5f)]
	public float delayRandom = 0.2f;

	private Vector3 basePos;

	protected void OnEnable()
	{
		basePos = base.transform.localPosition;
		StartCoroutine(Tween());
	}

	protected void OnDisable()
	{
		StopAllCoroutines();
		base.transform.localPosition = basePos;
	}

	protected IEnumerator Tween()
	{
		yield return CoroutineHelper.AnimateInTime(slideTime, delegate(float t)
		{
			t = Helpers.simple_smooth_step(t);
			transform.localPosition = Vector3.Lerp(basePos, basePos + movement, t);
		});
		yield return new WaitForSeconds(delayTime + Rand.uniform * delayRandom);
		StartCoroutine(Tween());
	}
}
