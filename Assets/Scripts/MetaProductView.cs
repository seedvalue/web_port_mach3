using UnityEngine.UI;
using Utils;

public class MetaProductView : MetaView
{
	public Text nameText;

	public Text priceText;

	public Text valueText;

	public Image iconImage;

	public new MetaProduct GetObject()
	{
		return base.GetObject() as MetaProduct;
	}

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
		MetaProduct @object = GetObject();
		Helpers.SetText(nameText, @object.displayName);
		Helpers.SetImage(iconImage, @object.icon);
		Helpers.SetText(valueText, @object.value.ToString());
	}

	protected override void OnChanged()
	{
		base.OnChanged();
		MetaProduct @object = GetObject();
		base.interactable = @object.active;
		if (@object.active)
		{
			Helpers.SetText(priceText, @object.priceText);
		}
		else
		{
			Helpers.SetText(priceText, "??");
		}
	}
}
