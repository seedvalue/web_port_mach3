using System.Collections.Generic;

public class Match3StartContext
{
	public MetaStage stage;

	public MetaStageDifficulty difficulty;

	public Stats stats;

	public List<MetaItem> items;

	public List<MetaLink> rewards;

	public M3Tutorial tutorialPrefab;

	public M3FixedBoard tileProvider;

	public bool useSave;

	public bool canQuit = true;
}
