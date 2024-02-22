using UnityEngine;
using UnityEngine.UI;
using Utils;

public class MetaSkillView : MetaView
{
	[Header("Meta Skill View")]
	public Text nameText;

	public Image iconImage;

	public Text descriptionText;

	public Text cooldownText;

	public string cooldownPostFix = string.Empty;

	public new MetaSkill GetObject()
	{
		return base.GetObject() as MetaSkill;
	}

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
		MetaSkill @object = GetObject();
		Helpers.SetText(nameText, @object.displayName);
		Helpers.SetImage(iconImage, @object.icon);
		Helpers.SetText(descriptionText, @object.description);
		Helpers.SetText(cooldownText, @object.cooldown.ToString() + cooldownPostFix);
	}
}
