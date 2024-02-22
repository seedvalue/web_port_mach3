using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public abstract class Window : MonoBehaviour, ISceneUnloadingElement, IInputCancelHandler
{
	public bool allowFade = true;

	public List<Button> closeButtons;

	private bool closing;

	public object context
	{
		get;
		set;
	}

	public Action<Window, object> onClosed
	{
		get;
		set;
	}

	public Action<Window> onOpened
	{
		get;
		set;
	}

	public virtual void CloseWindow()
	{
		CloseWindow(null, instant: false);
	}

	public virtual void CloseWindow(object returnValue)
	{
		CloseWindow(returnValue, instant: false);
	}

	public virtual void CloseWindowInstant()
	{
		CloseWindow(null, instant: true);
	}

	public virtual void CloseWindowInstant(object returnValue)
	{
		CloseWindow(returnValue, instant: true);
	}

	protected virtual void CloseWindow(object returnValue, bool instant)
	{
		closing = true;
		OnWindowClosing();
		Action action = delegate
		{
			Singleton<WindowManager>.Instance.CloseWindow(this, returnValue);
			closing = false;
		};
		UITweener uITweener = FindTweener();
		if ((bool)uITweener)
		{
			uITweener.InvokeHideAnim(instant, action);
		}
		else
		{
			action();
		}
	}

	protected virtual void Start()
	{
		for (int i = 0; i < closeButtons.Count; i++)
		{
			Helpers.AddListenerOnClick(closeButtons[i], CloseWindow);
		}
	}

	public virtual void OnWindowClosing()
	{
	}

	public virtual void OnWindowClosed()
	{
		InputManager.RemCancelHandler(this);
	}

	public virtual void OnWindowOpening()
	{
		InputManager.AddCancelHandler(this);
		Action action = delegate
		{
			OnWindowOpened();
		};
		UITweener uITweener = FindTweener();
		if ((bool)uITweener)
		{
			uITweener.InvokeShowAnim(instant: false, action);
		}
		else
		{
			action();
		}
	}

	public virtual void OnWindowOpened()
	{
	}

	public bool IsUnloading()
	{
		return closing;
	}

	public bool OnCancel()
	{
		CloseWindow();
		return true;
	}

	protected UITweener FindTweener()
	{
		UIWindowTweener[] componentsInChildren = GetComponentsInChildren<UIWindowTweener>();
		if (componentsInChildren.Length > 1)
		{
			UnityEngine.Debug.LogError("Window contains more than one tweener");
		}
		return componentsInChildren.FirstOrDefault();
	}
}
