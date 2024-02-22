using System.Collections.Generic;
using UnityEngine;

public class M3StageEvent : MonoBehaviour
{
	public float normalizedDistance;

	public List<GameObject> activate;

	public List<GameObject> deactivate;

	public List<GameObject> instantiate;

	private float prevDistance;

	public void UpdateEvents(float stageDistance)
	{
		if (normalizedDistance > prevDistance && normalizedDistance <= stageDistance)
		{
			foreach (GameObject item in activate)
			{
				item.SetActive(value: true);
			}
			foreach (GameObject item2 in deactivate)
			{
				item2.SetActive(value: false);
			}
			foreach (GameObject item3 in instantiate)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(item3, base.transform.position, base.transform.rotation);
				gameObject.transform.localScale = base.transform.localScale;
			}
		}
		prevDistance = stageDistance;
	}
}
