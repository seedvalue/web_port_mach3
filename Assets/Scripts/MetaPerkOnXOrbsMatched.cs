using System.Collections.Generic;

public class MetaPerkOnXOrbsMatched : MetaPerk, IPerkStructureMatchHandler
{
	public int orbsToTrigger;

	public bool inSingleStructure;

	public bool allowMultipleBoost;

	public float damageAmplifier = 1f;

	public int targetsAffected = 1;

	private int orbsMatched;

	public override void OnM3Start()
	{
		base.OnM3Start();
		orbsMatched = 0;
	}

	public void OnStructureMatch(M3Orb orbType, int orbCount, M3OrbStructure structure, List<M3AttackModifier> atkModifiers, List<M3DefenseModifier> defModifiers)
	{
		orbsMatched += orbCount;
		if ((!triggered || allowMultipleBoost) && ((inSingleStructure && orbCount >= orbsToTrigger) || (!inSingleStructure && orbsMatched >= orbsToTrigger)))
		{
			triggered = true;
			M3AttackModifier item = new M3AttackModifier(M3AttackModifierRange.Global, damageAmplifier, targetsAffected);
			atkModifiers.Add(item);
		}
	}
}
