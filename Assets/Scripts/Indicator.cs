using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Indicator : MonoBehaviour
{
	[Serializable]
	public struct Spawner
	{
		public Indicator prefab;

		private Indicator instance;

		public bool active
		{
			get
			{
				if ((bool)instance)
				{
					return instance.active;
				}
				return false;
			}
			set
			{
				if ((bool)instance)
				{
					instance.active = value;
				}
			}
		}

		public void Spawn(GameObject parent, GameObject target, Camera fromCamera = null, Camera toCamera = null)
		{
			if (!instance && (bool)prefab)
			{
				instance = UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent.transform);
			}
			if ((bool)instance)
			{
				instance.followTransform = target.transform;
				instance.fromCamera = fromCamera;
				instance.toCamera = toCamera;
				instance.active = false;
			}
		}
	}

	[Header("Follow")]
	public Transform followTransform;

	[Header("Transform")]
	public bool clearZ = true;

	public Camera fromCamera;

	public Camera toCamera;

	private bool _active;

	private Animator animator;

	public bool active
	{
		get
		{
			return _active;
		}
		set
		{
			if (_active != value)
			{
				_active = value;
				ActivityChanged();
			}
		}
	}

	protected virtual void Start()
	{
		animator = GetComponent<Animator>();
		ActivityChanged();
	}

	protected virtual void LateUpdate()
	{
		if ((bool)followTransform)
		{
			Vector3 position = followTransform.position;
			IndicatorAnchor component = followTransform.GetComponent<IndicatorAnchor>();
			if ((bool)component)
			{
				position = component.position;
			}
			Camera camera = (!(fromCamera != null)) ? Camera.main : fromCamera;
			Camera camera2 = (!(toCamera != null)) ? Camera.main : toCamera;
			if ((bool)camera && (bool)camera2 && camera != camera2)
			{
				position = camera2.ScreenToWorldPoint(camera.WorldToScreenPoint(position));
			}
			if (clearZ)
			{
				position.z = 0f;
			}
			base.transform.position = position;
		}
	}

	private void ActivityChanged()
	{
		if ((bool)animator)
		{
			animator.SetBool("Visible", active);
		}
	}
}
