using System.Collections.Generic;
using UnityEngine;

public class FXPart : MonoBehaviour
{
	public string m_AttachToObjectName = string.Empty;

	public Vector3 m_AttachLocalPos = Vector3.zero;

	public Vector3 m_AttachLocalRot = Vector3.zero;

	public Vector3 m_AttachLocalScale = Vector3.zero;

	public bool m_IgnoreRotationWhenAttaching;

	public bool m_KeepConstantWorldRotation;

	public Vector3 m_ConstantWorldRotationToKeep = Vector3.zero;

	public bool m_CalculateAttachParams;

	public bool m_GetNameOnly;

	public bool m_DestroyAfterTime;

	public float m_TimeTillDestroy = 1f;

	public float m_StopPerioLength = -1f;

	private float m_timeSinceStop;

	private bool m_stopped;

	public float m_Delay;

	private FXBase m_Owner;

	public Renderer m_Renderer1;

	public bool m_DoNotAttachToParent;

	public ParticleSystem[] m_specialParticleSystemsToFade;

	private static ParticleSystem.Particle[] m_particles = new ParticleSystem.Particle[50];

	private void LateUpdate()
	{
		if (m_KeepConstantWorldRotation)
		{
			base.transform.rotation = Quaternion.Euler(m_ConstantWorldRotationToKeep);
		}
		if (m_stopped)
		{
			UpdateStopped();
		}
		if (m_DestroyAfterTime)
		{
			m_TimeTillDestroy -= Time.deltaTime;
			if (m_TimeTillDestroy < 0f)
			{
				Object.DestroyObject(base.gameObject);
			}
			else if (!m_stopped && m_TimeTillDestroy <= m_StopPerioLength)
			{
				StopGeneration();
			}
		}
	}

	private void OnDestroy()
	{
		if (m_Owner != null)
		{
			m_Owner.OnPartDestroyed(this);
		}
	}

	public void SetOwner(FXBase owner)
	{
		m_Owner = owner;
	}

	public void AttachToObject(Transform obj)
	{
		Transform transform = obj.Find(m_AttachToObjectName);
		if (transform == null)
		{
			transform = obj.Find(GetBonePathNoFirstBone(m_AttachToObjectName));
			if (transform == null)
			{
				transform = FindBoneInHierarchy(0, obj, m_AttachToObjectName);
			}
			if (transform == null)
			{
				transform = obj;
			}
		}
		base.transform.parent = transform;
		base.transform.localPosition = m_AttachLocalPos;
		if (!m_IgnoreRotationWhenAttaching)
		{
			base.transform.localRotation = Quaternion.Euler(m_AttachLocalRot);
		}
		base.transform.localScale = m_AttachLocalScale;
		if (m_DoNotAttachToParent)
		{
			base.transform.parent = null;
		}
		base.enabled = (m_DestroyAfterTime || m_KeepConstantWorldRotation);
	}

	private void OnDrawGizmosSelected()
	{
		if (m_CalculateAttachParams)
		{
			m_AttachToObjectName = GetParentNamePath();
			m_AttachLocalPos = base.transform.localPosition;
			m_AttachLocalRot = base.transform.localRotation.eulerAngles;
			m_AttachLocalScale = base.transform.localScale;
			m_CalculateAttachParams = false;
		}
		if (m_GetNameOnly)
		{
			m_AttachToObjectName = GetParentNamePath();
			m_GetNameOnly = false;
		}
	}

	private string GetParentNamePath()
	{
		List<string> list = new List<string>(5);
		Transform transform = base.transform;
		while (transform.parent != null)
		{
			list.Add(transform.parent.name);
			transform = transform.parent;
		}
		if (list.Count == 0)
		{
			return string.Empty;
		}
		string str = string.Empty;
		for (int num = list.Count - 2; num > 0; num--)
		{
			str = str + list[num] + "/";
		}
		return str + list[0];
	}

