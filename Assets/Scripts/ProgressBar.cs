using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
	public float progress;

	public Image imageFull;

	private void Update()
	{
		if ((bool)imageFull)
		{
			imageFull.fillAmount = progress;
		}
	}

	public void SetScale(float scale)
	{
		base.transform.localScale = new Vector3(scale, scale, 1f);
	}

	public void SetWorldTransform(Vector3 worldPosition, Quaternion worldRotation)
	{
		base.transform.position = worldPosition;
		base.transform.rotation = worldRotation;
	}

	public void SetProgress(float _progress)
	{
		progress = Mathf.Max(0f, Mathf.Min(1f, _progress));
	}
}
