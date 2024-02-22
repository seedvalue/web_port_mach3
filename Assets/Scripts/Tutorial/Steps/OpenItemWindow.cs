using UnityEngine;
using Utils;

namespace Tutorial.Steps
{
	public class OpenItemWindow : Element
	{
		[Header("Requirements")]
		public MetaItem item;

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
			Window openWindow = instance.GetOpenWindow(typeof(ItemWindow));
			if (openWindow == null)
			{
				return false;
			}
			ItemWindowContext itemWindowContext = openWindow.context as ItemWindowContext;
			if (itemWindowContext == null)
			{
				return false;
			}
			if ((bool)item && item != itemWindowContext.item)
			{
				return false;
			}
			return true;
		}
	}
}
