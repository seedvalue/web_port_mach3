using UnityEngine;
using UnityEngine.UI;
using Utils;

[RequireComponent(typeof(Button))]
public class ArmorySortButton : MonoBehaviour
{
	public Text labelText;

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
			armory.OrderItems();
		}
	}

	public void SetLabel(string text)
	{
		Helpers.SetText(labelText, text);
	}
}
