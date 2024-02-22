using System;
using System.Collections;
using UnityEngine;

public class JumpTween : MonoBehaviour
{
	[Header("Effect")]
	[Range(0f, 120f)]
	public float movement = 60f;

	[Range(0f, 2f)]
	public float scaling = 1f;

	[Header("Timing")]
	[Range(0.25f, 2f)]
	public float jumpTime = 0.75f;

	[Range(0f, 5f)]
	public float delayTime = 0.25f;

	[Range(0f, 5f)]
	public float delayRandom = 0.2f;

	private Vector3 basePos;

	private Vector2 baseScale;

	private static AnimationCurve PosYCurve;

	private static AnimationCurve ScaleXCurve;

	private static AnimationCurve ScaleYCurve;

	private static readonly Keyframe[] PosYKeys;

	private static readonly Keyframe[] ScaleXKeys;

	private static readonly Keyframe[] ScaleYKeys;

	static JumpTween()
	{
		PosYKeys = new Keyframe[6]
		{
			new Keyframe(0f, 0f, 0f, 0f),
			new Keyframe(0.111111112f, 0f, 0f, 0f),
			new Keyframe(4f / 9f, 1f, 0f, 0f),
			new Keyframe(32f / 45f, 0f, 0f, 0f),
			new Keyframe(13f / 15f, 355f / (678f * (float)Math.PI), 0f, 0f),
			new Keyframe(1f, 0f, 0f, 0f)
		};
		ScaleXKeys = new Keyframe[5]
		{
			new Keyframe(0f, 1f, 0f, 0f),
			new Keyframe(0.111111112f, 1.1f, 0f, 0f),
			new Keyframe(11f / 45f, 0.9f, 0f, 0f),
			new Keyframe(32f / 45f, 1f, 0f, 0f),
			new Keyframe(1f, 1f, 0f, 0f)
		};
		ScaleYKeys = new Keyframe[7]
		{
			new Keyframe(0f, 1f, 0f, 0f),
			new Keyframe(0.111111112f, 0.85f, 0f, 0f),
			new Keyframe(11f / 45f, 1.2f, 0f, 0f),
			new Keyframe(32f / 45f, 1f, 0f, 0f),
			new Keyframe(7f / 9f, 0.95f, 0f, 0f),
			new Keyframe(13f / 15f, 1f, 0f, 0f),
			new Keyframe(1f, 1f, 0f, 0f)
		};
		PosYCurve = new AnimationCurve(PosYKeys);
		ScaleXCurve = new AnimationCurve(ScaleXKeys);
		ScaleYCurve = new AnimationCurve(ScaleYKeys);
	}

	protected void OnEnable()
	{
		basePos = base.transform.localPosition;
		baseScale = base.transform.localScale;
		StartCoroutine(Tween());
	}

	protected void OnDisable()
	{
		StopAllCoroutines();
		base.transform.localPosition = basePos;
		base.transform.localScale = baseScale;
	}

	protected IEnumerator Tween()
	{
		yield return CoroutineHelper.AnimateInTime(jumpTime, delegate(float t)
		{
			float num = PosYCurve.Evaluate(t) * movement;
			float num2 = (ScaleXCurve.Evaluate(t) - 1f) * scaling + 1f;
			float num3 = (ScaleYCurve.Evaluate(t) - 1f) * scaling + 1f;
			Vector3 localPosition = basePos;
			localPosition.y += num;
			Vector2 v = baseScale;
			v.x *= num2;
			v.y *= num3;
			transform.localPosition = localPosition;
			transform.localScale = v;
		});
		yield return new WaitForSeconds(delayTime + Rand.uniform * delayRandom);
		StartCoroutine(Tween());
	}
}
