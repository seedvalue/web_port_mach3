using UnityEngine;
using UnityEngine.UI;
using Utils;

[RequireComponent(typeof(Button))]
public class SettingsWindowButton : MonoBehaviour
{
	private void Start()
	{
		Button component = GetComponent<Button>();
		component.onClick.AddListener(delegate
		{
			Singleton<WindowManager>.Instance.OpenWindow<SettingsWindow>();
		});
	}
}
