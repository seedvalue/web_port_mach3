using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Text with Quads", 11)]
public class TextWithQuads : Text
{
	private class Quad
	{
		public int begin;

		public int end;

		public string name;

		public string prop;

		public Rect rect;

		public Image image;
	}

	private const string tagName = "name=";

	private const string tagProp = "prop=";

	private const string tagOpenQuad = "<quad";

	private const float yImageOffset = -0.15f;

	private static FieldInfo m_DisableFontTextureRebuiltCallbackField;

	private List<Quad> m_quads = new List<Quad>();

	private List<Image> m_images = new List<Image>();

	private bool m_needRefreshImages;

	private bool m_needRefreshPositions;

	public override string text
	{
		get
		{
			return base.text;
		}
		set
		{
			base.text = value;
			ParseQuads();
		}
	}

	static TextWithQuads()
	{
		m_DisableFontTextureRebuiltCallbackField = typeof(Text).GetField("m_DisableFontTextureRebuiltCallback", BindingFlags.Instance | BindingFlags.NonPublic);
		if (m_DisableFontTextureRebuiltCallbackField == null)
		{
			throw new InvalidOperationException("Unable to find m_DisableFontTextureRebuiltCallback field in Text");
		}
	}

	protected void ParseQuads()
	{
		int count = m_quads.Count;
		m_quads.Clear();
		for (int num = m_Text.IndexOf("<quad"); num >= 0; num = m_Text.IndexOf("<quad", num + 1))
		{
			int num2 = m_Text.IndexOf(">", num) + 1;
			if (num2 <= 0)
			{
				break;
			}
			string text = m_Text.Substring(num + "<quad".Length + 1, num2 - (num + "<quad".Length + 2));
			string prop = text.Substring(text.IndexOf("prop=") + "prop=".Length);
			string name = text.Substring("name=".Length, text.IndexOf("prop=") - "name=".Length - 1);
			Quad quad = new Quad();
			quad.begin = num;
			quad.end = num2;
			quad.name = name;
			quad.prop = prop;
			m_quads.Add(quad);
			m_needRefreshImages = true;
		}
		if (count != m_quads.Count)
		{
			m_needRefreshImages = true;
		}
	}

	protected bool RefreshImages()
	{
		if (!m_needRefreshImages)
		{
			return true;
		}
		m_needRefreshImages = false;
		for (int i = m_images.Count; i < m_quads.Count; i++)
		{
			GameObject gameObject = new GameObject("image", typeof(Image));
			gameObject.transform.SetParent(base.transform, worldPositionStays: false);
			RectTransform rectTransform = gameObject.transform as RectTransform;
			rectTransform.pivot = new Vector2(0f, 1f);
			rectTransform.anchorMin = new Vector2(0f, 1f);
			rectTransform.anchorMax = new Vector2(0f, 1f);
			m_images.Add(gameObject.GetComponent<Image>());
		}
		for (int num = m_images.Count; num > m_quads.Count; num--)
		{
			UnityEngine.Object.DestroyImmediate(m_images[num - 1].gameObject);
			m_images.RemoveAt(num - 1);
		}
		for (int j = 0; j < m_quads.Count; j++)
		{
			Quad quad = m_quads[j];
			UnityEngine.Object @object = Resources.Load(quad.name);
			FieldInfo field = @object.GetType().GetField(quad.prop);
			object value = field.GetValue(@object);
			quad.image = m_images[j];
			quad.image.sprite = (value as Sprite);
		}
		return !m_needRefreshImages;
	}

	protected bool RefreshPositions()
	{
		if (!m_needRefreshPositions)
		{
			return true;
		}
		m_needRefreshPositions = false;
		for (int i = 0; i < m_quads.Count; i++)
		{
			Quad quad = m_quads[i];
			if (quad.image == null)
			{
				m_needRefreshPositions = true;
				continue;
			}
			Rect rect = quad.rect;
			RectTransform rectTransform = quad.image.rectTransform;
			rectTransform.localPosition = new Vector3(rect.xMin, rect.yMax, 0f);
			rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
			quad.image.enabled = (rect != Rect.zero);
		}
		return !m_needRefreshPositions;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		m_images = GetComponentsInChildren<Image>().ToList();
		ParseQuads();
	}

