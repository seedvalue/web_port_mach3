using System;

[AttributeUsage(AttributeTargets.Field)]
public class MetaDataAttribute : Attribute
{
	public object DefaultValue
	{
		get;
		private set;
	}

	public int Count
	{
		get;
		private set;
	}

	public MetaDataAttribute(object defaultValue = null, int count = 0)
	{
		DefaultValue = defaultValue;
		Count = count;
	}
}
