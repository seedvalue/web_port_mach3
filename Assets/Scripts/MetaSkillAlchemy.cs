using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaSkillAlchemy : MetaSkill
{
	[WorkbookAlias("Delay")]
	public float delay;

	[WorkbookAlias("Orbs 1")]
	public List<M3Orb> orbsFrom;

	[WorkbookAlias("Orbs 2")]
	public List<M3Orb> orbsTo;

	[WorkbookAlias("Param bool")]
	public bool paired;

	private int FindOrb(List<M3Orb> orbs, M3Orb lookFor)
	{
		for (int i = 0; i < orbs.Count; i++)
		{
			if (orbs[i] == lookFor)
			{
				return i;
			}
		}
		return -1;
	}

	public override IEnumerator Execute(M3Battle battle, M3Board board, M3Player player)
	{
		if ((bool)fxPrefab)
		{
			Object.Instantiate<GameObject>(position: new Vector3(board.tileSize * board.columns / 2, board.tileSize * board.rows / 2, board.tileZ - 5f), original: fxPrefab, rotation: Quaternion.identity);
		}
		for (int i = 0; i < board.columns; i++)
		{
			for (int j = 0; j < board.rows; j++)
			{
				int num = FindOrb(orbsFrom, board.board[i, j].attackType);
				if (num > -1)
				{
					M3Orb orbNew = (!paired) ? orbsTo[Random.Range(0, orbsTo.Count)] : orbsTo[num];
					StartCoroutine(board.ChangeOrb(i, j, orbNew, delay));
				}
			}
		}
		AudioManager.PlaySafe(sfxSample);
		if (duration + delay > float.Epsilon)
		{
			yield return new WaitForSeconds(duration + delay);
		}
	}
}
