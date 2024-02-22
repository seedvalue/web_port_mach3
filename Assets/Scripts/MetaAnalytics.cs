using System.Linq;
using Utils;

public class MetaAnalytics : MetaComponent<MetaAnalytics>
{
	public int grinds => Singleton<Meta>.Instance.FindObjects<MetaStage>().Sum((MetaStage s) => s.easyGrinds + s.mediumGrinds + s.hardGrinds);

	public int gemsSpent => MetaResource.gems.spent;

	public int coinsSpent => MetaResource.coins.spent;

	public int gemsCurrent => MetaResource.gems.count;

	public int coinsCurrent => MetaResource.coins.count;

	public int itemsUpgradable => (from i in Singleton<Meta>.Instance.FindObjects<MetaItem>()
		where i.found && i.canUpgrade
		select i).Count();

	public float realTime => (float)SingletonComponent<Meta, MetaTimeManager>.Instance.realTimeSinceInstall.TotalDays;

	public float gameTime => (float)SingletonComponent<Meta, MetaTimeManager>.Instance.gameTimeSinceInstall.TotalHours;
}
