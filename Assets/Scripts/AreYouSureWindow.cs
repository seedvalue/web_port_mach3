using UnityEngine.UI;
using Utils;

public class AreYouSureWindow : Window
{
	public Text questionText;

	public Button positiveButton;

	public Button negativeButton;

	protected virtual void OnEnable()
	{
		string value = base.context as string;
		Helpers.SetText(questionText, value);
	}

	protected override void Start()
	{
		base.Start();
		if ((bool)positiveButton)
		{
			positiveButton.onClick.AddListener(delegate
			{
				CloseWindow(true);
			});
		}
		if ((bool)negativeButton)
		{
			negativeButton.onClick.AddListener(delegate
			{
				CloseWindow();
			});
		}
	}
}
