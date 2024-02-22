using System.Collections.Generic;
using UnityEngine;
using Utils;

[SingletonInitializeOnLoad]
public class InputManager : Singleton<InputManager>
{
	private static readonly List<IInputCancelHandler> cancelHandlers = new List<IInputCancelHandler>();

	public static void AddCancelHandler(IInputCancelHandler handler)
	{
		cancelHandlers.Add(handler);
	}

	public static void RemCancelHandler(IInputCancelHandler handler)
	{
		cancelHandlers.Remove(handler);
	}

	private void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			InvokeCancelHandler();
		}
	}

	private static bool InvokeCancelHandler()
	{
		if (Singleton<SceneLoader>.Instance.loading)
		{
			return false;
		}
		for (int num = cancelHandlers.Count; num > 0; num--)
		{
			IInputCancelHandler inputCancelHandler = cancelHandlers[num - 1];
			if (inputCancelHandler.OnCancel())
			{
				return true;
			}
		}
		if (Application.platform != RuntimePlatform.IPhonePlayer)
		{
			Singleton<WindowManager>.Instance.OpenWindow<AreYouSureWindow>("Are you sure you want to quit?", delegate(Window w, object c)
			{
				if (c != null)
				{
					Application.Quit();
				}
			});
		}
		return true;
	}
}
