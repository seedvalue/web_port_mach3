using System;
using UnityEngine;

public class M3OrbParticle : MonoBehaviour
{
	public float velocity = 1000f;

	public float arcAmplitude = 0.2f;

	private float duration;

	private Vector3 destination;

	private Vector3 source;

	private Vector3 offset;

	private M3InterpolatedFloat time;

	public float Fly(Vector3 dest)
	{
		destination = dest;
		source = base.transform.position;
		Vector3 vector = destination - source;
		vector = Vector3.Normalize(new Vector3(vector.y, 0f - vector.x, 0f)) * arcAmplitude * (destination.x - source.x);
		return Fly(dest, vector);
	}

	public float Fly(Vector3 dest, Vector3 arc)
	{
		destination = dest;
		source = base.transform.position;
		offset = arc;
		duration = M3TileManager.instance.debugTimeMultiplier * Vector3.Distance(source, destination) / velocity;
		if (time == null)
		{
			time = new M3InterpolatedFloat(this, M3InterpolationMode.Linear);
		}
		time.SlideUp(duration);
		return duration;
	}

	private void Update()
	{
		if (time != null)
		{
			Vector3 a = Vector3.Lerp(source, destination, time.Value);
			float d = Mathf.Sin(time.Value * (float)Math.PI);
			a += offset * d;
			base.transform.position = a;
		}
	}
}
