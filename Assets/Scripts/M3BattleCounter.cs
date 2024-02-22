using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class M3BattleCounter : MonoBehaviour
{
	public Image battleEmpty;

	public Image battleFull;

	public Image bossBattleEmpty;

	public Image bossBattleFull;

	public float maxBattleDistance = 120f;

	private List<Image> battlesEmpty = new List<Image>();

	private int battleNo = -1;

	public void Init(int numBattles)
	{
		if (numBattles >= 2 && ((bool)battleEmpty || (bool)battleFull))
		{
			battlesEmpty.Add(battleEmpty);
			for (int i = 1; i < numBattles - 1; i++)
			{
				Image image = UnityEngine.Object.Instantiate(battleEmpty, base.transform);
				image.transform.position += new Vector3((float)i * maxBattleDistance / (float)(numBattles - 1), 0f, 0f);
				battlesEmpty.Add(image);
				image.transform.SetSiblingIndex(0);
			}
			bossBattleEmpty.transform.position = battleEmpty.transform.position + new Vector3(maxBattleDistance, 0f, 0f);
			battlesEmpty.Add(bossBattleEmpty);
		}
		NextBattle();
	}

	public void NextBattle()
	{
		battleNo++;
		if (battleNo < battlesEmpty.Count)
		{
			bossBattleFull.enabled = (battleNo == battlesEmpty.Count - 1);
			battleFull.enabled = !bossBattleFull.enabled;
			Image image = (!bossBattleFull.enabled) ? battleFull : bossBattleFull;
			image.transform.position = battlesEmpty[battleNo].transform.position;
		}
	}
}
