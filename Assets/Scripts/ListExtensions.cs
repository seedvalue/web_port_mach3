using System;
using System.Collections.Generic;

public static class ListExtensions
{
	public static bool SortedUniqueAdd<T>(this List<T> list, T value)
	{
		return list.SortedUniqueAdd(value, Comparer<T>.Default);
	}

	public static bool SortedUniqueAdd<T>(this List<T> list, T value, IComparer<T> comparer)
	{
		int num = list.BinarySearch(value, comparer);
		if (num >= 0)
		{
			return false;
		}
		num = ~num;
		list.Insert(num, value);
		return true;
	}

	public static bool SortedUniqueRemove<T>(this List<T> list, T value)
	{
		return list.SortedUniqueRemove(value, Comparer<T>.Default);
	}

	public static bool SortedUniqueRemove<T>(this List<T> list, T value, IComparer<T> comparer)
	{
		int num = list.BinarySearch(value, comparer);
		if (num >= 0)
		{
			list.RemoveAt(num);
			return true;
		}
		return false;
	}

	public static bool SortedContains<T>(this List<T> list, T value)
	{
		return list.SortedContains(value, Comparer<T>.Default);
	}

	public static bool SortedContains<T>(this List<T> list, T value, IComparer<T> comparer)
	{
		return list.BinarySearch(value, comparer) >= 0;
	}

	public static int Partition<T>(this List<T> list, int start, int end, Predicate<T> predicate)
	{
		while (start != end)
		{
			while (predicate(list[start]))
			{
				if (++start == end)
				{
					return start;
				}
			}
			do
			{
				if (--end == start)
				{
					return start;
				}
			}
			while (!predicate(list[end]));
			T value = list[start];
			list[start] = list[end];
			list[end] = value;
			start++;
		}
		return start;
	}

	public static void InsertionSort<T>(this IList<T> list, Comparison<T> comparison)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		if (comparison == null)
		{
			throw new ArgumentNullException("comparison");
		}
		int count = list.Count;
		for (int i = 1; i < count; i++)
		{
			T val = list[i];
			int num = i - 1;
			while (num >= 0 && comparison(list[num], val) > 0)
			{
				list[num + 1] = list[num];
				num--;
			}
			list[num + 1] = val;
		}
	}

	public static void AddSorted<T>(this List<T> list, T item) where T : IComparable<T>
	{
		if (list.Count == 0)
		{
			list.Add(item);
			return;
		}
		if (list[list.Count - 1].CompareTo(item) <= 0)
		{
			list.Add(item);
			return;
		}
		if (list[0].CompareTo(item) >= 0)
		{
			list.Insert(0, item);
			return;
		}
		int num = list.BinarySearch(item);
		if (num < 0)
		{
			num = ~num;
		}
		list.Insert(num, item);
	}
}
