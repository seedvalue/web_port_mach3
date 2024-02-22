using UnityEngine;

public class M3TargetPicker : MonoBehaviour
{
	private const float tapAndHoldTimeMin = 0.6f;

	public Camera stageCamera;

	public M3CombatSight combatSight;

	public float zoomInTime = 0.5f;

	public float zoomOutTime = 0.5f;

	private M3TileManager tileManager;

	private M3MobZoomer zoomer;

	private M3TargetMode targetMode;

	private float tapAndHoldTime;

	private M3Mob tapAndHoldMob;

	private void Start()
	{
		tileManager = UnityEngine.Object.FindObjectOfType<M3TileManager>();
		zoomer = UnityEngine.Object.FindObjectOfType<M3MobZoomer>();
	}

	private void OnM3MobWindowClose(Window window, object returnValue)
	{
		zoomer.ZoomOut(zoomOutTime);
	}

	private void Update()
	{
		Vector3 position = Vector3.zero;
		bool flag = false;
		bool flag2 = false;
		M3Mob m3Mob = null;
		if (tileManager.IsInteractionAllowed() || targetMode != 0)
		{
			flag = (UnityEngine.Input.touchCount > 0);
			if (flag)
			{
				position = Input.touches[0].position;
			}
		}
		if ((bool)tileManager && tileManager.IsInteractionAllowed())
		{
			if (flag)
			{
				tapAndHoldTime += Time.deltaTime;
			}
			if (flag)
			{
				Ray ray = stageCamera.ScreenPointToRay(position);
				if (Physics.Raycast(ray, out RaycastHit hitInfo))
				{
					m3Mob = hitInfo.collider.gameObject.GetComponent<M3Mob>();
					if ((bool)m3Mob && targetMode != M3TargetMode.ClearTarget)
					{
						tapAndHoldMob = m3Mob;
						bool flag3 = combatSight.SetTarget(m3Mob, targetMode);
						if (flag3 && targetMode != 0)
						{
							tapAndHoldTime = 0f;
						}
						if (targetMode == M3TargetMode.Undefined)
						{
							targetMode = (flag3 ? M3TargetMode.SetTarget : M3TargetMode.ClearTarget);
						}
					}
					else if ((bool)m3Mob)
					{
						if (m3Mob != tapAndHoldMob)
						{
							tapAndHoldMob = m3Mob;
							tapAndHoldTime = 0f;
						}
					}
					else
					{
						tapAndHoldTime = 0f;
					}
				}
				else
				{
					tapAndHoldTime = 0f;
				}
				if (targetMode == M3TargetMode.Undefined)
				{
					targetMode = M3TargetMode.SetTarget;
				}
			}
			if (tapAndHoldTime > 0.6f && tileManager.TutorialAllows(M3TutorialActivity.MobPreview, tileManager.FindMobIndex(tapAndHoldMob)))
			{
				if (!tapAndHoldMob)
				{
					UnityEngine.Debug.LogWarning("Brak moba!!!");
				}
				tileManager.OpenWindow<M3MobWindow>(tapAndHoldMob, OnM3MobWindowClose);
				zoomer.ZoomIn(m3Mob, zoomInTime);
				flag2 = true;
				tileManager.TutorialActivityFinished(M3TutorialActivity.MobPreview);
			}
		}
		if (UnityEngine.Input.touchCount == 0)
		{
			flag2 = true;
		}
		if (flag2)
		{
			targetMode = M3TargetMode.Undefined;
			tapAndHoldTime = 0f;
			tapAndHoldMob = null;
		}
	}
}
