using System;
using UnityEngine;

[Serializable]
public class Stats
{
	public int HP;

	public int Def;

	public int Rcv;

	public int Mel;

	public int Rng;

	public int Hrm;

	public int Prm;

	public int Vd;

	public int this[int index]
	{
		get
		{
			if (index < 6)
			{
				switch (index)
				{
				case 4:
					return Hrm;
				case 0:
					return Mel;
				case 1:
					return Rng;
				case 2:
					return Prm;
				case 3:
					return Vd;
				case 5:
					return Rcv;
				default:
					return 0;
				}
			}
			return 0;
		}
	}

	public int totalAttack => Mel + Rng + Hrm + Prm + Vd;

	public static Stats Randomize()
	{
		Stats stats = new Stats();
		stats.HP = (int)Mathf.Max(0f, UnityEngine.Random.Range(0f, 20f) - 10f);
		stats.Def = (int)Mathf.Max(0f, UnityEngine.Random.Range(0f, 20f) - 10f);
		stats.Rcv = (int)Mathf.Max(0f, UnityEngine.Random.Range(0f, 20f) - 10f);
		stats.Mel = (int)Mathf.Max(0f, UnityEngine.Random.Range(0f, 20f) - 10f);
		stats.Rng = (int)Mathf.Max(0f, UnityEngine.Random.Range(0f, 20f) - 10f);
		stats.Hrm = (int)Mathf.Max(0f, UnityEngine.Random.Range(0f, 20f) - 10f);
		stats.Prm = (int)Mathf.Max(0f, UnityEngine.Random.Range(0f, 20f) - 10f);
		stats.Vd = (int)Mathf.Max(0f, UnityEngine.Random.Range(0f, 20f) - 10f);
		return stats;
	}

	public static Stats RandomizeHPandRCV()
	{
		Stats stats = new Stats();
		stats.HP = (int)UnityEngine.Random.Range(10f, 20f);
		stats.Def = 0;
		stats.Rcv = (int)UnityEngine.Random.Range(10f, 20f);
		stats.Mel = 0;
		stats.Rng = 0;
		stats.Hrm = 0;
		stats.Prm = 0;
		stats.Vd = 0;
		return stats;
	}

	public static Stats operator +(Stats lhs, Stats rhs)
	{
		Stats stats = new Stats();
		stats.HP = lhs.HP + rhs.HP;
		stats.Def = lhs.Def + rhs.Def;
		stats.Rcv = lhs.Rcv + rhs.Rcv;
		stats.Mel = lhs.Mel + rhs.Mel;
		stats.Rng = lhs.Rng + rhs.Rng;
		stats.Hrm = lhs.Hrm + rhs.Hrm;
		stats.Prm = lhs.Prm + rhs.Prm;
		stats.Vd = lhs.Vd + rhs.Vd;
		return stats;
	}

	public static Stats operator -(Stats lhs, Stats rhs)
	{
		Stats stats = new Stats();
		stats.HP = lhs.HP - rhs.HP;
		stats.Def = lhs.Def - rhs.Def;
		stats.Rcv = lhs.Rcv - rhs.Rcv;
		stats.Mel = lhs.Mel - rhs.Mel;
		stats.Rng = lhs.Rng - rhs.Rng;
		stats.Hrm = lhs.Hrm - rhs.Hrm;
		stats.Prm = lhs.Prm - rhs.Prm;
		stats.Vd = lhs.Vd - rhs.Vd;
		return stats;
	}

	public static Stats operator *(Stats lhs, Stats rhs)
	{
		Stats stats = new Stats();
		stats.HP = lhs.HP * rhs.HP;
		stats.Def = lhs.Def * rhs.Def;
		stats.Rcv = lhs.Rcv * rhs.Rcv;
		stats.Mel = lhs.Mel * rhs.Mel;
		stats.Rng = lhs.Rng * rhs.Rng;
		stats.Hrm = lhs.Hrm * rhs.Hrm;
		stats.Prm = lhs.Prm * rhs.Prm;
		stats.Vd = lhs.Vd * rhs.Vd;
		return stats;
	}

	public static Stats operator *(Stats lhs, float rhs)
	{
		Stats stats = new Stats();
		stats.HP = Mathf.RoundToInt((float)lhs.HP * rhs);
		stats.Def = Mathf.RoundToInt((float)lhs.Def * rhs);
		stats.Rcv = Mathf.RoundToInt((float)lhs.Rcv * rhs);
		stats.Mel = Mathf.RoundToInt((float)lhs.Mel * rhs);
		stats.Rng = Mathf.RoundToInt((float)lhs.Rng * rhs);
		stats.Hrm = Mathf.RoundToInt((float)lhs.Hrm * rhs);
		stats.Prm = Mathf.RoundToInt((float)lhs.Prm * rhs);
		stats.Vd = Mathf.RoundToInt((float)lhs.Vd * rhs);
		return stats;
	}

	public static Stats operator +(Stats lhs, int rhs)
	{
		Stats stats = new Stats();
		stats.HP = lhs.HP + rhs;
		stats.Def = lhs.Def + rhs;
		stats.Rcv = lhs.Rcv + rhs;
		stats.Mel = lhs.Mel + rhs;
		stats.Rng = lhs.Rng + rhs;
		stats.Hrm = lhs.Hrm + rhs;
		stats.Prm = lhs.Prm + rhs;
		stats.Vd = lhs.Vd + rhs;
		return stats;
	}

	public static bool operator ==(Stats lhs, Stats rhs)
	{
		if (object.ReferenceEquals(lhs, rhs))
		{
			return true;
		}
		if ((object)lhs == null || (object)rhs == null)
		{
			return false;
		}
		return lhs.HP == rhs.HP && lhs.Def == rhs.Def && lhs.Rcv == rhs.Rcv && lhs.Mel == rhs.Mel && lhs.Rng == rhs.Rng && lhs.Hrm == rhs.Hrm && lhs.Prm == rhs.Prm && lhs.Vd == rhs.Vd;
	}

	public static bool operator !=(Stats lhs, Stats rhs)
	{
		return !(lhs == rhs);
	}

	public override bool Equals(object obj)
	{
		if (object.ReferenceEquals(null, obj))
		{
			return false;
		}
		if (object.ReferenceEquals(this, obj))
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return this == obj as Stats;
	}

	public override int GetHashCode()
	{
		int num = 0;
		num = ((num * 397) ^ HP);
		num = ((num * 397) ^ Def);
		num = ((num * 397) ^ Rcv);
		num = ((num * 397) ^ Mel);
		num = ((num * 397) ^ Rng);
		num = ((num * 397) ^ Hrm);
		num = ((num * 397) ^ Prm);
		return (num * 397) ^ Vd;
	}
}
