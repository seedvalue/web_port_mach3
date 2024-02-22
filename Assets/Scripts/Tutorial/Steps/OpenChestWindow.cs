using UnityEngine;
using Utils;

namespace Tutorial.Steps
{
	public class OpenChestWindow : Element
	{
		[Header("Requirements")]
		public MetaChest chest;

		public MetaChestContent content;

		protected override void OnActivate()
		{
			base.OnActivate();
			WindowManager instance = Singleton<WindowManager>.Instance;
			instance.WindowOpened += WindowChanged;
			instance.WindowClosed += WindowChanged;
			base.complete = CheckConditions();
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			WindowManager instance = Singleton<WindowManager>.Instance;
			instance.WindowOpened -= WindowChanged;
			instance.WindowClosed -= WindowChanged;
		}

		private void WindowChanged(Window window)
		{
			base.complete = CheckConditions();
		}

		private bool CheckConditions()
		{
			WindowManager instance = Singleton<WindowManager>.Instance;
			Window openWindow = instance.GetOpenWindow(typeof(ChestWindow));
			if (openWindow == null)
			{
				return false;
			}
			ChestWindowContext chestWindowContext = openWindow.context as ChestWindowContext;
			if (chestWindowContext == null)
			{
				return false;
			}
			if ((bool)chest && chest != chestWindowContext.chest)
			{
				return false;
			}
			if ((bool)content && content != chestWindowContext.content)
			{
				return false;
			}
			return true;
		}
	}
}
