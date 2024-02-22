using System;
using System.Collections.Generic;
using Utils;

public class MetaFight : MetaComponent<MetaFight>, ISceneLoadedHandler
{
	public enum Result
	{
		Win,
		Lose,
		Cancel,
		Abort
	}

	private bool retryIfCantLoad;

	private bool runPending;

	private bool started;

	private Action<Result> callback;

	[MetaData(null, 0)]
	private MetaStage stage;

	[MetaData(null, 0)]
	private MetaStageDifficulty difficulty;

	[MetaData(null, 0)]
	private int rewardExp;

	[MetaData(null, 0)]
	private int rewardCoins;

	[MetaData(null, 0)]
	private bool canExit;

	public bool running => stage != null;

	protected virtual void MetaStart()
	{
		if (!IsValid())
		{
			Reset();
		}
	}

	public void OnSceneLoaded()
	{
		Match3ResultContext match3ResultContext = Singleton<SceneLoader>.Instance.context as Match3ResultContext;
		if (match3ResultContext != null)
		{
			HandleResult(match3ResultContext.result, match3ResultContext.stars, match3ResultContext.stats);
		}
		else if (Singleton<SceneLoader>.Instance.inSync)
		{
			if (runPending)
			{
				Run(retryIfCantLoad);
			}
			else if (running)
			{
				Run(useSave: true);
			}
			started = true;
		}
	}

	public void Fight(MetaStage stage, MetaStageDifficulty difficulty, Action<Result> callback = null, bool canExit = true)
	{
		bool flag = this.stage == stage && this.difficulty == difficulty;
		if (running)
		{
			if (flag)
			{
				retryIfCantLoad = true;
			}
			this.callback = callback;
			return;
		}
		this.callback = callback;
		this.stage = stage;
		this.difficulty = difficulty;
		this.canExit = canExit;
		rewardExp = stage.GetExp(difficulty);
		rewardCoins = Rand.NormalRangeInt(stage.GetCoinsMin(difficulty), stage.GetCoinsMax(difficulty));
		Singleton<Meta>.Instance.RequestSave();
		AnalyticsManager.Progression(AnalyticsProgression.Start, difficulty.ToString(), stage.location.analyticsID, stage.analyticsID);
		if (started)
		{
			Run(retryIfCantLoad);
		}
		else
		{
			runPending = true;
		}
	}

	private void Run(bool useSave)
	{
		Match3StartContext match3StartContext = new Match3StartContext();
		match3StartContext.stage = stage;
		match3StartContext.difficulty = difficulty;
		match3StartContext.useSave = useSave;
		match3StartContext.tutorialPrefab = stage.GetTutorial(difficulty);
		match3StartContext.tileProvider = stage.GetFixedBoard(difficulty);
		match3StartContext.canQuit = canExit;
		match3StartContext.stats = MetaPlayer.local.stats;
		match3StartContext.items = new List<MetaItem>();
		MetaItemSlot[] array = Singleton<Meta>.Instance.FindObjects<MetaItemSlot>();
		for (int i = 0; i < array.Length; i++)
		{
			MetaItem item = array[i].item;
			if (item != null)
			{
				match3StartContext.items.Add(item);
			}
		}
		match3StartContext.rewards = new List<MetaLink>();
		if ((bool)MetaRewardChestSlot.FindEmptySlot())
		{
			if (stage.GetGrinds(difficulty) == 0 && (bool)stage.GetFirstChest(difficulty))
			{
				match3StartContext.rewards.Add(MetaLink.Create(stage.GetFirstChest(difficulty), "count", 1));
			}
			else
			{
				match3StartContext.rewards.Add(MetaLink.Create(SingletonComponent<Meta, MetaChestSequence>.Instance.nextReward, "count", 1));
			}
		}
		if (rewardCoins > 0)
		{
			match3StartContext.rewards.Add(MetaLink.Create(MetaResource.coins, "count", rewardCoins));
		}
		if (rewardExp > 0)
		{
			match3StartContext.rewards.Add(MetaLink.Create(MetaResource.exp, "count", rewardExp));
		}
		Singleton<SceneLoader>.Instance.SwitchToMatch3(SceneLoader.Priority.Fight, match3StartContext);
	}

	private void Reset()
	{
		retryIfCantLoad = false;
		callback = null;
		stage = null;
		difficulty = MetaStageDifficulty.Easy;
		rewardExp = 0;
		rewardCoins = 0;
		canExit = false;
		Singleton<Meta>.Instance.RequestSave();
	}

