using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M3Battle : MonoBehaviour
{
	[Serializable]
	public class EnemyDirLight
	{
		public Color color = Color.white;

		public Vector3 rotation;

		[Range(0f, 10f)]
		public float power;
	}

	private const int spawnersMax = 5;

	public bool navpointOnly;

	public float spawnBeforeFightTimeOffset = 1f;

	public float enemyDistance = 4.5f;

	public float cameraXAngle;

	public EnemyDirLight enemyDirLight = new EnemyDirLight();

	[HideInInspector]
	public float distance;

	private List<M3Mob> mobs = new List<M3Mob>();

	private int targetedMob = -1;

	private bool spawned;

	private M3TileManager tileManager;

	public List<M3Mob> Mobs => mobs;

	public int AliveMobCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < mobs.Count; i++)
			{
				if (mobs[i].IsAlive())
				{
					num++;
				}
			}
			return num;
		}
	}

	private float SetMobPosition(MetaStage.Battle battle, ref float xLeft, ref float xRight, ref int firstOdd, int i, M3Mob mob)
	{
		float capsuleWidth = mob.CapsuleWidth;
		float num = 0f;
		if (i == 0 && battle.mobs.Length % 2 == 1)
		{
			xLeft = capsuleWidth / 2f;
			xRight = capsuleWidth / 2f;
			firstOdd = 1;
		}
		else if ((i + firstOdd) % 2 == 0)
		{
			num = 0f - xLeft - capsuleWidth / 2f;
			xLeft += capsuleWidth;
		}
		else
		{
			num = xRight + capsuleWidth / 2f;
			xRight += capsuleWidth;
		}
		float z = enemyDistance;
		mob.transform.localPosition = new Vector3(num, 0f, z);
		return num;
	}

	private void OptimizeMobPositions()
	{
		float num = 0f;
		float num2 = 0f;
		foreach (M3Mob mob in mobs)
		{
			num += mob.CapsuleWidth;
			num2 += mob.CapsulePerfectWidth;
		}
		float x = 0f;
		float num3 = 0f;
		float num4 = 0f;
		int num5 = 0;
		if (num < tileManager.optimalBattleArea)
		{
			float num6 = 1f;
			if (num2 > tileManager.optimalBattleArea)
			{
				num6 = (tileManager.optimalBattleArea - num) / (num2 - num);
			}
			for (int i = 0; i < mobs.Count; i++)
			{
				float num7 = (mobs[i].CapsulePerfectWidth - mobs[i].CapsuleWidth) * num6;
				if (i == 0 && mobs.Count % 2 == 1)
				{
					num3 = num7 / 2f;
					num4 = num7 / 2f;
					num5 = 1;
				}
				else if ((i + num5) % 2 == 0)
				{
					x = 0f - num3 - num7 / 2f;
					num3 += num7;
				}
				else
				{
					x = num4 + num7 / 2f;
					num4 += num7;
				}
				mobs[i].transform.localPosition += new Vector3(x, 0f, 0f);
			}
		}
		num3 = 0f;
		num4 = 0f;
		foreach (M3Mob mob2 in mobs)
		{
			Vector3 localPosition = mob2.transform.localPosition;
			if (localPosition.x + mob2.CapsuleWidth / 2f > num4)
			{
				Vector3 localPosition2 = mob2.transform.localPosition;
				num4 = localPosition2.x + mob2.CapsuleWidth / 2f;
			}
			Vector3 localPosition3 = mob2.transform.localPosition;
			if (localPosition3.x - mob2.CapsuleWidth / 2f < num3)
			{
				Vector3 localPosition4 = mob2.transform.localPosition;
				num3 = localPosition4.x - mob2.CapsuleWidth / 2f;
			}
		}
		float num8 = (num3 + num4) / 2f;
		if (Mathf.Abs(num8) > float.Epsilon)
		{
			foreach (M3Mob mob3 in mobs)
			{
				mob3.transform.localPosition += new Vector3(0f - num8, 0f, 0f);
			}
		}
	}

	public void Prepare(MetaStage.Battle battle, MobStatsMultipliers stageMultipliers)
	{
		tileManager = M3TileManager.instance;
		if (battle != null)
		{
			mobs.Clear();
			float xLeft = 0f;
			float xRight = 0f;
			int firstOdd = 0;
			for (int i = 0; i < battle.mobs.Length; i++)
			{
				if (i < 5)
				{
					M3Mob component = UnityEngine.Object.Instantiate(battle.mobs[i].prefab, base.transform).GetComponent<M3Mob>();
					float num = SetMobPosition(battle, ref xLeft, ref xRight, ref firstOdd, i, component);
					if ((component.mirrorMode == M3MirrorMode.MirrorOnRight && num > 0f) || (component.mirrorMode == M3MirrorMode.MirrorOnLeft && num < 0f))
					{
						Vector3 localScale = component.transform.localScale;
						component.transform.localScale = new Vector3(0f - localScale.x, localScale.y, localScale.z);
					}
					if ((bool)component)
					{
						component.Init();
						mobs.Add(component);
						component.Prepare(battle.mobs[i], stageMultipliers * battle.statsMultipliers);
						component.transform.localRotation = Quaternion.LookRotation(Vector3.zero - component.transform.localPosition, Vector3.up);
						component.ToggleVisibility(visible: false);
						component.gameObject.SetActive(value: false);
					}
				}
				else
				{
					UnityEngine.Debug.LogWarning("Cannot instantiate mob, got too many metaMobs (" + battle.mobs.Length + ")");
				}
			}
			OptimizeMobPositions();
		}
		else
		{
			navpointOnly = true;
		}
		spawned = (mobs.Count == 0);
		ApplyEnemyDirectionalLight();
	}

	public bool Enter(Quaternion playerRotation)
	{
		M3Stage componentInParent = GetComponentInParent<M3Stage>();
		for (int i = 0; i < mobs.Count; i++)
		{
			ProgressBar hPBar = componentInParent.GetHPBar();
			mobs[i].EnterFight(hPBar, playerRotation);
		}
		return mobs.Count > 0;
	}

	public bool IsSpawned()
	{
		return spawned;
	}

	public IEnumerator SpawnSequence(float moveTimeApprox)
	{
		if (moveTimeApprox > spawnBeforeFightTimeOffset)
		{
			yield return new WaitForSeconds(moveTimeApprox - spawnBeforeFightTimeOffset);
		}
		for (int i = 0; i < mobs.Count; i++)
		{
			mobs[i].gameObject.SetActive(value: true);
			StartCoroutine(mobs[i].Spawn());
		}
		spawned = true;
		yield return 0;
	}

	public bool IsNavpoint()
	{
		return navpointOnly;
	}

	private int GetTargetedMob()
	{
		M3CombatSight m3CombatSight = UnityEngine.Object.FindObjectOfType<M3CombatSight>();
		if ((bool)m3CombatSight)
		{
			for (int i = 0; i < mobs.Count; i++)
			{
				if (mobs[i].IsAlive() && m3CombatSight.IsTarget(mobs[i]))
				{
					return i;
				}
			}
			return -1;
		}
		return -1;
	}

	public M3Mob GetTarget(M3Damage damage)
	{
		int num = GetTargetedMob();
		if (num < 0 || !damage.IsApplicable(mobs[num]))
		{
			num = CalculateBestTarget(damage, out int _);
		}
		if (num >= 0)
		{
			return mobs[num];
		}
		return null;
	}

	private int CalculateBestTarget(M3Damage damage, out int wound, List<int> damageDealt = null)
	{
		int num = wound = 0;
		int result = -1;
		for (int i = 0; i < mobs.Count; i++)
		{
			if (mobs[i].IsAlive() && (damageDealt == null || damageDealt[i] < mobs[i].HP) && damage.IsApplicable(mobs[i]))
			{
				int num2 = (damageDealt != null) ? damageDealt[i] : 0;
				num = Mathf.Min(mobs[i].GetRealDamage(damage), mobs[i].HP - num2);
				if (num > wound)
				{
					wound = num;
					result = i;
				}
			}
		}
		return result;
	}

	private void FireAtMob(int mobIndex, List<M3Damage> damage, List<int> damageDealt, bool fireIfUnableToKill)
	{
		M3Mob m3Mob = mobs[mobIndex];
		int num = 0;
		List<int> list = new List<int>();
		for (int i = 0; i < damage.Count; i++)
		{
			if (damage[i].IsApplicable(m3Mob))
			{
				list.Add(m3Mob.GetRealDamage(damage[i]));
				num += list[i];
			}
			else
			{
				list.Add(0);
			}
		}
		if (num < m3Mob.HP - damageDealt[mobIndex] && !fireIfUnableToKill)
		{
			return;
		}
		while (num > 0 && m3Mob.HP > damageDealt[mobIndex])
		{
			int num2 = 0;
			int num3 = 0;
			int num4 = -1;
			for (int j = 0; j < list.Count; j++)
			{
				if (list[j] > 0)
				{
					int num5 = Mathf.Min(list[j], m3Mob.HP - damageDealt[mobIndex]);
					int num6 = list[j] - num5;
					if (num5 > num2 || (num5 == num2 && num6 < num3))
					{
						num2 = num5;
						num3 = num6;
						num4 = j;
					}
				}
			}
			if (num4 > -1)
			{
				num -= list[num4];
				list[num4] = 0;
				List<int> list2;
				int index;
				(list2 = damageDealt)[index = mobIndex] = list2[index] + num2;
				damage[num4].targets.Add(m3Mob);
			}
		}
	}

	public void TryLethalShot(int mobIndex, List<M3Damage> damage, List<int> damageDealt)
	{
		M3Mob m3Mob = mobs[mobIndex];
		int num = int.MaxValue;
		int num2 = -1;
		for (int i = 0; i < damage.Count; i++)
		{
			if (damage[i].IsApplicable(m3Mob))
			{
				int realDamage = m3Mob.GetRealDamage(damage[i]);
				if (realDamage > m3Mob.HP - damageDealt[mobIndex] && realDamage < num)
				{
					num = realDamage;
					num2 = i;
				}
			}
		}
		if (num2 > -1)
		{
			List<int> list;
			int index;
			(list = damageDealt)[index = mobIndex] = list[index] + num;
			damage[num2].targets.Add(m3Mob);
		}
	}

	public void SetupAttackSequence(List<M3Damage> damage)
	{
		if (damage.Count <= 0)
		{
			return;
		}
		List<int> list = new List<int>();
		for (int i = 0; i < mobs.Count; i++)
		{
			list.Add(0);
		}
		targetedMob = GetTargetedMob();
		if (targetedMob >= 0 && mobs[targetedMob].IsAlive())
		{
			FireAtMob(targetedMob, damage, list, fireIfUnableToKill: true);
		}
		for (int j = 0; j < mobs.Count; j++)
		{
			if (mobs[j].IsAlive() && list[j] < mobs[j].HP && mobs[j].Cooldown == 1)
			{
				FireAtMob(j, damage, list, fireIfUnableToKill: false);
			}
		}
		for (int k = 0; k < mobs.Count; k++)
		{
			if (mobs[k].IsAlive() && list[k] < mobs[k].HP)
			{
				TryLethalShot(k, damage, list);
			}
		}
		for (int l = 0; l < damage.Count; l++)
		{
			if (!damage[l].IsApplicable(null))
			{
				continue;
			}
			int num;
			do
			{
				int wound = 0;
				num = CalculateBestTarget(damage[l], out wound, list);
				if (num > -1)
				{
					damage[l].targets.Add(mobs[num]);
					List<int> list2;
					int index;
					(list2 = list)[index = num] = list2[index] + wound;
				}
			}
			while (num != -1 && damage[l].targets.Count < damage[l].numTargets);
		}
		if (targetedMob == -1)
		{
			return;
		}
		List<M3Damage> list3 = new List<M3Damage>();
		List<M3Damage> list4 = new List<M3Damage>();
		for (int m = 0; m < damage.Count; m++)
		{
			if (damage[m].IsTarget(mobs[targetedMob]))
			{
				list3.Add(damage[m]);
			}
			else
			{
				list4.Add(damage[m]);
			}
		}
		damage.Clear();
		foreach (M3Damage item in list3)
		{
			damage.Add(item);
		}
		foreach (M3Damage item2 in list4)
		{
			damage.Add(item2);
		}
	}

	public bool IsOver()
	{
		bool flag = false;
		for (int i = 0; i < mobs.Count; i++)
		{
			flag = (flag || mobs[i].IsAlive());
		}
		return !flag;
	}

	public IEnumerator EnemyTurn(M3Player player)
	{
		for (int i = 0; i < mobs.Count; i++)
		{
			if (mobs[i].IsAlive() && !player.IsDead())
			{
				yield return StartCoroutine(mobs[i].PerformTurn(player));
			}
		}
	}

	public void Save(M3SaveBattle saveBattle)
	{
		saveBattle.saveMobs.Clear();
		for (int i = 0; i < mobs.Count; i++)
		{
			M3SaveMob item = new M3SaveMob(mobs[i].HP, mobs[i].Cooldown);
			saveBattle.saveMobs.Add(item);
		}
	}

	public bool Load(M3SaveBattle saveBattle, Quaternion playerRotation)
	{
		if (saveBattle.saveMobs.Count != mobs.Count)
		{
			return false;
		}
		for (int i = 0; i < mobs.Count; i++)
		{
			mobs[i].gameObject.SetActive(value: true);
		}
		Enter(playerRotation);
		for (int j = 0; j < mobs.Count; j++)
		{
			if (!mobs[j].Load(saveBattle.saveMobs[j]))
			{
				return false;
			}
		}
		return true;
	}

	protected virtual void Update()
	{
	}

	private void ApplyEnemyDirectionalLight()
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetColor("Kamis_DirectionalLightColor", enemyDirLight.color);
		materialPropertyBlock.SetVector("Kamis_DirectionalLightDir", Quaternion.Euler(enemyDirLight.rotation) * Vector3.back * enemyDirLight.power);
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(includeInactive: true);
		foreach (Renderer renderer in componentsInChildren)
		{
			renderer.SetPropertyBlock(materialPropertyBlock);
		}
	}
}
