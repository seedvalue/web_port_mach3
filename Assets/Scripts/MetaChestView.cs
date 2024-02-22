public class MetaChestView : MetaView
{
	public new MetaChest GetObject()
	{
		return base.GetObject() as MetaChest;
	}

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
	}

	protected override void OnChanged()
	{
		base.OnChanged();
	}
}
