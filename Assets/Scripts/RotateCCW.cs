using UnityEngine;

public class RotateCCW : MonoBehaviour
{
	private void Update()
	{
		base.transform.Rotate(new Vector3(0f, -45f, 0f) * Time.deltaTime);
	}
}
