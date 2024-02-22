using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utils;

[RequireComponent(typeof(MetaItemView))]
public class MetaItemViewSelectable : MetaView, IPointerClickHandler, IPointerDownHandler, IEventSystemHandler
{
	public GameObject selectedNode;

	public GameObject selectedBkg;

	public Button useButton;

	public Button swapButton;

	public Button viewButton;

	public Button upgradeButton;

	public Text upgradeText;

	[Header("On select")]
	public bool disableGrid = true;

	public bool setOnTop = true;

	public bool setParentOnTop;

	[Header("Select/Unselect Anim")]
	public bool useAnim = true;

	public Vector3 offset = new Vector3(0f, 15f, 0f);

	public float animTime = 0.25f;

	public float scaleBump = 0.1f;

	[HideInInspector]
	public UnityEvent onSelect;

	[HideInInspector]
	public UnityEvent onDeselect;

	private Vector3 startPos;

	private Canvas canvas;

	private static MetaItemViewSelectable selectedItemView;

	public bool selected => this == selectedItemView;

	protected override void Start()
	{
		base.Start();
		canvas = GetComponentInChildren<Canvas>();
	}

	public new MetaItem GetObject()
	{
		return base.GetObject() as MetaItem;
	}

	protected override void OnObjectChanged()
	{
		base.OnObjectChanged();
	}

	private void DisableParentLayoutGroup(Transform obj)
	{
		if ((bool)obj.parent)
		{
			LayoutGroup component = obj.parent.GetComponent<LayoutGroup>();
			if ((bool)component)
			{
				component.enabled = false;
			}
		}
	}

	public void Select()
	{
		startPos = base.transform.localPosition;
		if (setOnTop)
		{
			DisableParentLayoutGroup(base.transform);
			base.transform.SetAsLastSibling();
		}
		if (setParentOnTop && (bool)base.transform.parent)
		{
			DisableParentLayoutGroup(base.transform.parent);
			base.transform.parent.SetAsLastSibling();
		}
		Helpers.SetActive(selectedNode, value: true);
		Helpers.SetActive(selectedBkg, value: true);
		selectedItemView = this;
		Helpers.SetActiveGameObject(GetComponent<MetaItemView>().itemLevelBar, value: false);
		StopAllCoroutines();
		StartCoroutine(selectUnselectAnim(show: true));
		onSelect.Invoke();
		GetObject().selected = true;
		if ((bool)base.transform.parent && (bool)canvas)
		{
			Canvas componentInParent = base.transform.parent.GetComponentInParent<Canvas>();
			canvas.overrideSorting = true;
			canvas.sortingOrder = componentInParent.sortingOrder + 1;
		}
		AudioManager.Play(AudioMechanics.ClickItem);
	}

	protected override void OnChanged()
	{
		base.OnChanged();
		MetaItem @object = GetObject();
		if ((bool)useButton)
		{
			Helpers.SetActive(useButton.gameObject, !@object.IsWorn());
		}
		if ((bool)swapButton)
		{
			Helpers.SetActive(swapButton.gameObject, @object.IsWorn());
		}
		if (@object.count >= @object.GetCardsNumRequiredToUpdate() && @object.GetMaxItemLevel() > @object.level)
		{
			if ((bool)upgradeButton)
			{
				Helpers.SetActive(upgradeButton.gameObject, value: true);
				Helpers.SetTextWithQuads(upgradeText, MetaResource.coins.quadText + " " + @object.GetCoinsRequiredToUpdate());
			}
			if ((bool)viewButton)
			{
				Helpers.SetActive(viewButton.gameObject, value: false);
			}
		}
		else
		{
			if ((bool)upgradeButton)
			{
				Helpers.SetActive(upgradeButton.gameObject, value: false);
			}
			if ((bool)viewButton)
			{
				Helpers.SetActive(viewButton.gameObject, value: true);
			}
		}
	}

	public void Deselect()
	{
		selectedItemView = null;
		base.transform.localScale = Vector3.one;
		Helpers.SetActive(selectedNode, value: false);
		Helpers.SetActive(selectedBkg, value: false);
		Helpers.SetActiveGameObject(GetComponent<MetaItemView>().itemLevelBar, value: true);
		StopAllCoroutines();
		StartCoroutine(selectUnselectAnim(show: false));
		onDeselect.Invoke();
		GetObject().selected = false;
		if ((bool)canvas)
		{
			canvas.overrideSorting = false;
		}
	}

	public IEnumerator selectUnselectAnim(bool show)
	{
		float startTime = Time.time;
		MetaItemView itemView = GetComponent<MetaItemView>();
		if (show)
		{
			while (Time.time < startTime + animTime)
			{
				if ((bool)itemView.bkgImage)
				{
					float value = (Time.time - startTime) / (animTime * 0.25f);
					value = Mathf.Clamp(value, 0f, 1f);
					Vector2 zero = Vector2.zero;
					Vector2 v = (!show) ? ((Vector2)Vector3.Lerp(startPos + offset, startPos, value)) : ((Vector2)Vector3.Lerp(startPos, startPos + offset, value));
					base.transform.localPosition = v;
					value = Helpers.simple_bounce3((Time.time - startTime) / animTime);
					Vector3 b = Vector3.one * value * scaleBump;
					b.z = 0f;
					itemView.bkgImage.transform.localScale = Vector3.one + b;
				}
				if ((bool)selectedBkg)
				{
					float d = Helpers.simple_bounce3((Time.time - startTime) / animTime);
					Vector3 b2 = Vector3.one * d * scaleBump;
					b2.z = 0f;
					selectedBkg.transform.localScale = Vector3.one + b2;
				}
				yield return null;
			}
		}
		if (show)
		{
			Helpers.SetLocalPosition(base.transform, startPos + offset);
		}
		else
		{
			Helpers.SetLocalPosition(base.transform, startPos);
		}
		Helpers.SetLocalScale(selectedBkg, Vector3.one);
		Helpers.SetLocalScale(itemView.bkgImage, Vector3.one);
	}

	public static void TryDeselect()
	{
		if ((bool)selectedItemView)
		{
			selectedItemView.Deselect();
		}
	}

	public static MetaItem GetSelectedItem()
	{
		if ((bool)selectedItemView)
		{
			return selectedItemView.GetObject();
		}
		return null;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		MetaItem @object = GetObject();
		if (!@object.found && @object.available)
		{
			ItemWindowContext itemWindowContext = new ItemWindowContext();
			itemWindowContext.item = GetObject();
			Singleton<WindowManager>.Instance.OpenWindow<ItemWindow>(itemWindowContext);
			AudioManager.Play(AudioMechanics.Click);
		}
		if (selectedItemView == this)
		{
			Deselect();
			return;
		}
		if ((bool)selectedItemView)
		{
			selectedItemView.Deselect();
		}
		if (GetObject().found)
		{
			Select();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}
}
