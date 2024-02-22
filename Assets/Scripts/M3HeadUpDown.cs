using UnityEngine;

public class M3HeadUpDown : MonoBehaviour, IM3CameraShaker
{
	public float headResetTime = 1f;

	private float angleCurr;

	private float angleSrc;

	private float angleDest;

	private float headUpTime;

	private float headUpTimeCurrent;

	private M3ShakeManager shakeManager;

	private void Start()
	{
		if (!shakeManager)
		{
			shakeManager = UnityEngine.Object.FindObjectOfType<M3ShakeManager>();
		}
		if ((bool)shakeManager)
		{
			shakeManager.StartShake(this);
		}
	}

	public int GetPriority()
	{
		return M3ShakeManager.priorZero;
	}

	public bool IsIdle()
	{
		return false;
	}

	public void UpdateState(float deltaTime, ref Vector3 outPos, ref Quaternion outRot)
	{
		headUpTimeCurrent += deltaTime;
		if (headUpTimeCurrent >= headUpTime)
		{
			angleCurr = angleDest;
		}
		else
		{
			angleCurr = Mathf.SmoothStep(angleSrc, angleDest, headUpTimeCurrent / headUpTime);
		}
		outPos = Vector3.zero;
		outRot = Quaternion.AngleAxis(angleCurr, Vector3.right);
	}

	public void HeadUp(float angle, float duration)
	{
		angleCurr = angleDest;
		angleDest = angle;
		angleSrc = angleCurr;
		headUpTime = duration;
		headUpTimeCurrent = 0f;
	}
}
