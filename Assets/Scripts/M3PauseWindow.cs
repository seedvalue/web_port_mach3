using UnityEngine.UI;
using Utils;

public class M3PauseWindow : MetaWindow
{
	public Text textLocation;

	public Text textStage;

	public Button buttonQuit;

	public Text[] attributeTexts = new Text[6];

	protected virtual void OnEnable()
	{
		M3PauseWindowContext m3PauseWindowContext = base.context as M3PauseWindowContext;
		if (m3PauseWindowContext != null)
		{
			Init(m3PauseWindowContext.metaStage, m3PauseWindowContext.canQuit);
		}
	}

	private void Init(MetaStage stage, bool canQuit)
	{
		if ((bool)textLocation)
		{
			textLocation.text = stage.location.displayName;
		}
		if ((bool)textStage)
		{
			textStage.text = stage.displayName;
		}
		for (int i = 0; i < attributeTexts.Length; i++)
		{
			attributeTexts[i].text = M3Player.instance.GetStat((M3Orb)i).ToString();
		}
		if ((bool)buttonQuit)
		{
			buttonQuit.gameObject.SetActive(canQuit);
		}
	}

	public void OnQuit()
	{
		Singleton<WindowManager>.Instance.OpenWindow<AreYouSureWindow>("Are you sure?", delegate(Window w, object o)
		{
			if (o != null)
			{
				CloseWindow(1);
			}
		});
	}

	public void OnSettings()
	{
		Singleton<WindowManager>.Instance.OpenWindow<SettingsWindow>();
	}
}
