using UnityEngine;

public class M3TimerTip : MonoBehaviour
{
	private ProgressBar progressBar;

	private void Start()
	{
		progressBar = GetComponentInParent<ProgressBar>();
	}

	private void Update()
	{
		if ((bool)progressBar)
		{
			float angle = progressBar.progress * 360f;
			base.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
	}
}
