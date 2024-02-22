using UnityEngine;

public class AnimatedGlow : MonoBehaviour
{
	public float rotationSpeed;

	public float scaleSpeed;

	public float scaleFactor;

	private void Update()
	{
		Vector3 a = new Vector3(0f, 0f, rotationSpeed);
		base.transform.Rotate(a * Time.deltaTime);
		float num = 1f + Mathf.Sin(Time.time * scaleSpeed) * scaleFactor - scaleFactor;
		base.transform.localScale = new Vector3(num, num, 1f);
	}
}
