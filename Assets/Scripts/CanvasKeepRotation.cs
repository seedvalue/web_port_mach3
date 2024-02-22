using UnityEngine;

public class CanvasKeepRotation : MonoBehaviour
{
	private Transform canvasTransform;

	private void Start()
	{
		Canvas componentInParent = GetComponentInParent<Canvas>();
		if (!componentInParent)
		{
			base.enabled = false;
			return;
		}
		componentInParent = componentInParent.rootCanvas;
		canvasTransform = componentInParent.transform;
		if ((bool)componentInParent.worldCamera)
		{
			canvasTransform = componentInParent.worldCamera.transform;
		}
	}

	private void Update()
	{
		if ((bool)canvasTransform)
		{
			base.transform.rotation = canvasTransform.rotation;
		}
	}
}
