using System.Collections.Generic;
using UnityEngine;

public class M3SmartPath : MonoBehaviour
{
	private const float lengthApproxMultiplier = 1.25f;

	public float sampleApproxLength = 0.1f;

	public float debugEvalDistance = 0.5f;

	private AnimationCurve xPath = new AnimationCurve();

	private AnimationCurve yPath = new AnimationCurve();

	private AnimationCurve zPath = new AnimationCurve();

	private bool preEvaluated;

	private float sampleTime;

	private List<M3PathPoint> points = new List<M3PathPoint>();

	public Vector3 this[int index]
	{
		get
		{
			if (index < xPath.length)
			{
				return new Vector3(xPath.keys[index].value, yPath.keys[index].value, zPath.keys[index].value);
			}
			return Vector3.zero;
		}
	}

	public int KeyCount => xPath.length;

	public float Length => (points.Count <= 0) ? 0f : points[points.Count - 1].distance;

	public M3PathPoint LastPoint => (!preEvaluated) ? null : points[points.Count - 1];

	public M3PathPoint FirstPoint => (!preEvaluated) ? null : points[0];

	public M3SmartPath()
	{
		xPath.postWrapMode = WrapMode.ClampForever;
		yPath.postWrapMode = WrapMode.ClampForever;
		zPath.postWrapMode = WrapMode.ClampForever;
	}

	public void Init()
	{
		ClearKeys();
		List<M3Battle> list = new List<M3Battle>();
		list.AddRange(GetComponentsInChildren<M3Battle>());
		for (int i = 0; i < list.Count; i++)
		{
			AddNavpoint(list[i].transform.position);
		}
		SmoothPath();
		PreEvaluate();
	}

	public M3PathPoint WorldPosToPath(Vector3 worldPos)
	{
		M3PathPoint pathPoint = null;
		if (KeyCount > 1)
		{
			float num = float.MaxValue;
			float num2 = Vector3.Distance(worldPos, this[0]);
			int num3 = -1;
			for (int i = 1; i < KeyCount; i++)
			{
				float num4 = num2 + Vector3.Distance(worldPos, this[i]);
				if (num4 < num)
				{
					num = num4;
					num3 = i - 1;
				}
				num2 = num4 - num2;
			}
			float num5 = CurveTimeToDistance(num3);
			float num6 = CurveTimeToDistance(num3 + 1);
			float distance = num5 + Vector3.Distance(worldPos, this[num3]) / num * (num6 - num5);
			Evaluate(distance, out pathPoint);
		}
		return pathPoint;
	}

	private void ClearKeys()
	{
		while (KeyCount > 0)
		{
			xPath.RemoveKey(0);
			yPath.RemoveKey(0);
			zPath.RemoveKey(0);
		}
	}

	private void AddNavpoint(Vector3 navpoint)
	{
		xPath.AddKey(xPath.length, navpoint.x);
		yPath.AddKey(yPath.length, navpoint.y);
		zPath.AddKey(zPath.length, navpoint.z);
		preEvaluated = false;
	}

	private void SmoothPath()
	{
		for (int i = 0; i < KeyCount; i++)
		{
			xPath.SmoothTangents(i, 0f);
			yPath.SmoothTangents(i, 0f);
			zPath.SmoothTangents(i, 0f);
		}
	}

	public M3PathPoint Evaluate(float distance)
	{
		Evaluate(distance, out M3PathPoint pathPoint);
		return pathPoint;
	}

	public void Evaluate(float distance, out M3PathPoint pathPoint)
	{
		FindPointsAround(distance, out M3PathPoint p, out M3PathPoint p2);
		if (p == null)
		{
			pathPoint = p2;
			return;
		}
		if (p2 == null)
		{
			pathPoint = p;
			return;
		}
		float t = (distance - p.distance) / (p2.distance - p.distance);
		pathPoint = new M3PathPoint();
		pathPoint.distance = distance;
		pathPoint.position = Vector3.Lerp(p.position, p2.position, t);
		pathPoint.pitch = Mathf.LerpAngle(p.pitch, p2.pitch, t);
		pathPoint.yaw = Mathf.LerpAngle(p.yaw, p2.yaw, t);
	}

	public float DistanceToNextNavpoint(int srcNavpoint)
	{
		return CurveTimeToDistance(srcNavpoint + 1) - CurveTimeToDistance(srcNavpoint);
	}

	public float CurveTimeToDistance(float curveTime)
	{
		if (preEvaluated && points.Count > 0)
		{
			float num = curveTime / sampleTime;
			int num2 = Mathf.FloorToInt(num);
			float num3 = num - (float)num2;
			if (num2 >= points.Count - 1)
			{
				return LastPoint.distance;
			}
			if (num2 < 0)
			{
				return FirstPoint.distance;
			}
			return points[num2].distance + num3 * (points[num2 + 1].distance - points[num2].distance);
		}
		return 0f;
	}

	private void FindPointsAround(float distance, out M3PathPoint p1, out M3PathPoint p2)
	{
		if (!preEvaluated || points.Count == 0)
		{
			p1 = (p2 = null);
		}
		else if (distance < points[0].distance)
		{
			p1 = null;
			p2 = points[0];
		}
		else if (distance > points[points.Count - 1].distance)
		{
			p1 = points[points.Count - 1];
			p2 = null;
		}
		else
		{
			FindPointsAroundInRange(distance, 0, points.Count - 1, out p1, out p2);
		}
	}

	private void FindPointsAroundInRange(float distance, int indexMin, int indexMax, out M3PathPoint p1, out M3PathPoint p2)
	{
		if (indexMin == indexMax - 1)
		{
			p1 = points[indexMin];
			p2 = points[indexMax];
			return;
		}
		int num = (indexMax + indexMin) / 2;
		if (points[num].distance > distance)
		{
			FindPointsAroundInRange(distance, indexMin, num, out p1, out p2);
		}
		else
		{
			FindPointsAroundInRange(distance, num, indexMax, out p1, out p2);
		}
	}

	private void PreEvaluate()
	{
		preEvaluated = false;
		points.Clear();
		if (KeyCount <= 1)
		{
			return;
		}
		float num = KeyCount - 1;
		int num2 = Mathf.Max(Mathf.RoundToInt(Vector3.Distance(this[0], this[KeyCount - 1]) * 1.25f / sampleApproxLength), 10);
		sampleTime = num / (float)num2;
		float num3 = 0f;
		M3PathPoint m3PathPoint = null;
		points.Capacity = num2;
		for (int i = 0; i <= num2; i++)
		{
			M3PathPoint m3PathPoint2 = new M3PathPoint();
			m3PathPoint2.position = new Vector3(xPath.Evaluate(num3), yPath.Evaluate(num3), zPath.Evaluate(num3));
			if (m3PathPoint != null)
			{
				m3PathPoint2.distance = Vector3.Distance(m3PathPoint2.position, m3PathPoint.position) + m3PathPoint.distance;
			}
			points.Add(m3PathPoint2);
			num3 += sampleTime;
			m3PathPoint = m3PathPoint2;
		}
		for (int j = 0; j < points.Count - 1; j++)
		{
			points[j].CalcPitchYaw(points[j + 1].position, points[j + 1].distance);
		}
		preEvaluated = true;
	}

	private void OnDrawGizmos()
	{
		Init();
		Gizmos.color = Color.blue;
		for (float num = 0f; num < Length; num += Mathf.Max(0.1f, debugEvalDistance))
		{
			Evaluate(num, out M3PathPoint pathPoint);
			Gizmos.DrawSphere(pathPoint.position, 0.05f);
		}
	}
}
