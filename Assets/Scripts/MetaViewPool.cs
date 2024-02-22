using System.Collections.Generic;
using UnityEngine;

public class MetaViewPool : MonoBehaviour
{
	public MetaView prefab;

	private readonly Stack<MetaView> inactiveInstances = new Stack<MetaView>();

	public MetaView GetObject(GameObject parent)
	{
		MetaView metaView;
		if (inactiveInstances.Count > 0)
		{
			metaView = inactiveInstances.Pop();
		}
		else
		{
			metaView = UnityEngine.Object.Instantiate(prefab);
			MetaViewPoolObject metaViewPoolObject = metaView.gameObject.AddComponent<MetaViewPoolObject>();
			metaViewPoolObject.pool = this;
		}
		metaView.transform.SetParent(parent.transform);
		metaView.gameObject.SetActive(value: true);
		return metaView;
	}

	public void ReturnObject(GameObject toReturn)
	{
		MetaView component = toReturn.GetComponent<MetaView>();
		if (component == null)
		{
			UnityEngine.Debug.LogWarning(toReturn.name + " was returned to a pool it wasn't spawned from! Destroying.");
			UnityEngine.Object.Destroy(toReturn);
		}
		else
		{
			ReturnObject(component);
		}
	}

	public void ReturnObject(MetaView toReturn)
	{
		MetaViewPoolObject component = toReturn.GetComponent<MetaViewPoolObject>();
		if (component != null && component.pool == this)
		{
			toReturn.transform.SetParent(base.transform);
			toReturn.gameObject.SetActive(value: false);
			toReturn.SetObject(null);
			inactiveInstances.Push(toReturn);
		}
		else
		{
			UnityEngine.Debug.LogWarning(toReturn.name + " was returned to a pool it wasn't spawned from! Destroying.");
			UnityEngine.Object.Destroy(toReturn);
		}
	}
}
