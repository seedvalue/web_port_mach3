using UnityEngine;

[WorkbookLoad("ItemSlots")]
[WorkbookAssetPath("ItemSlots")]
public class MetaItemSlot : MetaObject
{
	public Sprite icon;

	public MetaItemType acceptedItemType;

	public MetaItem initialItem;

	public const string itemProperty = "item";

	[MetaData(null, 0)]
	private MetaItem _item;

	public MetaItem item
	{
		get
		{
			return _item;
		}
		set
		{
			PropertySetter(ref _item, value, "item");
		}
	}

	protected virtual void MetaAwake()
	{
		_item = initialItem;
	}
}
