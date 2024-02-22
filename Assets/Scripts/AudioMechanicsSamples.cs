using System;

[Serializable]
public class AudioMechanicsSamples
{
	public AudioSample click;

	public AudioSample clickItem;

	public AudioSample clickCoins;

	public AudioSample clickGems;

	public AudioSample clickBattle;

	public AudioSample GetSample(AudioMechanics mechanics)
	{
		switch (mechanics)
		{
		case AudioMechanics.Click:
			return click;
		case AudioMechanics.ClickItem:
			return clickItem;
		case AudioMechanics.ClickCoins:
			return clickCoins;
		case AudioMechanics.ClickGems:
			return clickGems;
		case AudioMechanics.ClickBattle:
			return clickBattle;
		default:
			return null;
		}
	}
}
