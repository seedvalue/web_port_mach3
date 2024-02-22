using UnityEngine;
using Utils;

namespace Tutorial.Steps
{
	public class OpenLocationWindow : Element
	{
		[Header("Requirements")]
		public MetaLocation location;

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
			Window openWindow = instance.GetOpenWindow(typeof(LocationWindow));
			if (openWindow == null)
			{
				return false;
			}
			LocationWindowContext locationWindowContext = openWindow.context as LocationWindowContext;
			if (locationWindowContext == null)
			{
				return false;
			}
			if ((bool)location && location != locationWindowContext.location)
			{
				return false;
			}
			return true;
		}
	}
}
