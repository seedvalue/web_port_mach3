using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaSkillDealDamage : MetaSkill
{
	[WorkbookAlias("Value")]
	public int damageMultiplier;

	[WorkbookAlias("Orbs 1")]
	public List<M3Orb> orbType;

	[WorkbookAlias("Param int")]
	public int numTargets;

	[WorkbookAlias("Param bool")]
	public bool randomDamage;

	[WorkbookAlias("Param float")]
	public float pierceFactor;

	private M3Damage BuildDamageObj(M3Battle battle, int damage)
	{
		M3Damage m3Damage = new M3Damage(orbType[0], damage, 1);
		if (numTargets == 0)
		{
			m3Damage.numTargets = battle.Mobs.Count;
			for (int i = 0; i < battle.Mobs.Count; i++)
			{
				if (battle.Mobs[i].IsAlive())
				{
					m3Damage.targets.Add(battle.Mobs[i]);
				}
			}
		}
		else
		{
			m3Damage.numTargets = Mathf.Min(numTargets, battle.AliveMobCount);
			for (int j = 0; j < m3Damage.numTargets; j++)
			{
				m3Damage.targets.Add(battle.GetTarget(m3Damage));
			}
		}
		if (pierceFactor > float.Epsilon)
		{
			m3Damage.modifiers.Add(new M3AttackModifier(1f - Mathf.Min(pierceFactor, 1f)));
		}
		return m3Damage;
	}

	public override IEnumerator Execute(M3Battle battle, M3Board board, M3Player player)
	{
		float time = Time.time;
		int damage = 0;
		foreach (M3Orb item in orbType)
		{
			damage += player.GetStat(item) * damageMultiplier;
		}
		if (randomDamage)
		{
			damage = Random.Range(0, damage) + 1;
		}
		M3Damage dmgObj = BuildDamageObj(battle, damage);
		if (numTargets > 0)
		{
			List<M3Mob> splitTargets = new List<M3Mob>();
			for (int i = 0; i < dmgObj.targets.Count; i++)
			{
				splitTargets.Add(dmgObj.targets[i]);
				AudioManager.PlaySafe(sfxSample);
				yield return StartCoroutine(player.FireActionEffect(fxPrefab, splitTargets, dmgObj.orb));
				StartCoroutine(dmgObj.targets[i].Wound(dmgObj));
				if (i < dmgObj.targets.Count - 1)
				{
					yield return new WaitForSeconds(player.timings.actualAttackAnimationDuration);
					splitTargets.Clear();
				}
			}
		}
		else
		{
			AudioManager.PlaySafe(sfxSample);
			yield return StartCoroutine(player.FireActionEffect(fxPrefab, null, dmgObj.orb));
			for (int j = 0; j < dmgObj.targets.Count; j++)
			{
				StartCoroutine(dmgObj.targets[j].Wound(dmgObj));
			}
		}
		if (Time.time - time < duration)
		{
			yield return new WaitForSeconds(duration - (Time.time - time));
		}
	}
}
