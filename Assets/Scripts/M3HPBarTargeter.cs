using UnityEngine;
using UnityEngine.UI;

public class M3HPBarTargeter : MonoBehaviour
{
	public Image arrowLeft;

	public Image arrowRight;

	private void Start()
	{
		ToggleArrows(visible: false);
	}

	public void ToggleArrows(bool visible)
	{
		if ((bool)arrowLeft)
		{
			arrowLeft.enabled = visible;
		}
		if ((bool)arrowRight)
		{
			arrowRight.enabled = visible;
		}
	}
}
