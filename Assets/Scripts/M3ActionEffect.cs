using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M3ActionEffect : MonoBehaviour
{
	[Serializable]
	public class FxOnNoTarget
	{
		public GameObject fxPrefab;

		public Vector3 posFromPlayer;

		public bool cameraChild;
	}

	[Serializable]
	public class FxPowerTransfer
	{
		public M3OrbParticle fxPrefab;

		public AudioSample sfxPowerTransfer;

		public Transform startPoint;
	}

	public GameObject fxPrefabOnHitMob;

	public AudioSample sfxOnHitMob;

	public FxOnNoTarget fxOnNoTarget;

	public FxPowerTransfer fxPowerTransfer;

	public float effectMaxDuration = 5f;

	public float effectYieldAfter = 0.5f;

	public bool destroyAfter = true;

	private M3MoveShaker shaker;

	private float effectLifeTime;

	private bool triggered;

	private void Update()
	{
		if (!triggered)
		{
			return;
		}
		effectLifeTime += Time.deltaTime;
		if (effectLifeTime >= effectMaxDuration)
		{
			if (destroyAfter)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				triggered = false;
			}
		}
	}

	private IEnumerator CoFxSequence(Transform player, Transform playerCamera, List<M3Mob> targets)
	{
		if ((bool)fxPowerTransfer.fxPrefab)
		{
			AudioManager.PlaySafe(fxPowerTransfer.sfxPowerTransfer);
			Vector3 zero = Vector3.zero;
			Vector3 dest = (targets == null || targets.Count <= 0) ? (player.position + player.rotation * fxOnNoTarget.posFromPlayer) : (targets[0].transform.position + new Vector3(0f, targets[0].fxHitY, 0f));
			Camera cam = playerCamera.GetComponent<Camera>();
			Vector3 screenPos = cam.WorldToScreenPoint(dest);
			screenPos = Camera.main.ScreenPointToRay(screenPos).origin;
			M3OrbParticle fx = UnityEngine.Object.Instantiate(fxPowerTransfer.fxPrefab);
			if ((bool)fxPowerTransfer.startPoint)
			{
				fx.transform.position = fxPowerTransfer.startPoint.position;
				fx.transform.rotation = fxPowerTransfer.startPoint.rotation;
			}
			Vector3 position = fx.transform.position;
			screenPos.z = position.z;
			M3TileManager.Log(screenPos.ToString());
			yield return new WaitForSeconds(fx.Fly(screenPos));
		}
		if (targets != null && (bool)fxPrefabOnHitMob)
		{
			for (int i = 0; i < targets.Count; i++)
			{
				Vector3 position2 = targets[i].transform.position;
				position2.y += targets[i].fxHitY;
				UnityEngine.Object.Instantiate(fxPrefabOnHitMob, position2, targets[i].transform.rotation);
			}
		}
		else if ((bool)fxOnNoTarget.fxPrefab)
		{
			if (fxOnNoTarget.cameraChild)
			{
				UnityEngine.Object.Instantiate(fxOnNoTarget.fxPrefab, playerCamera, worldPositionStays: false);
			}
			else
			{
				Vector3 position3 = player.position + player.rotation * fxOnNoTarget.posFromPlayer;
				UnityEngine.Object.Instantiate(fxOnNoTarget.fxPrefab, position3, Quaternion.identity);
			}
		}
		AudioManager.PlaySafe(sfxOnHitMob);
	}

	public IEnumerator Trigger(Transform player, Transform playerCamera, List<M3Mob> targets = null)
	{
		triggered = true;
		effectLifeTime = 0f;
		if (!shaker)
		{
			shaker = GetComponent<M3MoveShaker>();
		}
		if ((bool)shaker)
		{
			shaker.TriggerShake();
		}
		yield return StartCoroutine(CoFxSequence((!player) ? base.transform : player, (!playerCamera) ? base.transform : playerCamera, targets));
		if (effectYieldAfter > float.Epsilon)
		{
			yield return new WaitForSeconds(effectYieldAfter);
		}
	}
}
