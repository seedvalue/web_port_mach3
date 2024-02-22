using UnityEngine;
using Utils;

public class HoverTween : MonoBehaviour
{
	[Header("Effect")]
	public Vector3 movement = new Vector3(0f, 10f, 0f);

	[Range(0f, 10f)]
	public float speed = 0.5f;

	private Vector3 basePosition;

	protected void OnEnable()
	{
		basePosition = base.transform.localPosition;
	}

	protected void OnDisable()
	{
		base.transform.localPosition = basePosition;
	}

	protected void Update()
	{
		float t = Mathf.PingPong(Time.time * speed, 1f);
		t = Helpers.simple_smooth_step(t);
		base.transform.localPosition = Vector3.Lerp(basePosition, basePosition + movement, t);
	}
}
