using System.Linq;
using UnityEngine.UI;
using Utils;

public class PickItemWindow : Window
{
	public Text titleText;

	public MetaItemView itemPrefab;

	public MetaContainer container;

	[WindowTestMethod]
	public static void TestWindow()
	{
		PickItemWindowContext pickItemWindowContext = new PickItemWindowContext();
		pickItemWindowContext.title = "-";
		pickItemWindowContext.items = Singleton<Meta>.Instance.FindObjects<MetaItem>().ToList();
		Singleton<WindowManager>.Instance.OpenWindow<PickItemWindow>(pickItemWindowContext);
	}

	protected virtual void OnEnable()
	{
		PickItemWindowContext pickItemWindowContext = base.context as PickItemWindowContext;
		if (pickItemWindowContext != null)
		{
			InitWithContext(pickItemWindowContext);
		}
	}

	protected virtual void OnDisable()
	{
		container.Clear();
	}

	private void InitWithContext(PickItemWindowContext context)
	{
		Helpers.SetText(titleText, context.title);
		container.Assign(context.items, itemPrefab);
		foreach (MetaView view in container.contents)
		{
			view.onClick.AddListener(delegate
			{
				OnViewClicked(view);
			});
		}
	}

	private void OnViewClicked(MetaView view)
	{
		CloseWindow(view.GetObject());
	}
}
