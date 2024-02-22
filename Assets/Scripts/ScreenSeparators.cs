using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSeparators : MonoBehaviour
{
	[Header("Separators")]
	public GameObject separatorsRoot;

	public Image separatorPrefab;

	private bool externalSeparators = true;

	[Header("Parallax Effect")]
	public bool parallaxEnabled = true;

	public float parallaxFactor = 100f;

	public Vector3 parallaxAxis = new Vector3(1f, 0f, 0f);

	public float maxAspect = 0.72f;

	private List<Image> createdSeparators;

	private float screenWidth;

	private bool initliazed;

	private float aspectParallaxCorrection;

	public void Initialze()
	{
		if (!separatorsRoot)
		{
			return;
		}
		CalculeteAspectParallaxCorrection();
		int screenCount = ScreenMgr.instance.GetScreenCount();
		createdSeparators = new List<Image>(screenCount);
		for (int i = 0; i < screenCount + 1; i++)
		{
			Image image = null;
			if (((i == 0 || i == screenCount) && externalSeparators) || (i > 0 && i < screenCount))
			{
				image = UnityEngine.Object.Instantiate(separatorPrefab, separatorsRoot.transform);
				image.transform.localScale = Vector3.one;
				image.transform.position = new Vector3(screenWidth * ((float)i - 0.5f), 0f, 0f);
			}
			createdSeparators.Add(image);
		}
		initliazed = true;
	}

	private void Update()
	{
		if (initliazed && parallaxEnabled)
		{
			for (int i = 0; i < createdSeparators.Count; i++)
			{
				Vector3 a = new Vector3(screenWidth * ((float)i - 0.5f), 0f, 0f);
				Vector3 position = Camera.main.transform.position;
				float value = (position.x - a.x) / screenWidth;
				value = Mathf.Clamp(value, -1f, 1f);
				createdSeparators[i].transform.position = a - (parallaxFactor - aspectParallaxCorrection) * parallaxAxis * value;
			}
		}
	}

	private void CalculeteAspectParallaxCorrection()
	{
		float a = (float)Screen.width * 1f / (float)Screen.height;
		a = Mathf.Min(a, maxAspect);
		screenWidth = ScreenMgr.instance.GetScreenWidth();
		float num = a * ScreenMgr.instance.GetScreenHeight();
		aspectParallaxCorrection = (screenWidth - num) * 0.5f;
	}
}
