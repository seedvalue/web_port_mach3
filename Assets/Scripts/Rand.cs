using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Rand
{
	private static float? normalDistributionSpare;

	public static float uniform => Random.value;

	public static float uniformExclusive
	{
		get
		{
			float uniform;
			for (uniform = Rand.uniform; uniform == 1f; uniform = Rand.uniform)
			{
			}
			return uniform;
		}
	}

	public static float uniformSymetric => Random.value * 2f - 1f;

	public static float normal
	{
		get
		{
			if (normalDistributionSpare.HasValue)
			{
				float value = normalDistributionSpare.Value;
				normalDistributionSpare = null;
				return value;
			}
			float uniformSymetric;
			float uniformSymetric2;
			float num;
			do
			{
				uniformSymetric = Rand.uniformSymetric;
				uniformSymetric2 = Rand.uniformSymetric;
				num = uniformSymetric * uniformSymetric + uniformSymetric2 * uniformSymetric2;
			}
			while (num >= 1f || num == 0f);
			num = Mathf.Sqrt(-2f * Mathf.Log(num) / num);
			normalDistributionSpare = uniformSymetric2 * num;
			return uniformSymetric * num;
		}
	}

	public static float normalSymetric
	{
		get
		{
			float normal = Rand.normal;
			while (normal < -3f || normal > 3f)
			{
				normal = Rand.normal;
			}
			return Mathf.Clamp(normal / 3f, -1f, 1f);
		}
	}

	public static int UniformInt(float expected)
	{
		int num = Mathf.FloorToInt(expected);
		if (expected - (float)num > uniform)
		{
			num++;
		}
		return num;
	}

	public static float UniformRange(float min, float max)
	{
		return min + uniform * (max - min);
	}

	public static int UniformRangeInt(int min, int max)
	{
		return min + Mathf.FloorToInt(uniformExclusive * (float)(max - min));
	}

	public static int UniformRangeInt(float min, float max)
	{
		float num = UniformRange(min, max);
		int num2 = Mathf.FloorToInt(num);
		if (num - (float)num2 > uniform)
		{
			num2++;
		}
		return num2;
	}

	public static float Normal(float stdev)
	{
		return normal * stdev;
	}

	public static float NormalRange(float min, float max)
	{
		float num = (max + min) * 0.5f;
		float num2 = num - min;
		float value = num + normalSymetric * num2;
		return Mathf.Clamp(value, min, max);
	}

	public static int NormalRangeInt(float min, float max)
	{
		float num = NormalRange(min, max);
		int num2 = Mathf.FloorToInt(num);
		if (num - (float)num2 > uniform)
		{
			num2++;
		}
		return num2;
	}

	public static void Shuffle(IList list)
	{
		for (int i = 0; i < list.Count - 1; i++)
		{
			int index = UniformRangeInt(i, list.Count);
			object value = list[index];
			list[index] = list[i];
			list[i] = value;
		}
	}

	public static int[] PickIndicesUnique(IList list, int count)
	{
		count = Mathf.Min(list.Count, count);
		return (from i in Enumerable.Range(0, list.Count)
			orderby uniform
			select i).Take(count).ToArray();
	}

	public static int[] PickIndicesUniqueByWeight(IList<float> list, int count)
	{
		count = Mathf.Min(list.Count((float w) => w > 0f), count);
		return (from i in Enumerable.Range(0, list.Count)
			orderby uniform * list[i]
			select i).Take(count).ToArray();
	}
}
