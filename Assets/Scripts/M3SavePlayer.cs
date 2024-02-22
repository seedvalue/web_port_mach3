using System;
using System.Collections.Generic;

[Serializable]
public class M3SavePlayer
{
	public int hp;

	public float distance;

	public List<int> skillCooldowns = new List<int>();

	public float[] position = new float[3];

	public float angle;
}
