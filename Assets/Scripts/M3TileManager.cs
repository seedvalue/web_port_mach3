using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

public class M3TileManager : MonoBehaviour, ISceneLoadedHandler, ISceneLoadingElement, IInputCancelHandler
{
	public static int GAMEVERSION = 1;

	private const string strSaveFileName = "\\match3.save";

	private static bool logExecution = false;

	private static List<string> strings = new List<string>();

	private const string strPlayer = "Player";

	private const string strStage3DCamera = "PlayerCameraPreview";

	public float debugTimeMultiplier = 1f;

	public GameObject skillViewPrefab;

	public MetaContainer skillContainer;

	public Image tutorialFakeWin;

	private Text tutorialText;

	public M3Tutorial debugTutorial;

	public M3FixedBoard debugFixedBoard;

	public MetaStage debugStage;

	public ProgressBar hpBarPrefab;

	public M3WoundText hpWoundPrefab;

	public float optimalBattleArea = 5f;

	public static M3TileManager instance;

	private M3Board board;

	private M3Player player;

	private M3Stage stage;

	private M3Battle battle;

	private MetaStage metaStage;

	private MetaStageDifficulty metaStageDifficulty;

	private M3Tutorial tutorial;

	private Match3StartContext context;

	private M3StatusBar statusBar;

	private Match3Stats match3Stats;

	private Vector2 anchorBase = Vector2.zero;

	private bool loaded;

	private int interactionSemaphores;

	private bool tutorialControlsBoardShading;

	private List<Window> openWindows = new List<Window>();

	public M3Battle Battle => battle;

	private static string savePath => Application.persistentDataPath + "\\match3.save";

	private void Start()
	{
		context = (Singleton<SceneLoader>.Instance.context as Match3StartContext);
		if (context != null)
		{
			metaStage = context.stage;
			metaStageDifficulty = context.difficulty;
		}
		if (!metaStage)
		{
			metaStage = debugStage;
		}
		if (!metaStage)
		{
			metaStage = Singleton<Meta>.Instance.FindRandomObject<MetaStage>();
		}
		M3Stage m3Stage = UnityEngine.Object.FindObjectOfType<M3Stage>();
		if ((bool)m3Stage && (bool)m3Stage.debugMetaStage)
		{
			metaStage = m3Stage.debugMetaStage;
		}
		instance = this;
		match3Stats = new Match3Stats();
		StartCoroutine(LoadStage3D(m3Stage != null));
	}

	private void OnEnable()
	{
		InputManager.AddCancelHandler(this);
	}

	private void OnDisable()
	{
		InputManager.RemCancelHandler(this);
	}

	public bool IsLoading()
	{
		return !loaded;
	}

	public bool OnCancel()
	{
		Pause();
		return true;
	}

	public void OnSceneLoaded()
	{
		if ((bool)player && (bool)stage && (context == null || !context.useSave))
		{
			StartCoroutine(ExecuteInitialSequence());
		}
	}

	private void OnSkillChosen(M3SkillView itemView)
	{
		StartCoroutine(ExecuteSkill(itemView));
	}

	public bool IsInteractionAllowed()
	{
		return interactionSemaphores == 0;
	}

	private IEnumerator LoadStage3D(bool initOnly)
	{
		if (!initOnly)
		{
			yield return SceneManager.LoadSceneAsync(metaStage.sceneName, LoadSceneMode.Additive);
		}
		InitStage();
		loaded = true;
	}

	private void InitM3Player()
	{
		player = UnityEngine.Object.FindObjectOfType<M3Player>();
		if (!player)
		{
			UnityEngine.Debug.LogWarning("M3Player object missing! Game will not work properly, more errors will follow soon!");
		}
		context = (Singleton<SceneLoader>.Instance.context as Match3StartContext);
		List<MetaItem> list;
		Stats metaStats;
		if (context == null)
		{
			list = new List<MetaItem>();
			int num = 6;
			for (int i = 0; i < num; i++)
			{
				list.Add(Singleton<Meta>.Instance.FindRandomObject<MetaItem>());
			}
			metaStats = player.debugStatBoosts;
		}
		else
		{
			list = context.items;
			metaStats = context.stats;
		}
		List<MetaSkill> list2 = new List<MetaSkill>();
		for (int j = 0; j < list.Count; j++)
		{
			list2.Add(list[j].skill);
		}
		skillContainer.Add(list2, skillViewPrefab.GetComponent<M3SkillView>());
		player.Init(metaStats);
		player.InitItems(list, skillContainer, OnSkillChosen);
	}

