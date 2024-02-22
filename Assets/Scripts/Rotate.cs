using UnityEngine;

public class Rotate : MonoBehaviour
{
	public Vector3 angle = new Vector3(0f, 45f, 0f);

	private void Update()
	{
		base.transform.Rotate(angle * Time.deltaTime);
	}
}