	private void Update()
	{
		RefreshImages();
		RefreshPositions();
	}

	protected override void OnPopulateMesh(VertexHelper toFill)
	{
		if (!Application.isPlaying)
		{
			ParseQuads();
		}
		toFill.Clear();
		List<UIVertex> list = GenerateVerticesAndPositionImages();
		if (list != null)
		{
			for (int i = 0; i + 3 < list.Count; i += 4)
			{
				toFill.AddVert(list[i]);
				toFill.AddVert(list[i + 1]);
				toFill.AddVert(list[i + 2]);
				toFill.AddVert(list[i + 3]);
				toFill.AddTriangle(i, i + 1, i + 2);
				toFill.AddTriangle(i + 2, i + 3, i);
			}
		}
	}

	private List<UIVertex> GenerateVerticesAndPositionImages()
	{
		if (base.font == null)
		{
			return null;
		}
		m_DisableFontTextureRebuiltCallbackField.SetValue(this, true);
		Vector2 size = base.rectTransform.rect.size;
		TextGenerationSettings generationSettings = GetGenerationSettings(size);
		base.cachedTextGenerator.Populate(m_Text, generationSettings);
		if (base.cachedTextGenerator.vertexCount != base.cachedTextGenerator.characterCount * 4)
		{
		}
		Rect rect = base.rectTransform.rect;
		Vector2 textAnchorPivot = Text.GetTextAnchorPivot(base.alignment);
		Vector2 zero = Vector2.zero;
		zero.x = ((textAnchorPivot.x != 1f) ? rect.xMin : rect.xMax);
		zero.y = ((textAnchorPivot.y != 0f) ? rect.yMax : rect.yMin);
		Vector2 vector = PixelAdjustPoint(zero) - zero;
		float num = 1f / base.pixelsPerUnit;
		List<UIVertex> list = base.cachedTextGenerator.verts.ToList();
		int num2 = 0;
		for (int i = 0; i < m_quads.Count; i++)
		{
			Quad quad = m_quads[i];
			int num3 = quad.begin * 4 - num2;
			int num4 = (quad.end - quad.begin) * 4;
			if (num3 >= list.Count || num3 + num4 > list.Count)
			{
				if (quad.rect != Rect.zero)
				{
					quad.rect = Rect.zero;
				}
				m_needRefreshPositions = true;
				break;
			}
			UIVertex uIVertex = list[num3];
			Vector3 a = uIVertex.position;
			UIVertex uIVertex2 = list[num3 + 2];
			Vector3 a2 = uIVertex2.position;
			a *= num;
			a.x += vector.x;
			a.y += vector.y;
			a2 *= num;
			a2.x += vector.x;
			a2.y += vector.y;
			float xmin = Mathf.Min(a.x, a2.x);
			float num5 = Mathf.Min(a.y, a2.y);
			float xmax = Mathf.Max(a.x, a2.x);
			float num6 = Mathf.Max(a.y, a2.y);
			float num7 = -0.15f * (float)base.fontSize;
			Rect rect2 = Rect.MinMaxRect(xmin, num5 + num7, xmax, num6 + num7);
			if (quad.rect != rect2)
			{
				quad.rect = rect2;
				m_needRefreshPositions = true;
			}
			list.RemoveRange(num3, num4);
			num2 += num4;
		}
		for (int j = 0; j < list.Count; j++)
		{
			UIVertex value = list[j];
			value.position *= num;
			value.position.x += vector.x;
			value.position.y += vector.y;
			list[j] = value;
		}
		m_DisableFontTextureRebuiltCallbackField.SetValue(this, false);
		return list;
	}

	public bool EditorUpdate()
	{
		RefreshImages();
		return RefreshPositions();
	}
}
