using UnityEngine;

public class MetaViewSpawner : MonoBehaviour
{
	public MetaView prefab;

	public MetaObject initialObject;

	public bool resetPosition = true;

	public bool resetRotation = true;

	public bool resetScale = true;

	private void Awake()
	{
		if ((bool)prefab)
		{
			MetaView metaView = UnityEngine.Object.Instantiate(prefab, base.transform);
			metaView.transform.localScale = ((!resetScale) ? prefab.transform.localScale : Vector3.one);
			metaView.transform.localRotation = ((!resetRotation) ? prefab.transform.localRotation : Quaternion.identity);
			metaView.transform.localPosition = ((!resetPosition) ? prefab.transform.localPosition : Vector3.zero);
			metaView.SetObject(initialObject);
		}
	}
}
