using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class M3Combo : MonoBehaviour
{
	[Serializable]
	public class Timings
	{
		public float labelAnimDuration = 0.3f;

		public float duration = 1.5f;

		public float fxInterval = 0.1f;

		public float fxDelay = 0.2f;

		public float fadeDuration = 0.5f;
	}

	private static string[] strComboDesc = new string[5]
	{
		"Great!",
		"Awesome!",
		"Super!",
		"Epic!",
		"Legendary!"
	};

	public Timings timings;

	public AnimationCurve sizeOverTime;

	public AnimationCurve alphaOverTime;

	public AnimationCurve alphaFadeOverTime;

	public List<M3OrbParticle> comboTrailFxPrefabs;

	public Transform fxStartPosition;

	public Text textComboValue;

	public Text textComboDesc;

	public M3SmartLabel scaledLabels;

	public List<Image> comboXX;

	public M3WoundText comboMultiplierText;

	public M3WoundText comboMultiplierTextRCV;

	private int comboLevel;

	private int fxComboActive;

	private List<Image> images = new List<Image>();

	private List<Text> texts = new List<Text>();

	public bool Valid => (bool)textComboDesc && (bool)textComboValue && comboXX.Count > 0;

	private void Start()
	{
		GetComponentsInChildren(includeInactive: true, images);
		GetComponentsInChildren(includeInactive: true, texts);
	}

	public void UpdateCombo(int level)
	{
		comboLevel = level;
		if (!Valid)
		{
			return;
		}
		if (level > 1)
		{
			base.transform.localScale = Vector3.one;
			SetAlpha(1f);
			base.gameObject.SetActive(value: true);
			for (int i = 0; i < comboXX.Count; i++)
			{
				comboXX[i].gameObject.SetActive(value: false);
			}
			if (comboXX.Count > level - 2)
			{
				comboXX[level - 2].gameObject.SetActive(value: true);
			}
			else
			{
				comboXX[comboXX.Count - 1].gameObject.SetActive(value: true);
			}
			textComboValue.text = level.ToString() + "x";
			if (level - 2 >= strComboDesc.Length)
			{
				textComboDesc.text = strComboDesc[strComboDesc.Length - 1];
			}
			else
			{
				textComboDesc.text = strComboDesc[level - 2];
			}
			if ((bool)scaledLabels)
			{
				scaledLabels.AddValue(100, timings.labelAnimDuration);
			}
		}
		else
		{
			base.gameObject.SetActive(value: false);
			if ((bool)scaledLabels)
			{
				scaledLabels.BackToBase(0);
			}
		}
	}

	private void SetAlpha(float alpha)
	{
		foreach (Image image in images)
		{
			Helpers.SetImageAlpha(image, alpha);
		}
		foreach (Text text in texts)
		{
			Helpers.SetTextAlpha(text, alpha);
		}
	}

	private IEnumerator CoFireFxTrail(M3Player player, M3Orb orb)
	{
		M3TileManager.Log("M3Combo.CoFireFxTrail: Begin");
		fxComboActive++;
		Vector3 destPos = player.GetDamageLabelPos(orb);
		Vector3 srcPos = (!fxStartPosition) ? base.transform.position : fxStartPosition.transform.position;
		srcPos.z = (destPos.z = M3Board.instance.tileFxZ);
		M3OrbParticle fx = UnityEngine.Object.Instantiate(comboTrailFxPrefabs[(int)orb], srcPos, Quaternion.identity);
		AudioManager.PlaySafe(M3Board.instance.sounds.comboPowerUp);
		yield return new WaitForSeconds(fx.Fly(destPos));
		M3WoundText mul = UnityEngine.Object.Instantiate((orb != M3Orb.Recovery) ? comboMultiplierText : comboMultiplierTextRCV, base.transform.parent, worldPositionStays: true);
		mul.Init(text: "x" + (1f + (float)(comboLevel - 1) * 0.25f).ToString("0.00"), pos: destPos, color: M3Board.orbColors[(int)orb], yScale: 1f);
		yield return StartCoroutine(player.CoApplyCombo(comboLevel, orb));
		fxComboActive--;
		M3TileManager.Log("M3Combo.CoFireFxTrail: End");
	}

	private IEnumerator CoAnimateCombo(bool fired)
	{
		M3InterpolatedFloat time = new M3InterpolatedFloat(this, M3InterpolationMode.Linear);
		time.SlideUp((!fired) ? timings.fadeDuration : timings.duration);
		do
		{
			yield return null;
			float scale = sizeOverTime.Evaluate(time.Value);
			float alpha = (!fired) ? alphaFadeOverTime.Evaluate(time.Value) : alphaOverTime.Evaluate(time.Value);
			if (fired)
			{
				base.transform.localScale = new Vector3(scale, scale, 1f);
			}
			SetAlpha(alpha);
		}
		while (time.Value < 1f);
		yield return new WaitUntil(() => fxComboActive == 0);
		yield return null;
		UpdateCombo(0);
	}

	public IEnumerator CoApply(M3Player player)
	{
		if (comboLevel <= 1)
		{
			yield break;
		}
		StartCoroutine(CoAnimateCombo(fired: true));
		if (timings.fxDelay > float.Epsilon)
		{
			yield return new WaitForSeconds(timings.fxDelay);
		}
		List<M3Damage> damage = player.GetAccumulatedDamage();
		fxComboActive++;
		for (int i = 0; i < damage.Count; i++)
		{
			if (damage[i].damage > 0)
			{
				StartCoroutine(CoFireFxTrail(player, damage[i].orb));
				yield return new WaitForSeconds(timings.fxInterval);
			}
		}
		fxComboActive--;
		yield return new WaitUntil(() => fxComboActive == 0);
	}

	public void FadeOut()
	{
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(CoAnimateCombo(fired: false));
		}
	}
}
