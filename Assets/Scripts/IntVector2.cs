using System;
using UnityEngine;

[Serializable]
public class IntVector2
{
	public int x;

	public int y;

	public IntVector2(int _x, int _y)
	{
		x = _x;
		y = _y;
	}

	public static IntVector2 operator +(IntVector2 lhs, IntVector2 rhs)
	{
		return new IntVector2(lhs.x + rhs.x, lhs.y + rhs.y);
	}

	public static IntVector2 operator -(IntVector2 lhs, IntVector2 rhs)
	{
		return new IntVector2(lhs.x - rhs.x, lhs.y - rhs.y);
	}

	public bool Compare(IntVector2 vec)
	{
		return vec.x == x && vec.y == y;
	}

	public int Distance(IntVector2 vec)
	{
		return Mathf.Abs(vec.x - x) + Mathf.Abs(vec.y - y);
	}
}
