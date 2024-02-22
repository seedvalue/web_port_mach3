using System.Collections;
using UnityEngine;

public class MetaSkillDummy : MetaSkill
{
	public override IEnumerator Execute(M3Battle battle, M3Board board, M3Player player)
	{
		UnityEngine.Debug.Log("Executing skill: " + displayName);
		yield return null;
	}
}
