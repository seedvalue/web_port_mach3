using UnityEngine;
using UnityEngine.UI;
using Utils;

public class MetaPerkView : MetaView
{
	[Header("Meta Stage")]
	public Text nameText;

	public Image iconImage;

	public new MetaPerk GetObject()
	{
		return base.GetObject() as MetaPerk;
	}

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
		MetaPerk @object = GetObject();
		Helpers.SetText(nameText, @object.displayName);
		Helpers.SetImage(iconImage, @object.icon);
	}
}
