using System.Collections.Generic;
using UnityEngine;

public class FXBase : MonoBehaviour
{
	public FXPart[] m_PartPrefabs;

	private FXPart[] m_CreatedParts;

	private bool m_IsEndableFromCode;

	private bool m_EndFXEnable;

	public float m_EndFXDelay;

	public float m_StopBeforeEndTime;

	public float m_TimeElapsed;

	private List<int> m_DelayedPartsId;

	private Transform m_ObjectToStart;

	private bool m_awoken;

	public FXPart[] CreatedParts => m_CreatedParts;

	private void Awake()
	{
		if (!m_awoken)
		{
			m_awoken = true;
			m_DelayedPartsId = new List<int>();
		}
	}

	public void StartForObject(Transform obj, bool endable_from_code)
	{
		if (!m_awoken)
		{
			Awake();
		}
		m_ObjectToStart = obj;
		m_IsEndableFromCode = endable_from_code;
		m_CreatedParts = new FXPart[m_PartPrefabs.Length];
		for (int i = 0; i < m_PartPrefabs.Length; i++)
		{
			if (m_PartPrefabs[i] != null)
			{
				if (m_PartPrefabs[i].m_Delay > 0f)
				{
					m_DelayedPartsId.Add(i);
					continue;
				}
				m_CreatedParts[i] = UnityEngine.Object.Instantiate(m_PartPrefabs[i], base.transform.position, base.transform.rotation);
				m_CreatedParts[i].SetOwner(this);
				m_CreatedParts[i].AttachToObject(obj);
			}
			else
			{
				m_CreatedParts[i] = null;
			}
		}
		if (!m_IsEndableFromCode)
		{
			DestroyIfNoPartsLeft();
		}
	}

	public void OnPartDestroyed(FXPart part)
	{
		for (int i = 0; i < m_CreatedParts.Length; i++)
		{
			if (m_CreatedParts[i] == part)
			{
				m_CreatedParts[i] = null;
				break;
			}
		}
		if (!m_IsEndableFromCode)
		{
			DestroyIfNoPartsLeft();
		}
	}

	private void DestroyIfNoPartsLeft()
	{
		if (m_DelayedPartsId.Count != 0)
		{
			return;
		}
		for (int i = 0; i < m_CreatedParts.Length; i++)
		{
			if (m_CreatedParts[i] != null)
			{
				return;
			}
		}
		Object.DestroyObject(base.gameObject);
	}

	private void Update()
	{
		InstantiateDelayed();
		if (m_DelayedPartsId.Count == 0 && m_EndFXEnable)
		{
			m_EndFXDelay -= Time.deltaTime;
			if (m_EndFXDelay < 0f)
			{
				EndFXInternal();
			}
		}
	}

	public void EndFX()
	{
		m_EndFXEnable = true;
		m_EndFXDelay = 0f;
		for (int i = 0; i < m_CreatedParts.Length; i++)
		{
			if (m_CreatedParts[i] != null)
			{
				if (m_CreatedParts[i].m_StopPerioLength > m_EndFXDelay)
				{
					m_EndFXDelay = m_CreatedParts[i].m_StopPerioLength;
				}
				m_CreatedParts[i].StopGeneration();
			}
		}
	}

	public void DetachFromObject()
	{
		for (int i = 0; i < m_CreatedParts.Length; i++)
		{
			if (m_CreatedParts[i] != null)
			{
				m_CreatedParts[i].DetachFromObject();
			}
		}
	}

	private void StopFXInternal()
	{
	}

	private void EndFXInternal()
	{
		for (int i = 0; i < m_CreatedParts.Length; i++)
		{
			if (m_CreatedParts[i] != null)
			{
				m_CreatedParts[i].SetOwner(null);
				Object.DestroyObject(m_CreatedParts[i].gameObject);
			}
		}
		Object.DestroyObject(base.gameObject);
	}

	public FXPart GetPart(int part_idx)
	{
		return m_CreatedParts[part_idx];
	}

	private void InstantiateDelayed()
	{
		if (m_DelayedPartsId.Count == 0)
		{
			return;
		}
		m_TimeElapsed += Time.deltaTime;
		for (int i = 0; i < m_DelayedPartsId.Count; i++)
		{
			int num = m_DelayedPartsId[i];
			if (m_PartPrefabs[num].m_Delay <= m_TimeElapsed)
			{
				m_CreatedParts[num] = UnityEngine.Object.Instantiate(m_PartPrefabs[i], base.transform.position, base.transform.rotation);
				m_CreatedParts[num].SetOwner(this);
				m_CreatedParts[num].AttachToObject(m_ObjectToStart);
				m_DelayedPartsId.RemoveAt(i);
				i--;
			}
		}
	}
}
