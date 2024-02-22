using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class HeroMesh : MonoBehaviour
{
	[Serializable]
	public class HeroMeshDefaultItems
	{
		public MetaItemType slot;

		public HeadVariants headVariant;

		public GameObject vis3DPrefab;
	}

	[Serializable]
	public class HeroMeshHeadVisVariant
	{
		public HeadVariants variant;

		public GameObject vis3DPrefab;
	}

	[Serializable]
	public class SelectionParams
	{
		public float duration;

		public AnimationCurve select;

		public AnimationCurve fadeIn;

		public AnimationCurve fadeOut;

		public IEnumerator Select(Renderer target)
		{
			return Invoke(target, select, "_selectionSelected");
		}

		public IEnumerator FadeOut(Renderer target)
		{
			return Invoke(target, fadeOut, "_selectionFaded");
		}

		public IEnumerator FadeIn(Renderer target)
		{
			return Invoke(target, fadeIn, "_selectionFaded");
		}

		private IEnumerator Invoke(Renderer target, AnimationCurve curve, string param)
		{
			float time = 0f;
			yield return null;
			while (time < duration)
			{
				time += Time.deltaTime;
				float t = time / duration;
				float v = curve.Evaluate(t);
				MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
				target.GetPropertyBlock(propertyBlock);
				propertyBlock.SetFloat(param, v);
				target.SetPropertyBlock(propertyBlock);
				yield return null;
			}
		}
	}

	public HeroMeshDefaultItems[] m_defaultItems;

	public HeroMeshHeadVisVariant[] m_heads = new HeroMeshHeadVisVariant[4];

	public SkinnedMeshRenderer m_EditorVisualization;

	public Transform m_RootBone;

	public Transform m_PartsParent;

	private Hashtable m_BonesHash = new Hashtable();

	private List<SkinnedMeshRenderer> m_CreatedRenderers = new List<SkinnedMeshRenderer>();

	private List<FXPart> m_CreatedFXes = new List<FXPart>(10);

	private readonly Dictionary<SkinnedMeshRenderer, MetaItemType> m_CreatedRenderersItems = new Dictionary<SkinnedMeshRenderer, MetaItemType>();

	public GameObject commonHeadPrefab;

	public SelectionParams selection;

	private bool m_started;

	private MetaItemType? selectedItem;

	private void Start()
	{
		Singleton<Meta>.Instance.AddTypeListener(typeof(MetaItemSlot), OnSlotChange);
		if (!m_started)
		{
			m_started = true;
			CreateBonesHashtable();
			RebuildVisualizationForHimself();
		}
	}

	private void OnEnable()
	{
		Animator component = GetComponent<Animator>();
		if (!component.isInitialized)
		{
			component.Rebind();
		}
		List<MetaItem> source = (from x in Singleton<Meta>.Instance.FindObjects<MetaItemSlot>().ToList()
			where x.item != null
			select x.item).ToList();
		MetaItem metaItem = (from x in source
			where x.itemType == MetaItemType.MainHand
			select x).FirstOrDefault();
		MetaItem metaItem2 = (from x in source
			where x.itemType == MetaItemType.OffHand
			select x).FirstOrDefault();
		MetaItemClass value = metaItem ? metaItem.itemClass : MetaItemClass.Default;
		MetaItemClass value2 = metaItem2 ? metaItem2.itemClass : MetaItemClass.Default;
		component.SetInteger("mainHandType", (int)value);
		component.SetTrigger("mainHandChanged");
		component.SetInteger("offHandType", (int)value2);
		component.SetTrigger("offHandChanged");
	}

	private void OnDestroy()
	{
		if (Singleton<Meta>.HasInstance)
		{
			Singleton<Meta>.Instance.RemTypeListener(typeof(MetaItemSlot), OnSlotChange);
		}
	}

	public void SlotSelected(MetaItemType slot)
	{
		UpdateSelected(slot);
	}

	public void SlotDeselected()
	{
		UpdateSelected(null);
	}

	private void UpdateSelected(MetaItemType? item)
	{
		MetaItemType? metaItemType = selectedItem;
		bool flag = metaItemType.GetValueOrDefault() != item.GetValueOrDefault() || (metaItemType.HasValue ^ item.HasValue);
		foreach (KeyValuePair<SkinnedMeshRenderer, MetaItemType> createdRenderersItem in m_CreatedRenderersItems)
		{
			SkinnedMeshRenderer key = createdRenderersItem.Key;
			MetaItemType value = createdRenderersItem.Value;
			bool flag2 = selectedItem.HasValue && selectedItem.Value != value;
			bool flag3 = item.HasValue && item.Value == value;
			bool flag4 = item.HasValue && item.Value != value;
			if (flag)
			{
				if (flag3)
				{
					StartCoroutine(selection.Select(key));
				}
				if (flag4 && !flag2)
				{
					StartCoroutine(selection.FadeOut(key));
				}
				else if (!flag4 && flag2)
				{
					StartCoroutine(selection.FadeIn(key));
				}
			}
			else
			{
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				key.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetFloat("_selectionSelected", 0f);
				materialPropertyBlock.SetFloat("_selectionFaded", (!flag4) ? 0f : 1f);
				key.SetPropertyBlock(materialPropertyBlock);
			}
		}
		selectedItem = item;
	}

	private SkinnedMeshRenderer[] AddCreatedRenderersItems(SkinnedMeshRenderer[] renderers, MetaItemType itemType)
	{
		if (renderers != null)
		{
			for (int i = 0; i < renderers.Length; i++)
			{
				m_CreatedRenderersItems.Add(renderers[i], itemType);
			}
		}
		return renderers;
	}

	private void OnSlotChange(MetaObject item, string propoerty)
	{
		RebuildVisualizationForHimself();
	}

	public void RebuildVisualizationForHimself()
	{
		if (!m_started)
		{
			Start();
		}
		RebuildVisualizationWithItemForHimself(null);
		UpdateSelected(selectedItem);
	}

	public void RebuildVisualization(List<MetaItem> wornItems)
	{
		if (!m_started)
		{
			Start();
		}
		RebuildVisualizationWithItem(null, wornItems);
		UpdateSelected(selectedItem);
	}

	public Renderer[] RebuildVisualizationWithItemForHimself(MetaItem itemToSwap)
	{
		List<MetaItem> wornItems = (from x in Singleton<Meta>.Instance.FindObjects<MetaItemSlot>().ToList()
			where x.item != null
			select x.item).ToList();
		return RebuildVisualizationWithItem(itemToSwap, wornItems);
	}

	public Renderer[] RebuildVisualizationWithItem(MetaItem itemToSwap, List<MetaItem> wornItems)
	{
		if (!m_started)
		{
			Start();
		}
		for (int i = 0; i < m_CreatedRenderers.Count; i++)
		{
			UnityEngine.Object.DestroyObject(m_CreatedRenderers[i].gameObject);
		}
		m_CreatedRenderers.Clear();
		m_CreatedRenderersItems.Clear();
		for (int j = 0; j < m_CreatedFXes.Count; j++)
		{
			UnityEngine.Object.DestroyObject(m_CreatedFXes[j].gameObject);
		}
		m_CreatedFXes.Clear();
		if (itemToSwap != null)
		{
			bool flag = false;
			for (int k = 0; k < wornItems.Count; k++)
			{
				if (wornItems[k].itemType == itemToSwap.itemType)
				{
					wornItems[k] = itemToSwap;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				wornItems.Add(itemToSwap);
			}
		}
		List<HeroMeshDefaultItems> list = new List<HeroMeshDefaultItems>(m_defaultItems);
		Renderer[] result = null;
		HeadVariants headVariant = HeadVariants.Original;
		for (int l = 0; l < wornItems.Count; l++)
		{
			if (wornItems[l].vis3D == null)
			{
				continue;
			}
			if (wornItems[l].itemType == MetaItemType.Head)
			{
				headVariant = wornItems[l].headVariant;
			}
			if (wornItems[l] == itemToSwap)
			{
				result = AddCreatedRenderersItems(Create3DItem(wornItems[l].vis3D, m_CreatedRenderers, m_CreatedFXes), wornItems[l].itemType);
			}
			else
			{
				AddCreatedRenderersItems(Create3DItem(wornItems[l].vis3D, m_CreatedRenderers, m_CreatedFXes), wornItems[l].itemType);
			}
			for (int m = 0; m < list.Count; m++)
			{
				if (list[m].slot == wornItems[l].itemType)
				{
					list.RemoveAt(m);
					m--;
				}
			}
		}
		for (int n = 0; n < list.Count; n++)
		{
			if (list[n].slot == MetaItemType.Head)
			{
				headVariant = list[n].headVariant;
			}
			AddCreatedRenderersItems(Create3DItem(list[n].vis3DPrefab, m_CreatedRenderers, m_CreatedFXes), list[n].slot);
		}
		if (m_EditorVisualization != null)
		{
			UnityEngine.Object.DestroyObject(m_EditorVisualization.gameObject);
			m_EditorVisualization = null;
		}
		HeroMeshHeadVisVariant heroMeshHeadVisVariant = (from x in m_heads
			where x.variant == headVariant
			select x).FirstOrDefault();
		if (heroMeshHeadVisVariant == null && headVariant != HeadVariants.NoHead)
		{
			AddCreatedRenderersItems(Create3DItem(commonHeadPrefab, m_CreatedRenderers, m_CreatedFXes), MetaItemType.Head);
		}
		else if (heroMeshHeadVisVariant != null)
		{
			if (heroMeshHeadVisVariant.variant == HeadVariants.Original)
			{
				AddCreatedRenderersItems(Create3DItem(commonHeadPrefab, m_CreatedRenderers, m_CreatedFXes), MetaItemType.Head);
			}
			else if (heroMeshHeadVisVariant.variant != HeadVariants.NoHead)
			{
				AddCreatedRenderersItems(Create3DItem(heroMeshHeadVisVariant.vis3DPrefab, m_CreatedRenderers, m_CreatedFXes), MetaItemType.Head);
			}
		}
		Animator component = GetComponent<Animator>();
		MetaItem metaItem = (from x in wornItems
			where x.itemType == MetaItemType.MainHand
			select x).FirstOrDefault();
		MetaItem metaItem2 = (from x in wornItems
			where x.itemType == MetaItemType.OffHand
			select x).FirstOrDefault();
		MetaItemClass value = metaItem ? metaItem.itemClass : MetaItemClass.Default;
		MetaItemClass value2 = metaItem2 ? metaItem2.itemClass : MetaItemClass.Default;
		component.SetInteger("mainHandType", (int)value);
		component.SetTrigger("mainHandChanged");
		component.SetInteger("offHandType", (int)value2);
		component.SetTrigger("offHandChanged");
		return result;
	}

	private SkinnedMeshRenderer[] Create3DItem(GameObject itemPrefab, List<SkinnedMeshRenderer> created_renderers, List<FXPart> created_fxes)
	{
		if (itemPrefab == null)
		{
			return null;
		}
		Transform created_part_parent = UnityEngine.Object.Instantiate(itemPrefab.transform);
		SetLayerRecursively(created_part_parent.gameObject, base.gameObject.layer);
		SkinnedMeshRenderer[] componentsInChildren = created_part_parent.GetComponentsInChildren<SkinnedMeshRenderer>();
		FXPart[] componentsInChildren2 = created_part_parent.GetComponentsInChildren<FXPart>();
		if (componentsInChildren.Length > 0)
		{
			created_renderers.AddRange(componentsInChildren);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].transform.parent = m_PartsParent;
				componentsInChildren[i].transform.localPosition = Vector3.zero;
				Transform[] array = new Transform[componentsInChildren[i].bones.Length];
				for (int j = 0; j < componentsInChildren[i].bones.Length; j++)
				{
					string name = componentsInChildren[i].bones[j].name;
					array[j] = (Transform)m_BonesHash[name];
				}
				componentsInChildren[i].bones = array;
				componentsInChildren[i].sharedMesh.RecalculateBounds();
			}
		}
		for (int k = 0; k < componentsInChildren2.Length; k++)
		{
			GameObject[] array2 = (from x in componentsInChildren2[k].GetComponentsInChildren<ParticleSystem>()
				where x.main.simulationSpace == ParticleSystemSimulationSpace.World && x.emission.enabled
				select x.gameObject into x
				where x.activeSelf
				select x).ToArray();
			GameObject[] array3 = array2;
			foreach (GameObject gameObject in array3)
			{
				gameObject.SetActive(value: false);
			}
			componentsInChildren2[k].AttachToObject(m_PartsParent);
			created_fxes.Add(componentsInChildren2[k]);
			GameObject[] array4 = array2;
			foreach (GameObject gameObject2 in array4)
			{
				gameObject2.SetActive(value: true);
			}
		}
		if (componentsInChildren.Length == 0 || componentsInChildren.Count((SkinnedMeshRenderer x) => x == created_part_parent) == 0)
		{
			UnityEngine.Object.DestroyObject(created_part_parent.gameObject);
		}
		return componentsInChildren;
	}

	public static void SetLayerRecursively(GameObject go, int layerNumber)
	{
		Transform[] componentsInChildren = go.GetComponentsInChildren<Transform>(includeInactive: true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			transform.gameObject.layer = layerNumber;
		}
	}

	private void CreateBonesHashtable()
	{
		List<Transform> list = new List<Transform>(30);
		AddBoneChildrenToList(list, m_RootBone);
		for (int i = 0; i < list.Count; i++)
		{
			m_BonesHash.Add(list[i].name, list[i]);
		}
	}

	private void AddBoneChildrenToList(List<Transform> all_bones, Transform parent_bone)
	{
		for (int i = 0; i < parent_bone.childCount; i++)
		{
			Transform child = parent_bone.GetChild(i);
			all_bones.Add(child);
			AddBoneChildrenToList(all_bones, child);
		}
	}

	private string PrepareFileName(string filePath, string extension)
	{
		if (extension != null && filePath.IndexOf('.') < 0)
		{
			filePath = filePath + "." + extension;
		}
		return filePath.ToLower();
	}

	public T GetAsset<T>(string resourcePath) where T : UnityEngine.Object
	{
		resourcePath = PrepareFileName(resourcePath, null);
		return Resources.Load<T>(resourcePath);
	}
}
