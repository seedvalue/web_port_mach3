using System.Linq;
using UnityEngine;

namespace Tutorial.Navigators
{
	public class ShowItemSlot : Element
	{
		[Header("Requirements")]
		public string namedGroup;

		public MetaItemSlot slot;

		[Header("Behaviour")]
		public Indicator.Spawner indicator;

		private MetaItemSlotView target;

		private MetaItemSlotView FindTarget()
		{
			GameObject gameObject = NamedObject.Find(namedGroup);
			if (!gameObject)
			{
				return null;
			}
			return gameObject.GetComponentsInChildren<MetaItemSlotView>().Where(delegate(MetaItemSlotView v)
			{
				MetaItemSlot @object = v.GetObject();
				if (!@object)
				{
					return false;
				}
				return ((bool)slot && slot != @object) ? false : true;
			}).FirstOrDefault();
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
