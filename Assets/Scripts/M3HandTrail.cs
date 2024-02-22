using System.Collections.Generic;
using UnityEngine;

public class M3HandTrail : MonoBehaviour, IM3FakeDrag, IM3TutorialAnimationClient
{
	public GameObject trailFxPrefab;

	public Transform trailSpawner;

	private GameObject trailFx;

	private List<ParticleSystem> particles = new List<ParticleSystem>();

	private bool active;

	public void ActivateTrail()
	{
		if ((bool)trailFxPrefab && (bool)trailSpawner && active)
		{
			trailFx = UnityEngine.Object.Instantiate(trailFxPrefab, trailSpawner, worldPositionStays: false);
			GetComponentsInChildren(particles);
			ToggleEmission(enabled: true);
			M3Board.instance.FakeDragStart(this);
		}
	}

	public void DeactivateTrail()
	{
		if ((bool)trailFxPrefab && (bool)trailSpawner && active)
		{
			ToggleEmission(enabled: false);
			if ((bool)trailFx)
			{
				trailFx.transform.SetParent(null, worldPositionStays: true);
				trailFx.GetComponent<M3SelfDestructor>().Detonate();
			}
		}
	}

	public void FakeDragEnd()
	{
		M3Board.instance.FakeDragEnd(instantRollback: false);
	}

	private void ToggleEmission(bool enabled)
	{
		foreach (ParticleSystem particle in particles)
		{
			//particle.emission.enabled = enabled;
			var emission = particle.emission;
			emission.enabled = enabled;
		}
	}

	public Vector3 GetPointerPos()
	{
		return (!trailFx) ? Vector3.zero : trailFx.transform.position;
	}

	public void AfterFadeIn()
	{
		active = true;
	}

	public void BeforeFadeOut()
	{
		active = false;
	}
}
