using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AudioButton : MonoBehaviour
{
	public AudioButtonType type;

	private Button target;

	protected virtual void OnEnable()
	{
		target = GetComponent<Button>();
		if ((bool)target)
		{
			target.onClick.AddListener(OnClick);
		}
	}

	protected virtual void OnDisable()
	{
		if ((bool)target)
		{
			target.onClick.RemoveListener(OnClick);
		}
		target = null;
	}

	protected virtual void OnClick()
	{
		AudioManager.Play(ToAudioMechanics(type));
	}

	public static AudioMechanics ToAudioMechanics(AudioButtonType type)
	{
		switch (type)
		{
		default:
			return AudioMechanics.Click;
		case AudioButtonType.Item:
			return AudioMechanics.ClickItem;
		case AudioButtonType.Coins:
			return AudioMechanics.ClickCoins;
		case AudioButtonType.Gems:
			return AudioMechanics.ClickGems;
		case AudioButtonType.Battle:
			return AudioMechanics.ClickBattle;
		}
	}
}
