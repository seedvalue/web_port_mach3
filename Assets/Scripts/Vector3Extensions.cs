using UnityEngine;

public static class Vector3Extensions
{
	public static Vector2 ToXZ(this Vector3 vect)
	{
		return new Vector2(vect.x, vect.z);
	}

	public static Vector3 ZeroY(this Vector3 vect)
	{
		return new Vector3(vect.x, 0f, vect.z);
	}

	public static string ToStringExact(this Vector3 vect)
	{
		return "x " + vect.x + " y " + vect.y + " z " + vect.z;
	}

	public static string ToStringWithFormatter(this Vector3 vect, string formatter)
	{
		return "x " + vect.x.ToString(formatter) + " y " + vect.y.ToString(formatter) + " z " + vect.z.ToString(formatter);
	}
}
