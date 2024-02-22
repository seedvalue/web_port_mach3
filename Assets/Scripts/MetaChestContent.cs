using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChestContent", menuName = "Meta/Chest Content")]
public class MetaChestContent : MetaObject
{
	[Serializable]
	private class Entry
	{
		public MetaObject metaObject;

		public int count = 1;
	}

	[SerializeField]
	private Entry[] entries;

	public List<MetaLink> content
	{
		get;
		private set;
	}

	protected virtual void MetaAwake()
	{
		content = new List<MetaLink>();
		Entry[] array = entries;
		foreach (Entry entry in array)
		{
			MetaLink metaLink = new MetaLink(entry.metaObject);
			metaLink.SetProperty("count", entry.count);
			content.Add(metaLink);
		}
	}
}
