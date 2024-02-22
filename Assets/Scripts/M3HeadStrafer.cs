using UnityEngine;

public class M3HeadStrafer : MonoBehaviour, IM3CameraShaker
{
	public float strafeMax = 0.5f;

	public float strafeTime = 2f;

	private M3ShakeManager shakeManager;

	private float strafe;

	private float strafeDest;

	private float strafePrev;

	private float strafeTimeCurrent;

	private float strafeTimeDest;

	private Vector3 lookAt;

	private bool active;

	private bool paused;

	public bool Paused
	{
		get
		{
			return paused;
		}
		set
		{
			paused = value;
		}
	}

	public int GetPriority()
	{
		return M3ShakeManager.priorZero;
	}

	public bool IsIdle()
	{
		return !active && strafeTimeCurrent > strafeTimeDest;
	}

	private void CalcNextStrafe(float dest, float timeDest)
	{
		strafePrev = strafe;
		strafeDest = UnityEngine.Random.Range(0.75f * dest, dest);
		strafeTimeCurrent = 0f;
		strafeTimeDest = timeDest;
	}

	public void Strafe(Vector3 lookAtPoint)
	{
		if (!shakeManager)
		{
			shakeManager = UnityEngine.Object.FindObjectOfType<M3ShakeManager>();
		}
		active = true;
		Vector3 position = base.transform.position;
		lookAtPoint.y = position.y;
		lookAt = Vector3.zero;
		lookAt.z = Vector3.Distance(lookAtPoint, base.transform.position);
		if (lookAt.z < 0.5f)
		{
			UnityEngine.Debug.Log("Suspicious lookAt in HeadStrafer!");
		}
		strafe = 0f;
		CalcNextStrafe(strafeDest, strafeTime / 2f);
		if ((bool)shakeManager)
		{
			shakeManager.StartShake(this);
		}
	}

	public void Stop()
	{
		active = false;
		CalcNextStrafe(0f, strafeTime * Mathf.Abs(strafe) / (strafeMax * 2f));
	}

	public void UpdateState(float deltaTime, ref Vector3 outPos, ref Quaternion outRot)
	{
		if (!paused)
		{
			strafeTimeCurrent += deltaTime;
			float t = (!(strafeTimeDest > float.Epsilon)) ? 1f : (strafeTimeCurrent / strafeTimeDest);
			strafe = Mathf.SmoothStep(strafePrev, strafeDest, t);
			if (strafeTimeCurrent > strafeTimeDest && active)
			{
				CalcNextStrafe(Mathf.Sign(strafe) * (0f - strafeMax), strafeTime);
			}
		}
		outPos = new Vector3(strafe, 0f, 0f);
		outRot = Quaternion.LookRotation(lookAt - outPos, Vector3.up);
	}
}
