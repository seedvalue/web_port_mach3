using System;

[Serializable]
public class MobStatsMultipliers
{
	[WorkbookAlias("HP")]
	public float hp = 1f;

	[WorkbookAlias("Def")]
	public float def = 1f;

	[WorkbookAlias("Atk")]
	public float atk = 1f;

	public static MobStatsMultipliers operator *(MobStatsMultipliers lhs, MobStatsMultipliers rhs)
	{
		MobStatsMultipliers mobStatsMultipliers = new MobStatsMultipliers();
		mobStatsMultipliers.hp = lhs.hp * rhs.hp;
		mobStatsMultipliers.def = lhs.def * rhs.def;
		mobStatsMultipliers.atk = lhs.atk * rhs.atk;
		return mobStatsMultipliers;
	}
}
