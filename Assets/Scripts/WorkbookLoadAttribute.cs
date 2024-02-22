using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
public class WorkbookLoadAttribute : Attribute
{
	public string sheet
	{
		get;
		private set;
	}

	public WorkbookLoadAttribute(string sheet)
	{
		this.sheet = sheet;
	}
}
