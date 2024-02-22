using UnityEngine;

public class IndicatorAnchor : MonoBehaviour
{
	[SerializeField]
	private Vector3 offset = Vector3.zero;

	public Vector3 position => base.transform.position + offset;
}
