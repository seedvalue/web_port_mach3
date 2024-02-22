using UnityEngine;

public class MobStatsF
{
	public float hp;

	public float def;

	public float atk;

	public int atkInterval;

	public MobStats Round()
	{
		MobStats mobStats = new MobStats();
		mobStats.hp = Mathf.RoundToInt(hp);
		mobStats.def = Mathf.RoundToInt(def);
		mobStats.atk = Mathf.RoundToInt(atk);
		mobStats.atkInterval = atkInterval;
		return mobStats;
	}

	public static MobStatsF operator *(MobStatsF lhs, MobStatsMultipliers rhs)
	{
		MobStatsF mobStatsF = new MobStatsF();
		mobStatsF.hp = lhs.hp * rhs.hp;
		mobStatsF.def = lhs.def * rhs.def;
		mobStatsF.atk = lhs.atk * rhs.atk;
		mobStatsF.atkInterval = lhs.atkInterval;
		return mobStatsF;
	}
}
