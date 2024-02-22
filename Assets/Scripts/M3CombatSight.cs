using UnityEngine;

public class M3CombatSight : MonoBehaviour
{
	public GameObject fxPrefab;

	public float rotateSpeed = 90f;

	private M3TileManager tileManager;

	private M3Mob target;

	private MeshRenderer mesh;

	private GameObject fx;

	private float angle;

	private void Start()
	{
		mesh = GetComponentInChildren<MeshRenderer>();
		tileManager = UnityEngine.Object.FindObjectOfType<M3TileManager>();
		if ((bool)fxPrefab)
		{
			fx = UnityEngine.Object.Instantiate(fxPrefab);
		}
		else
		{
			fx = null;
		}
	}

	public bool IsTarget(M3Mob mob)
	{
		return target == mob;
	}

	private void ToggleHPBarTargeter(M3Mob mob, bool visible)
	{
		if ((bool)mob)
		{
			M3HPBarTargeter component = mob.hpProgressBar.GetComponent<M3HPBarTargeter>();
			if ((bool)component)
			{
				component.ToggleArrows(visible);
			}
		}
	}

	public bool SetTarget(M3Mob newTarget, M3TargetMode mode)
	{
		if (IsTarget(newTarget))
		{
			if (mode != M3TargetMode.SetTarget && tileManager.TutorialAllows(M3TutorialActivity.MobTarget, tileManager.FindMobIndex(newTarget)))
			{
				tileManager.TutorialActivityFinished(M3TutorialActivity.MobTarget);
				ToggleHPBarTargeter(target, visible: false);
				target = null;
			}
			return false;
		}
		if (mode != M3TargetMode.ClearTarget && tileManager.TutorialAllows(M3TutorialActivity.MobTarget, tileManager.FindMobIndex(newTarget)))
		{
			tileManager.TutorialActivityFinished(M3TutorialActivity.MobTarget);
			ToggleHPBarTargeter(target, visible: false);
			ToggleHPBarTargeter(newTarget, visible: true);
			target = newTarget;
			return true;
		}
		return false;
	}

	public M3Mob GetTarget()
	{
		return target;
	}

	private void Update()
	{
		if ((bool)target && target.IsAlive())
		{
			mesh.enabled = true;
			angle += rotateSpeed * Time.deltaTime;
			while (angle > 360f)
			{
				angle -= 360f;
			}
			Transform transform = base.transform;
			Vector3 position = target.transform.position;
			float x = position.x;
			Vector3 position2 = target.transform.position;
			float y = position2.y;
			Vector3 position3 = target.transform.position;
			transform.position = new Vector3(x, y, position3.z);
			base.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
			if ((bool)fx)
			{
				fx.SetActive(value: true);
				fx.transform.position = base.transform.position;
			}
		}
		else
		{
			if ((bool)target)
			{
				ToggleHPBarTargeter(target, visible: false);
				target = null;
			}
			mesh.enabled = false;
			if ((bool)fx)
			{
				fx.SetActive(value: false);
			}
		}
	}
}
