using System.Collections;
using UnityEngine;

[WorkbookLoad("Skills")]
[WorkbookAssetPath("Skills")]
public abstract class MetaSkill : MetaObject
{
	public string displayName;

	public string description;

	public Sprite icon;

	public int cooldown;

	[WorkbookAlias("Sound")]
	public AudioSample sfxSample;

	[WorkbookAlias("FxPrefab")]
	public GameObject fxPrefab;

	[WorkbookAlias("Duration")]
	public float duration;

	private M3TileManager tileManager;

	public abstract IEnumerator Execute(M3Battle battle, M3Board board, M3Player player);

	public Coroutine StartCoroutine(IEnumerator routine)
	{
		return M3TileManager.instance.StartCoroutine(routine);
	}
}
