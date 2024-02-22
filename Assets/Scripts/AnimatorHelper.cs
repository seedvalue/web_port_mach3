using UnityEngine;

public class AnimatorHelper : MonoBehaviour
{
	public void Activate(GameObject target)
	{
		if ((bool)target)
		{
			target.SetActive(value: true);
		}
	}

	public void Deactivate(GameObject target)
	{
		if ((bool)target)
		{
			target.SetActive(value: false);
		}
	}

	public void Spawn(GameObject target)
	{
		if ((bool)target)
		{
			Transform transform = target.transform;
			GameObject gameObject = UnityEngine.Object.Instantiate(target, base.transform);
			gameObject.transform.localPosition = transform.localPosition;
			gameObject.transform.localScale = transform.localScale;
			gameObject.transform.localRotation = transform.localRotation;
		}
	}
}
