using System.Collections.Generic;
using UnityEngine;

[WorkbookLoad("Mobs")]
[WorkbookAssetPath("Mobs")]
public class MetaMob : MetaObject
{
	public string displayName;

	public float meleePriority;

	public GameObject prefab;

	[WorkbookFlat]
	public MobStats baseStats;

	public List<MetaPerk> perks;
}
