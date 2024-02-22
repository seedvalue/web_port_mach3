using System.Collections.Generic;

public class M3TileStructures
{
	public static int BuildStructures(M3Tile[,] board, List<M3TileStructure> list, int columns, int rows)
	{
		list.Clear();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				board[i, j].inStructure = false;
			}
		}
		num = AddRowStructures(board, list, columns, rows);
		num2 = AddColStructures(board, list, columns, rows) - num;
		num3 = JoinStructures(board, list);
		for (int k = 0; k < list.Count; k++)
		{
			list[k].CheckOrbStructure(columns, rows);
		}
		return num + num2 - num3;
	}

	private static int JoinStructures(M3Tile[,] board, List<M3TileStructure> list)
	{
		int num = 0;
		int num2 = 0;
		do
		{
			int i = 0;
			num2 = 0;
			for (; i < list.Count - 1; i++)
			{
				for (int num3 = list.Count - 1; num3 > i; num3--)
				{
					if (list[i].JoinWith(list[num3]))
					{
						list.RemoveAt(num3);
						num++;
						num2++;
					}
				}
			}
		}
		while (num2 != 0);
		return num;
	}

	private static int AddRowStructures(M3Tile[,] board, List<M3TileStructure> list, int columns, int rows)
	{
		for (int i = 0; i < rows; i++)
		{
			int j = 2;
			while (j < columns)
			{
				if (board[j, i].attackType == board[j - 1, i].attackType && board[j, i].attackType == board[j - 2, i].attackType)
				{
					M3TileStructure m3TileStructure = new M3TileStructure(board[j, i].attackType);
					m3TileStructure.AddTile(board, new IntVector2(j - 2, i));
					m3TileStructure.AddTile(board, new IntVector2(j - 1, i));
					m3TileStructure.AddTile(board, new IntVector2(j, i));
					for (j++; j < columns && board[j, i].attackType == m3TileStructure.attackType; j++)
					{
						m3TileStructure.AddTile(board, new IntVector2(j, i));
					}
					if (columns - j < 3)
					{
						j = columns;
					}
					list.Add(m3TileStructure);
				}
				else
				{
					j++;
				}
			}
		}
		return list.Count;
	}

	private static int AddColStructures(M3Tile[,] board, List<M3TileStructure> list, int columns, int rows)
	{
		for (int i = 0; i < columns; i++)
		{
			int j = 2;
			while (j < rows)
			{
				if (board[i, j].attackType == board[i, j - 1].attackType && board[i, j].attackType == board[i, j - 2].attackType)
				{
					M3TileStructure m3TileStructure = new M3TileStructure(board[i, j].attackType);
					m3TileStructure.AddTile(board, new IntVector2(i, j - 2));
					m3TileStructure.AddTile(board, new IntVector2(i, j - 1));
					m3TileStructure.AddTile(board, new IntVector2(i, j));
					for (j++; j < rows && board[i, j].attackType == m3TileStructure.attackType; j++)
					{
						m3TileStructure.AddTile(board, new IntVector2(i, j));
					}
					if (rows - j < 3)
					{
						j = rows;
					}
					list.Add(m3TileStructure);
				}
				else
				{
					j++;
				}
			}
		}
		return list.Count;
	}
}
