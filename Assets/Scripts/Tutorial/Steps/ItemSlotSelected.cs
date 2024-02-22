using System.Linq;
using UnityEngine;

namespace Tutorial.Steps
{
	public class ItemSlotSelected : Element
	{
		[Header("Requirements")]
		public string namedGroup;

		public MetaItemSlot slot;

		private MetaItemViewSelectable target;

		private MetaItemViewSelectable FindTarget()
		{
			GameObject gameObject = NamedObject.Find(namedGroup);
			if (!gameObject)
			{
				return null;
			}
			return (from s in gameObject.GetComponentsInChildren<MetaItemViewSelectable>()
				where ((bool)slot && slot.item != s.GetObject()) ? false : true
				select s).FirstOrDefault();
		}

		protected virtual void Update()
		{
			if ((bool)target)
			{
				base.complete = target.selected;
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
