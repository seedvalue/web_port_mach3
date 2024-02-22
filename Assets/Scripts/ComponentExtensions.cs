using System.Linq;
using UnityEngine;

public static class ComponentExtensions
{
	public static T GetComponentAs<T>(this Component self) where T : class
	{
		return (from c in self.GetComponents<Component>()
			select c as T into c
			where c != null
			select c).FirstOrDefault();
	}

	public static T[] GetComponentsAs<T>(this Component self) where T : class
	{
		return (from c in self.GetComponents<Component>()
			select c as T into c
			where c != null
			select c).ToArray();
	}

	public static T GetComponentInChildrenAs<T>(this Component self) where T : class
	{
		return (from c in self.GetComponentsInChildren<Component>()
			select c as T into c
			where c != null
			select c).FirstOrDefault();
	}

	public static T GetComponentInChildrenAs<T>(this Component self, bool includeInactive) where T : class
	{
		return (from c in self.GetComponentsInChildren<Component>(includeInactive)
			select c as T into c
			where c != null
			select c).FirstOrDefault();
	}

	public static T[] GetComponentsInChildrenAs<T>(this Component self) where T : class
	{
		return (from c in self.GetComponentsInChildren<Component>()
			select c as T into c
			where c != null
			select c).ToArray();
	}

	public static T[] GetComponentsInChildrenAs<T>(this Component self, bool includeInactive) where T : class
	{
		return (from c in self.GetComponentsInChildren<Component>(includeInactive)
			select c as T into c
			where c != null
			select c).ToArray();
	}

	public static T GetComponentInParentAs<T>(this Component self) where T : class
	{
		return (from c in self.GetComponentsInParent<Component>()
			select c as T into c
			where c != null
			select c).FirstOrDefault();
	}

	public static T[] GetComponentsInParentAs<T>(this Component self) where T : class
	{
		return (from c in self.GetComponentsInParent<Component>()
			select c as T into c
			where c != null
			select c).ToArray();
	}

	public static T[] GetComponentsInParentAs<T>(this Component self, bool includeInactive) where T : class
	{
		return (from c in self.GetComponentsInParent<Component>(includeInactive)
			select c as T into c
			where c != null
			select c).ToArray();
	}
}
