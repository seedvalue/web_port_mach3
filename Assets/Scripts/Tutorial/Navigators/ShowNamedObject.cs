using UnityEngine;

namespace Tutorial.Navigators
{
	public class ShowNamedObject : Element
	{
		[Header("Requirements")]
		public string namedObject;

		[Header("Behaviour")]
		public Indicator.Spawner indicator;

		private GameObject target;

		protected override void OnActivate()
		{
			base.OnActivate();
			target = NamedObject.Find(namedObject);
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
