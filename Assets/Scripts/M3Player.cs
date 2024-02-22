using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class M3Player : MonoBehaviour
{
	[Serializable]
	public class Timings
	{
		public float attackAnimationDuration = 0.2f;

		public float labelAnimTime = 0.4f;

		public float labelShadeTime = 0.25f;

		public float rotationTime = 0.3f;

		public float woundsHideDelay = 0.6f;

		public float actualAttackAnimationDuration => attackAnimationDuration * M3TileManager.instance.debugTimeMultiplier;

		public float actualLabelAnimTime => labelAnimTime * M3TileManager.instance.debugTimeMultiplier;

		public float actualLabelShadeTime => labelShadeTime * M3TileManager.instance.debugTimeMultiplier;

		public float actualRotationTime => rotationTime * M3TileManager.instance.debugTimeMultiplier;

		public float actualWoundsHideDelay => woundsHideDelay * M3TileManager.instance.debugTimeMultiplier;
	}

	public static M3Player instance;

	public Timings timings = new Timings();

	public ProgressBar barHP;

	public ProgressBar barHPHealed;

	public Text textHP;

	public M3SmartLabel textWound;

	public Stats debugStatBoosts = new Stats();

	public M3ActionEffect actionOnHitPrefab;

	public AnimationCurve comboCurve;

	[HideInInspector]
	public M3ActionEffect[] actionOnAttackPrefabs = new M3ActionEffect[6];

	[HideInInspector]
	public M3SmartLabel[] texts = new M3SmartLabel[6];

	private int hp;

	private bool inSequence;

	private Stats stats;

	private List<M3Damage> damage = new List<M3Damage>();

	private M3Damage wound;

	private M3Damage rcv;

	private M3WoundFlare woundFlare;

	public List<M3DefenseModifier> defModifiers = new List<M3DefenseModifier>();

	private List<M3SkillView> skills = new List<M3SkillView>();

	[HideInInspector]
	public List<MetaPerk> perks = new List<MetaPerk>();

	private M3PlayerMover mover;

	private M3StageCamera stageCamera;

	private M3HeadStrafer headStrafer;

	public int Defense => stats.Def;

	public int Recovery => stats.Rcv;

	public List<M3SkillView> Skills => skills;

	public bool Initialized => mover != null;

	private void ClearLabels()
	{
		for (int i = 0; i < texts.Length; i++)
		{
			if ((bool)texts[i])
			{
				texts[i].BackToBase(stats[i]);
			}
		}
		texts[5].BackToBase(0);
		if ((bool)textWound)
		{
			textWound.BackToBase(0);
		}
	}

	private void HideLabel(M3Orb type)
	{
		if ((bool)texts[(int)type])
		{
			if (type == M3Orb.Recovery)
			{
				texts[(int)type].BackToBase(0);
			}
			else
			{
				texts[(int)type].BackToBase(stats[(int)type]);
			}
		}
	}

	private void ToggleLabelShading(bool shading, float shadeTime)
	{
		for (int i = 0; i < texts.Length; i++)
		{
			if ((bool)texts[i])
			{
				if (GetStat((M3Orb)i) > 0)
				{
					texts[i].ToggleShade(shading: false, 0f);
				}
				else
				{
					texts[i].ToggleShade(shading: true, 0f);
				}
			}
		}
	}

	public Vector3 GetDamageLabelPos(M3Orb orbType)
	{
		return texts[(int)orbType].transform.position;
	}

	public void InitItems(List<MetaItem> metaItems, MetaContainer container, M3SkillView.OnSkillChosen skillChosenHandler)
	{
		for (int i = 0; i < metaItems.Count; i++)
		{
			perks.AddRange(metaItems[i].perks);
		}
		skills.Clear();
		for (int j = 0; j < container.contents.Count; j++)
		{
			skills.Add(container.contents[j] as M3SkillView);
			skills[skills.Count - 1].Init();
			skills[skills.Count - 1].SetOnSkillChosen(skillChosenHandler);
		}
	}

	public void Init(Stats metaStats)
	{
		stats = metaStats;
		hp = stats.HP;
		mover = GetComponent<M3PlayerMover>();
		stageCamera = UnityEngine.Object.FindObjectOfType<M3StageCamera>();
		headStrafer = UnityEngine.Object.FindObjectOfType<M3HeadStrafer>();
		woundFlare = UnityEngine.Object.FindObjectOfType<M3WoundFlare>();
		ClearLabels();
		instance = this;
		ToggleLabelShading(shading: false, 0f);
	}

	public void BoostStats(Stats boosts)
	{
		stats += boosts;
		hp = stats.HP;
	}

	public int GetStat(M3Orb attackType)
	{
		switch (attackType)
		{
		case M3Orb.Pyromancy:
			return stats.Prm;
		case M3Orb.Melee:
			return stats.Mel;
		case M3Orb.Void:
			return stats.Vd;
		case M3Orb.Ranged:
			return stats.Rng;
		case M3Orb.Harmony:
			return stats.Hrm;
		case M3Orb.Recovery:
			return stats.Rcv;
		default:
			return 0;
		}
	}

	public bool IsDead()
	{
		return hp <= 0;
	}

	private void Update()
	{
		if (Initialized)
		{
			if ((bool)barHP)
			{
				barHP.SetProgress((float)hp / (float)stats.HP);
			}
			if ((bool)textHP)
			{
				textHP.text = "HP: " + hp + "/" + stats.HP;
			}
		}
	}

	public IEnumerator ShowHealing(int hpHealed, float duration)
	{
		textWound.SetLabelColor(Color.green);
		textWound.prefix = "+";
		textWound.BackToBase(0);
		textWound.AddValue(hpHealed, duration);
		yield return new WaitForSeconds(timings.actualWoundsHideDelay + duration);
		textWound.BackToBase(0);
	}

	public IEnumerator FireActionEffect(GameObject actionEffectPrefab, List<M3Mob> targets, M3Orb powerTransferLabel = M3Orb.None, bool singleAttacks = false)
	{
		Transform powerTransferStartPoint = null;
		if (powerTransferLabel != M3Orb.None)
		{
			powerTransferStartPoint = texts[(int)powerTransferLabel].transform;
		}
		if ((bool)actionEffectPrefab)
		{
			GameObject obj = UnityEngine.Object.Instantiate(actionEffectPrefab, base.transform, worldPositionStays: false);
			M3ActionEffect effect = obj.GetComponent<M3ActionEffect>();
			if ((bool)effect)
			{
				effect.fxPowerTransfer.startPoint = powerTransferStartPoint;
				yield return StartCoroutine(effect.Trigger(base.transform, stageCamera.transform, targets));
			}
		}
	}

	public bool AttackSequenceIsStarted()
	{
		return inSequence;
	}

	public void AttackSequenceStart(bool structuresExist)
	{
		inSequence = true;
		for (int i = 0; i < perks.Count; i++)
		{
			perks[i].OnM3Start();
		}
		damage.Clear();
		for (int j = 0; j < 6; j++)
		{
			damage.Add(new M3Damage((M3Orb)j));
		}
	}

	public int AttackSequenceM3DamageDealt(bool includeRecovery)
	{
		int num = 0;
		for (int i = 0; i < damage.Count; i++)
		{
			if (includeRecovery || damage[i].orb != M3Orb.Recovery)
			{
				num += damage[i].damage;
			}
		}
		return num;
	}

	private M3Damage GetM3DamageOfOrb(M3Orb orbType)
	{
		for (int i = 0; i < damage.Count; i++)
		{
			if (damage[i].orb == orbType)
			{
				return damage[i];
			}
		}
		return null;
	}

	private void UpdateWhiteHPBar(int hpHealed)
	{
		if ((bool)barHPHealed && hpHealed > 0)
		{
			barHPHealed.SetProgress((float)(hp + hpHealed) / (float)stats.HP);
		}
	}

	public int AttackSequenceAddM3Damage(int orbCount, M3Orb attackType, List<M3AttackModifier> modifiers)
	{
		float num = (float)orbCount / 3f * (float)GetStat(attackType);
		if (num > 0f)
		{
			M3Damage m3DamageOfOrb = GetM3DamageOfOrb(attackType);
			num *= m3DamageOfOrb.multiplier;
			for (int i = 0; i < modifiers.Count; i++)
			{
				M3AttackModifier m3AttackModifier = modifiers[i];
				if (m3AttackModifier.filterKeywords.Count > 0)
				{
					if (m3AttackModifier.range == M3AttackModifierRange.Orb)
					{
						m3DamageOfOrb.modifiers.Add(m3AttackModifier);
						continue;
					}
					for (int j = 0; j < 5; j++)
					{
						damage[j].modifiers.Add(m3AttackModifier);
					}
					continue;
				}
				num *= m3AttackModifier.damageMultiplier;
				if (m3AttackModifier.range == M3AttackModifierRange.Orb)
				{
					m3DamageOfOrb.ApplyModifier(m3AttackModifier);
				}
				else if (m3AttackModifier.range == M3AttackModifierRange.Global)
				{
					for (int k = 0; k < 5; k++)
					{
						damage[k].ApplyModifier(m3AttackModifier);
					}
				}
			}
			int num2 = Mathf.RoundToInt(num);
			m3DamageOfOrb.damage += num2;
			texts[(int)attackType].AddValue(num2, timings.actualLabelAnimTime);
			UpdateWhiteHPBar(damage[5].damage);
			return num2;
		}
		return 0;
	}

	public IEnumerator AnimateAttack(M3Damage attack, List<IPerkAnimateAttackHandler> handlers)
	{
		M3TileManager.Log("M3Player.AnimateAttack: Begin");
		for (int i = 0; i < handlers.Count; i++)
		{
			handlers[i].OnAnimateAttack(attack, this);
		}
		yield return StartCoroutine(FireActionEffect(actionOnAttackPrefabs[(int)attack.orb].gameObject, attack.targets, attack.orb));
		HideLabel(attack.orb);
		for (int j = 0; j < attack.targets.Count; j++)
		{
			StartCoroutine(attack.targets[j].Wound(attack));
		}
		yield return new WaitForSeconds(timings.actualAttackAnimationDuration);
		M3TileManager.Log("M3Player.AnimateAttack: End");
	}

	public bool AnyDamageLocked()
	{
		for (int i = 0; i < texts.Length; i++)
		{
			if (texts[i].IsLocked())
			{
				return true;
			}
		}
		return false;
	}

	public IEnumerator CoApplyCombo(int level, M3Orb orb)
	{
		M3TileManager.Log("M3Player.CoApplyCombo: Begin");
		if (level > 1)
		{
			M3Damage dmgObj = null;
			M3SmartLabel lbl = null;
			for (int i = 0; i < damage.Count; i++)
			{
				if (damage[i].orb == orb)
				{
					dmgObj = damage[i];
					lbl = texts[i];
					break;
				}
			}
			int dmg = Mathf.RoundToInt((float)dmgObj.damage * 0.25f * (float)(level - 1));
			dmgObj.damage += dmg;
			lbl.AddValue(dmg, timings.actualLabelAnimTime);
			if (orb == M3Orb.Recovery)
			{
				UpdateWhiteHPBar(dmgObj.damage);
			}
			yield return new WaitUntil(() => !lbl.IsLocked());
		}
		M3TileManager.Log("M3Player.CoApplyCombo: End");
	}

	public List<M3Damage> GetAccumulatedDamage()
	{
		return damage;
	}

	public IEnumerator PrepareAttacks(M3Battle battle)
	{
		M3TileManager.Log("M3Player.PrepareAttacks: Begin");
		yield return new WaitUntil(() => !AnyDamageLocked());
		rcv = GetM3DamageOfOrb(M3Orb.Recovery);
		if (rcv != null && rcv.damage == 0)
		{
			rcv = null;
		}
		for (int num = damage.Count - 1; num >= 0; num--)
		{
			if (damage[num].damage == 0 || damage[num].orb == M3Orb.Recovery)
			{
				damage.RemoveAt(num);
			}
		}
		battle.SetupAttackSequence(damage);
		M3TileManager.Log("M3Player.PrepareAttacks: End");
	}

	public IEnumerator AnimateAttacks()
	{
		M3TileManager.Log("M3Player.AnimateAttacks: Begin");
		headStrafer.Paused = true;
		List<IPerkAnimateAttackHandler> handlers = new List<IPerkAnimateAttackHandler>();
		for (int j = 0; j < perks.Count; j++)
		{
			IPerkAnimateAttackHandler perkAnimateAttackHandler = perks[j] as IPerkAnimateAttackHandler;
			if (perkAnimateAttackHandler != null)
			{
				handlers.Add(perkAnimateAttackHandler);
			}
		}
		for (int i = 0; i < damage.Count; i++)
		{
			if (damage[i].targets.Count > 0)
			{
				yield return StartCoroutine(AnimateAttack(damage[i], handlers));
			}
		}
		headStrafer.Paused = false;
		M3TileManager.Log("M3Player.AnimateAttacks: End");
	}

	public IEnumerator Heal()
	{
		M3TileManager.Log("M3Player.Heal: Begin");
		if (rcv != null)
		{
			int hpHealed = rcv.damage;
			yield return StartCoroutine(FireActionEffect(actionOnAttackPrefabs[5].gameObject, null));
			HealHP(hpHealed);
			if ((bool)barHPHealed)
			{
				barHPHealed.SetProgress(0f);
			}
			HideLabel(M3Orb.Recovery);
		}
		M3TileManager.Log("M3Player.Heal: End");
	}

	public void HealHP(int hpHealed)
	{
		hp += hpHealed;
		if (hp > stats.HP)
		{
			hp = stats.HP;
		}
	}

	public IEnumerator Wound(M3Mob attacker)
	{
		M3TileManager.Log("M3Player.Wound: Begin");
		int dmg = Mathf.Max(attacker.MetaStats.atk - stats.Def, 1);
		hp -= dmg;
		wound.damage += dmg;
		textWound.SetLabelColor(Color.red);
		textWound.prefix = "-";
		textWound.AddValue(dmg, timings.actualLabelAnimTime);
		List<M3Mob> attackers = new List<M3Mob>
		{
			attacker
		};
		if ((bool)woundFlare)
		{
			woundFlare.Hit();
		}
		yield return StartCoroutine(FireActionEffect(actionOnHitPrefab.gameObject, attackers));
		M3TileManager.Log("M3Player.Wound: End");
	}

	public IEnumerator EnemyTurn(M3Battle battle)
	{
		M3TileManager.Log("M3Player.EnemyTurn: Begin");
		wound = new M3Damage(M3Orb.None);
		yield return StartCoroutine(battle.EnemyTurn(this));
		yield return new WaitUntil(() => !textWound.IsLocked());
		Invoke("ClearLabels", timings.actualWoundsHideDelay);
		M3TileManager.Log("M3Player.EnemyTurn: End");
	}

	public IEnumerator NewTurn(bool updateSkills = true)
	{
		if (updateSkills)
		{
			for (int i = 0; i < skills.Count; i++)
			{
				skills[i].NewTurn();
			}
		}
		yield return 0;
	}

	public IEnumerator AttackSequenceEnd()
	{
		M3TileManager.Log("M3Player.AttackSequenceEnd: Begin");
		yield return null;
		inSequence = false;
		ClearLabels();
		M3TileManager.Log("M3Player.AttackSequenceEnd: End");
	}

	public IEnumerator GoToBattle(M3Battle battle, bool slowStart)
	{
		yield return StartCoroutine(mover.GoToBattle(battle, slowStart));
		battle.Enter(base.transform.rotation);
	}

	public void InitStage(M3Stage stage, MetaStage metaStage, MetaStageDifficulty difficulty)
	{
		stage.Enter(metaStage, difficulty);
		mover.Init(stage);
	}

	public IEnumerator EndStage(M3Stage stage)
	{
		yield return new WaitForSeconds(1f);
		yield return 0;
	}

	public IEnumerator Debug_KillAll(List<M3Mob> mobs)
	{
		for (int i = 0; i < mobs.Count; i++)
		{
			M3Damage m3Damage = new M3Damage(M3Orb.Melee);
			m3Damage.targets.Add(mobs[i]);
			m3Damage.numTargets = 1;
			m3Damage.damage = int.MaxValue;
			damage.Add(m3Damage);
		}
		yield return StartCoroutine(AnimateAttacks());
		damage.Clear();
	}

	public void Save(M3SavePlayer savePlayer)
	{
		savePlayer.hp = hp;
		savePlayer.skillCooldowns.Clear();
		for (int i = 0; i < skills.Count; i++)
		{
			savePlayer.skillCooldowns.Add(skills[i].Cooldown);
		}
		mover.Save(savePlayer);
	}

	public bool Load(M3SavePlayer savePlayer)
	{
		hp = savePlayer.hp;
		if (hp <= 0 || hp > stats.HP)
		{
			return false;
		}
		if (skills.Count == savePlayer.skillCooldowns.Count)
		{
			for (int i = 0; i < savePlayer.skillCooldowns.Count; i++)
			{
				skills[i].Cooldown = savePlayer.skillCooldowns[i];
			}
			return mover.Load(savePlayer);
		}
		return false;
	}
}
