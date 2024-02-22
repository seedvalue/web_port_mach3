using UnityEngine.UI;

public class M3MobWindow : MetaWindow
{
	public Text mobName;

	public Text mobAttack;

	public Text mobDefense;

	public Text mobHP;

	private M3Mob mob;

	protected virtual void OnEnable()
	{
		mob = (base.context as M3Mob);
		if ((bool)mob)
		{
			Init();
		}
	}

	private void Init()
	{
		if ((bool)mobName)
		{
			mobName.text = mob.metaMob.displayName;
		}
		if ((bool)mobAttack)
		{
			mobAttack.text = mob.MetaStats.atk.ToString();
		}
		if ((bool)mobHP)
		{
			mobHP.text = mob.HP + "/" + mob.MetaStats.hp;
		}
		if ((bool)mobDefense)
		{
			mobDefense.text = mob.MetaStats.def.ToString();
		}
	}
}
