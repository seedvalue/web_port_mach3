using UnityEngine;

namespace Tutorial.Steps
{
	public class ScrollableOnScreen : Element
	{
		[Header("Requirements")]
		public string namedScrollable;

		public MetaObject metaObject;

		[Header("Behaviour")]
		public string indicatorNamedCamera;

		public Indicator.Spawner negativeIndicator;

		public Indicator.Spawner positiveIndicator;

		private IMetaScrollable target;

		private GameObject indicatorTarget;

		private IMetaScrollable FindTarget()
		{
			GameObject gameObject = NamedObject.Find(namedScrollable);
			if (!gameObject)
			{
				return null;
			}
			return gameObject.GetComponentInChildrenAs<IMetaScrollable>();
		}

		private GameObject FindIndicatorTarget()
		{
			return NamedObject.Find(namedScrollable);
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

		protected virtual void Update()
		{
			if (target != null)
			{
				float num = target.RequiredScrollToShow(metaObject);
				base.complete = (num == 0f);
				negativeIndicator.active = (base.active && !base.complete && num < 0f);
				positiveIndicator.active = (base.active && !base.complete && num > 0f);
			}
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			target = FindTarget();
			indicatorTarget = FindIndicatorTarget();
			if (target == null || indicatorTarget == null)
			{
				base.complete = true;
				return;
			}
			Camera fromCamera = FindCamera();
			negativeIndicator.Spawn(base.gameObject, indicatorTarget, fromCamera);
			positiveIndicator.Spawn(base.gameObject, indicatorTarget, fromCamera);
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			negativeIndicator.active = false;
			positiveIndicator.active = false;
		}
	}
}
