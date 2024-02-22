using System.Collections;
using UnityEngine;

public class MetaSkillStun : MetaSkill
{
	[WorkbookAlias("Value")]
	public int turns;

	[WorkbookAlias("Int Param")]
	public int numTargets;

	public override IEnumerator Execute(M3Battle battle, M3Board board, M3Player player)
	{
		M3Damage dmgObj = BuildDamageObj(battle);
		AudioManager.PlaySafe(sfxSample);
		if ((bool)fxPrefab)
		{
			yield return StartCoroutine(player.FireActionEffect(fxPrefab, dmgObj.targets));
		}
		for (int i = 0; i < dmgObj.targets.Count; i++)
		{
			dmgObj.targets[i].Cooldown++;
		}
	}

	private M3Damage BuildDamageObj(M3Battle battle)
	{
		M3Damage m3Damage = new M3Damage(M3Orb.None, 0, 1);
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
		return m3Damage;
	}
}
