using System.Linq;
using UnityEngine;

namespace Tutorial.Navigators
{
	public class ShowScreen : Element
	{
		[Header("Requirements")]
		public ScreenID expectedScreen;

		[Header("Behaviour")]
		public Indicator.Spawner indicator;

		protected virtual void Update()
		{
			if (base.active && (bool)ScreenMgr.instance)
			{
				base.complete = (ScreenMgr.instance.GetCurrentScreenID() == expectedScreen);
			}
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			ScreenButton screenButton = (from b in Object.FindObjectsOfType<ScreenButton>()
				where b.targetScreen == expectedScreen
				select b).FirstOrDefault();
			if ((bool)screenButton)
			{
				indicator.Spawn(base.gameObject, screenButton.gameObject);
				indicator.active = !base.complete;
			}
		}

		protected override void OnDeactivate()
		{
			indicator.active = false;
		}

		protected override void OnComplete()
		{
			indicator.active = false;
		}

		protected override void OnIncomplete()
		{
			indicator.active = true;
		}
	}
}
