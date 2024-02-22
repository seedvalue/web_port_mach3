using UnityEngine;

[ExecuteInEditMode]
public class EditorSimulateParticlesOnEnable : MonoBehaviour
{
	private void OnEnable()
	{
		if (Application.isEditor)
		{
			ParticleSystem[] componentsInChildren = base.gameObject.GetComponentsInChildren<ParticleSystem>();
			ParticleSystem[] array = componentsInChildren;
			foreach (ParticleSystem particleSystem in array)
			{
				particleSystem.Play();
			}
		}
	}
}
