using System;
using UnityEngine;

public class M3MobZoomer : MonoBehaviour, IM3CameraShaker
{
	private bool zooming;

	private float zoomTime;

	private float zoomTimeMax;

	private bool zoomed;

	private Vector3 srcPos;

	private Vector3 destPos;

	private Vector3 currPos;

	private Quaternion srcRot;

	private Quaternion destRot;

	private Quaternion currRot;

	private M3ShakeManager shakeManager;

	public void UpdateState(float deltaTime, ref Vector3 outPos, ref Quaternion outRot)
	{
		if (zooming)
		{
			zoomTime += deltaTime;
			float t = Mathf.Min(1f, zoomTime / zoomTimeMax);
			t = Mathf.SmoothStep(0f, 1f, t);
			currPos = Vector3.Lerp(srcPos, destPos, t);
			currRot = Quaternion.Slerp(srcRot, destRot, t);
			if (zoomTime >= zoomTimeMax)
			{
				zooming = false;
				zoomed = !zoomed;
			}
		}
		else if (zoomed)
		{
			currPos = destPos;
			currRot = destRot;
		}
		outPos = currPos;
		outRot = currRot;
	}

	public void ZoomIn(M3Mob zoomTarget, float zoomInTime)
	{
		if (!shakeManager)
		{
			shakeManager = UnityEngine.Object.FindObjectOfType<M3ShakeManager>();
		}
		if ((bool)zoomTarget)
		{
			srcPos = base.transform.position;
			srcRot = base.transform.rotation;
			Vector3 position = zoomTarget.transform.position;
			float x = position.x;
			Vector3 position2 = zoomTarget.transform.position;
			float y = position2.y;
			Vector3 position3 = zoomTarget.transform.position;
			Vector3 vector = new Vector3(x, y, position3.z);
			vector -= srcPos;
			vector = Quaternion.Inverse(srcRot) * vector;
			float x2 = vector.x;
			float y2 = vector.y + zoomTarget.zoom.yPos;
			Vector3 localPosition = zoomTarget.transform.localPosition;
			destPos = new Vector3(x2, y2, localPosition.z - zoomTarget.zoom.distance);
			float num = vector.y = vector.y + zoomTarget.zoom.yPos + zoomTarget.zoom.distance * Mathf.Tan(zoomTarget.zoom.yAngle * 2f * (float)Math.PI / 360f);
			Vector3 forward = Vector3.Normalize(vector - destPos);
			destRot = Quaternion.LookRotation(forward);
			srcPos = Vector3.zero;
			srcRot = Quaternion.identity;
			zoomTime = 0f;
			zoomTimeMax = zoomInTime;
			zooming = true;
			if ((bool)shakeManager)
			{
				shakeManager.StartShake(this);
			}
		}
	}

	public void ZoomOut(float zoomOutTime)
	{
		srcPos = currPos;
		srcRot = currRot;
		destPos = Vector3.zero;
		destRot = Quaternion.identity;
		if (zooming)
		{
			zoomed = true;
		}
		zoomTime = 0f;
		zoomTimeMax = zoomOutTime;
		zooming = true;
	}

	public bool IsIdle()
	{
		return !zoomed && !zooming;
	}

	public int GetPriority()
	{
		return M3ShakeManager.priorHigh;
	}
}
