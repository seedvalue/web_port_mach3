using System;
using System.Collections.Generic;
using System.Linq;

public class WeakReferenceSet<T> where T : class
{
	private readonly List<WeakReference> references = new List<WeakReference>();

	public void Add(T obj)
	{
		if (!Contains(obj))
		{
			references.Add(new WeakReference(obj));
		}
	}

	public void Remove(T obj)
	{
		references.RemoveAll(delegate(WeakReference r)
		{
			T val = r.Target as T;
			return val == null || object.ReferenceEquals(obj, val);
		});
	}

	public bool Contains(T obj)
	{
		WeakReference weakReference = references.Find(delegate(WeakReference r)
		{
			T val = r.Target as T;
			return (val == null) ? false : object.ReferenceEquals(obj, val);
		});
		return weakReference != null;
	}

	public void Cleanup()
	{
		references.RemoveAll((WeakReference r) => !r.IsAlive);
	}

	public T[] GetObjects()
	{
		return (from r in references
			select r.Target as T into t
			where t != null
			select t).ToArray();
	}

	public T[] GetObjectsAndCleanup()
	{
		Cleanup();
		return GetObjects();
	}
}
