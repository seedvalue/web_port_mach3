using UnityEngine;

public class M3StarAnimation : MonoBehaviour
{
	private StageWinWindow window;

	private void Start()
	{
		window = UnityEngine.Object.FindObjectOfType<StageWinWindow>();
	}

	public void OnStarAppear(int starIndex)
	{
		if ((bool)window)
		{
			window.PlayStarSound(starIndex);
		}
	}
}
