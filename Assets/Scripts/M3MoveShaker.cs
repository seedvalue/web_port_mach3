using UnityEngine;

public class M3MoveShaker : MonoBehaviour, IM3CameraShaker
{
	public AnimationCurve amplitudeX;

	public AnimationCurve amplitudeY;

	public AnimationCurve amplitudeZ;

	public AnimationCurve yaw;

	public AnimationCurve pitch;

	public AnimationCurve roll;

	public AnimationCurve posX;

	public AnimationCurve posY;

	public AnimationCurve posZ;

	public float shakePowerMultiplier = 1f;

	public float perlinNoiseSampleFrequency = 10f;

	public float movementTime = 1f;

	public float shakeDelay;

	private float movementSpeed = 1f;

	private bool moving;

	private float triggerTime;

	private Vector3 perlinNoiseBase;

	private M3ShakeManager shakeManager;

	private M3ShakeManager ShakeManager
	{
		get
		{
			if (!shakeManager)
			{
				shakeManager = UnityEngine.Object.FindObjectOfType<M3ShakeManager>();
			}
			return shakeManager;
		}
	}

	public void UpdateState(float deltaTime, ref Vector3 outPos, ref Quaternion outRot)
	{
		if (moving)
		{
			triggerTime += deltaTime;
			float num = (!(movementTime > 0f)) ? (movementSpeed * triggerTime) : (movementSpeed * triggerTime / movementTime);
			Vector3 b = new Vector3(2f * shakePowerMultiplier * amplitudeX.Evaluate(num) * (Mathf.PerlinNoise(perlinNoiseBase.x + num * perlinNoiseSampleFrequency, perlinNoiseBase.x) - 0.5f), 2f * shakePowerMultiplier * amplitudeY.Evaluate(num) * (Mathf.PerlinNoise(perlinNoiseBase.y + num * perlinNoiseSampleFrequency, perlinNoiseBase.y) - 0.5f), 2f * shakePowerMultiplier * amplitudeZ.Evaluate(num) * (Mathf.PerlinNoise(perlinNoiseBase.z + num * perlinNoiseSampleFrequency, perlinNoiseBase.z) - 0.5f));
			outPos = new Vector3(posX.Evaluate(num), posY.Evaluate(num), posZ.Evaluate(num)) + b;
			outRot = Quaternion.Euler(new Vector3(pitch.Evaluate(num), yaw.Evaluate(num), roll.Evaluate(num)));
			if (num >= 1f)
			{
				moving = false;
			}
		}
	}

	private void TriggerShakeDelayed()
	{
		ShakeManager.StartShake(this);
	}

	public void TriggerShake(float speed = 1f)
	{
		if ((bool)ShakeManager && IsIdle())
		{
			triggerTime = 0f;
			movementSpeed = speed;
			moving = true;
			perlinNoiseBase = new Vector3(perlinNoiseSampleFrequency * (float)UnityEngine.Random.Range(0, 1), perlinNoiseSampleFrequency * (float)UnityEngine.Random.Range(0, 1), perlinNoiseSampleFrequency * (float)UnityEngine.Random.Range(0, 1));
			if (shakeDelay > float.Epsilon)
			{
				Invoke("TriggerShakeDelayed", shakeDelay);
			}
			else
			{
				TriggerShakeDelayed();
			}
		}
	}

	public bool IsIdle()
	{
		return !moving;
	}

	public int GetPriority()
	{
		return M3ShakeManager.priorZero;
	}
}
