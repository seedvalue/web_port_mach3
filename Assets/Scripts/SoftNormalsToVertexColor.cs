using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SoftNormalsToVertexColor : MonoBehaviour
{
	public enum Method
	{
		Simple,
		AngularDeviation
	}

	public Method method = Method.AngularDeviation;

	public bool generateOnAwake;

	public bool generateNow;

	private void OnDrawGizmos()
	{
		if (generateNow)
		{
			generateNow = false;
			TryGenerate();
		}
	}

	private void Awake()
	{
		if (generateOnAwake)
		{
			TryGenerate();
		}
	}

	private void TryGenerate()
	{
		MeshFilter component = GetComponent<MeshFilter>();
		if (component == null)
		{
			UnityEngine.Debug.LogError("MeshFilter missing on the vertex color generator", base.gameObject);
			return;
		}
		if (component.sharedMesh == null)
		{
			UnityEngine.Debug.LogError("Assign a mesh to the MeshFilter before generating vertex colors", base.gameObject);
			return;
		}
		Generate(component.sharedMesh);
		UnityEngine.Debug.Log("Vertex colors generated", base.gameObject);
	}

	private void Generate(Mesh m)
	{
		Vector3[] normals = m.normals;
		Vector3[] vertices = m.vertices;
		Color[] array = new Color[normals.Length];
		List<List<int>> list = new List<List<int>>();
		for (int i = 0; i < vertices.Length; i++)
		{
			bool flag = false;
			foreach (List<int> item in list)
			{
				if (vertices[item[0]] == vertices[i])
				{
					item.Add(i);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				List<int> list2 = new List<int>();
				list2.Add(i);
				list.Add(list2);
			}
		}
		foreach (List<int> item2 in list)
		{
			Vector3 vector = Vector3.zero;
			foreach (int item3 in item2)
			{
				vector += normals[item3];
			}
			vector.Normalize();
			if (method == Method.AngularDeviation)
			{
				float num = 0f;
				foreach (int item4 in item2)
				{
					num += Vector3.Dot(normals[item4], vector);
				}
				num /= (float)item2.Count;
				float num2 = Mathf.Acos(num) * 57.29578f;
				float num3 = 180f - num2 - 90f;
				float d = 0.5f / Mathf.Sin(num3 * ((float)Math.PI / 180f));
				vector *= d;
			}
			foreach (int item5 in item2)
			{
				array[item5] = new Color(vector.x, vector.y, vector.z);
			}
		}
		m.colors = array;
	}
}