	private void HandleResult(Result result, int stars, Match3Stats stats)
	{
		if (result == Result.Abort && retryIfCantLoad)
		{
			retryIfCantLoad = false;
			Run(useSave: false);
			return;
		}
		if (callback != null)
		{
			callback(result);
			callback = null;
		}
		switch (result)
		{
		case Result.Win:
		{
			int grinds = stage.GetGrinds(difficulty);
			MetaRewardChestSlot metaRewardChestSlot = MetaRewardChestSlot.FindEmptySlot();
			MetaRewardChest rewardChest = null;
			MetaChestContent rewardContent = null;
			MetaItem rewardChestItem = stage.GetChanceItem(difficulty);
			if ((bool)metaRewardChestSlot)
			{
				if (grinds == 0 && (bool)stage.GetFirstChest(difficulty))
				{
					rewardChest = stage.GetFirstChest(difficulty);
					rewardContent = stage.GetFirstChestContent(difficulty);
				}
				else
				{
					rewardChest = SingletonComponent<Meta, MetaChestSequence>.Instance.GetReward();
				}
			}
			if ((bool)rewardChest)
			{
				MetaRewardChestSlotView metaRewardChestSlotView = MetaView.FindMasterView<MetaRewardChestSlotView>(metaRewardChestSlot);
				if ((bool)metaRewardChestSlotView)
				{
					SequenceManager.Enqueue(metaRewardChestSlotView.InsertSequencePre());
					SequenceManager.Enqueue(delegate
					{
						if ((bool)rewardContent)
						{
							MetaRewardChestSlot.AddChest(rewardChest, rewardContent);
						}
						else
						{
							MetaRewardChestSlot.AddChest(rewardChest, rewardChestItem);
						}
					});
					SequenceManager.Enqueue(metaRewardChestSlotView.InsertSequencePost());
				}
				else if ((bool)rewardContent)
				{
					MetaRewardChestSlot.AddChest(rewardChest, rewardContent);
				}
				else
				{
					MetaRewardChestSlot.AddChest(rewardChest, rewardChestItem);
				}
			}
			if (rewardCoins > 0)
			{
				int diff = rewardCoins;
				MetaResourceView metaResourceView = MetaView.FindMasterView<MetaResourceView>(MetaResource.coins);
				if ((bool)metaResourceView)
				{
					SequenceManager.Enqueue(metaResourceView.DropSequencePre(diff));
					SequenceManager.Enqueue(delegate
					{
						MetaResource.coins.count += diff;
					});
				}
				else
				{
					MetaResource.coins.count += diff;
				}
				AnalyticsManager.ResourceSource(MetaResource.coins.analyticsID, diff, "LocationStage", stage.location.analyticsID);
			}
			if (rewardExp > 0)
			{
				int diff2 = rewardExp;
				MetaPlayerView metaPlayerView = MetaView.FindMasterView<MetaPlayerView>(MetaPlayer.local);
				if ((bool)metaPlayerView)
				{
					SequenceManager.Enqueue(metaPlayerView.ExpSequencePre(diff2));
					SequenceManager.Enqueue(delegate
					{
						MetaResource.exp.count += diff2;
					});
				}
				else
				{
					MetaResource.exp.count += diff2;
				}
				AnalyticsManager.ResourceSource(MetaResource.exp.analyticsID, diff2, "LocationStage", stage.location.analyticsID);
			}
			MetaVictoryChestSlot.AddStars(stars);
			stage.AddGrind(difficulty, 1);
			stage.AddStar(difficulty, stars);
			if (grinds == 0 && stage.IsLastStageOnLocation())
			{
				MetaLocation metaLocation = stage.location.FindNextLocation();
				if ((bool)metaLocation)
				{
					MetaLocationView metaLocationView = MetaView.FindMasterView<MetaLocationView>(metaLocation);
					if ((bool)metaLocationView)
					{
						MetaStage stageCapture = stage;
						SequenceManager.Enqueue(metaLocationView.UnlockSequencePre());
						SequenceManager.Enqueue(delegate
						{
							stageCapture.UnlockNextIfPossible();
						});
						SequenceManager.Enqueue(metaLocationView.UnlockSequencePost());
					}
					else
					{
						stage.UnlockNextIfPossible();
					}
				}
				else
				{
					stage.UnlockNextIfPossible();
				}
			}
			else
			{
				stage.UnlockNextIfPossible();
			}
			MetaAnalytics instance = SingletonComponent<Meta, MetaAnalytics>.Instance;
			AnalyticsManager.Progression(AnalyticsProgression.Complete, difficulty.ToString(), stage.location.analyticsID, stage.analyticsID, stats.allMoves);
			if (grinds == 0)
			{
				AnalyticsManager.Design("StageBeaten", difficulty.ToString(), stage.location.analyticsID, stage.analyticsID, new Dictionary<string, object>
				{
					{
						"Grinds",
						instance.grinds
					},
					{
						"GemsSpent",
						instance.gemsSpent
					},
					{
						"CoinsSpent",
						instance.coinsSpent
					},
					{
						"GemsCurrent",
						instance.gemsCurrent
					},
					{
						"CoinsCurrent",
						instance.coinsCurrent
					},
					{
						"ItemsUpgradable",
						instance.itemsUpgradable
					},
					{
						"RealTime",
						instance.realTime
					},
					{
						"GameTime",
						instance.gameTime
					}
				});
			}
			Reset();
			break;
		}
		case Result.Abort:
			AnalyticsManager.Progression(AnalyticsProgression.Fail, difficulty.ToString(), stage.location.analyticsID, stage.analyticsID);
			Reset();
			Singleton<SceneLoader>.Instance.SwitchToSync(SceneLoader.Priority.Fight);
			break;
		default:
			AnalyticsManager.Progression(AnalyticsProgression.Fail, difficulty.ToString(), stage.location.analyticsID, stage.analyticsID, stats.allMoves);
			Reset();
			break;
		}
	}

	private bool IsValid()
	{
		return true;
	}
}
