using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

[WorkbookLoad("Stages")]
[WorkbookAssetPath("Stages")]
public class MetaStage : MetaObject, IAnalyticsItem
{
	public class Battle
	{
		public MobStatsMultipliers statsMultipliers;

		public MetaMob[] mobs;
	}

	public const string easyGrindsProperty = "easyGrinds";

	public const string mediumGrindsProperty = "mediumGrinds";

	public const string hardGrindsProperty = "hardGrinds";

	public const string easyUnlockedProperty = "easyUnlocked";

	public const string mediumUnlockedProperty = "mediumUnlocked";

	public const string hardUnlockedProperty = "hardUnlocked";

	public const string easyStarsProperty = "easyStars";

	public const string mediumStarsProperty = "mediumUStars";

	public const string hardStarsProperty = "hardStars";

	public const string easyStarsShownProperty = "easyStarsShown";

	public const string mediumStarsShownProperty = "mediumUStarsShown";

	public const string hardStarsShownProperty = "hardStarsShown";

	public string displayName;

	[WorkbookAlias("Level")]
	public string sceneName;

	public Sprite icon;

	public MetaLocation location;

	public int index;

	public int star2;

	public int star3;

	private List<Battle> _battles;

	public static MetaStage[] globalStages;

	public int easyExp;

	public float easyCoinsMin;

	public float easyCoinsMax;

	public MetaRewardChest easyFirstChest;

	public MetaChestContent easyFirstChestContent;

	public M3Tutorial easyTutorial;

	public M3FixedBoard easyFixedBoard;

	public MetaItem easyChanceItem;

	[MetaData(null, 0)]
	private int _easyStarsShown;

	[MetaData(null, 0)]
	private int _easyStars;

	[WorkbookFlat(prefix = "Easy")]
	public MobStatsMultipliers easyMultipliers;

	[MetaData(null, 0)]
	private bool _easyUnlocked;

	[MetaData(null, 0)]
	private int _easyGrinds;

	public int mediumExp;

	public float mediumCoinsMin;

	public float mediumCoinsMax;

	public MetaItem mediumChanceItem;

	[WorkbookFlat(prefix = "Medium")]
	public MobStatsMultipliers mediumMultipliers;

	[MetaData(null, 0)]
	private int _mediumStarsShown;

	[MetaData(null, 0)]
	private int _mediumStars;

	[MetaData(null, 0)]
	private bool _mediumUnlocked;

	[MetaData(null, 0)]
	private int _mediumGrinds;

	public int hardExp;

	public float hardCoinsMin;

	public float hardCoinsMax;

	public MetaItem hardChanceItem;

	[WorkbookFlat(prefix = "Hard")]
	public MobStatsMultipliers hardMultipliers;

	[MetaData(null, 0)]
	private int _hardStarsShown;

	[MetaData(null, 0)]
	private int _hardStars;

	[MetaData(null, 0)]
	private bool _hardUnlocked;

	[MetaData(null, 0)]
	private int _hardGrinds;

	public List<Battle> battles
	{
		get
		{
			BuildBattles();
			return _battles;
		}
	}

	public int globalIndex
	{
		get;
		private set;
	}

	public int easyStarsShown
	{
		get
		{
			return _easyStarsShown;
		}
		set
		{
			PropertySetter(ref _easyStarsShown, value, "easyStarsShown");
		}
	}

	public int easyStars
	{
		get
		{
			return _easyStars;
		}
		set
		{
			PropertySetter(ref _easyStars, value, "easyStars");
		}
	}

	public bool easyUnlocked
	{
		get
		{
			return _easyUnlocked;
		}
		set
		{
			PropertySetter(ref _easyUnlocked, value, "easyUnlocked");
		}
	}

	public int easyGrinds
	{
		get
		{
			return _easyGrinds;
		}
		set
		{
			PropertySetter(ref _easyGrinds, value, "easyGrinds");
		}
	}

	public int mediumStarsShown
	{
		get
		{
			return _mediumStarsShown;
		}
		set
		{
			PropertySetter(ref _mediumStarsShown, value, "mediumUStarsShown");
		}
	}

	public int mediumStars
	{
		get
		{
			return _mediumStars;
		}
		set
		{
			PropertySetter(ref _mediumStars, value, "mediumUStars");
		}
	}

