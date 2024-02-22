using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FixedBoard", menuName = "Meta/FixedBoard")]
public class M3FixedBoard : ScriptableObject, IM3OrbProvider
{
	[Serializable]
	public class M3Column
	{
		public List<M3Orb> rows = new List<M3Orb>();

		public M3Orb this[int i]
		{
			get
			{
				return rows[i];
			}
			set
			{
				rows[i] = value;
			}
		}

		public int Count => rows.Count;

		public void Add(M3Orb orb)
		{
			rows.Add(orb);
		}

		public void Insert(int row, M3Orb orb)
		{
			rows.Insert(row, orb);
		}

		public void RemoveAt(int i)
		{
			rows.RemoveAt(i);
		}
	}

	[HideInInspector]
	public List<bool> availableOrbs = new List<bool>();

	public List<M3Column> orbs = new List<M3Column>();

	private List<int> orbsGenerated = new List<int>();

	public int Columns
	{
		get
		{
			return orbs.Count;
		}
		set
		{
			while (value > orbs.Count)
			{
				orbs.Add(new M3Column());
			}
			while (Mathf.Max(value, 0) < orbs.Count)
			{
				orbs.RemoveAt(orbs.Count - 1);
			}
			Rows = Rows;
		}
	}

	public int Rows
	{
		get
		{
			if (Columns > 0)
			{
				return orbs[0].Count;
			}
			return 0;
		}
		set
		{
			for (int i = 0; i < Columns; i++)
			{
				while (orbs[i].Count < value)
				{
					orbs[i].Add(GenerateRandomOrb());
				}
				while (orbs[i].Count > Mathf.Max(value, 0))
				{
					orbs[i].RemoveAt(orbs[i].Count - 1);
				}
			}
		}
	}

	public M3FixedBoard()
	{
		for (int i = 0; i < 6; i++)
		{
			availableOrbs.Add(item: true);
		}
	}

	public void MoveOrbsUp()
	{
		for (int i = 0; i < Columns; i++)
		{
			for (int num = Rows - 2; num >= 0; num--)
			{
				orbs[i][num + 1] = orbs[i][num];
			}
		}
		GenerateRandomRow(0);
	}

	public void MoveOrbsDown()
	{
		for (int i = 0; i < Columns; i++)
		{
			for (int j = 1; j < Rows; j++)
			{
				orbs[i][j - 1] = orbs[i][j];
			}
		}
		GenerateRandomRow(Rows - 1);
	}

	public void RemoveRow(int rowIndex)
	{
		for (int i = 0; i < Columns; i++)
		{
			orbs[i].RemoveAt(rowIndex);
		}
	}

	public void AddRow(int rowIndex)
	{
		for (int i = 0; i < Columns; i++)
		{
			orbs[i].Insert(rowIndex, GenerateRandomOrb());
		}
	}

	private void GenerateRandomRow(int row)
	{
		for (int i = 0; i < Columns; i++)
		{
			orbs[i][row] = GenerateRandomOrb();
		}
	}

	private M3Orb GenerateRandomOrb()
	{
		M3Orb m3Orb;
		do
		{
			m3Orb = (M3Orb)UnityEngine.Random.Range(0, 6);
		}
		while (!availableOrbs[(int)m3Orb]);
		return m3Orb;
	}

	public M3Orb GenerateOrb(int column)
	{
		if (column < Columns && orbs[column].Count > orbsGenerated[column])
		{
			M3Orb result = orbs[column][orbsGenerated[column]];
			List<int> list;
			int index;
			(list = orbsGenerated)[index = column] = list[index] + 1;
			return result;
		}
		return GenerateRandomOrb();
	}

	public void StartGeneration()
	{
		orbsGenerated.Clear();
		for (int i = 0; i < Columns; i++)
		{
			orbsGenerated.Add(0);
		}
	}

	public void Save(List<int> saveGeneratedOrbs)
	{
		saveGeneratedOrbs.Clear();
		for (int i = 0; i < Columns; i++)
		{
			saveGeneratedOrbs.Add(orbsGenerated[i]);
		}
	}

	public bool Load(List<int> saveGeneratedOrbs)
	{
		if (saveGeneratedOrbs.Count == Columns)
		{
			StartGeneration();
			for (int i = 0; i < Columns; i++)
			{
				orbsGenerated[i] = saveGeneratedOrbs[i];
				if (orbsGenerated[i] > orbs[i].Count)
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}
}
