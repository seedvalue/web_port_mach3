using UnityEngine;

[RequireComponent(typeof(Renderer))]
[ExecuteInEditMode]
public class HeroLightRenderer : MonoBehaviour
{
	public HeroLightManager manager
	{
		get;
		private set;
	}

	public Renderer renderer
	{
		get;
		private set;
	}

	private void Start()
	{
		manager = GetComponentInParent<HeroLightManager>();
		renderer = GetComponent<Renderer>();
		if ((bool)manager)
		{
			manager.AddRenderer(this);
		}
	}

	private void OnDestroy()
	{
		if ((bool)manager)
		{
			manager.RemoveRenderer(this);
		}
	}
}
