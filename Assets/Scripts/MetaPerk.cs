using UnityEngine;

[WorkbookLoad("Perks")]
[WorkbookAssetPath("Perks")]
public abstract class MetaPerk : MetaObject
{
	public string displayName;

	public Sprite icon;

	protected bool triggered;

	public virtual void OnM3Start()
	{
		triggered = false;
	}
}
