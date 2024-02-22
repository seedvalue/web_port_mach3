using System;

[AttributeUsage(AttributeTargets.Class)]
public class WorkbookAssetPathAttribute : Attribute
{
	public string path
	{
		get;
		private set;
	}

	public WorkbookAssetPathAttribute(string path)
	{
		this.path = path;
	}
}
