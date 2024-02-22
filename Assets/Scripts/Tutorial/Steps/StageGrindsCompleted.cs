using UnityEngine;

namespace Tutorial.Steps
{
	public class StageGrindsCompleted : Element
	{
		[Header("Requirements")]
		public MetaStage stage;

		public MetaStageDifficulty difficulty;

		public int grinds;

		protected virtual void Update()
		{
			if ((bool)stage)
			{
				base.complete = (stage.GetGrinds(difficulty) >= grinds);
			}
		}
	}
}
