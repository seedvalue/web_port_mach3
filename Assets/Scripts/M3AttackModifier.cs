using System.Collections.Generic;

public class M3AttackModifier
{
	public M3AttackModifierRange range;

	public float damageMultiplier = 1f;

	public float defenseMultiplier = 1f;

	public int numTargets = 1;

	public List<int> filterKeywords = new List<int>();

	public M3AttackModifier(M3AttackModifierRange modifierRange, float multiplier, int targets = 1, params int[] keywords)
	{
		range = modifierRange;
		damageMultiplier = multiplier;
		if (range != 0)
		{
			numTargets = targets;
			for (int i = 0; i < keywords.Length; i++)
			{
				filterKeywords.Add(keywords[i]);
			}
		}
	}

	public M3AttackModifier(float defMultiplier)
	{
		defenseMultiplier = defMultiplier;
	}

	public bool AppliesTo(M3Mob mob)
	{
		bool flag = filterKeywords.Count == 0;
		for (int i = 0; i < filterKeywords.Count; i++)
		{
			flag = (flag || mob.IsKeyword(filterKeywords[i]));
		}
		return flag;
	}
}
