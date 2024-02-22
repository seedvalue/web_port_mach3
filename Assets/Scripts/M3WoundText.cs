using UnityEngine;
using UnityEngine.UI;

public class M3WoundText : MonoBehaviour
{
	public float duration = 1f;

	public float startFadeAt = 0.5f;

	public float yDistanceToGo = 0.5f;

	public float xDistanceToGo;

	private Text label;

	private M3InterpolatedFloat time;

	private Vector3 basePos;

	public void Init(Transform hpBar, int wound, M3Orb damageType)
	{
		Vector3 position = hpBar.position;
		Vector3 localScale = hpBar.localScale;
		Init(position, wound, damageType, localScale.y);
	}

	public void Init(Vector3 pos, int wound, M3Orb damageType, float yScale = 1f)
	{
		string text = (-wound).ToString();
		Color color = (damageType == M3Orb.None) ? M3Board.orbColors[2] : M3Board.orbColors[(int)damageType];
		Init(pos, text, color, yScale);
	}

	public void Init(Vector3 pos, string text, Color color, float yScale)
	{
		label = GetComponent<Text>();
		time = new M3InterpolatedFloat(this, M3InterpolationMode.Linear);
		time.SlideUp(duration);
		label.text = text;
		label.color = color;
		base.transform.position = pos + base.transform.position * yScale;
		basePos = base.transform.position;
	}

	private void Update()
	{
		if ((bool)label && time.Value > startFadeAt)
		{
			Color color = label.color;
			color.a = 1f - (time.Value - startFadeAt) / (1f - startFadeAt);
			label.color = color;
		}
		base.transform.position = basePos + new Vector3(time.Value * xDistanceToGo, time.Value * yDistanceToGo, 0f);
		if (time.Value >= 1f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
