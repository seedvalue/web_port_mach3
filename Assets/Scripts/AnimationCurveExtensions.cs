using UnityEngine;

public static class AnimationCurveExtensions
{
	public static float MaxTime(this AnimationCurve curve)
	{
		if (curve.length == 0)
		{
			return 0f;
		}
		return curve[curve.length - 1].time;
	}

	public static float MinTime(this AnimationCurve curve)
	{
		if (curve.length == 0)
		{
			return 0f;
		}
		return curve[0].time;
	}
}
