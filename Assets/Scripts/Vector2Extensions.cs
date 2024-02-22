using UnityEngine;

public static class Vector2Extensions
{
	public static string ToStringExact(this Vector2 vect)
	{
		return "x " + vect.x + " y " + vect.y;
	}

	public static string ToStringWithFormatter(this Vector2 vect, string formatter)
	{
		return "x " + vect.x.ToString(formatter) + " y " + vect.y.ToString(formatter);
	}
}
