using UnityEngine;

[RequireComponent(typeof(Light))]
public class DynamicVertexPointLight : MonoBehaviour
{
	private Light m_light;

	private bool m_started;

	private void OnValidate()
	{
		m_light = GetComponent<Light>();
		if (m_light.type != LightType.Point)
		{
			UnityEngine.Debug.LogWarning("Only point lights are supported, changing", this);
			m_light.type = LightType.Point;
		}
		if (m_light.renderMode != LightRenderMode.ForceVertex)
		{
			UnityEngine.Debug.LogWarning("Light needs to be per vertex, changing", this);
			m_light.renderMode = LightRenderMode.ForceVertex;
		}
	}

	private void Awake()
	{
		OnValidate();
	}

	private void Start()
	{
		HeroLightManager.Instance.AddPointLight(m_light);
		m_started = true;
	}

	private void OnEnable()
	{
		if (m_started)
		{
			HeroLightManager.Instance.AddPointLight(m_light);
		}
	}

	private void OnDisable()
	{
		if (m_started)
		{
			HeroLightManager.Instance.RemovePointLight(m_light);
		}
	}
}