	private void InitStage()
	{
		ToggleInteractionAllowed(allowed: false);
		GameObject gameObject = GameObject.Find("PlayerCameraPreview");
		if ((bool)gameObject)
		{
			M3StageCamera m3StageCamera = UnityEngine.Object.FindObjectOfType<M3StageCamera>();
			if ((bool)m3StageCamera)
			{
				m3StageCamera.SetBaseLocalTransform(gameObject.transform.localPosition, Quaternion.identity);
			}
			gameObject.SetActive(value: false);
		}
		else
		{
			UnityEngine.Debug.LogWarning("Stage3D camera not found, is the name proper? (should be PlayerCameraPreview)");
		}
		board = GetComponent<M3Board>();
		if (context != null)
		{
			board.Init(clearStructures: true, context.tileProvider);
		}
		else
		{
			board.Init(clearStructures: true, debugFixedBoard);
		}
		if (context != null && context.tutorialPrefab != null)
		{
			tutorial = UnityEngine.Object.Instantiate(context.tutorialPrefab).GetComponent<M3Tutorial>();
			tutorial.Init();
		}
		else if ((bool)debugTutorial)
		{
			tutorial = debugTutorial;
			tutorial.gameObject.SetActive(value: true);
			tutorial.Init();
		}
		else
		{
			tutorial = null;
		}
		if ((bool)tutorialFakeWin)
		{
			tutorialText = tutorialFakeWin.gameObject.GetComponentInChildren<Text>();
			anchorBase = tutorialFakeWin.GetComponent<RectTransform>().anchoredPosition;
		}
		stage = UnityEngine.Object.FindObjectOfType<M3Stage>();
		if (!stage)
		{
			UnityEngine.Debug.LogWarning("No M3Stage on scene, more errors will follow soon!");
		}
		InitM3Player();
		if ((bool)player)
		{
			player.InitStage(stage, metaStage, metaStageDifficulty);
		}
		if (context != null && context.useSave && !LoadFile("\\match3.save"))
		{
			UnityEngine.Debug.LogWarning("Cannot load saveData, game versions do not match?");
			QuitToMainMenu(MetaFight.Result.Abort);
		}
		statusBar = UnityEngine.Object.FindObjectOfType<M3StatusBar>();
		if ((bool)statusBar)
		{
			statusBar.Init(metaStage);
		}
		TutorialInitOverridesGeneral();
		ToggleInteractionAllowed(allowed: true);
	}

	public void Reshuffle(bool clearStructures)
	{
		board.Init(clearStructures, context.tileProvider);
	}

	public void OnTileDragEnd()
	{
		TutorialActivityFinished(M3TutorialActivity.M3Move);
		StartCoroutine(ExecuteM3Move());
	}

	private IEnumerator GameOver()
	{
		bool windowOpen = true;
		OpenWindow<StageLoseWindow>(new StageLoseWindowContext
		{
			stage = metaStage
		}, delegate
		{
			windowOpen = false;
		});
		yield return new WaitUntil(() => !windowOpen);
		QuitToMainMenu(MetaFight.Result.Lose);
	}

	private IEnumerator StageWon()
	{
		bool windowOpen = true;
		StageWinWindowContext windowContext = new StageWinWindowContext();
		if (context != null)
		{
			windowContext.rewards = context.rewards;
			windowContext.stage = context.stage;
		}
		else
		{
			windowContext.stage = metaStage;
		}
		windowContext.stars = ((!statusBar) ? 3 : statusBar.StageResult);
		OpenWindow<StageWinWindow>(windowContext, delegate
		{
			windowOpen = false;
		});
		yield return new WaitUntil(() => !windowOpen);
		QuitToMainMenu(MetaFight.Result.Win);
	}

