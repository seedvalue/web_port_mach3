using System;
using UnityEngine;

public class M3PathPoint
{
	public Vector3 position;

	public float distance;

	public float pitch;

	public float yaw;

	public void CalcPitchYaw(Vector3 nextPos, float nextDistance)
	{
		Vector3 to = nextPos - position;
		to.y = 0f;
		to.Normalize();
		pitch = Vector3.Angle(Vector3.forward, to);
		if (to.x < 0f)
		{
			pitch = 0f - pitch;
		}
		yaw = Mathf.Atan(Mathf.Abs(nextPos.y - position.y) / (nextDistance - distance)) * 360f / 2f * (float)Math.PI;
		if (nextPos.y < position.y)
		{
			yaw = 0f - yaw;
		}
	}
}
