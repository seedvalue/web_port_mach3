using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class M3TutorialAnimM3Move : MonoBehaviour, IM3TutorialAnimationClient
{
	public SpriteRenderer prefabOrbMoveStart;

	public SpriteRenderer prefabOrbMoveEnd;

	public SpriteRenderer prefabOrbMoveLeft;

	public SpriteRenderer prefabOrbTurnUp;

	public float alphaFrequency = 0.5f;

	public float alphaBase = 0.7f;

	public float alphaAmplitude = 0.1f;

	private List<SpriteRenderer> orbPath = new List<SpriteRenderer>();

	private M3Board board;

	private bool active;

	private void Awake()
	{
		board = UnityEngine.Object.FindObjectOfType<M3Board>();
		M3TutorialStep componentInParent = GetComponentInParent<M3TutorialStep>();
		if ((bool)componentInParent && componentInParent.tutorialActivity == M3TutorialActivity.M3Move && componentInParent.coords.Count > 1)
		{
			List<IntVector2> list = new List<IntVector2>();
			list.Add(componentInParent.coords[0]);
			for (int i = 1; i < componentInParent.coords.Count; i++)
			{
				AddPathCoords(list, componentInParent.coords[i - 1], componentInParent.coords[i]);
			}
			IntVector2 intVector = list[0];
			for (int j = 1; j < list.Count; j++)
			{
				CreatePathPiece(intVector, list[j], (j != 1) ? list[j - 1] : new IntVector2(0, 0));
				intVector += list[j];
			}
			CreatePathPiece(intVector, new IntVector2(0, 0), list[list.Count - 1]);
		}
	}

	private void CreatePathPiece(IntVector2 pos, IntVector2 dirNext, IntVector2 dirCurr)
	{
		bool flag = dirNext.x + dirNext.y == 0;
		bool flag2 = dirCurr.x + dirCurr.y == 0;
		SpriteRenderer spriteRenderer = null;
		float angle = 0f;
		float y = 1f;
		float x = 1f;
		if (flag2)
		{
			spriteRenderer = prefabOrbMoveStart;
			if (dirNext.x == 1)
			{
				angle = 180f;
			}
			else if (dirNext.y == 1)
			{
				angle = 270f;
			}
			else if (dirNext.y == -1)
			{
				angle = 90f;
			}
		}
		else if (flag)
		{
			spriteRenderer = prefabOrbMoveEnd;
			if (dirCurr.x == 1)
			{
				angle = 180f;
			}
			else if (dirCurr.y == 1)
			{
				angle = 270f;
			}
			else if (dirCurr.y == -1)
			{
				angle = 90f;
			}
		}
		else
		{
			spriteRenderer = ((Mathf.Abs(dirCurr.x + dirNext.x) != 2 && Mathf.Abs(dirCurr.y + dirNext.y) != 2) ? prefabOrbTurnUp : prefabOrbMoveLeft);
			if (dirCurr.x == -1)
			{
				if (dirNext.y == -1)
				{
					y = -1f;
				}
			}
			else if (dirCurr.x == 1)
			{
				if (dirNext.x == 1)
				{
					x = -1f;
				}
				else if (dirNext.y == 1)
				{
					x = -1f;
				}
				else if (dirNext.y == -1)
				{
					x = -1f;
					y = -1f;
				}
			}
			else if (dirCurr.y == 1)
			{
				if (dirNext.y == 1)
				{
					angle = 270f;
				}
				else if (dirNext.x == 1)
				{
					angle = 270f;
				}
				else if (dirNext.x == -1)
				{
					angle = 90f;
					x = -1f;
				}
			}
			else if (dirNext.y == -1)
			{
				angle = 90f;
			}
			else if (dirNext.x == 1)
			{
				angle = 270f;
				x = -1f;
			}
			else if (dirNext.x == -1)
			{
				angle = 90f;
			}
		}
		SpriteRenderer original = spriteRenderer;
		float x2 = pos.x * board.tileSize + board.tileSize / 2;
		float y2 = (float)(pos.y * board.tileSize + board.tileSize / 2) + board.tileYOffset;
		float tileZ = board.tileZ;
		Vector3 position = spriteRenderer.transform.position;
		spriteRenderer = UnityEngine.Object.Instantiate(original, new Vector3(x2, y2, tileZ + position.z), Quaternion.AngleAxis(angle, Vector3.forward));
		spriteRenderer.transform.localScale = new Vector3(x, y, 1f);
		spriteRenderer.transform.SetParent(base.transform, worldPositionStays: true);
		orbPath.Add(spriteRenderer);
	}

	private void AddPathCoords(List<IntVector2> list, IntVector2 from, IntVector2 to)
	{
		IntVector2 intVector = new IntVector2(0, 0);
		if (from.x < to.x)
		{
			intVector.x = 1;
		}
		else if (from.x > to.x)
		{
			intVector.x = -1;
		}
		else if (from.y < to.y)
		{
			intVector.y = 1;
		}
		else
		{
			intVector.y = -1;
		}
		do
		{
			from += intVector;
			list.Add(intVector);
		}
		while (from.x != to.x || from.y != to.y);
	}

	private void Update()
	{
		if (active)
		{
			float alpha = alphaBase + alphaAmplitude * Mathf.Sin(Time.time * alphaFrequency * 2f * (float)Math.PI);
			foreach (SpriteRenderer item in orbPath)
			{
				Helpers.SetSpriteAlpha(item, alpha);
			}
		}
	}

	public void AfterFadeIn()
	{
		active = true;
	}

	public void BeforeFadeOut()
	{
		active = false;
	}
}
