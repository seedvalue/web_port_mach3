using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class M3Mob : MonoBehaviour
{
	[Serializable]
	public class HpBar
	{
		public float scale = 1f;

		public float yOffset = 3f;

		public float zOffset = 0.5f;
	}

	[Serializable]
	public class Zoom
	{
		public float yAngle;

		public float yPos = 1.5f;

		public float distance = 3f;
	}

	[Serializable]
	public class FX
	{
		public GameObject prefab;

		public Transform spawnTransform;

		public bool instantiateInWorldSpace = true;
	}

	[Serializable]
	public class Sounds
	{
		public AudioSample sfxSpawn;

		public AudioSample sfxAttack;

		public AudioSample sfxDeath;
	}

	[Serializable]
	public class Outline
	{
		public float delayFromSpawn;

		public float duration;

		public AnimationCurve width;

		public AnimationCurve alpha;

		public AnimationCurve shadow;

		public float fadeOutDelay;

		public float fadeOutDuration;
	}

	[Serializable]
	public class Destro
	{
		public float delay;

		public float duration = 1f;
	}

	private class Flags
	{
		public bool damageDealt;

		public bool isIdle;

		public bool outlineOn;
	}

	private class Hashes
	{
		public int triggerHit;

		public int triggerDie;

		public int triggerDieInstant;

		public int triggerAttack;

		public int triggerSpawn;

		public int triggerIdleAction;

		public int triggerIdle;

		public int stateIdle;

		public int stateDead;

		public int stateHit;

		public int stateDeath;

		public int stateAttack;

		public int stateSpawn;

		public int paramSpeed;

		public int paramVariant;

		public int paramVariantCount;
	}

	private const string strAttackIn = "AttackIn";

	private const string strTriggerDie = "Death";

	private const string strTriggerHit = "Hit";

	private const string strTriggerAttack = "Attack";

	private const string strTriggerSpawn = "Spawn";

	private const string strTriggerDieInstant = "InstantDeath";

	private const string strTriggerIdleAction = "IdleAction";

	private const string strTriggerIdle = "Idle";

	private const string strStateIdle = "Base.IDLE";

	private const string strStateDead = "Base.DEAD";

	private const string strStateHit = "Base.HIT";

	private const string strStateDeath = "Base.DEATH";

	private const string strStateAttack = "Base.ATTACK";

	private const string strStateSpawn = "Base.SPAWN";

	private const string strParamSpeed = "Speed";

	private const string strParamVariant = "Variant";

	private const string strParamVariantCount = "VariantCount";

	private const float speedRandomizePercent = 10f;

	public HpBar hpBar = new HpBar();

	public Zoom zoom = new Zoom();

	public FX spawnFx = new FX();

	public FX deathFx = new FX();

	public List<FX> attackFxs;

	public Sounds sounds;

	public Outline outline = new Outline();

	private List<MeshRenderer> shadowMeshes = new List<MeshRenderer>();

	public Destro destro = new Destro();

	public float capsuleWidth = 2f;

	public float capsulePerfectWidth = 3f;

	public float fxHitY = 1.6f;

	public M3MirrorMode mirrorMode;

	public float idleIntervalConst = 8f;

	public float idleIntervalRandom = 8f;

	public M3MoveShaker shakerOnSpawn;

	public M3MoveShaker shakerOnDeath;

	[HideInInspector]
	public MetaMob metaMob;

	private MobStats metaStats = new MobStats();

	private int hp;

	private Collider mobCollider;

	private Flags flags = new Flags();

	private Hashes hashes = new Hashes();

	private int turnsToAttack;

	[HideInInspector]
	public ProgressBar hpProgressBar;

	private Text turnsText;

	private M3AttackWarning attackWarning;

	private M3Stage stage;

	private List<Animator> anims = new List<Animator>();

	private List<SkinnedMeshRenderer> skinnedMeshes = new List<SkinnedMeshRenderer>();

	private List<Material> skinnedMeshMaterials = new List<Material>();

	private float nextIdleAction;

	public float CapsuleWidth => capsuleWidth;

	public float CapsulePerfectWidth => capsulePerfectWidth;

	public MobStats MetaStats => metaStats;

	public int HP => hp;

	public int Cooldown
	{
		get
		{
			return turnsToAttack;
		}
		set
		{
			if (value > 1 && turnsToAttack == 1)
			{
				attackWarning.Fade(fadeIn: false);
			}
			turnsToAttack = value;
		}
	}

	public void Init()
	{
		GetComponentsInChildren(anims);
		if (anims.Count > 0)
		{
			hashes.triggerHit = Animator.StringToHash("Hit");
			hashes.triggerDie = Animator.StringToHash("Death");
			hashes.triggerDieInstant = Animator.StringToHash("InstantDeath");
			hashes.triggerAttack = Animator.StringToHash("Attack");
			hashes.triggerSpawn = Animator.StringToHash("Spawn");
			hashes.triggerIdleAction = Animator.StringToHash("IdleAction");
			hashes.triggerIdle = Animator.StringToHash("Idle");
			hashes.stateIdle = Animator.StringToHash("Base.IDLE");
			hashes.stateDead = Animator.StringToHash("Base.DEAD");
			hashes.stateHit = Animator.StringToHash("Base.HIT");
			hashes.stateDeath = Animator.StringToHash("Base.DEATH");
			hashes.stateAttack = Animator.StringToHash("Base.ATTACK");
			hashes.stateSpawn = Animator.StringToHash("Base.SPAWN");
			hashes.paramSpeed = Animator.StringToHash("Speed");
			hashes.paramVariant = Animator.StringToHash("Variant");
			hashes.paramVariantCount = Animator.StringToHash("VariantCount");
		}
		else
		{
			UnityEngine.Debug.LogWarning("Animator missing, mob will not be animated properly!");
		}
		GetComponentsInChildren(skinnedMeshes);
		skinnedMeshMaterials = skinnedMeshes.SelectMany((SkinnedMeshRenderer m) => m.materials).ToList();
		mobCollider = GetComponent<Collider>();
		if ((bool)mobCollider)
		{
			mobCollider.enabled = false;
		}
		else
		{
			UnityEngine.Debug.LogWarning("Mob Collider not found, targeting this mob will be impossible!");
		}
		stage = UnityEngine.Object.FindObjectOfType<M3Stage>();
		M3ShadowBlob[] componentsInChildren = GetComponentsInChildren<M3ShadowBlob>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			MeshRenderer component = componentsInChildren[i].GetComponent<MeshRenderer>();
			if (component != null)
			{
				shadowMeshes.Add(component);
				component.material = UnityEngine.Object.Instantiate(component.material);
			}
		}
	}

	private void RandomizeControllers()
	{
		float value = UnityEngine.Random.Range(0.9f, 1.1f);
		foreach (Animator anim in anims)
		{
			anim.SetFloat("Speed", value);
		}
		PickNextIdleAction();
	}

	private int VariantizeControllers()
	{
		if (anims.Count > 0)
		{
			int num = UnityEngine.Random.Range(0, anims[0].GetInteger(hashes.paramVariantCount));
			{
				foreach (Animator anim in anims)
				{
					anim.SetInteger(hashes.paramVariant, num);
				}
				return num;
			}
		}
		return 0;
	}

	private void PickNextIdleAction()
	{
		nextIdleAction = UnityEngine.Random.Range(idleIntervalConst, idleIntervalConst + idleIntervalRandom);
	}

	private void Update()
	{
		if ((bool)hpProgressBar)
		{
			hpProgressBar.SetProgress((float)hp / (float)metaStats.hp);
			turnsText.text = turnsToAttack.ToString();
		}
		if (AnimatorIsInState(hashes.stateIdle))
		{
			nextIdleAction -= Time.deltaTime;
			if (nextIdleAction <= 0f)
			{
				VariantizeControllers();
				foreach (Animator anim in anims)
				{
					anim.SetTrigger(hashes.triggerIdleAction);
				}
				PickNextIdleAction();
			}
		}
	}

	public void Prepare(MetaMob mob, MobStatsMultipliers multipliers)
	{
		metaMob = mob;
		metaStats = (metaMob.baseStats * multipliers).Round();
		hp = metaStats.hp;
	}

	public void EnterFight(ProgressBar bar, Quaternion playerRotation)
	{
		hpProgressBar = bar;
		turnsToAttack = metaStats.atkInterval;
		if ((bool)hpProgressBar)
		{
			hpProgressBar.SetProgress(1f);
			hpProgressBar.SetScale(hpBar.scale);
			hpProgressBar.SetWorldTransform(base.transform.position + new Vector3(0f, hpBar.yOffset, 0f) + base.transform.rotation * new Vector3(0f, 0f, hpBar.zOffset), playerRotation);
			turnsText = hpProgressBar.transform.Find("AttackIn").GetComponent<Text>();
			attackWarning = hpProgressBar.GetComponentInChildren<M3AttackWarning>();
			attackWarning.Fade(fadeIn: false, instant: true);
			if (!turnsText)
			{
				UnityEngine.Debug.LogWarning("Nie znaleziono tekstu do odliczania tur!");
			}
			else
			{
				turnsText.text = turnsToAttack.ToString();
			}
		}
		if ((bool)mobCollider)
		{
			mobCollider.enabled = true;
		}
	}

	public bool IsKeyword(int keyword)
	{
		return false;
	}

	public bool IsAlive()
	{
		return hp > 0;
	}

	public bool IsIdle()
	{
		return flags.isIdle;
	}

	public void ToggleVisibility(bool visible)
	{
		foreach (SkinnedMeshRenderer skinnedMesh in skinnedMeshes)
		{
			skinnedMesh.enabled = visible;
		}
		ParticleSystemRenderer[] componentsInChildren = GetComponentsInChildren<ParticleSystemRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = visible;
		}
		if (!visible)
		{
			foreach (MeshRenderer shadowMesh in shadowMeshes)
			{
				if ((bool)shadowMesh)
				{
					shadowMesh.material.SetFloat("Kamis_ShadowPower", 0f);
				}
			}
			foreach (Material skinnedMeshMaterial in skinnedMeshMaterials)
			{
				skinnedMeshMaterial.DisableKeyword("FANCY_OUTLINE_ENABLED");
				skinnedMeshMaterial.EnableKeyword("FANCY_OUTLINE_DISABLED");
			}
		}
	}

	public int GetRealDamage(M3Damage dmg)
	{
		int num = dmg.damage;
		float num2 = metaStats.def;
		for (int i = 0; i < dmg.modifiers.Count; i++)
		{
			if (dmg.modifiers[i].AppliesTo(this))
			{
				num = Mathf.RoundToInt((float)num * dmg.modifiers[i].damageMultiplier);
				num2 *= dmg.modifiers[i].defenseMultiplier;
			}
		}
		return Mathf.Max(1, (int)((float)num - num2));
	}

	private void SpawnFx(FX fx)
	{
		if (fx.prefab != null)
		{
			Transform transform = (!(fx.spawnTransform != null)) ? base.transform : fx.spawnTransform;
			if (fx.instantiateInWorldSpace)
			{
				UnityEngine.Object.Instantiate(fx.prefab, transform.position, Quaternion.identity);
			}
			else
			{
				UnityEngine.Object.Instantiate(fx.prefab, transform, worldPositionStays: false);
			}
		}
	}

	private IEnumerator RunDestroAndDeactivate(bool instant = false)
	{
		if (destro.delay > float.Epsilon && !instant)
		{
			yield return new WaitForSeconds(destro.delay);
		}
		foreach (Material skinnedMeshMaterial in skinnedMeshMaterials)
		{
			skinnedMeshMaterial.DisableKeyword("FANCY_DESTRO_DISABLED");
			skinnedMeshMaterial.EnableKeyword("FANCY_DESTRO_ENABLED");
			skinnedMeshMaterial.SetFloat("_destroPower", 0f);
		}
		if (instant)
		{
			foreach (Material skinnedMeshMaterial2 in skinnedMeshMaterials)
			{
				skinnedMeshMaterial2.SetFloat("_destroPower", 1f);
			}
		}
		else
		{
			float time = 0f;
			do
			{
				time += Time.deltaTime;
				foreach (Material skinnedMeshMaterial3 in skinnedMeshMaterials)
				{
					skinnedMeshMaterial3.SetFloat("_destroPower", Mathf.Min(1f, time / destro.duration));
				}
				yield return null;
			}
			while (time < destro.duration);
		}
		for (int i = 0; i < base.transform.childCount; i++)
		{
			base.transform.GetChild(i).gameObject.SetActive(value: false);
		}
	}

	private IEnumerator TurnOffOutlineAndShadow()
	{
		if (outline.fadeOutDelay > float.Epsilon)
		{
			yield return new WaitForSeconds(outline.fadeOutDelay);
		}
		float[] widthAmplitude = new float[skinnedMeshMaterials.Count];
		for (int i = 0; i < skinnedMeshMaterials.Count; i++)
		{
			if (skinnedMeshMaterials[i].HasProperty("_OutlineWidth"))
			{
				widthAmplitude[i] = skinnedMeshMaterials[i].GetFloat("_OutlineWidth");
			}
		}
		float time = 0f;
		flags.outlineOn = false;
		do
		{
			time += Time.deltaTime;
			if (outline.fadeOutDuration > float.Epsilon)
			{
				for (int j = 0; j < skinnedMeshMaterials.Count; j++)
				{
					if (skinnedMeshMaterials[j].HasProperty("_OutlineWidth"))
					{
						skinnedMeshMaterials[j].SetFloat("_OutlineWidth", widthAmplitude[j] * Mathf.SmoothStep(1f, 0f, time / outline.fadeOutDuration));
					}
				}
				for (int k = 0; k < shadowMeshes.Count; k++)
				{
					float value = Mathf.Max(0f, 1f - time / outline.fadeOutDuration);
					shadowMeshes[k].material.SetFloat("Kamis_ShadowPower", value);
				}
			}
			else
			{
				foreach (Material skinnedMeshMaterial in skinnedMeshMaterials)
				{
					if (skinnedMeshMaterial.HasProperty("_OutlineWidth"))
					{
						skinnedMeshMaterial.SetFloat("_OutlineWidth", 0f);
					}
				}
				foreach (MeshRenderer shadowMesh in shadowMeshes)
				{
					if (shadowMesh != null)
					{
						shadowMesh.material.SetFloat("Kamis_ShadowPower", 0f);
					}
				}
			}
			yield return null;
		}
		while (time < outline.fadeOutDuration);
		foreach (Material skinnedMeshMaterial2 in skinnedMeshMaterials)
		{
			skinnedMeshMaterial2.DisableKeyword("FANCY_OUTLINE_ENABLED");
			skinnedMeshMaterial2.EnableKeyword("FANCY_OUTLINE_DISABLED");
		}
	}

	private IEnumerator TurnOnOutlineAndShadow()
	{
		if (outline.delayFromSpawn > float.Epsilon)
		{
			yield return new WaitForSeconds(outline.delayFromSpawn);
		}
		float[] widthAmplitude = new float[skinnedMeshMaterials.Count];
		for (int i = 0; i < skinnedMeshMaterials.Count; i++)
		{
			if (skinnedMeshMaterials[i].HasProperty("_OutlineIntensity"))
			{
				skinnedMeshMaterials[i].DisableKeyword("FANCY_OUTLINE_DISABLED");
				skinnedMeshMaterials[i].EnableKeyword("FANCY_OUTLINE_ENABLED");
				widthAmplitude[i] = skinnedMeshMaterials[i].GetFloat("_OutlineWidth");
			}
		}
		float time = 0f;
		float duration = (!(outline.duration > float.Epsilon)) ? 1f : outline.duration;
		flags.outlineOn = true;
		do
		{
			time += Time.deltaTime;
			for (int j = 0; j < skinnedMeshMaterials.Count; j++)
			{
				if (skinnedMeshMaterials[j].HasProperty("_OutlineIntensity"))
				{
					skinnedMeshMaterials[j].SetFloat("_OutlineWidth", outline.width.Evaluate(time / duration) * widthAmplitude[j]);
					skinnedMeshMaterials[j].SetFloat("_OutlineIntensity", outline.alpha.Evaluate(time / duration));
				}
			}
			foreach (MeshRenderer shadowMesh in shadowMeshes)
			{
				if ((bool)shadowMesh)
				{
					shadowMesh.material.SetFloat("Kamis_ShadowPower", outline.shadow.Evaluate(time / duration));
				}
			}
			yield return null;
		}
		while (time < duration && flags.outlineOn);
	}

	public IEnumerator Spawn()
	{
		RandomizeControllers();
		VariantizeControllers();
		foreach (Animator anim in anims)
		{
			anim.SetTrigger(hashes.triggerSpawn);
		}
		yield return new WaitUntil(() => AnimatorIsInState(hashes.stateSpawn));
		AudioManager.PlaySafe(sounds.sfxSpawn);
		ToggleVisibility(visible: true);
		StartCoroutine(TurnOnOutlineAndShadow());
		if ((bool)shakerOnSpawn)
		{
			shakerOnSpawn.TriggerShake();
		}
		yield return new WaitUntil(() => AnimatorIsInState(hashes.stateIdle));
	}

	public IEnumerator Wound(M3Damage dmg)
	{
		flags.isIdle = false;
		int realDamage = GetRealDamage(dmg);
		hp -= realDamage;
		M3WoundText woundText = UnityEngine.Object.Instantiate(M3TileManager.instance.hpWoundPrefab, hpProgressBar.transform.parent, worldPositionStays: true);
		woundText.Init(hpProgressBar.transform, -realDamage, dmg.orb);
		if (hp <= 0)
		{
			yield return StartCoroutine(Die());
		}
		else
		{
			yield return StartCoroutine(Hit());
		}
		flags.isIdle = true;
	}

	private bool AnimatorIsInState(int stateTagHash)
	{
		if (anims.Count > 0)
		{
			bool flag = true;
			{
				foreach (Animator anim in anims)
				{
					AnimatorStateInfo currentAnimatorStateInfo = anim.GetCurrentAnimatorStateInfo(0);
					flag = (flag && currentAnimatorStateInfo.tagHash == stateTagHash);
				}
				return flag;
			}
		}
		return false;
	}

	private IEnumerator Hit()
	{
		M3TileManager.Log("M3Mob.Hit: Begin");
		VariantizeControllers();
		foreach (Animator anim in anims)
		{
			anim.SetTrigger(hashes.triggerHit);
		}
		yield return new WaitUntil(() => AnimatorIsInState(hashes.stateHit));
		yield return new WaitUntil(() => AnimatorIsInState(hashes.stateIdle) || AnimatorIsInState(hashes.stateDead));
		M3TileManager.Log("M3Mob.Hit: End");
	}

	private IEnumerator Die()
	{
		M3TileManager.Log("M3Mob.Die: Begin");
		M3FaceStageCamera faceCam = GetComponent<M3FaceStageCamera>();
		if ((bool)faceCam)
		{
			faceCam.enabled = false;
		}
		foreach (Animator anim in anims)
		{
			anim.SetTrigger(hashes.triggerDie);
		}
		yield return new WaitUntil(() => AnimatorIsInState(hashes.stateDeath));
		AudioManager.PlaySafe(sounds.sfxDeath);
		if ((bool)shakerOnDeath)
		{
			shakerOnDeath.TriggerShake();
		}
		StartCoroutine(TurnOffOutlineAndShadow());
		StartCoroutine(RunDestroAndDeactivate());
		yield return new WaitForSeconds(0.5f);
		stage.GiveHPBarBack(hpProgressBar);
		yield return new WaitUntil(() => AnimatorIsInState(hashes.stateDead));
		M3TileManager.Log("M3Mob.Die: End");
	}

	private void OnDamageDealt()
	{
		flags.damageDealt = true;
	}

	private void OnSpawnFxSpawn()
	{
		if ((bool)spawnFx.prefab)
		{
			SpawnFx(spawnFx);
		}
	}

	private void OnDeathFxSpawn()
	{
		if ((bool)deathFx.prefab)
		{
			SpawnFx(deathFx);
		}
	}

	private void OnAttackFxSpawn(int variant)
	{
		if (anims.Count > 0 && variant < attackFxs.Count && (bool)attackFxs[variant].prefab)
		{
			SpawnFx(attackFxs[variant]);
		}
	}

	private IEnumerator AttackAnim()
	{
		foreach (Animator anim in anims)
		{
			anim.SetTrigger(hashes.triggerAttack);
		}
		flags.damageDealt = false;
		AudioManager.PlaySafe(sounds.sfxAttack);
		yield return new WaitUntil(() => flags.damageDealt);
	}

	public IEnumerator PerformTurn(M3Player player)
	{
		M3TileManager.Log("M3Mob.PerformTurn: Begin");
		if (turnsToAttack > 1)
		{
			turnsToAttack--;
			if (turnsToAttack == 1)
			{
				attackWarning.Fade(fadeIn: true);
			}
		}
		else
		{
			yield return StartCoroutine(AttackAnim());
			yield return StartCoroutine(player.Wound(this));
			turnsToAttack = metaStats.atkInterval;
			if (turnsToAttack > 1)
			{
				attackWarning.Fade(fadeIn: false);
			}
		}
		M3TileManager.Log("M3Mob.PerformTurn: End");
	}

	public bool Load(M3SaveMob saveMob)
	{
		turnsToAttack = saveMob.cooldown;
		hp = Mathf.Min(saveMob.hp, metaStats.hp);
		ToggleVisibility(visible: true);
		RandomizeControllers();
		if (!IsAlive())
		{
			foreach (Animator anim in anims)
			{
				anim.SetTrigger(hashes.triggerDieInstant);
			}
			StartCoroutine(RunDestroAndDeactivate(instant: true));
			UnityEngine.Object.Destroy(hpProgressBar.gameObject);
		}
		else
		{
			foreach (Animator anim2 in anims)
			{
				anim2.SetTrigger(hashes.triggerIdle);
			}
			StartCoroutine(TurnOnOutlineAndShadow());
		}
		return hp <= metaStats.hp;
	}
}
