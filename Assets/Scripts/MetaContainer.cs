using System.Collections.Generic;
using UnityEngine;

public class MetaContainer : MonoBehaviour
{
	private readonly List<MetaView> _contents = new List<MetaView>();

	private readonly List<MetaCacheable> _cache = new List<MetaCacheable>();

	public bool cacheEnabled = true;

	public List<MetaView> contents => _contents;

	public void Assign(MetaObject metaObject, MetaView prefab)
	{
		Clear();
		Add(metaObject, prefab);
	}

	public void Assign<T>(List<T> metaObjects, MetaView prefab) where T : MetaObject
	{
		Clear();
		Add(metaObjects, prefab);
	}

	public void Assign<T>(T[] metaObjects, MetaView prefab) where T : MetaObject
	{
		Clear();
		Add(metaObjects, prefab);
	}

	public void Assign(MetaLink metaLink, MetaView prefab)
	{
		Clear();
		Add(metaLink, prefab);
	}

	public void Assign(List<MetaLink> metaLinks, MetaView prefab)
	{
		Clear();
		Add(metaLinks, prefab);
	}

	public void Assign<T>(MetaLink[] metaLinks, MetaView prefab)
	{
		Clear();
		Add(metaLinks, prefab);
	}

	public void Add(MetaObject metaObject, MetaView prefab)
	{
		MetaView metaView = CreateView(metaObject, prefab);
		metaView.transform.SetSiblingIndex(_contents.Count);
		_contents.Add(metaView);
	}

	public void Add<T>(List<T> metaObjects, MetaView prefab) where T : MetaObject
	{
		for (int i = 0; i < metaObjects.Count; i++)
		{
			Add(metaObjects[i], prefab);
		}
	}

	public void Add<T>(T[] metaObjects, MetaView prefab) where T : MetaObject
	{
		for (int i = 0; i < metaObjects.Length; i++)
		{
			Add(metaObjects[i], prefab);
		}
	}

	public void Add(MetaLink metaLink, MetaView prefab)
	{
		MetaView metaView = CreateView(metaLink, prefab);
		metaView.transform.SetSiblingIndex(_contents.Count);
		_contents.Add(metaView);
	}

	public void Add(List<MetaLink> metaLinks, MetaView prefab)
	{
		for (int i = 0; i < metaLinks.Count; i++)
		{
			Add(metaLinks[i], prefab);
		}
	}

	public void Add(MetaLink[] metaLinks, MetaView prefab)
	{
		for (int i = 0; i < metaLinks.Length; i++)
		{
			Add(metaLinks[i], prefab);
		}
	}

	public void Clear()
	{
		for (int i = 0; i < _contents.Count; i++)
		{
			ReturnView(_contents[i]);
		}
		_contents.Clear();
	}

	protected MetaView CreateView(MetaObject metaObject, MetaView prefab)
	{
		MetaView metaView = CachePop(prefab);
		if (metaView == null)
		{
			metaView = UnityEngine.Object.Instantiate(prefab, base.transform);
			MetaCacheable metaCacheable = metaView.gameObject.AddComponent<MetaCacheable>();
			metaCacheable.prefab = prefab;
			metaCacheable.thisView = metaView;
		}
		MetaView[] components = metaView.GetComponents<MetaView>();
		MetaView[] array = components;
		foreach (MetaView metaView2 in array)
		{
			metaView2.SetObject(metaObject);
		}
		metaView.transform.localScale = Vector3.one;
		return metaView;
	}

	protected MetaView CreateView(MetaLink metaLink, MetaView prefab)
	{
		MetaView metaView = CachePop(prefab);
		if (metaView == null)
		{
			metaView = UnityEngine.Object.Instantiate(prefab, base.transform);
			MetaCacheable metaCacheable = metaView.gameObject.AddComponent<MetaCacheable>();
			metaCacheable.prefab = prefab;
			metaCacheable.thisView = metaView;
		}
		MetaView[] components = metaView.GetComponents<MetaView>();
		MetaView[] array = components;
		foreach (MetaView metaView2 in array)
		{
			metaView2.SetLink(metaLink);
		}
		metaView.transform.localScale = Vector3.one;
		return metaView;
	}

	protected void ReturnView(MetaView metaView)
	{
		MetaView[] components = metaView.GetComponents<MetaView>();
		MetaView[] array = components;
		foreach (MetaView metaView2 in array)
		{
			metaView2.SetObject(null);
		}
		MetaCacheable component = metaView.GetComponent<MetaCacheable>();
		if (!component || !cacheEnabled)
		{
			Object.DestroyObject(metaView.gameObject);
		}
		else
		{
			_cache.Add(component);
		}
	}

	protected MetaView CachePop(MetaView prefab)
	{
		int num = _cache.FindIndex((MetaCacheable c) => c.prefab == prefab);
		if (num < 0)
		{
			return null;
		}
		MetaCacheable metaCacheable = _cache[num];
		_cache.RemoveAt(num);
		metaCacheable.gameObject.SetActive(value: true);
		return metaCacheable.thisView;
	}

	protected void LateUpdate()
	{
		foreach (MetaCacheable item in _cache)
		{
			item.gameObject.SetActive(value: false);
		}
	}
}
