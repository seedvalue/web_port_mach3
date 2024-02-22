using UnityEngine;

namespace Tutorial.Steps
{
	public class Dummy : Element
	{
		public bool completeLocal;

		protected void Update()
		{
			base.complete = completeLocal;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			UnityEngine.Debug.Log("OnActivate: " + base.name);
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			UnityEngine.Debug.Log("OnDeactivate: " + base.name);
		}

		protected override void OnComplete()
		{
			base.OnComplete();
			UnityEngine.Debug.Log("OnComplete: " + base.name);
		}

		protected override void OnIncomplete()
		{
			base.OnIncomplete();
			UnityEngine.Debug.Log("OnIncomplete: " + base.name);
		}

		protected override void OnSubComplete()
		{
			base.OnSubComplete();
			UnityEngine.Debug.Log("OnSubComplete: " + base.name);
		}

		protected override void OnSubIncomplete()
		{
			base.OnSubIncomplete();
			UnityEngine.Debug.Log("OnSubIncomplete: " + base.name);
		}
	}
}
