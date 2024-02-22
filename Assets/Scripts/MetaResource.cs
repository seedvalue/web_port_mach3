using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

[WorkbookLoad("Resources")]
[WorkbookAssetPath("Res")]
public class MetaResource : MetaObject, IAnalyticsItem
{
	public const string countProperty = "count";

	public string displayName;

	public Sprite icon;

	public Sprite cardIcon;

	public int initialCount;

	public MetaResourceType type;

	[MetaData(null, 0)]
	private int _count;

	[MetaData(null, 0)]
	private int _spent;

	public static MetaResource coins
	{
		get;
		private set;
	}

	public static MetaResource gems
	{
		get;
		private set;
	}

	public static MetaResource exp
	{
		get;
		private set;
	}

	public int count
	{
		get
		{
			return _count;
		}
		set
		{
			PropertySetter(ref _count, value, "count");
		}
	}

	public int spent => _spent;

	public string quadText => "<quad name=Meta/Res/" + base.name + " prop=icon>";

	public string analyticsID
	{
		get;
		private set;
	}

	public string analyticsType
	{
		get;
		private set;
	}

	protected virtual void Awake()
	{
		analyticsID = AnalyticsManager.ResolveID(base.metaID);
		analyticsType = AnalyticsManager.ResolveType(base.metaID);
	}

	protected static void MetaStaticAwake()
	{
		IEnumerable<IGrouping<MetaResourceType, MetaResource>> enumerable = from r in Singleton<Meta>.Instance.FindObjects<MetaResource>()
			group r by r.type;
		foreach (IGrouping<MetaResourceType, MetaResource> item in enumerable)
		{
			if (item.Key == MetaResourceType.Coins)
			{
				if (item.Count() > 1)
				{
					throw new ArgumentException($"Found {item.Count()} MetaResources of type '{item.Key}'!");
				}
				coins = item.FirstOrDefault();
			}
			else if (item.Key == MetaResourceType.Gems)
			{
				if (item.Count() > 1)
				{
					throw new ArgumentException($"Found {item.Count()} MetaResources of type '{item.Key}'!");
				}
				gems = item.FirstOrDefault();
			}
			else if (item.Key == MetaResourceType.Exp)
			{
				if (item.Count() > 1)
				{
					throw new ArgumentException($"Found {item.Count()} MetaResources of type '{item.Key}'!");
				}
				exp = item.FirstOrDefault();
			}
		}
	}

	protected virtual void MetaAwake()
	{
		_count = initialCount;
	}

	protected override void OnPropertyChanged(string propertyName, object before, object after)
	{
		base.OnPropertyChanged(propertyName, before, after);
		if (propertyName == "count")
		{
			int num = (int)after - (int)before;
			if (num < 0)
			{
				_spent += -num;
			}
		}
	}
}
