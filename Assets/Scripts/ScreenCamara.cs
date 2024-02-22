using UnityEngine;
using UnityEngine.UI;

public class ScreenCamara : MonoBehaviour
{
	[Range(-1f, 1f)]
	public float offset;

	[Range(-1f, 1f)]
	public float offsetV;

	public float oversize = 1f;

	public float cameraOffsetFactor = 2f;

	[Header("Vertical Layout")]
	public ScrollRect scrollRect;

	public VerticalLayoutGroup layout;

	private Camera cam;

	private float screenHeight;

	private float layoutStartPos;

	private void Start()
	{
		cam = GetComponent<Camera>();
		if ((bool)layout)
		{
			Vector3 localPosition = layout.transform.localPosition;
			layoutStartPos = localPosition.y;
		}
		screenHeight = 1024f;
		RectTransform component = GetComponent<RectTransform>();
		if ((bool)component)
		{
			Vector2 sizeDelta = component.sizeDelta;
			screenHeight = sizeDelta.y;
		}
	}

	private void OnPreRender()
	{
		if ((bool)scrollRect && (bool)layout)
		{
			Vector3 localPosition = layout.transform.localPosition;
			float value = localPosition.y - layoutStartPos;
			value = Mathf.Clamp(value, 0f - screenHeight, screenHeight);
			offsetV = (0f - value) / screenHeight;
		}
		offsetV = Mathf.Clamp(offsetV, -0.99f, 0.99f);
		Vector2 vector = new Vector2(offset, offsetV);
		Vector3 zero = Vector3.zero;
		float num = 1f - 1f / oversize;
		if (vector.x > 0f)
		{
			vector.x = Mathf.Max(0f, vector.x - num);
		}
		else if (vector.x < 0f)
		{
			vector.x = Mathf.Min(0f, vector.x + num);
		}
		vector.x /= 1f / oversize;
		zero.x = (offset - vector.x) * cameraOffsetFactor;
		Rect rect = new Rect(0f, 0f, 1f, 1f);
		Vector2 one = Vector2.one;
		if (vector.y >= 0f)
		{
			rect.height = 1f - vector.y;
			one.y = 1f;
		}
		else
		{
			rect.y = 0f - vector.y;
			rect.height = 1f + vector.y;
			one.y = -1f;
		}
		if (vector.x >= 0f)
		{
			rect.width = 1f - vector.x;
			one.x = 1f;
		}
		else
		{
			rect.x = 0f - vector.x;
			rect.width = 1f + vector.x;
			one.x = -1f;
		}
		if (rect.width == 0f)
		{
			rect.width = 0.001f;
		}
		if (rect.height == 0f)
		{
			rect.height = 0.001f;
		}
		cam.rect = new Rect(0f, 0f, 1f, 1f);
		cam.ResetProjectionMatrix();
		Matrix4x4 projectionMatrix = cam.projectionMatrix;
		cam.rect = rect;
		Matrix4x4 lhs = Matrix4x4.TRS(new Vector3(one.x * (-1f / rect.width + 1f), one.y * (-1f / rect.height + 1f), 0f) - zero, Quaternion.identity, new Vector3(1f / rect.width, 1f / rect.height, 1f));
		cam.projectionMatrix = lhs * projectionMatrix;
	}
}
