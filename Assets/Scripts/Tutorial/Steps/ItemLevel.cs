using UnityEngine;

namespace Tutorial.Steps
{
	public class ItemLevel : Element
	{
		[Header("Requirements")]
		public MetaItem item;

		public int level;

		protected virtual void Update()
		{
			if ((bool)item)
			{
				base.complete = (item.level >= level);
			}
		}
	}
}
