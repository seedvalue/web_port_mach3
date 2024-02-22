using System;

[AttributeUsage(AttributeTargets.Field)]
public class WorkbookFlatAttribute : Attribute
{
	public string prefix;

	public string postfix;
}
