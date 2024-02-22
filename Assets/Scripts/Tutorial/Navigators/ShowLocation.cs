using System.Linq;
using UnityEngine;

namespace Tutorial.Navigators
{
	public class ShowLocation : Element
	{
		[Header("Requirements")]
		public string namedGroup;

		public MetaLocation location;

		[Header("Behaviour")]
		public Indicator.Spawner indicator;

		public string indicatorNamedCamera;

		private MetaLocationView target;

		private MetaLocationView FindTarget()
		{
			GameObject gameObject = NamedObject.Find(namedGroup);
			if (!gameObject)
			{
				return null;
			}
			return gameObject.GetComponentsInChildren<MetaLocationView>().Where(delegate(MetaLocationView v)
			{
				MetaLocation @object = v.GetObject();
				if (!@object)
				{
					return false;
				}
				return ((bool)location && location != @object) ? false : true;
			}).FirstOrDefault();
		}

		private Camera FindCamera()
		{
			GameObject gameObject = NamedObject.Find(indicatorNamedCamera);
			if (!gameObject)
			{
				return null;
			}
			return gameObject.GetComponentInChildren<Camera>();
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
			indicator.Spawn(base.gameObject, target.gameObject, FindCamera());
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
