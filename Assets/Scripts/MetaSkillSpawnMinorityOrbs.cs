using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaSkillSpawnMinorityOrbs : MetaSkill
{
	[WorkbookAlias("Value")]
	public int orbCount;

	[WorkbookAlias("Delay")]
	public float delay;

	public override IEnumerator Execute(M3Battle battle, M3Board board, M3Player player)
	{
		List<IntVector2> orbCandidates = new List<IntVector2>();
		M3Orb orbType = GetMinorityOrb(board);
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

	private M3Orb GetMinorityOrb(M3Board board)
	{
		int[] array = new int[6];
		for (int i = 0; i < board.columns; i++)
		{
			for (int j = 0; j < board.rows; j++)
			{
				array[(int)board.board[i, j].attackType]++;
			}
		}
		int num = 1;
		int num2 = array[0];
		int result = 0;
		for (int k = 1; k < array.Length; k++)
		{
			if (array[k] < num2)
			{
				result = k;
				num2 = array[k];
				num = 1;
			}
			else if (array[k] == num2)
			{
				num++;
			}
		}
		if (num == 1)
		{
			return (M3Orb)result;
		}
		num = Random.Range(0, num);
		for (int l = 0; l < array.Length; l++)
		{
			if (array[l] == num2)
			{
				if (num == 0)
				{
					return (M3Orb)l;
				}
				num--;
			}
		}
		return M3Orb.None;
	}
}
