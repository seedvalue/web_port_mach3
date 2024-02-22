using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FloatingText : MonoBehaviour
{
	[Header("Appearance")]
	public float fadeInTime = 0.3f;

	public float keepTime = 1f;

	public float fadeOutTime = 0.2f;

	[Header("Movement")]
	public float moveTime = 0.5f;

	public AnimationCurve moveCurve;

	public Vector3 moveSpeed;

	private Text uiText;

	public string text
	{
		get
		{
			return uiText.text;
		}
		set
		{
			uiText.text = value;
		}
	}

	protected virtual void Awake()
	{
		uiText = GetComponent<Text>();
	}

	protected virtual void Start()
	{
		StartCoroutine(FadeSequence());
		StartCoroutine(Move());
	}

	protected IEnumerator FadeSequence()
	{
		yield return FadeIn();
		yield return new WaitForSeconds(keepTime);
		yield return FadeOut();
		UnityEngine.Object.Destroy(base.gameObject);
	}

	protected IEnumerator FadeIn()
	{
		float startTime = Time.time;
		while (Time.time < startTime + fadeInTime)
		{
			float t = (Time.time - startTime) / fadeInTime;
			SetAlpha(t);
			yield return null;
		}
		SetAlpha(1f);
	}

	protected IEnumerator FadeOut()
	{
		float startTime = Time.time;
		while (Time.time < startTime + fadeOutTime)
		{
			float t = (Time.time - startTime) / fadeOutTime;
			SetAlpha(1f - t);
			yield return null;
		}
		SetAlpha(0f);
	}

	protected IEnumerator Move()
	{
		float startTime = Time.time;
		while (Time.time < startTime + moveTime)
		{
			float t = (Time.time - startTime) / moveTime;
			float s = moveCurve.Evaluate(t);
			base.transform.position += moveSpeed * s * Time.deltaTime;
			yield return null;
		}
	}

	protected void SetAlpha(float a)
	{
		Color color = uiText.color;
		color.a = a;
		uiText.color = color;
	}
}
