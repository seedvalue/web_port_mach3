using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class KamisPitFogOverride : MonoBehaviour
{
	[Range(0f, 1f)]
	public float PitFogIntesity;

	private Renderer affectedRenderer;

	protected virtual void Start()
	{
		affectedRenderer = GetComponent<Renderer>();
		Refresh();
	}

	protected virtual void Update()
	{
	}

	private void Refresh()
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetFloat("Kamis_FogPitIntensity", PitFogIntesity);
		affectedRenderer.SetPropertyBlock(materialPropertyBlock);
	}
}
