using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroPresentation : MonoBehaviour
{
	[Serializable]
	public class SlotPresentation
	{
		public MetaItemType slot;

		public float rotation;
	}

	public string triggerName;

	public float rotationSpeed = 180f;

	public List<SlotPresentation> presentations = new List<SlotPresentation>(6);

	public void SlotSelected(MetaItemType slot)
	{
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		float y = eulerAngles.y;
		float num = 0f;
		SlotPresentation slotPresentation = (from x in presentations
			where x.slot == slot
			select x).FirstOrDefault();
		if (slotPresentation != null)
		{
			num = slotPresentation.rotation;
		}
		if (Mathf.Abs(y - num) > 180f)
		{
			num = 360f + num;
		}
		StopAllCoroutines();
		StartCoroutine(rotateBySpeed(base.gameObject, y, num, rotationSpeed, delegate
		{
			HeroMesh component2 = GetComponent<HeroMesh>();
			if ((bool)component2)
			{
				component2.SlotSelected(slot);
			}
		}));
		Animator component = GetComponent<Animator>();
		if ((bool)component)
		{
			component.SetTrigger("slotChanged");
			component.SetInteger("slotType", (int)slot);
			if (slot == MetaItemType.Back)
			{
				component.SetLayerWeight(1, 1f);
				component.SetLayerWeight(2, 1f);
			}
		}
	}

	public void SlotDeselected()
	{
		ShowByAngle(0f, delegate
		{
			HeroMesh component2 = GetComponent<HeroMesh>();
			if ((bool)component2)
			{
				component2.SlotDeselected();
			}
		});
		Animator component = GetComponent<Animator>();
		if ((bool)component)
		{
			component.SetTrigger("slotChanged");
			component.SetInteger("slotType", -1);
		}
	}

	public void ShowByAngle(float endAngle, Action onEnd = null)
	{
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		float y = eulerAngles.y;
		if (Mathf.Abs(y - endAngle) > 180f)
		{
			endAngle = 360f + endAngle;
		}
		StopAllCoroutines();
		StartCoroutine(rotateBySpeed(base.gameObject, y, endAngle, rotationSpeed, onEnd));
		Animator component = GetComponent<Animator>();
		if ((bool)component)
		{
			component.SetTrigger("default");
		}
	}

	public IEnumerator rotateBySpeed(GameObject obj, float startAngle, float endAngle, float speed, Action onEnd = null)
	{
		float angle = Mathf.Abs(endAngle - startAngle);
		float duration = angle / speed;
		yield return rotate(obj, startAngle, endAngle, duration);
		onEnd?.Invoke();
	}

	public IEnumerator rotate(GameObject obj, float startAngle, float endAngle, float duration)
	{
		float startTime = Time.time;
		Quaternion q;
		while (Time.time < startTime + duration)
		{
			float angle = Mathf.SmoothStep(startAngle, endAngle, (Time.time - startTime) / duration);
			q = Quaternion.identity;
			q.eulerAngles = new Vector3(0f, angle, 0f);
			obj.transform.localRotation = q;
			yield return null;
		}
		q = Quaternion.identity;
		q.eulerAngles = new Vector3(0f, endAngle, 0f);
		obj.transform.localRotation = q;
	}
}
