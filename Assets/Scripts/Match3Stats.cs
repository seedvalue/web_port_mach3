using System;

[Serializable]
public class Match3Stats
{
	public int simpleMoves;

	public int oneStructureMoves;

	public int allMoves;

	public int orbSwapTotal;

	public int orbStructureTotal;

	public int skillsUsed;

	public void Assign(Match3Stats stats)
	{
		simpleMoves = stats.simpleMoves;
		oneStructureMoves = stats.oneStructureMoves;
		allMoves = stats.allMoves;
		orbSwapTotal = stats.orbSwapTotal;
		orbStructureTotal = stats.orbStructureTotal;
		skillsUsed = stats.skillsUsed;
	}
}