	private IEnumerator ExecuteSkill(M3SkillView itemView)
	{
		ToggleInteractionAllowed(allowed: false);
		yield return StartCoroutine(itemView.ExecuteSkill(battle, board, player));
		match3Stats.skillsUsed++;
		board.UpdateStructures(0, 0);
		TutorialActivityFinished(M3TutorialActivity.ItemUse);
		if (battle.IsOver())
		{
			yield return StartCoroutine(CoNextStepIfBattleOver());
			yield return StartCoroutine(player.NewTurn(updateSkills: false));
		}
		ToggleInteractionAllowed(allowed: true);
		SaveGame();
	}

	private IEnumerator ExecuteM3Move()
	{
		ClearLog();
		Log("ExecuteM3Move: Begin");
		ToggleInteractionAllowed(allowed: false);
		if (!stage || !battle)
		{
			UnityEngine.Debug.LogWarning("Brak stageu i/lub walki, wychodzę do metagry!");
			yield return new WaitForSeconds(1f);
			QuitToMainMenu(MetaFight.Result.Win);
			ToggleInteractionAllowed(allowed: true);
			yield break;
		}
		StartCoroutine(board.ToggleGrayOut(grayOut: true, board.timings.actualTileGrayOutDuration));
		yield return StartCoroutine(board.AttackSequence(player, battle, match3Stats));
		TutorialActivityFinished(M3TutorialActivity.M3WaitUntilExecuted);
		if (battle.IsOver())
		{
			yield return StartCoroutine(CoNextStepIfBattleOver());
		}
		else
		{
			yield return StartCoroutine(player.EnemyTurn(battle));
			TutorialActivityFinished(M3TutorialActivity.WaitUntilEnemyTurn);
		}
		if (player.IsDead())
		{
			yield return StartCoroutine(GameOver());
		}
		else
		{
			yield return StartCoroutine(player.NewTurn(board.GetLastAttackComboCount() > 0));
			if ((bool)statusBar)
			{
				statusBar.NextMove();
			}
		}
		if (!tutorialControlsBoardShading)
		{
			StartCoroutine(board.ToggleGrayOut(grayOut: false, board.timings.actualTileGrayOutDuration));
		}
		ToggleInteractionAllowed(allowed: true);
		SaveGame();
		Log("ExecuteM3Move: End");
	}

	private IEnumerator CoNextStepIfBattleOver()
	{
		bool slowStart = true;
		bool done;
		do
		{
			battle = stage.NextBattle();
			if ((bool)battle)
			{
				yield return StartCoroutine(player.GoToBattle(battle, slowStart));
				done = !battle.IsNavpoint();
				if (done && (bool)statusBar)
				{
					statusBar.NextBattle();
				}
			}
			else
			{
				yield return StartCoroutine(player.EndStage(stage));
				yield return StartCoroutine(StageWon());
				done = true;
			}
			slowStart = false;
		}
		while (!done);
		TutorialActivityFinished(M3TutorialActivity.NextBattle);
	}

	public IEnumerator ExecuteInitialSequence()
	{
		ToggleInteractionAllowed(allowed: false);
		StartCoroutine(board.ToggleGrayOut(grayOut: true, board.timings.actualTileGrayOutDuration));
		bool slowStart = true;
		do
		{
			battle = stage.CurrentBattle;
			if ((bool)battle)
			{
				yield return StartCoroutine(player.GoToBattle(battle, slowStart));
			}
			else
			{
				UnityEngine.Debug.LogWarning("Brak walki na scenie, po pierwszym ruchu nastąpi wyjście do metagry!");
			}
			slowStart = false;
			if (battle.IsNavpoint())
			{
				stage.NextBattle();
			}
		}
		while ((bool)battle && battle.IsNavpoint());
		yield return StartCoroutine(player.NewTurn(updateSkills: false));
		ToggleInteractionAllowed(allowed: true);
		TutorialInitOverridesFirstBattle();
		TutorialActivityFinished(M3TutorialActivity.None);
		if (!tutorialControlsBoardShading)
		{
			StartCoroutine(board.ToggleGrayOut(grayOut: false, board.timings.actualTileGrayOutDuration));
		}
		SaveGame();
	}

