using System.Collections.Generic;

public class MetaPerkOn2OrbTypesMatched : MetaPerk, IPerkStructureMatchHandler
{
	public M3Orb orbType1;

	public M3Orb orbType2;

	public float damageAmplifier = 1f;

	private bool orbType1Matched;

	private bool orbType2Matched;

	public override void OnM3Start()
	{
		base.OnM3Start();
		orbType1Matched = (orbType2Matched = false);
	}

	public void OnStructureMatch(M3Orb orbType, int orbCount, M3OrbStructure structure, List<M3AttackModifier> atkModifiers, List<M3DefenseModifier> defModifiers)
	{
		if (!triggered)
		{
			orbType1Matched = (orbType1Matched || orbType1 == orbType);
			orbType2Matched = (orbType2Matched || orbType2 == orbType);
			if (orbType1Matched && orbType2Matched)
			{
				triggered = true;
				M3AttackModifier item = new M3AttackModifier(M3AttackModifierRange.Global, damageAmplifier, 1);
				atkModifiers.Add(item);
			}
		}
	}
}
