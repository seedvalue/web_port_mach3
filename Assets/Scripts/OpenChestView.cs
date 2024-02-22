using UnityEngine;
using UnityEngine.UI;
using Utils;

public class OpenChestView : MetaView
{
	[Header("Open Chest View")]
	public Text nameText;

	public Text countText;

	public Image iconImage;

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
		Helpers.SetText(nameText, GetProperty<string>("displayName"));
		Helpers.SetText(countText, GetPropertyOrDefault<int>("count").ToString());
		Helpers.SetImage(iconImage, GetProperty<Sprite>("icon"));
	}
}
