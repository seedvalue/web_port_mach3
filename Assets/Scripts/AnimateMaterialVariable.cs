using UnityEngine;

public class AnimateMaterialVariable : MonoBehaviour
{
	public enum Type
	{
		Float,
		ColorAlpha
	}

	public string m_AnimationEventName;

	public Type m_Type;

	public string m_VariableName = "_no_name";

	public int m_MaterialIndex;

	public AnimationCurve m_AnimationCurve;

	private float m_CurrTime;

	public float m_TimePeriodLength = 1f;

	public Renderer m_renderer;

	private Material m_material;

	private bool m_Active = true;

	private void Start()
	{
		m_Active = string.IsNullOrEmpty(m_AnimationEventName);
		if (m_renderer == null)
		{
			m_renderer = GetComponent<Renderer>();
		}
		if (m_renderer == null)
		{
			base.enabled = false;
			return;
		}
		m_material = m_renderer.materials[m_MaterialIndex];
		if (m_Active)
		{
			UpdateMaterial();
		}
	}

	public void Update()
	{
		if (!(m_material == null) && m_Active)
		{
			m_CurrTime += Time.deltaTime / m_TimePeriodLength;
			UpdateMaterial();
			if (!string.IsNullOrEmpty(m_AnimationEventName))
			{
				m_Active = (m_CurrTime < m_AnimationCurve.MaxTime());
			}
		}
	}

	private void UpdateMaterial()
	{
		float num = m_AnimationCurve.Evaluate(m_CurrTime);
		Type type = m_Type;
		if (type == Type.ColorAlpha)
		{
			Color color = m_material.GetColor(m_VariableName);
			color.a = num;
			m_material.SetColor(m_VariableName, color);
		}
		else
		{
			m_material.SetFloat(m_VariableName, num);
		}
	}

	public void OnAnimationEvent(string name)
	{
		if (!string.IsNullOrEmpty(m_AnimationEventName) && !(m_AnimationEventName != name))
		{
			m_CurrTime = 0f;
			m_Active = true;
		}
	}
}