	public void ToggleInteractionAllowed(bool allowed)
	{
		if (allowed)
		{
			interactionSemaphores--;
		}
		else
		{
			interactionSemaphores++;
		}
		if (interactionSemaphores < 0)
		{
			interactionSemaphores = 0;
			UnityEngine.Debug.LogWarning("invalid ToggleInteractionAllowed(true) call, semaphore count already == 0");
		}
	}

	private void OnCloseWindow(Window window, object context)
	{
		Invoke("WindowClosed", 0.25f);
	}

	private void WindowClosed()
	{
		ToggleInteractionAllowed(allowed: true);
	}

	public T OpenWindow<T>(object context = null, Action<Window, object> onClosedCallback = null) where T : Window
	{
		ToggleInteractionAllowed(allowed: false);
		T val = Singleton<WindowManager>.Instance.OpenWindow<T>(context, delegate(Window w, object c)
		{
			OnCloseWindow(w, c);
			openWindows.Remove(w);
			if (onClosedCallback != null)
			{
				onClosedCallback(w, c);
			}
		});
		openWindows.Add(val);
		return val;
	}

	public void QuitToMainMenu(MetaFight.Result result)
	{
		Match3ResultContext match3ResultContext = new Match3ResultContext();
		match3ResultContext.result = result;
		match3ResultContext.stats = match3Stats;
		match3ResultContext.stars = ((!statusBar) ? 3 : statusBar.StageResult);
		for (int num = openWindows.Count - 1; num >= 0; num--)
		{
			openWindows[num].CloseWindow(0);
		}
		Singleton<SceneLoader>.Instance.SwitchToMeta(SceneLoader.Priority.Default, match3ResultContext);
	}

	public int FindMobIndex(M3Mob mob)
	{
		for (int i = 0; i < Battle.Mobs.Count; i++)
		{
			if (mob == Battle.Mobs[i])
			{
				return i;
			}
		}
		return -1;
	}

	private void OnInfoWindowClose(Window window, object obj)
	{
		TutorialActivityFinished(M3TutorialActivity.ShowInfo);
	}

	public void Pause()
	{
		M3PauseWindowContext m3PauseWindowContext = new M3PauseWindowContext();
		m3PauseWindowContext.metaStage = metaStage;
		m3PauseWindowContext.canQuit = (context != null && context.canQuit);
		OpenWindow<M3PauseWindow>(m3PauseWindowContext, delegate(Window w, object c)
		{
			if (c != null)
			{
				QuitToMainMenu(MetaFight.Result.Cancel);
			}
		});
	}

	private void TutorialInitOverridesGeneral()
	{
		if ((bool)tutorial)
		{
			skillContainer.gameObject.SetActive(tutorial.itemsActive);
		}
	}

	private void TutorialInitOverridesFirstBattle()
	{
		if ((bool)tutorial)
		{
			if (tutorial.mobIndex > -1 && tutorial.mobCooldownOverride >= 1)
			{
				battle.Mobs[tutorial.mobIndex].Cooldown = tutorial.mobCooldownOverride;
			}
			if (tutorial.itemIndex > -1 && tutorial.itemCooldownOverride >= 0)
			{
				player.Skills[tutorial.itemIndex].Cooldown = tutorial.itemCooldownOverride;
			}
		}
	}

	public bool TutorialAllows(M3TutorialActivity activity, int index1 = -1, int index2 = -1)
	{
		if ((bool)tutorial && tutorial.CurrentStep != null)
		{
			return tutorial.CurrentStep.IsActivityAllowed(activity, index1, index2);
		}
		return true;
	}

