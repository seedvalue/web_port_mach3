using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class M3UIEvents : MonoBehaviour
{
	public Image logImage;

	public Text logText;

	private M3TileManager tileManager;

	private void Start()
	{
		tileManager = UnityEngine.Object.FindObjectOfType<M3TileManager>();
	}

	public void WinBattle()
	{
		StartCoroutine(tileManager.Debug_WinBattle());
	}

	public void WinStage()
	{
		tileManager.Debug_WinStage();
	}

	public void LoseStage()
	{
		tileManager.Debug_GameOver();
	}

	public void LoadMenu()
	{
		tileManager.QuitToMainMenu(MetaFight.Result.Cancel);
	}

	public void ShowLog()
	{
		if ((bool)logText && (bool)logImage)
		{
			if (logImage.IsActive())
			{
				logText.text = string.Empty;
				logImage.gameObject.SetActive(value: false);
			}
			else
			{
				logImage.gameObject.SetActive(value: true);
				M3TileManager.ShowLog(logText);
			}
		}
	}

	public void SaveGame()
	{
		M3SaveData m3SaveData = new M3SaveData(M3TileManager.GAMEVERSION);
		tileManager.Save(m3SaveData);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = File.Open(Application.persistentDataPath + "\\match3.save", FileMode.OpenOrCreate, FileAccess.Write);
		binaryFormatter.Serialize(fileStream, m3SaveData);
		fileStream.Close();
	}

	public void ShakeCamera(bool all)
	{
		M3ActionEffect m3ActionEffect = UnityEngine.Object.FindObjectOfType<M3ActionEffect>();
		if (all)
		{
			StartCoroutine(m3ActionEffect.Trigger(null, null));
			return;
		}
		List<M3Mob> list = new List<M3Mob>();
		int index = UnityEngine.Random.Range(0, tileManager.Battle.Mobs.Count);
		list.Add(tileManager.Battle.Mobs[index]);
		StartCoroutine(m3ActionEffect.Trigger(null, null, list));
	}
}
