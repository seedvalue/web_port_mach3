using System;

namespace Utils
{
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public class SingletonPrefabAttribute : Attribute
	{
		private string _name;

		public string Name => _name;

		public SingletonPrefabAttribute()
		{
			_name = string.Empty;
		}

		public SingletonPrefabAttribute(string name)
		{
			_name = name;
		}
	}
}
