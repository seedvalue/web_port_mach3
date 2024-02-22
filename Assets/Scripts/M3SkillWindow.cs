using UnityEngine.UI;

public class M3SkillWindow : MetaWindow
{
	private const string strSkillCooldown1 = "Fully charged in ";

	private const string strSkillCooldown2 = " turn(s).";

	public Image skillIcon;

	public Text skillName;

	public Text skillDescription;

	public Text skillCooldown;

	private MetaSkill skill;

	protected virtual void OnEnable()
	{
		skill = (base.context as MetaSkill);
		if ((bool)skill)
		{
			Init();
		}
	}

	private void Init()
	{
		if ((bool)skillName)
		{
			skillName.text = skill.displayName;
		}
		if ((bool)skillDescription)
		{
			skillDescription.text = skill.description;
		}
		if ((bool)skillCooldown)
		{
			skillCooldown.text = "Fully charged in " + skill.cooldown + " turn(s).";
		}
		if ((bool)skillIcon)
		{
			skillIcon.sprite = skill.icon;
		}
	}

	public void OnUse()
	{
		CloseWindow(1);
	}
}
