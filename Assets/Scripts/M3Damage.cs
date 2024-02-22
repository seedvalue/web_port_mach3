using System.Collections.Generic;
using UnityEngine;

public class M3Damage
{
	public M3Orb orb;

	public int damage;

	public int numTargets = 1;

	public List<M3Mob> targets = new List<M3Mob>();

	public float multiplier = 1f;

	public List<M3AttackModifier> modifiers = new List<M3AttackModifier>();

	public M3Damage(M3Orb orbType, int dmg, int maxTargets)
	{
		Init(orbType);
		damage = dmg;
		numTargets = maxTargets;
	}

	public M3Damage(M3Orb orbType)
	{
		Init(orbType);
	}

	public void Init(M3Orb orbType)
	{
		damage = 0;
		multiplier = 1f;
		numTargets = 1;
		orb = orbType;
		modifiers.Clear();
		targets.Clear();
	}

	public void ApplyModifier(M3AttackModifier mod)
	{
		damage = Mathf.RoundToInt((float)damage * mod.damageMultiplier);
		multiplier *= mod.damageMultiplier;
		numTargets = Mathf.Max(numTargets, mod.numTargets);
	}

	public bool IsTarget(M3Mob mob)
	{
		for (int i = 0; i < targets.Count; i++)
		{
			if (targets[i] == mob)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsAttackActive()
	{
		bool flag = false;
		for (int i = 0; i < targets.Count; i++)
		{
			flag = (flag || (targets[i].IsAlive() && !targets[i].IsIdle()));
		}
		return flag;
	}

	public bool IsApplicable(M3Mob mob)
	{
		if ((bool)mob)
		{
			return targets.Count < numTargets && !IsTarget(mob);
		}
		return targets.Count < numTargets;
	}
}
