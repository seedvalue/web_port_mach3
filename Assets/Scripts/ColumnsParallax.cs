using UnityEngine;

public class ColumnsParallax : MonoBehaviour
{
	public GameObject UICamera;

	private void Update()
	{
		base.transform.position = UICamera.transform.position * -1f / 2f;
	}
}
