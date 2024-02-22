using UnityEngine;

namespace Tutorial.Steps
{
	public class ItemInSlot : Element
	{
		[Header("Requirements")]
		public MetaItem item;

		protected virtual void Update()
		{
			if ((bool)item)
			{
				base.complete = (item.slot != null);
			}
		}
	}
}
