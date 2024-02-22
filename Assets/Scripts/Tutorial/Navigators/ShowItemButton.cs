using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial.Navigators
{
	public class ShowItemButton : Element
	{
		public enum ButtonType
		{
			Use,
			Swap,
			View,
			Upgrade
		}

		[Header("Requirements")]
		public string namedGroup;

		public MetaItem item;

		public ButtonType buttonType;

		[Header("Behaviour")]
		public Indicator.Spawner indicator;

		private Button target;

		private Button FindTarget()
		{
			GameObject gameObject = NamedObject.Find(namedGroup);
			if (!gameObject)
			{
				return null;
			}
			return (from b in (from s in gameObject.GetComponentsInChildren<MetaItemViewSelectable>()
					where ((bool)item && item != s.GetObject()) ? false : true
					select s).Select(delegate(MetaItemViewSelectable s)
				{
					switch (buttonType)
					{
					case ButtonType.Use:
						return s.useButton;
					case ButtonType.Swap:
						return s.swapButton;
					case ButtonType.View:
						return s.viewButton;
					case ButtonType.Upgrade:
						return s.upgradeButton;
					default:
						return null;
					}
				})
				where b != null
				select b).FirstOrDefault();
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			target = FindTarget();
			if (!target)
			{
				base.complete = true;
				return;
			}
			indicator.Spawn(base.gameObject, target.gameObject);
			indicator.active = !base.complete;
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
