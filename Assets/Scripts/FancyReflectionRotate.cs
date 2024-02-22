using UnityEngine;

public class FancyReflectionRotate : MonoBehaviour
{
	public Vector3 baseAngle;

	public Vector3 speed;

	private Vector3 currentAngle;

	private Renderer affectedRenderer;

	protected void Start()
	{
		affectedRenderer = GetComponent<Renderer>();
	}

	private float NormalizeAngle(float angle)
	{
		return angle % 360f + ((!(angle < 0f)) ? 0f : 360f);
	}

	protected void Update()
	{
		currentAngle += speed * Time.deltaTime;
		currentAngle.x = NormalizeAngle(currentAngle.x);
		currentAngle.y = NormalizeAngle(currentAngle.y);
		currentAngle.z = NormalizeAngle(currentAngle.z);
		Quaternion q = Quaternion.Euler(baseAngle + currentAngle);
		Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
		Material[] materials = affectedRenderer.materials;
		foreach (Material material in materials)
		{
			material.SetMatrix("Fancy_ReflectionRotation", matrix);
		}
	}
}
