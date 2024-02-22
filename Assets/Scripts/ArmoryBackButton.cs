using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ArmoryBackButton : MonoBehaviour
{
	private Armory armory;

	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(OnClick);
		armory = GetComponentInParent<Armory>();
	}

	public void OnClick()
	{
		if ((bool)armory)
		{
			armory.ScrollToTop();
		}
	}
}
