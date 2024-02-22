using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaSkillAttributeCall : MetaSkill
{
	[WorkbookAlias("Value")]
	public int orbCount;

	[WorkbookAlias("Orbs 1")]
	public M3Orb orbType;

	[WorkbookAlias("Delay")]
	public float delay;

	public override IEnumerator Execute(M3Battle battle, M3Board board, M3Player player)
	{
		List<IntVector2> orbCandidates = new List<IntVector2>();
		board.FindOrbs(orbCandidates, orbType, exclude: true, includeRecovery: true);
		int changes = Mathf.Min(orbCount, orbCandidates.Count);
		for (int i = 0; i < changes; i++)
		{
			int index = Random.Range(0, orbCandidates.Count);
			int column = orbCandidates[index].x;
			int row = orbCandidates[index].y;
			orbCandidates.RemoveAt(index);
			StartCoroutine(board.ChangeOrb(column, row, orbType, delay));
		}
		AudioManager.PlaySafe(sfxSample);
		UnityEngine.Debug.Log("Executing skill: " + displayName);
		if (delay + duration > float.Epsilon)
		{
			yield return new WaitForSeconds(delay + duration);
		}
	}
}
