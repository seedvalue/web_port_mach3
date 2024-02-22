using UnityEngine;
using Utils;

namespace Tutorial.Actions
{
	public class Fight : Element
	{
		[Header("Stage")]
		public MetaStage metaStage;

		public MetaStageDifficulty metaStageDifficulty;

		protected override void OnActivate()
		{
			base.OnActivate();
			if (!base.complete)
			{
				SingletonComponent<Meta, MetaFight>.Instance.Fight(metaStage, metaStageDifficulty, HandleResult, canExit: false);
			}
		}

		private void HandleResult(MetaFight.Result result)
		{
			base.complete = (result == MetaFight.Result.Win);
		}
	}
}