	public static string GetFullBonePath(Transform bone, Transform stopAtParent)
	{
		if (bone == stopAtParent)
		{
			return string.Empty;
		}
		List<string> list = new List<string>(5);
		Transform transform = bone;
		while (transform != null && !(transform == stopAtParent))
		{
			list.Add(transform.name);
			transform = transform.parent;
		}
		string str = string.Empty;
		for (int num = list.Count - 1; num > 0; num--)
		{
			str = str + list[num] + "/";
		}
		return str + list[0];
	}

	public static string GetParentNamePath(Transform bone)
	{
		List<string> list = new List<string>(5);
		Transform transform = bone;
		while (transform.parent != null)
		{
			list.Add(transform.parent.name);
			transform = transform.parent;
		}
		if (list.Count <= 1)
		{
			return string.Empty;
		}
		string str = string.Empty;
		for (int num = list.Count - 2; num > 0; num--)
		{
			str = str + list[num] + "/";
		}
		return str + list[0];
	}

	public void DetachFromObject()
	{
		base.transform.parent = null;
	}

	public static string GetBoneNamePath(Transform bone, Transform stopAtBone)
	{
		return GetFullBonePath(bone, stopAtBone);
	}

	public static string GetBoneNamePath(Transform bone)
	{
		string parentNamePath = GetParentNamePath(bone);
		if (parentNamePath.Length > 0)
		{
			return parentNamePath + "/" + bone.name;
		}
		return bone.name;
	}

	private Transform FindBoneInHierarchy(int recurrence_idx, Transform curr_parent, string current_path)
	{
		if (recurrence_idx > 3)
		{
			return null;
		}
		string bonePathNoFirstBone = GetBonePathNoFirstBone(current_path);
		if (bonePathNoFirstBone.Length == 0)
		{
			return null;
		}
		int childCount = curr_parent.childCount;
		for (int i = 0; i < childCount; i++)
		{
			Transform transform = curr_parent.GetChild(i).Find(current_path);
			if (transform != null)
			{
				return transform;
			}
		}
		for (int j = 0; j < childCount; j++)
		{
			Transform child = curr_parent.GetChild(j);
			Transform transform2 = FindBoneInHierarchy(recurrence_idx + 1, child, bonePathNoFirstBone);
			if (transform2 != null)
			{
				return transform2;
			}
		}
		return null;
	}

	private string GetBonePathNoFirstBone(string bone_path)
	{
		int num = bone_path.IndexOf('/');
		if (num < 0 || num >= bone_path.Length)
		{
			return string.Empty;
		}
		return bone_path.Substring(num + 1);
	}

	public void StopGeneration()
	{
		if (m_stopped)
		{
			return;
		}
		m_stopped = true;
		m_timeSinceStop = 0f;
		if (m_StopPerioLength < 0f)
		{
			return;
		}
		ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			bool flag = false;
			for (int j = 0; j < m_specialParticleSystemsToFade.Length; j++)
			{
				if (m_specialParticleSystemsToFade[j] == componentsInChildren[i])
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				//componentsInChildren[i].emission.enabled = true;
				var emission = componentsInChildren[i].emission;
				emission.enabled = true;

            }
		}
		base.enabled = true;
	}

	public bool DidStopProcessFinished()
	{
		return m_timeSinceStop > m_StopPerioLength;
	}

	private void UpdateStopped()
	{
		if (!DidStopProcessFinished())
		{
			m_timeSinceStop += Time.deltaTime;
			float num = m_timeSinceStop / m_StopPerioLength;
			if (num > 1f)
			{
				num = 1f;
			}
			UpdateStopSpecialParticles(num);
		}
	}

	private void UpdateStopSpecialParticles(float currStopProgress)
	{
		float num = 1f - currStopProgress;
		for (int i = 0; i < m_specialParticleSystemsToFade.Length; i++)
		{
			int particles = m_specialParticleSystemsToFade[i].GetParticles(m_particles);
			for (int j = 0; j < particles; j++)
			{
				Color32 startColor = m_particles[j].startColor;
				startColor.a = (byte)((float)(int)startColor.a * num);
				m_particles[j].startColor = startColor;
			}
			m_specialParticleSystemsToFade[i].SetParticles(m_particles, particles);
		}
	}
}
