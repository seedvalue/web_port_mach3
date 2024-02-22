using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class M3Board : MonoBehaviour, IM3OrbProvider
{
	[Serializable]
	public class Sounds
	{
		public AudioSample orbMove;

		public AudioSample structureWaste;

		public AudioSample structurePowerUp;

		public AudioSample comboPowerUp;

		public AudioSample damageAdd;

		public AudioSample skillInactive;

		public AudioSample orbMoveTimeUp;
	}

	[Serializable]
	public class Timings
	{
		public float tileTransitionDuration = 0.15f;

		public float tileFallingDuration = 0.2f;

		public float tileSelectDuration = 0.25f;

		public float tileLightUpDuration = 0.25f;

		public float tileGrayOutDuration = 0.25f;

		public float tileDeathDuration = 0.2f;

		public float tileDeathGrayScaleDelay = 0.25f;

		public float tileDeathGrayScaleDuration = 0.25f;

		public float tileDeathGrayScaleFxDuration = 0.4f;

		public float maxDragDuration = 5f;

		public float structureRemoveDelay = 0.3f;

		public float wastedStructureRemoveDelay = 0.3f;

		public float structureCollapseDuration = 0.25f;

		public float tilesFallDelay = 0.3f;

		public float actualTileTransitionDuration => tileTransitionDuration * M3TileManager.instance.debugTimeMultiplier;

		public float actualTileFallingDuration => tileFallingDuration * M3TileManager.instance.debugTimeMultiplier;

		public float actualTileSelectDuration => tileSelectDuration * M3TileManager.instance.debugTimeMultiplier;

		public float actualTileLightUpDuration => tileLightUpDuration * M3TileManager.instance.debugTimeMultiplier;

		public float actualTileGrayOutDuration => tileGrayOutDuration * M3TileManager.instance.debugTimeMultiplier;

		public float actualTileDeathDuration => tileDeathDuration * M3TileManager.instance.debugTimeMultiplier;

		public float actualTileDeathGrayScaleDelay => tileDeathGrayScaleDelay * M3TileManager.instance.debugTimeMultiplier;

		public float actualTileDeathGrayScaleDuration => tileDeathGrayScaleDuration * M3TileManager.instance.debugTimeMultiplier;

		public float actualTileDeathGrayScaleFxDuration => tileDeathGrayScaleFxDuration * M3TileManager.instance.debugTimeMultiplier;

		public float actualMaxDragDuration => maxDragDuration * M3TileManager.instance.debugTimeMultiplier;

		public float actualStructureRemoveDelay => structureRemoveDelay * M3TileManager.instance.debugTimeMultiplier;

		public float actualWastedStructureRemoveDelay => wastedStructureRemoveDelay * M3TileManager.instance.debugTimeMultiplier;

		public float actualStructureCollapseDuration => structureCollapseDuration * M3TileManager.instance.debugTimeMultiplier;

		public float actualTilesFallDelay => tilesFallDelay * M3TileManager.instance.debugTimeMultiplier;
	}

	[Serializable]
	public class TimingBar
	{
		public float barDelay = 1f;

		public float barYOffset = 10f;

		[HideInInspector]
		public ProgressBar bar;
	}

	public class Semaphores
	{
		public int boardNotReady;

		public int fxFlying;

		public int orbsDying;
	}

	public static string[] orbNames = new string[6]
	{
		"Melee",
		"Ranged",
		"Pyromancy",
		"Void",
		"Harmony",
		"Recovery"
	};

	public static Color[] orbColors = new Color[6]
	{
		new Color(0f, 248f / 255f, 248f / 255f),
		new Color(0.8235294f, 66f / 85f, 29f / 255f),
		new Color(1f, 0f, 0f),
		new Color(196f / 255f, 2f / 51f, 1f),
		new Color(36f / 85f, 218f / 255f, 0.1764706f),
		new Color(0.9411765f, 148f / 255f, 78f / 85f)
	};

	private const string strMoveTimeBar = "MoveTimeBar";

	public GameObject[] tilePrefabs;

	public GameObject[] tileChangeFxPrefabs;

	public M3OrbParticle[] orbTrailFxPrefabs;

	public int rows = 5;

	public int columns = 6;

	public int tileSize = 96;

	public float draggedTileScale = 1.2f;

	public float draggedTileYOffset = 24f;

	public float draggedTileAlpha = 0.9f;

	public Color draggedTileColor = new Color(1f, 1f, 1f, 0.4f);

	public Color grayedOutTileColor = new Color(0.25f, 0.25f, 0.25f, 0.6f);

	public Sounds sounds;

	public float tileZ = -50f;

	public float tileFxZ = -60f;

	public float tileYOffset = 90f;

	public bool playMode;

	public float orbPulseFrequency = 1f;

	public float orbPulseSizeDiff = 0.2f;

	public M3Combo combo;

	public Timings timings = new Timings();

	public TimingBar timingBar = new TimingBar();

	public static M3Board instance;

	private GameObject boardHolder;

	[HideInInspector]
	public M3Tile[,] board;

	private GameObject draggedTile;

	private Vector3 draggedOffset;

	private int dragColumn;

	private int dragRow;

	private float dragTime;

	private int tilesMoved;

	private int comboLevel;

	private bool pulsing;

	private IM3FakeDrag fakeDrag;

	public Semaphores semaphores = new Semaphores();

	private List<M3TileStructure> tileStructures = new List<M3TileStructure>();

	private List<M3TileSwap> rollback = new List<M3TileSwap>();

	private IM3OrbProvider tileProvider;

	private M3TileManager tileManager;

	public void StartGeneration()
	{
	}

	public void Save(List<int> saveGeneratedOrbs)
	{
	}

	public bool Load(List<int> saveGeneratedOrbs)
	{
		return true;
	}

	public M3Orb GenerateOrb(int column)
	{
		return (M3Orb)UnityEngine.Random.Range(0, tilePrefabs.Length);
	}

	private void Clear()
	{
		if (board == null)
		{
			board = new M3Tile[columns, rows];
		}
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				if ((bool)board[i, j])
				{
					UnityEngine.Object.Destroy(board[i, j].gameObject);
					board[i, j] = null;
				}
			}
		}
	}

	private M3Tile CreateNewTileAt(int column, int row, bool grayedOut = false, M3Orb orb = M3Orb.None)
	{
		int num = (int)((orb == M3Orb.None) ? tileProvider.GenerateOrb(column) : orb);
		GameObject original = tilePrefabs[num];
		GameObject gameObject = UnityEngine.Object.Instantiate(original, new Vector3(column * tileSize + tileSize / 2, tileYOffset + (float)(row * tileSize) + (float)(tileSize / 2), 0f), Quaternion.identity);
		gameObject.transform.SetParent(boardHolder.transform, worldPositionStays: false);
		board[column, row] = gameObject.GetComponent<M3Tile>();
		if (grayedOut)
		{
			board[column, row].GrayOutOn(grayedOutTileColor, 0f);
		}
		return board[column, row];
	}

	private int RemoveStructure(M3TileStructure structure, bool animate = true)
	{
		for (int i = 0; i < structure.tiles.Count; i++)
		{
			int x = structure.tiles[i].x;
			int y = structure.tiles[i].y;
			if (!board[x, y])
			{
				continue;
			}
			if (animate)
			{
				if (M3Player.instance.GetStat(structure.attackType) == 0)
				{
					StartCoroutine(board[x, y].AnimateDeath(grayScaleFirst: true, 0f, timings.actualTileDeathGrayScaleDelay, timings.actualTileDeathGrayScaleDuration, (i != 0) ? 0f : timings.actualTileDeathGrayScaleFxDuration));
				}
				else
				{
					StartCoroutine(board[x, y].AnimateDeath(grayScaleFirst: false, timings.actualTileDeathDuration, 0f, 0f, 0f));
				}
			}
			else
			{
				UnityEngine.Object.Destroy(board[x, y].gameObject);
			}
			board[x, y] = null;
		}
		return structure.tiles.Count;
	}

	private IEnumerator CoRemoveStructure(M3Player player, M3TileStructure structure)
	{
		semaphores.fxFlying++;
		int orbsDestroyed = structure.tiles.Count;
		List<M3AttackModifier> modifiers = new List<M3AttackModifier>();
		M3Orb attack = structure.attackType;
		for (int i = 0; i < player.perks.Count; i++)
		{
			(player.perks[i] as IPerkStructureMatchHandler)?.OnStructureMatch(attack, orbsDestroyed, structure.structureType, modifiers, player.defModifiers);
		}
		Vector3 destPos = player.GetDamageLabelPos(attack);
		int stat = player.GetStat(attack) * structure.tiles.Count / 3;
		yield return StartCoroutine(structure.CoCollapse(destPos.x, stat));
		float fxFlyDuration = 0f;
		if (stat > 0)
		{
			fxFlyDuration = FxOrbToLabel(destPos, structure, attack);
		}
		RemoveStructure(structure);
		if (stat > 0)
		{
			yield return new WaitForSeconds(fxFlyDuration);
		}
		comboLevel++;
		combo.UpdateCombo(comboLevel);
		for (int j = 0; j < player.perks.Count; j++)
		{
			(player.perks[j] as IPerkComboLevelHandler)?.OnComboLevel(comboLevel, modifiers, player.defModifiers);
		}
		player.AttackSequenceAddM3Damage(orbsDestroyed, attack, modifiers);
		semaphores.fxFlying--;
	}

	private void LetTilesFall(float fallingTime)
	{
		for (int i = 0; i < columns; i++)
		{
			int j = 0;
			int num = 0;
			while (j < rows)
			{
				if (!board[i, j])
				{
					int num2 = j;
					for (; j < rows && !board[i, j]; j++)
					{
					}
					if (j < rows)
					{
						board[i, num2] = board[i, j];
						board[i, j] = null;
						board[i, num2].SetDestination(new Vector3(i * tileSize + tileSize / 2, tileYOffset + (float)(num2 * tileSize) + (float)(tileSize / 2), tileZ), fallingTime);
					}
					else
					{
						M3Tile m3Tile = CreateNewTileAt(i, num2, grayedOut: true);
						Vector3 position = m3Tile.transform.position;
						m3Tile.transform.position = position + new Vector3(0f, tileYOffset + (float)((rows - num2 + num) * tileSize) + (float)(tileSize / 2), 0f);
						num++;
						m3Tile.SetDestination(position, fallingTime);
					}
					j = num2 + 1;
				}
				else
				{
					j++;
				}
			}
		}
		ClearTimeStamps();
	}

	public void Init(bool clearStructures, IM3OrbProvider provider)
	{
		tileManager = M3TileManager.instance;
		instance = this;
		Clear();
		if (boardHolder == null)
		{
			boardHolder = new GameObject("Board");
			if (!boardHolder)
			{
				UnityEngine.Debug.LogWarning("Brak boardHoldera");
			}
			else
			{
				boardHolder.transform.position = new Vector3(0f, 0f, tileZ);
			}
		}
		tileProvider = provider;
		if (tileProvider == null)
		{
			tileProvider = this;
		}
		else
		{
			tileProvider.StartGeneration();
		}
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				CreateNewTileAt(i, j);
			}
		}
		if (clearStructures && M3TileStructures.BuildStructures(board, tileStructures, columns, rows) > 0)
		{
			do
			{
				foreach (M3TileStructure tileStructure in tileStructures)
				{
					RemoveStructure(tileStructure, animate: false);
				}
				tileStructures.Clear();
				LetTilesFall(0f);
				M3TileStructures.BuildStructures(board, tileStructures, columns, rows);
			}
			while (tileStructures.Count != 0);
			StartCoroutine(ToggleGrayOut(grayOut: false, 0f));
		}
		timingBar.bar = GameObject.FindGameObjectWithTag("MoveTimeBar").GetComponent<ProgressBar>();
		if (!timingBar.bar)
		{
			UnityEngine.Debug.LogWarning("Brak Progress bara czasu na ruch gracza!");
		}
		else
		{
			timingBar.bar.gameObject.SetActive(value: false);
		}
		combo.UpdateCombo(0);
	}

	private void MoveTowards(Vector3 pos)
	{
		int column = dragColumn;
		int row = dragRow;
		int num = 0;
		GetColumnRow(new Vector3(pos.x, pos.y, 0f), ref column, ref row);
		num = Mathf.Abs(column - dragColumn) + Mathf.Abs(row - dragRow);
		if (num > 1)
		{
			Vector3 vector = pos - (draggedTile.transform.localPosition - draggedOffset);
			MoveTowards(Vector3.MoveTowards(draggedTile.transform.localPosition - draggedOffset, pos, vector.magnitude / 2f));
			MoveTowards(Vector3.MoveTowards(draggedTile.transform.localPosition - draggedOffset, pos, vector.magnitude / 2f));
		}
		else if (num == 1)
		{
			MoveTile(column, row, dragColumn, dragRow);
		}
		dragColumn = column;
		dragRow = row;
		draggedTile.transform.localPosition = pos + draggedOffset;
	}

	private void MoveTile(int columnFrom, int rowFrom, int columnTo, int rowTo)
	{
		M3Tile tile = board[columnTo, rowTo];
		ResetTile(board[columnFrom, rowFrom], columnTo, rowTo, timings.actualTileTransitionDuration, tileSize / 3);
		ResetTile(tile, columnFrom, rowFrom, timings.actualTileTransitionDuration, tileSize / 3);
		tilesMoved++;
		board[columnTo, rowTo].moveTimeStamp = tilesMoved;
		rollback.Add(new M3TileSwap(columnFrom, rowFrom, columnTo, rowTo));
		AudioManager.PlaySafe(sounds.orbMove);
	}

	private void ResetTile(M3Tile tile, int columnTo, int rowTo, float tileTransitionTime, float arcAmplitude = 0f)
	{
		board[columnTo, rowTo] = tile;
		tile.SetDestination(new Vector3(columnTo * tileSize + tileSize / 2, tileYOffset + (float)(rowTo * tileSize) + (float)(tileSize / 2), tileZ), tileTransitionTime, arcAmplitude);
	}

	private bool GetColumnRow(Vector3 pos, ref int column, ref int row)
	{
		if (pos.x >= 0f && pos.x < (float)(columns * tileSize) && pos.y >= tileYOffset && pos.y < tileYOffset + (float)(rows * tileSize))
		{
			column = Mathf.FloorToInt(pos.x / (float)tileSize);
			row = Mathf.FloorToInt((pos.y - tileYOffset) / (float)tileSize);
			return true;
		}
		return false;
	}

	private void ClearTimeStamps()
	{
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				board[i, j].moveTimeStamp = 0;
			}
		}
	}

	private GameObject DragStart(Vector3 pos)
	{
		if (GetColumnRow(pos, ref dragColumn, ref dragRow) && (fakeDrag != null || tileManager.TutorialAllows(M3TutorialActivity.M3MoveStart, dragColumn, dragRow)))
		{
			if (fakeDrag != null)
			{
				board[dragColumn, dragRow].Deselect();
				board[dragColumn, dragRow].ClearFx();
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(board[dragColumn, dragRow].gameObject);
			gameObject.transform.SetParent(boardHolder.transform, worldPositionStays: false);
			gameObject.transform.localScale = new Vector3(draggedTileScale, draggedTileScale, 1f);
			Transform transform = gameObject.transform;
			float y = (fakeDrag == null) ? draggedTileYOffset : 0f;
			Vector3 position = gameObject.transform.position;
			transform.localPosition = pos + new Vector3(0f, y, position.z);
			SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
			Helpers.SetSpriteAlpha(component, draggedTileAlpha);
			draggedOffset = gameObject.transform.localPosition - pos;
			draggedOffset.z = 0f;
			component = board[dragColumn, dragRow].GetComponent<SpriteRenderer>();
			component.color = draggedTileColor;
			board[dragColumn, dragRow].ColorDeselected = draggedTileColor;
			dragTime = 0f;
			tilesMoved = 0;
			rollback.Clear();
			ClearTimeStamps();
			if (fakeDrag == null)
			{
				tileManager.ToggleInteractionAllowed(allowed: false);
			}
			return gameObject;
		}
		return null;
	}

	private void DragEnd(bool dragTimeUp, bool instantRollback)
	{
		SpriteRenderer component = board[dragColumn, dragRow].GetComponent<SpriteRenderer>();
		board[dragColumn, dragRow].ColorDeselected = Color.white;
		component.color = Color.white;
		if (dragTimeUp)
		{
			AudioManager.PlaySafe(sounds.orbMoveTimeUp);
		}
		UnityEngine.Object.Destroy(draggedTile);
		draggedTile = null;
		board[dragColumn, dragRow].moveTimeStamp = tilesMoved + 1;
		if (fakeDrag == null)
		{
			if ((bool)timingBar.bar)
			{
				timingBar.bar.gameObject.SetActive(value: false);
			}
			UpdateStructures(dragColumn, dragRow);
			tileManager.ToggleInteractionAllowed(allowed: true);
			if (playMode && tilesMoved > 0)
			{
				if (tileManager.TutorialAllows(M3TutorialActivity.M3MoveEnd, dragColumn, dragRow))
				{
					tileManager.OnTileDragEnd();
				}
				else
				{
					RollbackM3Move(instant: false);
				}
			}
		}
		else
		{
			RollbackM3Move(instantRollback);
		}
	}

	private void RollbackM3Move(bool instant)
	{
		if (rollback.Count <= 0)
		{
			return;
		}
		M3TileSwap m3TileSwap = rollback[rollback.Count - 1];
		M3TileSwap m3TileSwap2 = rollback[0];
		if (m3TileSwap2.colTo != m3TileSwap.colFrom || m3TileSwap2.rowTo != m3TileSwap.rowFrom)
		{
			for (int num = rollback.Count - 1; num >= 0; num--)
			{
				M3TileSwap m3TileSwap3 = rollback[num];
				M3Tile tile = board[m3TileSwap3.colFrom, m3TileSwap3.rowFrom];
				ResetTile(board[m3TileSwap3.colTo, m3TileSwap3.rowTo], m3TileSwap3.colFrom, m3TileSwap3.rowFrom, (!instant) ? timings.actualTileTransitionDuration : 0f);
				ResetTile(tile, m3TileSwap3.colTo, m3TileSwap3.rowTo, (!instant) ? timings.actualTileTransitionDuration : 0f);
			}
			UpdateStructures(-1, -1);
		}
	}

	public int GetLastAttackComboCount()
	{
		return comboLevel;
	}

	private IEnumerator LightStructures(bool withSelection)
	{
		semaphores.boardNotReady++;
		foreach (M3TileStructure tileStructure in tileStructures)
		{
			for (int i = 0; i < tileStructure.tiles.Count; i++)
			{
				M3Tile m3Tile = board[tileStructure.tiles[i].x, tileStructure.tiles[i].y];
				if (withSelection)
				{
					m3Tile.Select(timings.actualTileLightUpDuration);
				}
				m3Tile.Highlight(timings.actualTileLightUpDuration);
			}
		}
		yield return new WaitForSeconds(timings.actualTileLightUpDuration);
		semaphores.boardNotReady--;
	}

	public IEnumerator ToggleGrayOut(bool grayOut, float grayOutTime)
	{
		semaphores.boardNotReady++;
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				if (board[i, j].inStructure || !grayOut)
				{
					board[i, j].GrayOutOff(timings.actualTileGrayOutDuration);
				}
				else
				{
					board[i, j].GrayOutOn(grayedOutTileColor, timings.actualTileGrayOutDuration);
				}
			}
		}
		if (grayOutTime > float.Epsilon)
		{
			yield return new WaitForSeconds(grayOutTime);
		}
		semaphores.boardNotReady--;
	}

	public void UpdateStructures(int draggedColumn, int draggedRow)
	{
		M3TileStructures.BuildStructures(board, tileStructures, columns, rows);
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				if (board[i, j].inStructure)
				{
					board[i, j].Select(timings.actualTileSelectDuration);
				}
				else
				{
					board[i, j].Deselect(timings.actualTileSelectDuration);
				}
			}
		}
		if ((bool)draggedTile)
		{
			M3Tile component = draggedTile.GetComponent<M3Tile>();
			if (board[draggedColumn, draggedRow].inStructure)
			{
				component.Select(timings.actualTileSelectDuration);
			}
			else
			{
				component.Deselect(timings.actualTileSelectDuration);
			}
		}
	}

	private void Update()
	{
		bool flag = false;
		int num = tilesMoved;
		int column = 0;
		int row = 0;
		if ((bool)draggedTile)
		{
			if (tilesMoved > 0)
			{
				dragTime += Time.deltaTime;
			}
			bool flag2 = true;
			Vector3 pos = default(Vector3);
			if (fakeDrag != null)
			{
				pos = fakeDrag.GetPointerPos();
			}
			else
			{
				flag2 = (UnityEngine.Input.touchCount > 0);
				if (flag2)
				{
					pos = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
				}
			}
			if (flag2)
			{
				pos.x = Mathf.Clamp(pos.x, 2f, (float)(columns * tileSize) - 2f);
				pos.y = Mathf.Clamp(pos.y, 2f + tileYOffset, tileYOffset + (float)(rows * tileSize) - 2f);
				Vector3 localPosition = draggedTile.transform.localPosition;
				pos.z = localPosition.z;
				GetColumnRow(new Vector3(pos.x, pos.y, 0f), ref column, ref row);
				if (tileManager.TutorialAllows(M3TutorialActivity.M3Move, column, row))
				{
					MoveTowards(pos);
				}
				else
				{
					flag = true;
				}
				if ((bool)timingBar.bar && fakeDrag == null)
				{
					ProgressBar bar = timingBar.bar;
					Vector3 position = draggedTile.transform.position;
					float x = position.x;
					Vector3 position2 = draggedTile.transform.position;
					float y = position2.y + (float)(tileSize / 2) + timingBar.barYOffset;
					Vector3 position3 = timingBar.bar.transform.position;
					bar.SetWorldTransform(new Vector3(x, y, position3.z), Quaternion.identity);
					timingBar.bar.SetProgress(1f - (dragTime - timingBar.barDelay) / (timings.actualMaxDragDuration - timingBar.barDelay));
					if (dragTime > timingBar.barDelay)
					{
						timingBar.bar.gameObject.SetActive(value: true);
					}
				}
			}
		}
		bool flag3 = false;
		bool flag4 = false;
		Vector3 pos2 = default(Vector3);
		flag3 = ((!draggedTile || ((bool)draggedTile && fakeDrag != null)) && UnityEngine.Input.touchCount > 0);
		flag4 = ((bool)draggedTile && UnityEngine.Input.touchCount == 0);
		if (flag3)
		{
			pos2 = Camera.main.ScreenToWorldPoint(Input.touches[0].position);
		}
		pos2.z = 0f;
		if (flag3 && tileManager.IsInteractionAllowed())
		{
			if (fakeDrag != null)
			{
				FakeDragEnd(instantRollback: true);
			}
			draggedTile = DragStart(pos2);
			if ((bool)draggedTile)
			{
				draggedTile.GetComponent<M3Tile>().ClearFx();
			}
		}
		if ((flag4 || flag || dragTime > timings.actualMaxDragDuration) && (bool)draggedTile && fakeDrag == null)
		{
			DragEnd(dragTime > timings.actualMaxDragDuration, instantRollback: false);
		}
		else if (tilesMoved > num)
		{
			UpdateStructures(column, row);
		}
	}

	public float FxOrbToLabel(Vector3 destPos, M3TileStructure structure, M3Orb orbType)
	{
		Vector3 mainTilePos = structure.GetMainTilePos(destPos.x);
		destPos.z = (mainTilePos.z = tileFxZ);
		M3OrbParticle m3OrbParticle = UnityEngine.Object.Instantiate(orbTrailFxPrefabs[(int)orbType], mainTilePos, Quaternion.identity);
		return m3OrbParticle.Fly(destPos);
	}

	private void GatherStats(Match3Stats stats)
	{
		stats.allMoves++;
		if (tileStructures.Count <= 1)
		{
			stats.oneStructureMoves++;
		}
		stats.orbStructureTotal += tileStructures.Count;
		if (tilesMoved == 1)
		{
			stats.simpleMoves++;
		}
		stats.orbSwapTotal += tilesMoved;
	}

	public IEnumerator AttackSequence(M3Player player, M3Battle battle, Match3Stats stats)
	{
		M3TileManager.Log("AttackSequence: Begin");
		if (!player.AttackSequenceIsStarted())
		{
			player.AttackSequenceStart(tileStructures.Count > 0);
			comboLevel = 0;
			GatherStats(stats);
		}
		int count = (tileStructures.Count != 0) ? tileStructures.Count : M3TileStructures.BuildStructures(board, tileStructures, columns, rows);
		tileStructures.Sort((M3TileStructure s1, M3TileStructure s2) => Comparer.Default.Compare(s1.moveTimeStamp, s2.moveTimeStamp));
		StartCoroutine(LightStructures(comboLevel > 0));
		yield return new WaitUntil(() => semaphores.boardNotReady == 0);
		for (int i = 0; i < tileStructures.Count; i++)
		{
			StartCoroutine(CoRemoveStructure(player, tileStructures[i]));
			if (player.GetStat(tileStructures[i].attackType) > 0)
			{
				yield return new WaitForSeconds(timings.actualStructureRemoveDelay);
			}
			else
			{
				yield return new WaitForSeconds(timings.actualWastedStructureRemoveDelay);
			}
		}
		yield return new WaitUntil(() => semaphores.fxFlying == 0);
		tileStructures.Clear();
		if (count > 0)
		{
			yield return new WaitForSeconds(timings.actualTilesFallDelay);
			yield return new WaitUntil(() => semaphores.orbsDying == 0);
			LetTilesFall(timings.actualTileFallingDuration);
			yield return new WaitForSeconds(timings.actualTileFallingDuration);
			yield return StartCoroutine(AttackSequence(player, battle, stats));
		}
		else
		{
			if (player.AttackSequenceM3DamageDealt(includeRecovery: true) > 0)
			{
				yield return StartCoroutine(combo.CoApply(player));
				yield return StartCoroutine(player.PrepareAttacks(battle));
				yield return StartCoroutine(player.AnimateAttacks());
				yield return StartCoroutine(player.Heal());
			}
			else
			{
				combo.FadeOut();
			}
			yield return StartCoroutine(player.AttackSequenceEnd());
		}
		M3TileManager.Log("AttackSequence: End");
	}

	public void ColorOrbsAccordingToTutorial(M3TutorialStep step)
	{
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				if (((bool)step && step.grayOutOrbs) || ((bool)step && step.tutorialActivity == M3TutorialActivity.M3Move && !step.IsM3MoveStartEnd(i, j)))
				{
					board[i, j].ColorDeselected = grayedOutTileColor;
					board[i, j].GrayOutOn(grayedOutTileColor, timings.actualTileGrayOutDuration);
				}
				else
				{
					board[i, j].ColorDeselected = Color.white;
					board[i, j].GrayOutOff(timings.actualTileGrayOutDuration);
				}
			}
		}
	}

	public IEnumerator PulseOrb(int column, int row)
	{
		pulsing = true;
		float time = 0f;
		M3Tile tile = board[column, row];
		Vector3 scale = tile.transform.localScale;
		while (true)
		{
			time += Time.deltaTime;
			float scaledTime = time * orbPulseFrequency * 2f * (float)Math.PI;
			float scaling = 1f + orbPulseSizeDiff * Mathf.Sin(scaledTime);
			tile.transform.localScale = new Vector3(scale.x * scaling, scale.y * scaling, scale.z);
			yield return null;
			if (!(scaledTime < (float)Math.PI * 2f))
			{
				time = 0f;
				yield return new WaitUntil(() => draggedTile == null);
				if (!pulsing)
				{
					break;
				}
			}
		}
	}

	public void PulseOrbStop()
	{
		pulsing = false;
	}

	public bool FakeDragStart(IM3FakeDrag fake)
	{
		if (!draggedTile)
		{
			fakeDrag = fake;
			Vector3 pointerPos = fakeDrag.GetPointerPos();
			pointerPos.z = 0f;
			draggedTile = DragStart(pointerPos);
			return true;
		}
		return false;
	}

	public bool FakeDragEnd(bool instantRollback)
	{
		if (fakeDrag != null)
		{
			DragEnd(dragTimeUp: false, instantRollback);
			fakeDrag = null;
			return true;
		}
		return false;
	}

	public bool FindOrbs(List<IntVector2> coords, M3Orb orb, bool exclude, bool includeRecovery)
	{
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				if ((board[i, j].attackType != M3Orb.Recovery || includeRecovery) && ((board[i, j].attackType == orb && !exclude) || (board[i, j].attackType != orb && exclude)))
				{
					coords.Add(new IntVector2(i, j));
				}
			}
		}
		return coords.Count > 0;
	}

	public IEnumerator ChangeOrb(int column, int row, M3Orb orbNew, float exchangeDelay)
	{
		M3Tile tile = board[column, row];
		GameObject fxPrefab = tileChangeFxPrefabs[(int)orbNew];
		Vector3 pos = tile.transform.position;
		pos.z = tileFxZ;
		if ((bool)fxPrefab)
		{
			UnityEngine.Object.Instantiate(fxPrefab, pos, Quaternion.identity);
		}
		yield return new WaitForSeconds(exchangeDelay);
		UnityEngine.Object.Destroy(tile.gameObject);
		CreateNewTileAt(column, row, grayedOut: false, orbNew);
	}

	public void Save(M3SaveBoard saveBoard)
	{
		saveBoard.tiles.Clear();
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				saveBoard.tiles.Add(board[i, j].attackType);
			}
		}
		if (tileProvider != null)
		{
			tileProvider.Save(saveBoard.generatedOrbs);
		}
	}

	public bool Load(M3SaveBoard saveBoard)
	{
		if (saveBoard.tiles.Count != columns * rows)
		{
			return false;
		}
		Clear();
		for (int i = 0; i < columns; i++)
		{
			for (int j = 0; j < rows; j++)
			{
				CreateNewTileAt(i, j, grayedOut: false, saveBoard.tiles[i * rows + j]);
			}
		}
		if (tileProvider != null)
		{
			return tileProvider.Load(saveBoard.generatedOrbs);
		}
		return true;
	}

	public void Debug_TestStructures()
	{
		if (tileStructures.Count == 0)
		{
			M3TileStructures.BuildStructures(board, tileStructures, columns, rows);
		}
		for (int i = 0; i < tileStructures.Count; i++)
		{
			RemoveStructure(tileStructures[i]);
		}
		tileStructures.Clear();
	}

	public void Debug_LetTilesFall()
	{
		LetTilesFall(timings.actualTileFallingDuration);
	}

	public void Debug_TogglePlayMode()
	{
		playMode = !playMode;
	}
}
