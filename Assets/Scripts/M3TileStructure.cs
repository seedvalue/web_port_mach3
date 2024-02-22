using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M3TileStructure
{
	public List<IntVector2> tiles;

	public M3Orb attackType;

	public M3OrbStructure structureType;

	public int moveTimeStamp;

	private M3Board board;

	private Canvas overlayCanvas;

	public M3Tile this[int index] => board.board[tiles[index].x, tiles[index].y];

	public M3TileStructure(M3Orb type)
	{
		attackType = type;
		tiles = new List<IntVector2>();
		board = M3Board.instance;
		overlayCanvas = GameObject.FindGameObjectWithTag("OverlayCanvas").GetComponent<Canvas>();
	}

	private void CalcXToGo(M3WoundText text, Vector3 from, float xTo)
	{
		float num = board.tileYOffset + (float)board.tileSize * ((float)board.rows + 0.5f) - from.y;
		text.xDistanceToGo = text.yDistanceToGo / num * (xTo - from.x);
		Vector2 vector = new Vector2(text.xDistanceToGo, text.yDistanceToGo);
		vector.Normalize();
		text.xDistanceToGo = vector.x * text.yDistanceToGo;
		text.yDistanceToGo = vector.y * text.yDistanceToGo;
	}

	public IEnumerator CoCollapse(float refXPos, int stat)
	{
		Vector3 dest = GetMainTilePos(refXPos);
		float duration = board.timings.actualStructureCollapseDuration;
		bool fireText = false;
		for (int i = 0; i < tiles.Count; i++)
		{
			this[i].SetDestination(dest, duration);
		}
		GameObject fx = null;
		if ((bool)this[0].fxOnPowerCumulate)
		{
			fx = Object.Instantiate(this[0].fxOnPowerCumulate);
			if (stat > 0)
			{
				AudioManager.PlaySafe(board.sounds.structurePowerUp);
			}
			else
			{
				AudioManager.PlaySafe(board.sounds.structureWaste);
			}
			fireText = (stat > 0);
		}
		if ((bool)fx)
		{
			fx.transform.position = new Vector3(dest.x, dest.y, board.tileFxZ);
		}
		if (fireText)
		{
			M3WoundText m3WoundText = Object.Instantiate(this[0].textOnPowerUpPrefab, overlayCanvas.transform, worldPositionStays: false);
			m3WoundText.transform.position = Vector3.zero;
			CalcXToGo(m3WoundText, dest, refXPos);
			m3WoundText.Init(dest, -stat, attackType);
		}
		yield return new WaitForSeconds(duration);
	}

	public bool JoinWith(M3TileStructure structure)
	{
		M3Tile[,] b = board.board;
		if (attackType == structure.attackType)
		{
			for (int i = 0; i < tiles.Count; i++)
			{
				for (int j = 0; j < structure.tiles.Count; j++)
				{
					if (tiles[i].Distance(structure.tiles[j]) > 1)
					{
						continue;
					}
					for (int k = 0; k < structure.tiles.Count; k++)
					{
						if (IsUniqueTile(structure.tiles[k]))
						{
							AddTile(b, structure.tiles[k]);
						}
					}
					return true;
				}
			}
			return false;
		}
		return false;
	}

	public Vector3 GetMainTilePos(float refXPos)
	{
		int y = tiles[0].y;
		float num = Mathf.Abs(refXPos - (float)(tiles[0].x * board.tileSize + board.tileSize / 2));
		IntVector2 intVector = tiles[0];
		for (int i = 1; i < tiles.Count; i++)
		{
			if (tiles[i].y > y || (tiles[i].y == y && Mathf.Abs(refXPos - (float)(tiles[i].x * board.tileSize + board.tileSize / 2)) < num))
			{
				intVector = tiles[i];
				y = tiles[i].y;
				num = Mathf.Abs(refXPos - (float)(tiles[0].x * board.tileSize + board.tileSize / 2));
			}
		}
		return board.board[intVector.x, intVector.y].transform.position;
	}

	public void AddTile(M3Tile[,] b, IntVector2 pos)
	{
		tiles.Add(pos);
		moveTimeStamp = Mathf.Max(moveTimeStamp, b[pos.x, pos.y].moveTimeStamp);
		b[pos.x, pos.y].inStructure = true;
	}

	private bool IsUniqueTile(IntVector2 tile)
	{
		bool flag = true;
		for (int i = 0; i < tiles.Count; i++)
		{
			flag = (flag && tiles[i].Distance(tile) > 0);
		}
		return flag;
	}

	private bool IsRow(int cols, int rows)
	{
		if (tiles.Count >= cols)
		{
			bool flag = false;
			int[] array = new int[rows];
			for (int i = 0; i < tiles.Count; i++)
			{
				array[tiles[i].y]++;
			}
			for (int j = 0; j < rows; j++)
			{
				flag = (flag || array[j] == cols);
			}
			return flag;
		}
		return false;
	}

	private bool IsFour()
	{
		if (tiles.Count == 4)
		{
			return tiles[0].y + tiles[1].y - tiles[2].y - tiles[3].y == 0 || tiles[0].x + tiles[1].x - tiles[2].x - tiles[3].x == 0;
		}
		return false;
	}

	private bool IsCross()
	{
		if (tiles.Count == 5)
		{
			int num = tiles[0].x;
			int num2 = tiles[0].x;
			int num3 = tiles[0].y;
			int num4 = tiles[0].y;
			for (int i = 1; i < tiles.Count; i++)
			{
				num = Mathf.Min(num, tiles[i].x);
				num2 = Mathf.Max(num2, tiles[i].x);
				num3 = Mathf.Min(num3, tiles[i].y);
				num4 = Mathf.Max(num4, tiles[i].y);
			}
			bool flag = num2 - num == 2 && num4 - num3 == 2;
			if (flag)
			{
				flag = false;
				for (int j = 0; j < tiles.Count; j++)
				{
					flag = (flag || (tiles[j].x == num + 1 && tiles[j].y == num3 + 1));
				}
			}
			return flag;
		}
		return false;
	}

	public M3OrbStructure CheckOrbStructure(int cols, int rows)
	{
		structureType = M3OrbStructure.three;
		if (IsRow(cols, rows))
		{
			structureType = M3OrbStructure.row;
		}
		else if (IsFour())
		{
			structureType = M3OrbStructure.four;
		}
		else if (IsCross())
		{
			structureType = M3OrbStructure.cross;
		}
		return structureType;
	}
}
