using UnityEngine;

namespace Tutorial.Steps
{
	public class ItemJustFound : Element
	{
		[Header("Requirements")]
		public MetaItem item;

		public bool justFound = true;

		protected virtual void Update()
		{
			if ((bool)item)
			{
				base.complete = (item.justFound == justFound);
			}
		}
	}
}