	public bool mediumUnlocked
	{
		get
		{
			return _mediumUnlocked;
		}
		set
		{
			PropertySetter(ref _mediumUnlocked, value, "mediumUnlocked");
		}
	}

	public int mediumGrinds
	{
		get
		{
			return _mediumGrinds;
		}
		set
		{
			PropertySetter(ref _mediumGrinds, value, "mediumGrinds");
		}
	}

	public int hardStarsShown
	{
		get
		{
			return _hardStarsShown;
		}
		set
		{
			PropertySetter(ref _hardStarsShown, value, "hardStarsShown");
		}
	}

	public int hardStars
	{
		get
		{
			return _hardStars;
		}
		set
		{
			PropertySetter(ref _hardStars, value, "hardStars");
		}
	}

	public bool hardUnlocked
	{
		get
		{
			return _hardUnlocked;
		}
		set
		{
			PropertySetter(ref _hardUnlocked, value, "hardUnlocked");
		}
	}

	public int hardGrinds
	{
		get
		{
			return _hardGrinds;
		}
		set
		{
			PropertySetter(ref _hardGrinds, value, "hardGrinds");
		}
	}

	public string analyticsID
	{
		get;
		private set;
	}

	public string analyticsType
	{
		get;
		private set;
	}

	public int GetExp(MetaStageDifficulty difficulty)
	{
		switch (difficulty)
		{
		default:
			return easyExp;
		case MetaStageDifficulty.Medium:
			return mediumExp;
		case MetaStageDifficulty.Hard:
			return hardExp;
		}
	}

	public float GetCoinsMin(MetaStageDifficulty difficulty)
	{
		switch (difficulty)
		{
		default:
			return easyCoinsMin;
		case MetaStageDifficulty.Medium:
			return mediumCoinsMin;
		case MetaStageDifficulty.Hard:
			return hardCoinsMin;
		}
	}

	public float GetCoinsMax(MetaStageDifficulty difficulty)
	{
		switch (difficulty)
		{
		default:
			return easyCoinsMax;
		case MetaStageDifficulty.Medium:
			return mediumCoinsMax;
		case MetaStageDifficulty.Hard:
			return hardCoinsMax;
		}
	}

	public MetaRewardChest GetFirstChest(MetaStageDifficulty difficulty)
	{
		switch (difficulty)
		{
		default:
			return easyFirstChest;
		case MetaStageDifficulty.Medium:
			return null;
		case MetaStageDifficulty.Hard:
			return null;
		}
	}

	public MetaChestContent GetFirstChestContent(MetaStageDifficulty difficulty)
	{
		switch (difficulty)
		{
		default:
			return easyFirstChestContent;
		case MetaStageDifficulty.Medium:
			return null;
		case MetaStageDifficulty.Hard:
			return null;
		}
	}

	public M3Tutorial GetTutorial(MetaStageDifficulty difficulty)
	{
		switch (difficulty)
		{
		default:
			return easyTutorial;
		case MetaStageDifficulty.Medium:
			return null;
		case MetaStageDifficulty.Hard:
			return null;
		}
	}

	public M3FixedBoard GetFixedBoard(MetaStageDifficulty difficulty)
	{
		switch (difficulty)
		{
		default:
			return easyFixedBoard;
		case MetaStageDifficulty.Medium:
			return null;
		case MetaStageDifficulty.Hard:
			return null;
		}
	}

	public MobStatsMultipliers GetMultipliers(MetaStageDifficulty difficulty)
	{
		switch (difficulty)
		{
		default:
			return easyMultipliers;
		case MetaStageDifficulty.Medium:
			return mediumMultipliers;
		case MetaStageDifficulty.Hard:
			return hardMultipliers;
		}
	}

	public bool GetUnlocked(MetaStageDifficulty difficulty)
	{
		switch (difficulty)
		{
		default:
			return easyUnlocked;
		case MetaStageDifficulty.Medium:
			return mediumUnlocked;
		case MetaStageDifficulty.Hard:
			return hardUnlocked;
		}
	}

	public int GetGrinds(MetaStageDifficulty difficulty)
	{
		switch (difficulty)
		{
		default:
			return easyGrinds;
		case MetaStageDifficulty.Medium:
			return mediumGrinds;
		case MetaStageDifficulty.Hard:
			return hardGrinds;
		}
	}

	public void AddGrind(MetaStageDifficulty difficulty, int value)
	{
		switch (difficulty)
		{
		default:
			easyGrinds++;
			break;
		case MetaStageDifficulty.Medium:
			mediumGrinds++;
			break;
		case MetaStageDifficulty.Hard:
			hardGrinds++;
			break;
		}
	}

