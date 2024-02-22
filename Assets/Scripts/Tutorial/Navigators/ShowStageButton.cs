using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial.Navigators
{
	public class ShowStageButton : Element
	{
		public enum ButtonType
		{
			Fight
		}

		[Header("Requirements")]
		public string namedGroup;

		public MetaStage stage;

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
			return (from s in gameObject.GetComponentsInChildren<MetaStageView>()
				where ((bool)stage && stage != s.GetObject()) ? false : true
				select (buttonType == ButtonType.Fight) ? s.fightButton : null into b
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
