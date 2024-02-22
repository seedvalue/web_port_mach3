using System;

namespace Utils
{
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public class SingletonInitializeOnLoadAttribute : Attribute
	{
	}
}
