using System.Collections.Generic;
using UnityEngine;

public class M3TutorialStep : MonoBehaviour
{
	public string analyticsID;

	public int yPixelsFromCenter = 200;

	public bool grayOutOrbs;

	[HideInInspector]
	public string stepDescription;

	[HideInInspector]
	public M3TutorialActivity tutorialActivity;

	[HideInInspector]
	public List<IntVector2> coords = new List<IntVector2>();

	[HideInInspector]
	public int itemIndex = -1;

	[HideInInspector]
	public int mobIndex = -1;

	[HideInInspector]
	public bool allowM3Only;

	private List<M3TutorialAnimation> anims = new List<M3TutorialAnimation>();

	public int Keycount
	{
		get
		{
			return (tutorialActivity == M3TutorialActivity.M3Move) ? coords.Count : 0;
		}
		set
		{
			if (tutorialActivity != M3TutorialActivity.M3Move || value < 0)
			{
				return;
			}
			if (value < coords.Count)
			{
				for (int i = value; i < coords.Count; i++)
				{
					coords.RemoveAt(i);
				}
			}
			else if (value > coords.Count)
			{
				for (int j = coords.Count; j < value; j++)
				{
					coords.Add(new IntVector2(0, 0));
				}
			}
		}
	}

	private bool IsM3Activity(M3TutorialActivity activity)
	{
		return activity == M3TutorialActivity.M3MoveStart || activity == M3TutorialActivity.M3MoveEnd || activity == M3TutorialActivity.M3Move || activity == M3TutorialActivity.M3WaitUntilExecuted;
	}

	public bool IsM3MoveStartEnd(int column, int row)
	{
		if (tutorialActivity == M3TutorialActivity.M3Move)
		{
			return AreCoordsEqual(0, column, row);
		}
		return false;
	}

	public bool IsActivityAllowed(M3TutorialActivity activity, int index1, int index2)
	{
		if (IsActivityCompatible(activity))
		{
			switch (tutorialActivity)
			{
			case M3TutorialActivity.M3Move:
				switch (activity)
				{
				case M3TutorialActivity.M3MoveStart:
					return AreCoordsEqual(0, index1, index2);
				case M3TutorialActivity.M3MoveEnd:
					return AreCoordsEqual(coords.Count - 1, index1, index2);
				default:
					return IsM3MoveAcceptable(index1, index2);
				}
			case M3TutorialActivity.ItemUse:
			case M3TutorialActivity.ItemUseShowOnly:
			case M3TutorialActivity.ItemPreview:
				return index1 == itemIndex;
			case M3TutorialActivity.MobTarget:
			case M3TutorialActivity.MobPreview:
				return index1 == mobIndex;
			case M3TutorialActivity.M3WaitUntilExecuted:
				return IsM3Activity(activity) || !allowM3Only;
			default:
				return true;
			}
		}
		return false;
	}

	private bool IsActivityCompatible(M3TutorialActivity activity)
	{
		bool flag = activity == tutorialActivity;
		if (!flag && tutorialActivity == M3TutorialActivity.M3Move)
		{
			flag = (activity == M3TutorialActivity.M3MoveStart || activity == M3TutorialActivity.M3MoveEnd);
		}
		if (!flag && tutorialActivity == M3TutorialActivity.M3WaitUntilExecuted)
		{
			flag = (activity == M3TutorialActivity.M3MoveStart || activity == M3TutorialActivity.M3MoveEnd || activity == M3TutorialActivity.M3Move);
		}
		if (!flag && tutorialActivity == M3TutorialActivity.NextBattle)
		{
			flag = true;
		}
		if (!flag && tutorialActivity == M3TutorialActivity.WaitUntilEnemyTurn)
		{
			flag = true;
		}
		if (!flag && tutorialActivity == M3TutorialActivity.ItemUseShowOnly && activity == M3TutorialActivity.ItemUse)
		{
			flag = true;
		}
		return flag;
	}

	private bool AreCoordsEqual(int coordIndex, int column, int row)
	{
		if (coordIndex >= coords.Count)
		{
			return false;
		}
		return coords[coordIndex].x == column && coords[coordIndex].y == row;
	}

	private bool IsM3MoveAcceptable(int column, int row)
	{
		if (coords.Count >= 2)
		{
			bool flag = false;
			for (int i = 0; i < coords.Count - 1; i++)
			{
				flag = (flag || (column == coords[i].x && row >= coords[i].y && row <= coords[i + 1].y) || (column == coords[i].x && row >= coords[i + 1].y && row <= coords[i].y) || (row == coords[i].y && column >= coords[i].x && column <= coords[i + 1].x) || (row == coords[i].y && column >= coords[i + 1].x && column <= coords[i].x));
			}
			return flag;
		}
		return true;
	}

	public void Init()
	{
		GetComponentsInChildren(anims);
	}

	public void EnterStep(M3Tutorial tutorial)
	{
		if (tutorialActivity == M3TutorialActivity.ItemUseShowOnly || tutorialActivity == M3TutorialActivity.ItemUse)
		{
			foreach (M3TutorialAnimation anim in anims)
			{
				Transform transform = anim.transform;
				Vector3 position = M3Player.instance.Skills[itemIndex].transform.position;
				float x = position.x;
				Vector3 position2 = anim.transform.position;
				float y = position2.y;
				Vector3 position3 = anim.transform.position;
				transform.position = new Vector3(x, y, position3.z);
			}
		}
		foreach (M3TutorialAnimation anim2 in anims)
		{
			StartCoroutine(anim2.FadeIn());
		}
	}

	public void ExitStep(M3Tutorial tutorial)
	{
		foreach (M3TutorialAnimation anim in anims)
		{
			StartCoroutine(anim.FadeOut());
		}
		if (!string.IsNullOrEmpty(analyticsID))
		{
			AnalyticsManager.Design("TutorialM3", analyticsID);
		}
	}
}
