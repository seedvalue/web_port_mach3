using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Utils;

[SingletonPrefab]
[SingletonInitializeOnLoad]
[RequireComponent(typeof(RectTransform))]
public class WindowManager : Singleton<WindowManager>, ISceneLoadingElement
{
	public int windowSortingOrder = 100;

	public int windowSortingOrderOffset = 10;

	public GameObject fadeObject;

	public List<string> windowsScenes;

	private readonly List<Window> allWindows = new List<Window>();

	private readonly List<Window> openWindows = new List<Window>();

	private RectTransform root;

	private Dictionary<Type, List<MethodInfo>> testMethods;

	private bool loading;

	public event Action<Window> WindowOpened;

	public event Action<Window> WindowClosed;

	public bool CloseWindow(Window window, object returnValue = null)
	{
		if (!openWindows.Contains(window))
		{
			UnityEngine.Debug.LogError("Can't close window '" + window.name + "', not found in open windows.", this);
			return false;
		}
		openWindows.Remove(window);
		window.gameObject.SetActive(value: false);
		if (fadeObject != null)
		{
			fadeObject.SetActive(openWindows.Count > 0 && openWindows[openWindows.Count - 1].allowFade);
		}
		RefreshSortingOrder();
		OnWindowClosed(window, returnValue);
		return true;
	}

	public T OpenWindow<T>(object context = null, Action<Window, object> onClosedCallback = null, Action<Window> onOpenedCallback = null) where T : Window
	{
		return OpenWindow(typeof(T), context, onClosedCallback, onOpenedCallback) as T;
	}

	public Window OpenWindow(Type windowType, object context = null, Action<Window, object> onClosedCallback = null, Action<Window> onOpenedCallback = null)
	{
		Window openWindow = GetOpenWindow(windowType);
		if (openWindow != null)
		{
			UnityEngine.Debug.LogWarning("Window of type '" + windowType + " already open.", this);
			return openWindow;
		}
		openWindow = allWindows.Find((Window win) => win.GetType() == windowType);
		if (openWindow == null)
		{
			UnityEngine.Debug.LogError("Window of type '" + windowType + " does not exists.", this);
			return null;
		}
		openWindow.context = context;
		openWindow.onOpened = onOpenedCallback;
		openWindow.onClosed = onClosedCallback;
		openWindows.Add(openWindow);
		if (fadeObject != null)
		{
			fadeObject.SetActive(openWindow.allowFade);
		}
		RefreshSortingOrder();
		openWindow.gameObject.SetActive(value: true);
		OnWindowOpened(openWindow);
		return openWindow;
	}

	public Window GetOpenWindow(Type windowType)
	{
		return openWindows.Find((Window win) => win.GetType() == windowType);
	}

	public T GetOpenWindow<T>() where T : Window
	{
		return GetOpenWindow(typeof(T)) as T;
	}

	public bool IsLoading()
	{
		return loading;
	}

	protected virtual void Awake()
	{
		root = GetComponent<RectTransform>();
	}

	protected virtual void Start()
	{
		if (fadeObject != null)
		{
			AnchorCanvas(fadeObject);
			fadeObject.SetActive(value: false);
		}
		StartCoroutine(DoLoadWindows());
	}

	protected virtual void OnGUI()
	{
		if (openWindows.Count == 0)
		{
			DrawTestMethodsGUI();
		}
	}

	private void AnchorCanvas(GameObject go)
	{
		RectTransform component = go.GetComponent<RectTransform>();
		if (!(component == null))
		{
			Canvas component2 = go.GetComponent<Canvas>();
			if (!(component2 == null))
			{
				component.SetParent(root, worldPositionStays: false);
				component2.overrideSorting = true;
				component.anchorMin = Vector2.zero;
				component.anchorMax = Vector2.one;
				component.sizeDelta = Vector2.zero;
				component.localScale = Vector3.one;
			}
		}
	}

	private void SetSortingOrderOnCanvas(GameObject go, int order)
	{
		Canvas component = go.GetComponent<Canvas>();
		if (!(component == null))
		{
			component.sortingOrder = windowSortingOrder + order * windowSortingOrderOffset;
		}
	}

