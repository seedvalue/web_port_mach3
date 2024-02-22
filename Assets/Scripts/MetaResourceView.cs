using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class MetaResourceView : MetaView
{
	[Header("Meta Resource")]
	public Text nameText;

	public Text countText;

	public Image iconImage;

	public Image cardImage;

	public new MetaResource GetObject()
	{
		return base.GetObject() as MetaResource;
	}

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
		MetaResource @object = GetObject();
		Helpers.SetText(nameText, @object.displayName);
		Helpers.SetImage(iconImage, @object.icon);
		Helpers.SetImage(cardImage, @object.cardIcon);
	}

	protected override void OnChanged()
	{
		base.OnChanged();
		MetaResource @object = GetObject();
		int propertyOrDefault = GetPropertyOrDefault<int>("count");
		if (propertyOrDefault != @object.count)
		{
			Helpers.SetText(countText, "+" + propertyOrDefault.ToString());
		}
		else
		{
			Helpers.SetText(countText, @object.count.ToString());
		}
	}

	public IEnumerator DropSequencePre(int diff)
	{
		MetaResource @object = GetObject();
		int start = @object.count;
		int end = start + diff;
		float a = 0.25f;
		float b = 0.75f;
		float duration = Mathf.Lerp(a, b, Mathf.Clamp01(Mathf.Sqrt((float)diff * 1f - 1f) / 10f));
		return CoroutineHelper.AnimateInTime(duration, delegate(float t)
		{
			Helpers.SetText(value: ((int)Mathf.Lerp(start, end, t)).ToString(), target: countText);
		});
	}
}
