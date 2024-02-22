using System;

[Serializable]
public class M3SaveData
{
	public int m3GameVersion;

	public int saveCurrentBattle;

	public Match3Stats match3Stats = new Match3Stats();

	public M3SavePlayer player = new M3SavePlayer();

	public M3SaveBattle battle = new M3SaveBattle();

	public M3SaveBoard board = new M3SaveBoard();

	public M3SaveTutorial tutorial = new M3SaveTutorial();

	public M3SaveData(int gameVersion)
	{
		m3GameVersion = gameVersion;
	}
}
