using System.Collections.Generic;
using UnityEngine;

public class ScreenRoot : MonoBehaviour
{
	[Header("Common")]
	public Canvas uiCanvas;

	public GameObject sceneRoot3D;

	public List<ScreenCamara> camera3D;

	[Header("Clipping")]
	public bool clippingEnabled = true;

	public float clippingTolerance = 0.95f;

	[Header("Parallax Effect")]
	public bool parallaxEnabled = true;

	public float parallaxFactor = 10f;

	public Vector3 parallaxAxis = new Vector3(1f, 0f, 0f);

	private RectTransform uiCanvasRect;

	private List<Vector3> parallaxCameraStartPos;

	private void Start()
	{
		parallaxCameraStartPos = new List<Vector3>();
		foreach (ScreenCamara item in camera3D)
		{
			parallaxCameraStartPos.Add(item.transform.localPosition);
		}
		if ((bool)uiCanvas)
		{
			uiCanvasRect = uiCanvas.GetComponent<RectTransform>();
		}
	}

	private void LateUpdate()
	{
		if (UpdateClipping())
		{
			float screenWidth = ScreenMgr.instance.GetScreenWidth();
			Vector3 position = Camera.main.transform.position;
			float x = position.x;
			int num = Mathf.RoundToInt(x / screenWidth);
			float num2 = x / screenWidth - (float)num;
			Vector3 position2 = Camera.main.transform.position;
			float x2 = position2.x;
			Vector3 position3 = uiCanvasRect.transform.position;
			num2 = (x2 - position3.x) / screenWidth;
			num2 = Mathf.Clamp(num2, -1f, 1f);
			UpdateParallax(num2);
			UpdateViewport(num2);
		}
	}

	private bool UpdateClipping()
	{
		if (!sceneRoot3D)
		{
			return true;
		}
		if (!clippingEnabled)
		{
			sceneRoot3D.gameObject.SetActive(value: true);
			return true;
		}
		Vector2 sizeDelta = uiCanvasRect.sizeDelta;
		float num = sizeDelta.x * clippingTolerance;
		if ((Camera.main.transform.position - uiCanvasRect.transform.position).sqrMagnitude > num * num)
		{
			sceneRoot3D.gameObject.SetActive(value: false);
			return false;
		}
		sceneRoot3D.gameObject.SetActive(value: true);
		return true;
	}

	private void UpdateParallax(float parallaxOffset)
	{
		if (camera3D.Count != parallaxCameraStartPos.Count)
		{
			UnityEngine.Debug.LogError("Powinno byc tyle samo elemetów");
		}
		else
		{
			if (!parallaxEnabled)
			{
				return;
			}
			for (int i = 0; i < camera3D.Count; i++)
			{
				if ((bool)camera3D[i])
				{
					camera3D[i].transform.localPosition = parallaxCameraStartPos[i] + parallaxFactor * parallaxOffset * parallaxAxis;
				}
			}
		}
	}

	private void UpdateViewport(float offset)
	{
		foreach (ScreenCamara item in camera3D)
		{
			if ((bool)item)
			{
				float num = (float)Screen.width * 1f / (float)Screen.height;
				float num2 = num;
				if ((bool)ScreenMgr.instance)
				{
					num2 = Mathf.Max(ScreenMgr.instance.minAspect, num);
				}
				item.oversize = num2 / num;
				item.offset = offset;
			}
		}
	}
}
