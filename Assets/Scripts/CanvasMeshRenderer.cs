using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class CanvasMeshRenderer : MaskableGraphic
{
	[SerializeField]
	private Vector3 rotation = Vector3.zero;

	private MeshFilter meshFilter;

	private Color meshColor;

	private Vector3 meshRotation;

	private Vector2 meshSizeDelta;

	private readonly List<UIVertex> vertices = new List<UIVertex>();

	private readonly List<int> indices = new List<int>();

	protected override void OnEnable()
	{
		base.OnEnable();
		Init();
		BuildMesh();
	}

	protected virtual void Update()
	{
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		RefreshMesh();
		vh.Clear();
		vh.AddUIVertexStream(vertices, indices);
	}

	private void Init()
	{
		meshFilter = GetComponent<MeshFilter>();
	}

	private void RefreshMesh()
	{
		if (vertices.Count <= 0 || !(meshColor == color) || !(meshRotation == rotation) || !(meshSizeDelta == base.rectTransform.sizeDelta))
		{
			BuildMesh();
		}
	}

	private void BuildMesh()
	{
		vertices.Clear();
		indices.Clear();
		Mesh sharedMesh = meshFilter.sharedMesh;
		Color[] colors = sharedMesh.colors;
		Vector3[] normals = sharedMesh.normals;
		Vector3[] array = sharedMesh.vertices;
		Vector2[] uv = sharedMesh.uv;
		Vector2[] uv2 = sharedMesh.uv2;
		int[] triangles = sharedMesh.triangles;
		Quaternion quaternion = Quaternion.Euler(rotation);
		Vector3 vector = quaternion * sharedMesh.bounds.size;
		Vector2 sizeDelta = base.rectTransform.sizeDelta;
		float num = Mathf.Abs(sizeDelta.x / vector.x);
		Vector2 sizeDelta2 = base.rectTransform.sizeDelta;
		float num2 = Mathf.Abs(sizeDelta2.y / vector.y);
		for (int i = 0; i < array.Length; i++)
		{
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.position = quaternion * array[i];
			simpleVert.position.x *= num;
			simpleVert.position.y *= num2;
			if (colors.Length > 0)
			{
				simpleVert.color = color * colors[i];
			}
			else
			{
				simpleVert.color = color;
			}
			if (normals.Length > 0)
			{
				simpleVert.normal = quaternion * normals[i];
			}
			if (uv.Length > 0)
			{
				simpleVert.uv0 = uv[i];
			}
			if (uv2.Length > 0)
			{
				simpleVert.uv1 = uv2[i];
			}
			vertices.Add(simpleVert);
		}
		indices.AddRange(triangles);
		meshColor = color;
		meshRotation = rotation;
		meshSizeDelta = base.rectTransform.sizeDelta;
	}
}
