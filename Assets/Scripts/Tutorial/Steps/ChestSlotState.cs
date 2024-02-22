using System.Linq;
using UnityEngine;

namespace Tutorial.Steps
{
	public class ChestSlotState : Element
	{
		[Header("Requirements")]
		public string namedGroup;

		public MetaChest chest;

		public MetaChestContent content;

		public MetaRewardChestSlotState state;

		private MetaRewardChestSlot target;

		private MetaRewardChestSlot FindTarget()
		{
			GameObject gameObject = NamedObject.Find(namedGroup);
			if (!gameObject)
			{
				return null;
			}
			return (from v in gameObject.GetComponentsInChildren<MetaView>()
				select v.GetObject()).Cast<MetaRewardChestSlot>().Where(delegate(MetaRewardChestSlot s)
			{
				if (!s)
				{
					return false;
				}
				if ((bool)chest && chest != s.chest)
				{
					return false;
				}
				return ((bool)content && content != s.content) ? false : true;
			}).FirstOrDefault();
		}

		protected virtual void Update()
		{
			if ((bool)target)
			{
				base.complete = (target.state == state);
			}
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			target = FindTarget();
			if (!target)
			{
				base.complete = true;
			}
		}
	}
}