	public void AddStar(MetaStageDifficulty difficulty, int value)
	{
		switch (difficulty)
		{
		default:
			easyStars = Mathf.Clamp(Mathf.Max(easyStars, value), 0, 3);
			break;
		case MetaStageDifficulty.Medium:
			mediumStars = Mathf.Clamp(Mathf.Max(mediumStars, value), 0, 3);
			break;
		case MetaStageDifficulty.Hard:
			hardStars = Mathf.Clamp(Mathf.Max(hardStars, value), 0, 3);
			break;
		}
	}

	public int GetStars(MetaStageDifficulty difficulty)
	{
		switch (difficulty)
		{
		default:
			return easyStars;
		case MetaStageDifficulty.Medium:
			return mediumStars;
		case MetaStageDifficulty.Hard:
			return hardStars;
		}
	}

	public int GetStarsShown(MetaStageDifficulty difficulty)
	{
		switch (difficulty)
		{
		default:
			return easyStarsShown;
		case MetaStageDifficulty.Medium:
			return mediumStarsShown;
		case MetaStageDifficulty.Hard:
			return hardStarsShown;
		}
	}

	public void StarsShown(MetaStageDifficulty difficulty)
	{
		switch (difficulty)
		{
		default:
			easyStarsShown = easyStars;
			break;
		case MetaStageDifficulty.Medium:
			mediumStarsShown = mediumStars;
			break;
		case MetaStageDifficulty.Hard:
			hardStarsShown = hardStars;
			break;
		}
	}

	public MetaItem GetChanceItem(MetaStageDifficulty difficulty)
	{
		switch (difficulty)
		{
		default:
			return easyChanceItem;
		case MetaStageDifficulty.Medium:
			return mediumChanceItem;
		case MetaStageDifficulty.Hard:
			return hardChanceItem;
		}
	}

	protected virtual void Awake()
	{
		analyticsID = AnalyticsManager.ResolveID(base.metaID);
		analyticsType = AnalyticsManager.ResolveType(base.metaID);
	}

	protected static void MetaStaticAwake()
	{
		globalStages = (from s in Singleton<Meta>.Instance.FindObjects<MetaStage>()
			orderby s.index
			orderby s.location.index
			orderby s.location.settingIndex
			select s).ToArray();
		for (int i = 0; i < globalStages.Length; i++)
		{
			globalStages[i].globalIndex = i;
		}
	}

	protected virtual void MetaAwake()
	{
		if ((bool)location)
		{
			location.stages.Add(this);
		}
	}

	protected virtual void MetaStart()
	{
		if (globalIndex <= 0)
		{
			_easyUnlocked = true;
			_mediumUnlocked = true;
			_hardUnlocked = true;
		}
		UnlockNextIfPossible();
	}

	public bool IsLastStageOnLocation()
	{
		return location.stages.Last() == this;
	}

	public void UnlockNextIfPossible()
	{
		if (globalIndex < globalStages.Length - 1)
		{
			MetaStage metaStage = globalStages[globalIndex + 1];
			if (easyGrinds > 0)
			{
				metaStage.easyUnlocked = true;
			}
			if (mediumGrinds > 0)
			{
				metaStage.mediumUnlocked = true;
			}
			if (hardGrinds > 0)
			{
				metaStage.hardUnlocked = true;
			}
		}
	}

	protected void BuildBattles()
	{
		if (_battles == null)
		{
			Meta meta = Singleton<Meta>.Instance;
			MetaBalance instance = SingletonComponent<Meta, MetaBalance>.Instance;
			List<MetaBalance.StageBattlesData> stageBattlesData = instance.stageBattlesData;
			_battles = new List<Battle>();
			MetaBalance.StageBattlesData[] array = (from b in stageBattlesData
				where b.Stage == this
				orderby b.BattleIndex
				select b).ToArray();
			foreach (MetaBalance.StageBattlesData stageBattlesData2 in array)
			{
				Battle battle = new Battle();
				battle.statsMultipliers = stageBattlesData2.Multipliers;
				battle.mobs = (from m in stageBattlesData2.Mobs
					select meta.FindObject(m) as MetaMob into m
					where m != null
					select m).ToArray();
				_battles.Add(battle);
			}
		}
	}
}
