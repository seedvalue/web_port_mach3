using System.Collections;
using UnityEngine;

public class M3PlayerMover : MonoBehaviour
{
	private const float angleAccAbs = 45f;

	public float moveSpeed = 4f;

	public float moveSpeedDampTime = 1f;

	public int pathSamplingFrequency = 60;

	private float moveTime;

	private float moveTimeFull;

	private float moveSpeedDampTimeActual;

	private bool dampBeginning;

	private bool dampEnding;

	private bool movingTowardsBattle;

	private float distance;

	private float angle;

	private float angleDest;

	private float angleVel;

	private float speedPrev;

	private bool endingTurn;

	private M3SmartPath path;

	private M3StageAnimator anim;

	private M3StageCamera stageCamera;

	private M3HeadStrafer headStrafer;

	private M3HeadUpDown headUpDown;

	public void Init(M3Stage stage)
	{
		stageCamera = GetComponentInChildren<M3StageCamera>();
		if (!stageCamera)
		{
			UnityEngine.Debug.LogWarning("No M3StageCamera found, cannot visualize Stage!");
		}
		path = stage.GetComponent<M3SmartPath>();
		distance = 0f;
		M3PathPoint m3PathPoint = path.Evaluate(distance);
		base.transform.position = m3PathPoint.position;
		base.transform.rotation = Quaternion.AngleAxis(m3PathPoint.pitch, Vector3.up);
		angle = m3PathPoint.pitch;
		angleDest = m3PathPoint.pitch;
		headStrafer = UnityEngine.Object.FindObjectOfType<M3HeadStrafer>();
		headUpDown = UnityEngine.Object.FindObjectOfType<M3HeadUpDown>();
		anim = UnityEngine.Object.FindObjectOfType<M3StageAnimator>();
	}

	private float CalcActualSpeed(float moveTimeFull, float moveTime, float speedDampTime, float baseSpeed, bool startDamped, bool endDamped)
	{
		float result = baseSpeed;
		if (speedDampTime > float.Epsilon && moveTime < moveTimeFull)
		{
			if (moveTime < speedDampTime && startDamped)
			{
				result = Mathf.Lerp(0f, baseSpeed, moveTime / speedDampTime);
			}
			else if (moveTime > moveTimeFull - speedDampTime && endDamped)
			{
				result = Mathf.Lerp(baseSpeed, 0f, (moveTime - (moveTimeFull - speedDampTime)) / speedDampTime);
			}
		}
		if (moveTime < moveTimeFull || !endDamped)
		{
			return result;
		}
		return 0f;
	}

	private void UpdateStageAnim()
	{
		if ((bool)anim)
		{
			anim.SetDistance(distance / path.Length);
		}
	}

	private void UpdateRotationDuringMovement()
	{
		angle = Mathf.SmoothDampAngle(angle, angleDest, ref angleVel, 0.3f);
		base.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
		endingTurn = (Mathf.Abs(angle - angleDest) > float.Epsilon);
	}

	private void Update()
	{
		if (movingTowardsBattle)
		{
			moveTime += Time.deltaTime;
			if (moveTime >= moveTimeFull)
			{
				moveTime = moveTimeFull;
				movingTowardsBattle = false;
			}
			float num = CalcActualSpeed(moveTimeFull, moveTime, moveSpeedDampTimeActual, moveSpeed, dampBeginning, dampEnding);
			float num2 = Time.deltaTime * (num + speedPrev) / 2f;
			speedPrev = num;
			distance += num2;
			M3PathPoint m3PathPoint = path.Evaluate(distance);
			base.transform.position = m3PathPoint.position;
			angleDest = m3PathPoint.pitch;
			UpdateStageAnim();
			if (movingTowardsBattle)
			{
				stageCamera.SetDamping(num / moveSpeed);
			}
			else
			{
				stageCamera.Stop();
			}
		}
		if (movingTowardsBattle || endingTurn)
		{
			UpdateRotationDuringMovement();
		}
	}

	public void RunHeadStrafer(M3Battle battle)
	{
		Vector3 lookAtPoint;
		if (battle.Mobs.Count % 2 == 1)
		{
			lookAtPoint = battle.Mobs[0].transform.position;
		}
		else
		{
			lookAtPoint = battle.Mobs[0].transform.position + battle.Mobs[1].transform.position;
			lookAtPoint *= 0.5f;
		}
		headStrafer.Strafe(lookAtPoint);
	}

	public IEnumerator RunHeadUpDown(float delay, float moveDuration, float angle)
	{
		if (delay > float.Epsilon)
		{
			yield return new WaitForSeconds(delay);
		}
		if ((bool)headUpDown)
		{
			headUpDown.HeadUp(angle, moveDuration);
		}
	}

	public IEnumerator GoToBattle(M3Battle battle, bool slowStart)
	{
		if (slowStart && (bool)headStrafer)
		{
			headStrafer.Stop();
		}
		if (moveSpeed < float.Epsilon)
		{
			moveSpeed = 1f;
		}
		dampBeginning = slowStart;
		dampEnding = !battle.IsNavpoint();
		float distanceFull = battle.distance - distance;
		moveTimeFull = distanceFull / moveSpeed;
		moveSpeedDampTimeActual = Mathf.Min(moveSpeedDampTime, moveTimeFull);
		if (dampBeginning)
		{
			moveTimeFull += moveSpeedDampTimeActual / 2f;
		}
		if (dampEnding)
		{
			moveTimeFull += moveSpeedDampTimeActual / 2f;
		}
		moveTime = 0f;
		if ((bool)headUpDown)
		{
			headUpDown.HeadUp(battle.cameraXAngle, moveTimeFull);
		}
		if (!battle.IsNavpoint())
		{
			StartCoroutine(battle.SpawnSequence(moveTimeFull));
		}
		movingTowardsBattle = true;
		if ((bool)stageCamera)
		{
			stageCamera.Walk();
		}
		yield return new WaitWhile(() => movingTowardsBattle || ((bool)battle && !battle.IsSpawned()));
		if (!battle.IsNavpoint() && (bool)headStrafer)
		{
			RunHeadStrafer(battle);
		}
	}

	public void Save(M3SavePlayer savePlayer)
	{
		savePlayer.distance = distance;
		for (int i = 0; i < 3; i++)
		{
			savePlayer.position[i] = base.transform.position[i];
		}
		savePlayer.angle = angle;
	}

	public bool Load(M3SavePlayer savePlayer)
	{
		distance = savePlayer.distance;
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < 3; i++)
		{
			zero[i] = savePlayer.position[i];
		}
		base.transform.position = zero;
		angle = (angleDest = savePlayer.angle);
		UpdateRotationDuringMovement();
		UpdateStageAnim();
		return true;
	}
}
