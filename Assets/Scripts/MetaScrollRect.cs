using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect), typeof(ScrollRectTweener))]
public class MetaScrollRect : MonoBehaviour, IMetaScrollable
{
	public RectOffset offset;

	private ScrollRect scroll;

	private ScrollRectTweener tweener;

	public float RequiredScrollToShow(MetaObject metaObject)
	{
		RectTransform rectTransform = FindTarget(metaObject);
		if (!rectTransform)
		{
			return 0f;
		}
		if (scroll.horizontal)
		{
			Vector2 vector = tweener.CalculateDistanceToShow(rectTransform);
			return vector.x;
		}
		Vector2 vector2 = tweener.CalculateDistanceToShow(rectTransform);
		return vector2.y;
	}

	public RectTransform FindTarget(MetaObject metaObject)
	{
		return (from v in scroll.content.GetComponentsInChildren<MetaView>()
			where v.GetObject() == metaObject
			select v.GetComponent<RectTransform>() into t
			where t != null
			select t).FirstOrDefault();
	}

	protected void Start()
	{
		scroll = GetComponent<ScrollRect>();
		tweener = GetComponent<ScrollRectTweener>();
	}
}