	private void RefreshSortingOrder()
	{
		if (openWindows.Count != 0)
		{
			for (int i = 0; i < openWindows.Count - 1; i++)
			{
				SetSortingOrderOnCanvas(openWindows[i].gameObject, i);
			}
			if (fadeObject != null)
			{
				SetSortingOrderOnCanvas(fadeObject, openWindows.Count - 1);
			}
			SetSortingOrderOnCanvas(openWindows[openWindows.Count - 1].gameObject, openWindows.Count);
		}
	}

	private void LoadWindowsFromScene(Scene scene, bool destroyOther = false)
	{
		GameObject[] rootGameObjects = scene.GetRootGameObjects();
		foreach (GameObject gameObject in rootGameObjects)
		{
			Window component = gameObject.GetComponent<Window>();
			if (component == null)
			{
				if (destroyOther)
				{
					UnityEngine.Object.DestroyImmediate(gameObject);
				}
			}
			else
			{
				allWindows.Add(component);
				AnchorCanvas(gameObject);
				component.gameObject.SetActive(value: false);
			}
		}
	}

	private void DrawTestMethodsGUI()
	{
		GUIStyle gUIStyle = new GUIStyle(GUI.skin.button);
		gUIStyle.fontSize = 12;
		gUIStyle.normal.textColor = Color.white;
		if (testMethods == null)
		{
			return;
		}
		for (int i = 0; i < testMethods.Count; i++)
		{
			KeyValuePair<Type, List<MethodInfo>> keyValuePair = testMethods.ElementAt(i);
			Type key = keyValuePair.Key;
			List<MethodInfo> value = keyValuePair.Value;
			for (int j = 0; j < value.Count; j++)
			{
				string text = key.Name + "\n" + value[j].Name;
				Rect position = new Rect(5 + j * 125, 5 + i * 45, 120f, 40f);
				if (GUI.Button(position, text, gUIStyle))
				{
					value[j].Invoke(null, null);
				}
			}
		}
	}

	private void OnWindowClosed(Window window, object returnValue)
	{
		this.WindowClosed?.Invoke(window);
		window.onClosed?.Invoke(window, returnValue);
		window.OnWindowClosed();
	}

	private void OnWindowOpened(Window window)
	{
		this.WindowOpened?.Invoke(window);
		window.onOpened?.Invoke(window);
		window.OnWindowOpening();
	}

	private IEnumerator DoLoadWindows()
	{
		loading = true;
		Scene activeScene = SceneManager.GetActiveScene();
		bool testing = windowsScenes.Contains(activeScene.name);
		for (int i = 0; i < windowsScenes.Count; i++)
		{
			if (activeScene.name == windowsScenes[i])
			{
				LoadWindowsFromScene(activeScene);
			}
			else
			{
				yield return DoLoadWindowsScene(windowsScenes[i]);
			}
		}
		if (testing)
		{
			testMethods = new Dictionary<Type, List<MethodInfo>>();
			for (int j = 0; j < allWindows.Count; j++)
			{
				List<MethodInfo> list = new List<MethodInfo>();
				Type type = allWindows[j].GetType();
				MethodInfo[] methods = type.GetMethods();
				foreach (MethodInfo methodInfo in methods)
				{
					if (methodInfo.GetCustomAttributes(typeof(WindowTestMethod), inherit: false).Length != 0 && methodInfo.IsStatic)
					{
						list.Add(methodInfo);
					}
				}
				testMethods.Add(type, list);
			}
			if (EventSystem.current == null)
			{
				EventSystem eventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();
				if (eventSystem != null)
				{
					eventSystem.enabled = true;
				}
			}
		}
		loading = false;
	}

	private IEnumerator DoLoadWindowsScene(string sceneName)
	{
		yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		Scene scene = SceneManager.GetSceneByName(sceneName);
		if (!scene.IsValid())
		{
			UnityEngine.Debug.LogWarningFormat("Couldn't open windows scene '{0}' because it doesn't exist in build.", sceneName);
			yield break;
		}
		LoadWindowsFromScene(scene, destroyOther: true);
		yield return DoUnloadScene(scene.buildIndex);
	}

	private IEnumerator DoUnloadScene(int buildIndex)
	{
		yield return new WaitForSeconds(0.1f);
		yield return SceneManager.UnloadSceneAsync(buildIndex);
	}
}
