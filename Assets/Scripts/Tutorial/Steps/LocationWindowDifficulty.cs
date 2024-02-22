using UnityEngine;
using Utils;

namespace Tutorial.Steps
{
	public class LocationWindowDifficulty : Element
	{
		[Header("Requirements")]
		public MetaStageDifficulty difficulty;

		protected override void OnActivate()
		{
			base.OnActivate();
			base.complete = CheckConditions();
		}

		protected virtual void Update()
		{
			base.complete = CheckConditions();
		}

		private bool CheckConditions()
		{
			WindowManager instance = Singleton<WindowManager>.Instance;
			LocationWindow openWindow = instance.GetOpenWindow<LocationWindow>();
			if (openWindow == null)
			{
				return false;
			}
			if (openWindow.currentDifficulty != difficulty)
			{
				return false;
			}
			return true;
		}
	}
}
