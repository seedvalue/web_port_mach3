using System.Linq;
using UnityEngine;

namespace Tutorial.Navigators
{
	public class ShowChestSlot : Element
	{
		[Header("Requirements")]
		public string namedGroup;

		public MetaChest chest;

		public MetaChestContent content;

		[Header("Behaviour")]
		public Indicator.Spawner indicator;

		private MetaView target;

		private MetaView FindTarget()
		{
			GameObject gameObject = NamedObject.Find(namedGroup);
			if (!gameObject)
			{
				return null;
			}
			return gameObject.GetComponentsInChildren<MetaView>().Where(delegate(MetaView v)
			{
				MetaRewardChestSlot metaRewardChestSlot = v.GetObject() as MetaRewardChestSlot;
				if (!metaRewardChestSlot)
				{
					return false;
				}
				if ((bool)chest && chest != metaRewardChestSlot.chest)
				{
					return false;
				}
				return ((bool)content && content != metaRewardChestSlot.content) ? false : true;
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
