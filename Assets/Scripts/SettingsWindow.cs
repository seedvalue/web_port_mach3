using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class SettingsWindow : Window
{
	[Header("Info")]
	public Text playerIdText;

	public Text versionText;

	[Header("Music")]
	public Button musicOnButton;

	public Button musicOffButton;

	[Header("Sfx")]
	public Button sfxOnButton;

	public Button sfxOffButton;

	[Header("Other")]
	public Button supportButton;

	public Button termsButton;

	protected virtual void OnEnable()
	{
		Helpers.SetText(playerIdText, "Player ID: " + MetaPlayer.local.id);
		Helpers.SetText(versionText, "ver. " + Application.version + ((!Development.developmentMode) ? string.Empty : " (dev)"));
		SetMusic(AudioManager.musicAllowed);
		SetSfx(AudioManager.soundAllowed);
	}

	protected override void Start()
	{
		base.Start();
		if ((bool)musicOnButton)
		{
			musicOnButton.onClick.AddListener(delegate
			{
				SetMusic(value: false);
			});
		}
		if ((bool)musicOffButton)
		{
			musicOffButton.onClick.AddListener(delegate
			{
				SetMusic(value: true);
			});
		}
		if ((bool)sfxOnButton)
		{
			sfxOnButton.onClick.AddListener(delegate
			{
				SetSfx(value: false);
			});
		}
		if ((bool)sfxOffButton)
		{
			sfxOffButton.onClick.AddListener(delegate
			{
				SetSfx(value: true);
			});
		}
		if ((bool)supportButton)
		{
			supportButton.onClick.AddListener(OnSupport);
		}
		if ((bool)termsButton)
		{
			termsButton.onClick.AddListener(OnTerms);
		}
	}

	private void SetMusic(bool value)
	{
		AudioManager.musicAllowed = value;
		Helpers.SetActiveGameObject(musicOnButton, value);
		Helpers.SetActiveGameObject(musicOffButton, !value);
	}

	private void SetSfx(bool value)
	{
		AudioManager.soundAllowed = value;
		Helpers.SetActiveGameObject(sfxOnButton, value);
		Helpers.SetActiveGameObject(sfxOffButton, !value);
	}

	private void OnSupport()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine();
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("---- DO NOT DELETE ----");
		stringBuilder.Append("Time: ");
		stringBuilder.AppendLine(SingletonComponent<Meta, MetaTimeManager>.Instance.utcNow.ToString());
		stringBuilder.Append("Version: ");
		stringBuilder.AppendLine(Application.version);
		stringBuilder.Append("Player ID: ");
		stringBuilder.AppendLine(MetaPlayer.local.id);
		stringBuilder.Append("Device: ");
		if (SystemInfo.deviceType == DeviceType.Handheld)
		{
			stringBuilder.AppendLine(SystemInfo.deviceModel);
		}
		else
		{
			stringBuilder.AppendLine(SystemInfo.deviceType.ToString());
		}
		stringBuilder.Append("System: ");
		stringBuilder.AppendLine(SystemInfo.operatingSystem);
		stringBuilder.Append("System Language: ");
		stringBuilder.AppendLine(Application.systemLanguage.ToString());
		stringBuilder.Append("Language: ");
		stringBuilder.AppendLine("English");
		stringBuilder.AppendLine("-----------------------");
		MailHelper.SendMail("support@artifexmundi.com", "Shrubbery Support", stringBuilder.ToString());
	}

	private void OnTerms()
	{
		Application.OpenURL("http://www.artifexmundi.com");
	}
}
