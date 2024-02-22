using UnityEngine;

public class RotateTween : MonoBehaviour
{
	[Header("Effect")]
	[Range(-100f, 100f)]
	public float speed = -15f;

	private Quaternion baseRotation;

	private static readonly Vector3 Axis = new Vector3(0f, 0f, 1f);

	protected void OnEnable()
	{
		baseRotation = base.transform.localRotation;
	}

	protected void OnDisable()
	{
		base.transform.localRotation = baseRotation;
	}

	protected void Update()
	{
		base.transform.Rotate(Axis * speed * Time.deltaTime);
	}
}
