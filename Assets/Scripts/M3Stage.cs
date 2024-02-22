using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class M3Stage : MonoBehaviour, ISceneLoadingElement
{
	private const string strAttackIn = "AttackIn";

	private const string strMatch3Scene = "match3";

	private const string strWorldPosCanvas = "WorldPosCanvas";

	public MetaStage debugMetaStage;

	private List<ProgressBar> hpBarPool = new List<ProgressBar>();

	private List<M3Battle> battles = new List<M3Battle>();

	private int currentBattle;

	private bool loaded;

	public M3Battle CurrentBattle
	{
		get
		{
			if (currentBattle < battles.Count)
			{
				return battles[currentBattle];
			}
			return null;
		}
	}

	private void Start()
	{
		M3TileManager exists = UnityEngine.Object.FindObjectOfType<M3TileManager>();
		if (!exists)
		{
			StartCoroutine(LoadMatch3Scene("match3"));
		}
		else
		{
			loaded = true;
		}
	}

	public bool IsLoading()
	{
		return !loaded;
	}

	private IEnumerator LoadMatch3Scene(string sceneName)
	{
		yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		loaded = true;
	}

	private void InitBackgroundAnims(List<M3Battle> battles)
	{
		M3StageAnimator m3StageAnimator = UnityEngine.Object.FindObjectOfType<M3StageAnimator>();
		if ((bool)m3StageAnimator)
		{
			m3StageAnimator.Init();
		}
	}

	public bool Enter(MetaStage metaStage, MetaStageDifficulty difficulty)
	{
		BuildHPBarPool();
		battles.Clear();
		int num = 0;
		GetComponentsInChildren(battles);
		M3SmartPath component = GetComponent<M3SmartPath>();
		component.Init();
		for (int i = 0; i < battles.Count; i++)
		{
			M3Battle m3Battle = battles[i];
			M3PathPoint m3PathPoint = component.WorldPosToPath(battles[i].transform.position);
			battles[i].transform.position = m3PathPoint.position;
			battles[i].transform.rotation = Quaternion.AngleAxis(m3PathPoint.pitch, Vector3.up);
			battles[i].distance = m3PathPoint.distance;
			num += ((!m3Battle.IsNavpoint()) ? 1 : 0);
		}
		if (num < metaStage.battles.Count)
		{
			UnityEngine.Debug.LogWarning("Not enough battle spots, not all battles will be set up!");
		}
		int[] array = new int[Mathf.Min(metaStage.battles.Count, num)];
		if (array.Length > 0)
		{
			array[0] = 0;
			if (array.Length > 1)
			{
				array[array.Length - 1] = num - 1;
			}
			float num2 = 0f;
			for (int j = 1; j < array.Length - 1; j++)
			{
				num2 += (float)(num - 1) / (float)(array.Length - 1);
				array[j] = Mathf.RoundToInt(num2);
			}
			int num3 = 0;
			int num4 = -1;
			for (int k = 0; k < battles.Count; k++)
			{
				if (!battles[k].IsNavpoint())
				{
					num4++;
					if (array.Length > num3 && num4 == array[num3])
					{
						battles[k].Prepare(metaStage.battles[num3], metaStage.GetMultipliers(difficulty));
						num3++;
					}
					else
					{
						battles[k].Prepare(null, null);
					}
				}
				else
				{
					battles[k].Prepare(null, null);
				}
			}
		}
		else
		{
			UnityEngine.Debug.LogWarning("No battles generated, you either have no battle spots on stage or no battles were received from meta. Or both :)");
		}
		InitBackgroundAnims(battles);
		currentBattle = 1;
		return battles.Count > 0;
	}

	private void BuildHPBarPool()
	{
		RectTransform component = GameObject.FindGameObjectWithTag("WorldPosCanvas").GetComponent<RectTransform>();
		M3TileManager m3TileManager = UnityEngine.Object.FindObjectOfType<M3TileManager>();
		hpBarPool.Clear();
		for (int i = 0; i < 5; i++)
		{
			ProgressBar progressBar = UnityEngine.Object.Instantiate(m3TileManager.hpBarPrefab, component);
			Text component2 = progressBar.transform.Find("AttackIn").GetComponent<Text>();
			if ((bool)component2)
			{
				component2.font.RequestCharactersInTexture("In 0123456789", component2.fontSize, component2.fontStyle);
			}
			progressBar.gameObject.SetActive(value: false);
			hpBarPool.Add(progressBar);
		}
	}

	public ProgressBar GetHPBar()
	{
		if (hpBarPool.Count > 0)
		{
			ProgressBar progressBar = hpBarPool[hpBarPool.Count - 1];
			hpBarPool.RemoveAt(hpBarPool.Count - 1);
			progressBar.gameObject.SetActive(value: true);
			return progressBar;
		}
		return null;
	}

	public void GiveHPBarBack(ProgressBar hpBar)
	{
		if ((bool)hpBar)
		{
			hpBar.gameObject.SetActive(value: false);
			hpBarPool.Add(hpBar);
		}
	}

	private bool IsLastBattle()
	{
		bool flag = true;
		for (int i = currentBattle + 1; i < battles.Count; i++)
		{
			flag = (flag && battles[i].IsNavpoint());
		}
		return flag;
	}

	public M3Battle NextBattle()
	{
		if (!IsLastBattle())
		{
			currentBattle++;
			return CurrentBattle;
		}
		return null;
	}

	public void Save(M3SaveData saveData)
	{
		saveData.saveCurrentBattle = currentBattle;
	}

	public bool Load(M3SaveData saveData)
	{
		currentBattle = saveData.saveCurrentBattle;
		return currentBattle < battles.Count;
	}
}