	private void TutorialShowCurrentStep()
	{
		if (tutorial.CurrentActivity == M3TutorialActivity.ShowInfo)
		{
			OpenWindow<M3InfoWindow>(tutorial.CurrentStep, OnInfoWindowClose);
		}
		if (tutorial.CurrentActivity != 0 && tutorial.CurrentActivity != M3TutorialActivity.ShowInfo)
		{
			tutorialFakeWin.gameObject.SetActive(tutorial.CurrentStep.stepDescription != string.Empty);
			RectTransform component = tutorialFakeWin.GetComponent<RectTransform>();
			component.anchoredPosition = anchorBase + new Vector2(0f, tutorial.CurrentStep.yPixelsFromCenter);
			tutorialText.text = tutorial.CurrentStep.stepDescription;
		}
		else
		{
			tutorialFakeWin.gameObject.SetActive(value: false);
		}
		tutorialControlsBoardShading = (tutorial.CurrentActivity != 0 && tutorial.CurrentActivity != M3TutorialActivity.M3WaitUntilExecuted && tutorial.CurrentActivity != M3TutorialActivity.NextBattle && tutorial.CurrentActivity != M3TutorialActivity.WaitUntilEnemyTurn);
		board.ColorOrbsAccordingToTutorial(tutorial.CurrentStep);
		if (tutorial.CurrentActivity == M3TutorialActivity.M3Move)
		{
			StartCoroutine(board.PulseOrb(tutorial.CurrentStep.coords[0].x, tutorial.CurrentStep.coords[0].y));
		}
		else
		{
			board.PulseOrbStop();
		}
	}

	public void TutorialActivityFinished(M3TutorialActivity activity)
	{
		if ((bool)tutorial && tutorial.UpdateAfterActivity(activity))
		{
			TutorialShowCurrentStep();
		}
	}

	public static void RemoveSave()
	{
		try
		{
			File.Delete(savePath);
		}
		catch (Exception)
		{
		}
	}

	public void SaveGame()
	{
		M3SaveData m3SaveData = new M3SaveData(GAMEVERSION);
		Save(m3SaveData);
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate, FileAccess.Write);
		binaryFormatter.Serialize(fileStream, m3SaveData);
		fileStream.Close();
	}

	public void Save(M3SaveData saveData)
	{
		if ((bool)battle)
		{
			saveData.match3Stats.Assign(match3Stats);
			stage.Save(saveData);
			board.Save(saveData.board);
			player.Save(saveData.player);
			battle.Save(saveData.battle);
			if ((bool)tutorial)
			{
				tutorial.Save(saveData.tutorial);
			}
		}
	}

	public bool Load(M3SaveData saveData)
	{
		match3Stats.Assign(saveData.match3Stats);
		bool flag = true && player.Load(saveData.player) && stage.Load(saveData) && board.Load(saveData.board);
		if ((bool)tutorial)
		{
			flag = (flag && tutorial.Load(saveData.tutorial));
			TutorialShowCurrentStep();
			tutorial.CurrentStep.EnterStep(tutorial);
		}
		if (flag)
		{
			battle = stage.CurrentBattle;
			M3PlayerMover component = player.GetComponent<M3PlayerMover>();
			component.RunHeadStrafer(battle);
			StartCoroutine(component.RunHeadUpDown(0f, 0f, battle.cameraXAngle));
			flag = (flag && battle.Load(saveData.battle, player.transform.rotation));
		}
		return flag;
	}

	public static void Log(string message)
	{
		if (logExecution)
		{
			UnityEngine.Debug.Log(message);
		}
		strings.Add(message);
	}

	public static void ClearLog()
	{
		strings.Clear();
	}

	public static void ShowLog(Text text)
	{
		string text2 = string.Empty;
		for (int i = 0; i < strings.Count; i++)
		{
			text2 = text2 + strings[i] + "\n";
		}
		text.text = text2;
	}

	private bool LoadFile(string fileName)
	{
		bool flag = false;
		try
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			FileStream fileStream = File.Open(Application.persistentDataPath + fileName, FileMode.Open, FileAccess.Read);
			M3SaveData saveData = binaryFormatter.Deserialize(fileStream) as M3SaveData;
			fileStream.Close();
			return Load(saveData);
		}
		catch (Exception)
		{
			return false;
		}
	}

	public IEnumerator Debug_WinBattle()
	{
		yield return StartCoroutine(player.Debug_KillAll(battle.Mobs));
		yield return CoNextStepIfBattleOver();
	}

	public void Debug_FullSequence()
	{
		StartCoroutine(ExecuteM3Move());
	}

	public void Debug_GameOver()
	{
		StartCoroutine(GameOver());
	}

	public void Debug_WinStage()
	{
		StartCoroutine(StageWon());
	}
}
