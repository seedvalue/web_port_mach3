using System.Collections;
using UnityEngine;

public class MetaSkillHeal : MetaSkill
{
	[WorkbookAlias("Value")]
	public int multiplier;

	public override IEnumerator Execute(M3Battle battle, M3Board board, M3Player player)
	{
		float time = Time.time;
		int hpHealed = player.GetStat(M3Orb.Recovery) * multiplier;
		AudioManager.PlaySafe(sfxSample);
		if ((bool)fxPrefab)
		{
			yield return StartCoroutine(player.FireActionEffect(fxPrefab, null));
		}
		float animDuration = duration - (Time.time - time);
		StartCoroutine(player.ShowHealing(hpHealed, animDuration));
		if (animDuration <= 0f)
		{
			player.HealHP(hpHealed);
			yield break;
		}
		float animTime = 0f;
		int hpLeftToHeal = hpHealed;
		do
		{
			yield return null;
			int hpPlus = Mathf.RoundToInt((float)hpHealed * Time.deltaTime / animDuration);
			hpLeftToHeal -= hpPlus;
			if (hpLeftToHeal < 0)
			{
				hpPlus += hpLeftToHeal;
				hpLeftToHeal = 0;
			}
			player.HealHP(hpPlus);
			animTime += Time.deltaTime;
		}
		while (animTime < animDuration);
		if (hpLeftToHeal > 0)
		{
			player.HealHP(hpLeftToHeal);
		}
	}
}
