using UnityEngine;

[ExecuteInEditMode]
public class SortedRenderer : MonoBehaviour
{
	public int sortingOffset;

	private Canvas canvas;

	private Renderer[] renderers;

	private void OnEnable()
	{
		Refresh();
	}

	private void OnDisable()
	{
		canvas = null;
		renderers = null;
	}

	private void Update()
	{
		if (canvas != null && renderers != null)
		{
			Renderer[] array = renderers;
			foreach (Renderer renderer in array)
			{
				renderer.sortingLayerID = canvas.sortingLayerID;
				renderer.sortingOrder = canvas.sortingOrder + sortingOffset;
			}
		}
	}

	private void Refresh()
	{
		canvas = GetComponentInParent<Canvas>();
		renderers = GetComponents<Renderer>();
	}
}
