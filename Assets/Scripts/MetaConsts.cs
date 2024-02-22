using UnityEngine;

public class MetaConsts : MetaComponent<MetaConsts>
{
	public Sprite[] bkgByRarity = new Sprite[4];

	public Sprite[] bkgLvlByRarity = new Sprite[4];

	public Color[] textColorByRarity = new Color[4];

	public Sprite[] iconSlotIcon = new Sprite[8];

	public Sprite questionMarkIcon;

	[Header("Balance")]
	public float[] elitarismByRarity = new float[4];

	[Header("Unlock Levels")]
	public int shopUnlockLevel;

	public int pvpUnlockLevel;

	public int guildsUnlockLevel;
}
