using UnityEngine.UI;

public class M3SkillPreviewWindow : MetaWindow
{
	private const string strCooldown1 = "Fully charged in <b>";

	private const string strCooldown2 = "</b> turn(s).";

	public Text skillName;

	public Text skillDesc;

	public Text skillCooldown;

	public Image skillIcon;

	private M3SkillView skillView;

	protected virtual void OnEnable()
	{
		skillView = (base.context as M3SkillView);
		if ((bool)skillView)
		{
			Init();
		}
	}

	private void Init()
	{
		if ((bool)skillName)
		{
			skillName.text = skillView.skill.displayName;
		}
		if ((bool)skillDesc)
		{
			skillDesc.text = skillView.skill.description;
		}
		if ((bool)skillCooldown)
		{
			skillCooldown.text = "Fully charged in <b>" + skillView.Cooldown + "</b> turn(s).";
		}
		if ((bool)skillIcon)
		{
			skillIcon.sprite = skillView.skill.icon;
		}
	}
}
