using Tutorial;
using UnityEngine;
using Utils;

[CreateAssetMenu(fileName = "Tutorial", menuName = "Meta/Tutorial")]
public class MetaTutorial : MetaObject, IAnalyticsItem
{
	public MetaTutorialRequirement[] requirements;

	public Tutorial.Tutorial prefab;

	public const string stateProperty = "state";

	private const string EditorPrefsDisable = "Meta.DisableTutorial";

	[MetaData(null, 0)]
	private MetaTutorialState _state;

	private Tutorial.Tutorial instance;

	public MetaTutorialState state
	{
		get
		{
			return _state;
		}
		set
		{
			PropertySetter(ref _state, value, "state");
		}
	}

	public static bool editorDisable
	{
		get
		{
			return EditorPrefsHelper.GetBool("Meta.DisableTutorial");
		}
		set
		{
			EditorPrefsHelper.SetBool("Meta.DisableTutorial", value);
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

	protected virtual void Awake()
	{
		analyticsID = AnalyticsManager.ResolveID(base.metaID);
		analyticsType = AnalyticsManager.ResolveType(base.metaID);
	}

	protected virtual void MetaAwake()
	{
		for (int i = 0; i < requirements.Length; i++)
		{
			if (!requirements[i].Validate())
			{
				UnityEngine.Debug.LogErrorFormat("MetaTutorial '{0}' requirement {1} is invalid!", base.metaID, i);
			}
		}
	}

	protected virtual void MetaStart()
	{
		if (editorDisable || !IsValid())
		{
			return;
		}
		if (state == MetaTutorialState.Inactive)
		{
			if (AreRequirementsMet())
			{
				state = MetaTutorialState.Active;
				return;
			}
			for (int i = 0; i < requirements.Length; i++)
			{
				DependsOn(requirements[i].target);
			}
		}
		else if (state == MetaTutorialState.Active)
		{
			Start();
		}
	}

	protected override void OnDependencyChanged(MetaObject metaObject, string propertyName, object before, object after)
	{
		base.OnDependencyChanged(metaObject, propertyName, before, after);
		if (state == MetaTutorialState.Inactive && AreRequirementsMet())
		{
			state = MetaTutorialState.Active;
		}
	}

	protected override void OnPropertyChanged(string propertyName, object before, object after)
	{
		base.OnPropertyChanged(propertyName, before, after);
		if (propertyName == "state" && state == MetaTutorialState.Active)
		{
			Start();
		}
	}

	protected bool IsValid()
	{
		for (int i = 0; i < requirements.Length; i++)
		{
			if (!requirements[i].Validate())
			{
				UnityEngine.Debug.LogErrorFormat("MetaTutorial '{0}' requirement {1} is invalid!", base.metaID, i);
				return false;
			}
		}
		return true;
	}

	protected bool AreRequirementsMet()
	{
		if (requirements.Length == 0)
		{
			return true;
		}
		for (int i = 0; i < requirements.Length; i++)
		{
			if (!requirements[i].completed)
			{
				return false;
			}
		}
		return true;
	}

	protected void Start()
	{
		if (!instance)
		{
			instance = Singleton<TutorialManager>.Instance.CreateTutorial(prefab, End);
		}
	}

	protected void End()
	{
		if ((bool)instance)
		{
			instance = null;
			state = MetaTutorialState.Completed;
			AnalyticsManager.Design(analyticsType, analyticsID);
		}
	}
}
