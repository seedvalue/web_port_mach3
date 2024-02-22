using UnityEngine;

[RequireComponent(typeof(Canvas), typeof(RectTransform))]
[ExecuteInEditMode]
public class CanvasWorldOverlay : MonoBehaviour
{
	private Canvas canvas;

	private RectTransform rectTransform;

	private DrivenRectTransformTracker rectTracker;

	private void OnEnable()
	{
		canvas = GetComponent<Canvas>();
		rectTransform = GetComponent<RectTransform>();
		Adjust();
	}

	private void OnDisable()
	{
		rectTracker.Clear();
	}

	private void LateUpdate()
	{
		Adjust();
	}

	private void Adjust()
	{
		if ((bool)Camera.main && CheckConditions(Camera.main))
		{
			AdjustToCamera(Camera.main);
		}
		else
		{
			rectTracker.Clear();
		}
	}

	private void AdjustToCamera(Camera camera)
	{
		rectTracker.Clear();
		rectTracker.Add(this, rectTransform, DrivenTransformProperties.All);
		Transform transform = camera.transform;
		RectTransform obj = rectTransform;
		Vector3 position = transform.position;
		float x = position.x;
		Vector3 position2 = transform.position;
		float y = position2.y;
		Vector3 position3 = rectTransform.position;
		obj.position = new Vector3(x, y, position3.z);
		rectTransform.sizeDelta = new Vector2(camera.orthographicSize * 2f * camera.aspect, camera.orthographicSize * 2f);
		rectTransform.localScale = Vector3.one;
		rectTransform.anchorMin = Vector2.one * 0.5f;
		rectTransform.anchorMax = Vector2.one * 0.5f;
		rectTransform.pivot = Vector2.one * 0.5f;
	}

	private bool CheckConditions(Camera camera)
	{
		if (canvas.renderMode != RenderMode.WorldSpace)
		{
			return false;
		}
		if (!camera.orthographic)
		{
			return false;
		}
		return true;
	}
}
