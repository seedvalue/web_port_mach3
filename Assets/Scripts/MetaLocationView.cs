using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

public class MetaLocationView : MetaView, IPointerClickHandler, IEventSystemHandler
{
	[Header("Meta Location")]
	public Text displayNameText;

	public Color displayNameUnlockedColor = Color.white;

	public Color displayNameLockedColor = Color.white;

	public FloatingTextSpawner inactiveMessage;

	public bool showMarkes;

	public Sprite marker;

	[Header("Medals")]
	public Image easyLocked;

	public Image easy0;

	public Image easy1;

	public Image easy2;

	public Image easy3;

	public Image mediumLocked;

	public Image medium0;

	public Image medium1;

	public Image medium2;

	public Image medium3;

	public Image hardLocked;

	public Image hard0;

	public Image hard1;

	public Image hard2;

	public Image hard3;

	[Header("Unlock")]
	public CanvasGroup unlockAdd;

	public AudioSample unlockSFX;

	private Map3D map;

	protected override void Start()
	{
		base.Start();
		map = GetComponentInParent<Map3D>();
	}

	public new MetaLocation GetObject()
	{
		return base.GetObject() as MetaLocation;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		OnInteract();
	}

	protected override void OnInteract()
	{
		MetaLocation @object = GetObject();
		if ((bool)map)
		{
			map.ScrollToShow(@object);
		}
		SingletonComponent<Meta, MapComponent>.Instance.lastLocation = @object;
		if (@object.anyUnlocked)
		{
			LocationWindowContext locationWindowContext = new LocationWindowContext();
			locationWindowContext.location = GetObject();
			Singleton<WindowManager>.Instance.OpenWindow<LocationWindow>(locationWindowContext);
		}
		else
		{
			inactiveMessage.Spawn(base.gameObject);
		}
	}

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
		Helpers.SetActiveGameObject(unlockAdd, value: false);
	}

	protected override void OnChanged()
	{
		base.OnChanged();
		MetaLocation @object = GetObject();
		if (showMarkes)
		{
			if (@object.IsAnyRewardAvailable())
			{
				Helpers.SetTextWithQuads(displayNameText, @object.displayName + "<quad name=Markers/ExclamationMark prop=icon>");
			}
			else
			{
				Helpers.SetText(displayNameText, @object.displayName);
			}
		}
		else
		{
			Helpers.SetTextColor(displayNameText, (!@object.anyUnlocked) ? displayNameLockedColor : displayNameUnlockedColor);
		}
		base.interactable = @object.anyUnlocked;
		MetaLocationState state = @object.GetState(MetaStageDifficulty.Easy);
		Helpers.SetActiveGameObject(easyLocked, state == MetaLocationState.Locked);
		Helpers.SetActiveGameObject(easy0, state == MetaLocationState.Unlocked);
		Helpers.SetActiveGameObject(easy1, state == MetaLocationState.Bronze);
		Helpers.SetActiveGameObject(easy2, state == MetaLocationState.Silver);
		Helpers.SetActiveGameObject(easy3, state == MetaLocationState.Gold);
		MetaLocationState state2 = @object.GetState(MetaStageDifficulty.Medium);
		Helpers.SetActiveGameObject(mediumLocked, state2 == MetaLocationState.Locked);
		Helpers.SetActiveGameObject(medium0, state2 == MetaLocationState.Unlocked);
		Helpers.SetActiveGameObject(medium1, state2 == MetaLocationState.Bronze);
		Helpers.SetActiveGameObject(medium2, state2 == MetaLocationState.Silver);
		Helpers.SetActiveGameObject(medium3, state2 == MetaLocationState.Gold);
		MetaLocationState state3 = @object.GetState(MetaStageDifficulty.Hard);
		Helpers.SetActiveGameObject(hardLocked, state3 == MetaLocationState.Locked);
		Helpers.SetActiveGameObject(hard0, state3 == MetaLocationState.Unlocked);
		Helpers.SetActiveGameObject(hard1, state3 == MetaLocationState.Bronze);
		Helpers.SetActiveGameObject(hard2, state3 == MetaLocationState.Silver);
		Helpers.SetActiveGameObject(hard3, state3 == MetaLocationState.Gold);
	}

	public IEnumerator UnlockSequencePre()
	{
		yield return new WaitForEndOfFrame();
		Helpers.SetActiveGameObject(unlockAdd, value: true);
		unlockAdd.alpha = 0f;
		unlockAdd.transform.localScale = Vector3.zero;
		if ((bool)map)
		{
			yield return new WaitForSeconds(3.5f * map.ScrollToShow(GetObject()));
		}
		UITweenerEx.fadeIn(unlockAdd, 0.125f);
		UITweenerEx.scaleTo(unlockAdd, 1f, 0.125f);
		AudioManager.PlaySafe(unlockSFX);
		yield return new WaitForSeconds(0.125f);
	}

	public IEnumerator UnlockSequencePost()
	{
		UITweenerEx.scaleTo(unlockAdd, 3f, 1f);
		UITweenerEx.fadeOut(unlockAdd, 1f);
		yield return new WaitForSeconds(1f);
		unlockAdd.alpha = 0f;
		unlockAdd.transform.localScale = Vector3.zero;
		Helpers.SetActiveGameObject(unlockAdd, value: false);
	}
}
