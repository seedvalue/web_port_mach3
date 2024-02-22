using System;
using UnityEngine;

[Serializable]
public struct FloatingTextSpawner
{
	public FloatingText prefab;

	public string text;

	public Vector3 offset;

	public FloatingText Spawn(GameObject go)
	{
		if (!prefab || string.IsNullOrEmpty(text))
		{
			return null;
		}
		FloatingText floatingText = UnityEngine.Object.Instantiate(prefab, go.transform);
		floatingText.text = text;
		floatingText.transform.localScale = prefab.transform.localScale;
		floatingText.transform.localRotation = prefab.transform.localRotation;
		floatingText.transform.localPosition = prefab.transform.localPosition + offset;
		return floatingText;
	}
}
