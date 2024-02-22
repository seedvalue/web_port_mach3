using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class M3StatInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IEventSystemHandler
{
	public GameObject stats;

	public float minDisplayDuration = 1f;

	public Text[] statTexts = new Text[6];

	private bool canExit = true;

	private bool isActive;

	private float activeTime;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!M3TileManager.instance.TutorialAllows(M3TutorialActivity.StatsPreview))
		{
			return;
		}
		isActive = true;
		activeTime = 0f;
		stats.SetActive(value: true);
		for (int i = 0; i < 6; i++)
		{
			if ((bool)statTexts[i])
			{
				statTexts[i].text = M3Player.instance.GetStat((M3Orb)i).ToString();
			}
		}
		M3TileManager.instance.TutorialActivityFinished(M3TutorialActivity.StatsPreview);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		canExit = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		OnPointerExit(eventData);
	}

	private void Start()
	{
		stats.SetActive(value: false);
	}

	private void Update()
	{
		if (isActive)
		{
			activeTime += Time.deltaTime;
		}
		if (canExit && activeTime >= minDisplayDuration)
		{
			canExit = false;
			isActive = false;
			stats.SetActive(value: false);
		}
	}
}
