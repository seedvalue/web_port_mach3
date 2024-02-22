using System;

[Serializable]
public class MobStats
{
	[WorkbookAlias("HP")]
	public int hp;

	public int def;

	public int atk;

	public int atkInterval;

	public static MobStatsF operator *(MobStats lhs, MobStatsMultipliers rhs)
	{
		MobStatsF mobStatsF = new MobStatsF();
		mobStatsF.hp = (float)lhs.hp * rhs.hp;
		mobStatsF.def = (float)lhs.def * rhs.def;
		mobStatsF.atk = (float)lhs.atk * rhs.atk;
		mobStatsF.atkInterval = lhs.atkInterval;
		return mobStatsF;
	}
}
