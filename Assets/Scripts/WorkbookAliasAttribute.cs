using System;

[AttributeUsage(AttributeTargets.Field)]
public class WorkbookAliasAttribute : Attribute
{
	public string name
	{
		get;
		private set;
	}

	public WorkbookAliasAttribute(string name)
	{
		this.name = name;
	}
}
