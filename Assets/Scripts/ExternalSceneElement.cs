using UnityEngine;
using UnityEngine.EventSystems;

public class ExternalSceneElement : MonoBehaviour
{
	public bool worldPositionStays;

	public bool loadAsRoot;

	private bool standalone = true;

	public void OnLoaded()
	{
		standalone = false;
	}

	protected virtual void Start()
	{
		if (standalone && EventSystem.current == null)
		{
			EventSystem eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
			if (eventSystem != null)
			{
				eventSystem.enabled = true;
			}
		}
	}
}
