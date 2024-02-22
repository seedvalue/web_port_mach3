using System;
using UnityEngine;

public class M3StageCamera : MonoBehaviour
{
	private const float stepFrequencyBase = (float)Math.PI * 2f;

	public float playerHeadYAmplitude = 0.05f;

	public float playerStepFrequency = 1f;

	public AnimationCurve step;

	private bool walking;

	private float walkTime;

	private float dampFactor = 1f;

	private M3ShakeManager shakeManager;

	private Vector3 basePos;

	private Quaternion baseRot;

	private void Start()
	{
		float num = Screen.height;
		Camera component = GetComponent<Camera>();
		float num2 = 0.5f;
		component.rect = new Rect(0f, num2, 1f, 1f - num2);
		step.postWrapMode = WrapMode.Loop;
		shakeManager = GetComponent<M3ShakeManager>();
		basePos = base.transform.localPosition;
		baseRot = base.transform.localRotation;
	}

	public void SetBaseLocalTransform(Vector3 locPos, Quaternion locRot)
	{
		basePos = locPos;
		baseRot = locRot;
	}

	public void SetDamping(float damping)
	{
		dampFactor = damping;
	}

	public void Walk()
	{
		walking = true;
		walkTime = 0f;
	}

	public void Stop()
	{
		walking = false;
	}

	private void LateUpdate()
	{
		Vector3 camPos = Vector3.zero;
		Quaternion camRot = Quaternion.identity;
		if ((bool)shakeManager)
		{
			shakeManager.UpdateState(ref camPos, ref camRot, Time.deltaTime);
		}
		Vector3 zero = Vector3.zero;
		if (walking)
		{
			walkTime += Time.deltaTime;
			float num = dampFactor * playerHeadYAmplitude;
			float num2 = zero.y = num * step.Evaluate(walkTime * playerStepFrequency);
		}
		base.transform.localPosition = basePos + camPos + zero;
		base.transform.localRotation = baseRot * camRot;
	}
}
