using UnityEngine;

[ExecuteInEditMode]
public class PaintableMesh : MonoBehaviour
{
	public enum Options
	{
		BRUSH,
		SETTINGS,
		__COUNT
	}

	public enum Brush
	{
		RED,
		GREEN,
		BLUE,
		__COUNT
	}

	public enum UVNumber
	{
		UV_0,
		UV_1
	}
}
