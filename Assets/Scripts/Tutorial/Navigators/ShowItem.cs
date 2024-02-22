using System.Linq;
using UnityEngine;

namespace Tutorial.Navigators
{
	public class ShowItem : Element
	{
		[Header("Requirements")]
		public string namedGroup;

		public MetaItem item;

		[Header("Behaviour")]
		public Indicator.Spawner indicator;

		private MetaItemView target;

		private MetaItemView FindTarget()
		{
			GameObject gameObject = NamedObject.Find(namedGroup);
			if (!gameObject)
			{
				return null;
			}
			return gameObject.GetComponentsInChildren<MetaItemView>().Where(delegate(MetaItemView v)
			{
				MetaItem @object = v.GetObject();
				if (!@object)
				{
					return false;
				}
				return ((bool)item && item != @object) ? false : true;
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
