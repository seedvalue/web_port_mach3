using System;
using UnityEngine;
using Utils;

public class UITweener : MonoBehaviour
{
	public enum InterpolationType
	{
		Linear,
		Hermite,
		Sinerp,
		Coserp,
		Berp,
		SmoothStep,
		Bounce6,
		Bounce3,
		QuadIn,
		QuadOut,
		QuadInOut,
		SinIn,
		SinOut,
		SinInOut,
		ExpIn,
		ExpOut,
		ExpInOut,
		CircIn,
		CircOut,
		CircInOut,
		CubeIn,
		CubeOut,
		CubeInOut,
		QuarticIn,
		QuarticOut,
		QuarticInOut,
		QuinticIn,
		QuinticOut,
		QuinticInOut
	}

	public enum WindowAnimation
	{
		Fade,
		ScaleFromPivot,
		OutsideTopScreenEdge,
		OutsideBottomScreenEdge,
		OutsideLeftScreenEdge,
		OutsideRightScreenEdge,
		ForcedPosition
	}

	private enum State
	{
		Shown,
		Hidden,
		Showing,
		Hiding
	}

	[Header("Show animation")]
	public WindowAnimation showAnimation = WindowAnimation.ScaleFromPivot;

	public InterpolationType showInterpolation;

	public float showTime;

	[Header("Hide animation")]
	public WindowAnimation hideAnimation = WindowAnimation.ScaleFromPivot;

	public InterpolationType hideInterpolation;

	public float hideTime;

	public Vector2 animForcedHidePosition;

	private State animState = State.Hidden;

	private float animTime;

	private float animDurr;

	private Vector2 animPositionShow;

	private Vector2 animPositionHide;

	private Vector2 animScaleShow;

	private Vector2 animScaleHide;

	private float animAlphaShow;

	private float animAlphaHide;

	private bool defaultsInitialized;

	private Vector2 defaultScale;

	private Vector2 defaultPosition;

	private float defaultAlpha;

	private Action onAnimEnd;

	private float Apply(float t, InterpolationType type)
	{
		switch (type)
		{
		case InterpolationType.Hermite:
			return Helpers.simple_hermite(t);
		case InterpolationType.Sinerp:
			return Helpers.simple_sinerp(t);
		case InterpolationType.Coserp:
			return Helpers.simple_coserp(t);
		case InterpolationType.Berp:
			return Helpers.simple_berp(t);
		case InterpolationType.SmoothStep:
			return Helpers.simple_smooth_step(t);
		case InterpolationType.Bounce6:
			return Helpers.simple_bounce6(t);
		case InterpolationType.Bounce3:
			return Helpers.simple_bounce3(t);
		case InterpolationType.QuadIn:
			return Helpers.simple_quad_in(t);
		case InterpolationType.QuadOut:
			return Helpers.simple_quad_out(t);
		case InterpolationType.QuadInOut:
			return Helpers.simple_quad_inout(t);
		case InterpolationType.SinIn:
			return Helpers.simple_sin_in(t);
		case InterpolationType.SinOut:
			return Helpers.simple_sin_out(t);
		case InterpolationType.SinInOut:
			return Helpers.simple_sin_inout(t);
		case InterpolationType.ExpIn:
			return Helpers.simple_exp_in(t);
		case InterpolationType.ExpOut:
			return Helpers.simple_exp_out(t);
		case InterpolationType.ExpInOut:
			return Helpers.simple_exp_inout(t);
		case InterpolationType.CircIn:
			return Helpers.simple_circ_in(t);
		case InterpolationType.CircOut:
			return Helpers.simple_circ_out(t);
		case InterpolationType.CircInOut:
			return Helpers.simple_circ_inout(t);
		case InterpolationType.CubeIn:
			return Helpers.simple_cube_in(t);
		case InterpolationType.CubeOut:
			return Helpers.simple_cube_out(t);
		case InterpolationType.CubeInOut:
			return Helpers.simple_cube_inout(t);
		case InterpolationType.QuarticIn:
			return Helpers.simple_quartic_in(t);
		case InterpolationType.QuarticOut:
			return Helpers.simple_quartic_out(t);
		case InterpolationType.QuarticInOut:
			return Helpers.simple_quartic_inout(t);
		case InterpolationType.QuinticIn:
			return Helpers.simple_quintic_in(t);
		case InterpolationType.QuinticOut:
			return Helpers.simple_quintic_out(t);
		case InterpolationType.QuinticInOut:
			return Helpers.simple_quintic_inout(t);
		default:
			return t;
		}
	}

	protected void OnEnable()
	{
		if (!defaultsInitialized)
		{
			defaultPosition = base.transform.localPosition;
			defaultScale = base.transform.localScale;
			defaultAlpha = GetAlpha();
			defaultsInitialized = true;
		}
		animPositionShow = defaultPosition;
		animScaleShow = defaultScale;
		animAlphaShow = defaultAlpha;
	}

	public void SetDefaults()
	{
		defaultPosition = base.transform.localPosition;
		defaultScale = base.transform.localScale;
		defaultAlpha = GetAlpha();
		defaultsInitialized = true;
		animPositionShow = defaultPosition;
		animScaleShow = defaultScale;
		animAlphaShow = defaultAlpha;
	}

	private void Update()
	{
		UpdateAnim(Time.deltaTime);
	}

	public bool InvokeShowAnim(bool instant, Action callback)
	{
		return InvokeAnim(show: true, instant, callback);
	}

	public bool InvokeHideAnim(bool instant, Action callback)
	{
		return InvokeAnim(show: false, instant, callback);
	}

	private bool InvokeAnim(bool show, bool instant, Action callback)
	{
		if (show && (animState == State.Showing || animState == State.Shown))
		{
			return true;
		}
		if (!show && (animState == State.Hiding || animState == State.Hidden))
		{
			return true;
		}
		WindowAnimation animation = (!show) ? hideAnimation : showAnimation;
		float num = instant ? 0f : ((!show) ? hideTime : showTime);
		num *= GetAnimProgress();
		SkipAnim();
		CalculateAnimHideProperties(animation);
		if (show)
		{
			animState = State.Showing;
		}
		else
		{
			animState = State.Hiding;
		}
		OnAnimStart();
		onAnimEnd = callback;
		if (num <= 0f)
		{
			SkipAnim();
			return true;
		}
		animTime = 0f;
		animDurr = num;
		UpdateAnim(0f);
		return true;
	}

	private void SkipAnim()
	{
		UpdateAnim(float.MaxValue);
	}

	private bool CalculateAnimHideProperties(WindowAnimation animation)
	{
		animPositionHide = animPositionShow;
		animScaleHide = animScaleShow;
		animAlphaHide = animAlphaShow;
		Canvas componentInParent = GetComponentInParent<Canvas>();
		RectTransform component = componentInParent.GetComponent<RectTransform>();
		Vector2 vector = 2f * component.anchoredPosition;
		Vector2 sizeDelta = GetComponent<RectTransform>().sizeDelta;
		switch (animation)
		{
		case WindowAnimation.Fade:
			animAlphaHide = 0f;
			break;
		case WindowAnimation.ScaleFromPivot:
			animScaleHide = Vector2.zero;
			break;
		case WindowAnimation.OutsideTopScreenEdge:
			animPositionHide.y = vector.y * 0.5f + sizeDelta.y * 0.5f;
			break;
		case WindowAnimation.OutsideBottomScreenEdge:
			animPositionHide.y = (0f - vector.y) * 0.5f - sizeDelta.y * 0.5f;
			break;
		case WindowAnimation.OutsideLeftScreenEdge:
			animPositionHide.x = (0f - vector.x) * 0.5f - sizeDelta.x * 0.5f;
			break;
		case WindowAnimation.OutsideRightScreenEdge:
			animPositionHide.x = vector.x * 0.5f + sizeDelta.x * 0.5f;
			break;
		case WindowAnimation.ForcedPosition:
			animPositionHide = animForcedHidePosition;
			break;
		}
		return true;
	}

	private void UpdateAnim(float dt)
	{
		if (animState != State.Showing && animState != State.Hiding)
		{
			return;
		}
		animTime += dt;
		float animProgress = GetAnimProgress();
		if (animProgress >= 1f)
		{
			animTime = 0f;
			animDurr = 0f;
			if (animState == State.Showing)
			{
				base.gameObject.transform.localPosition = animPositionShow;
				base.gameObject.transform.localScale = animScaleShow;
				SetAlpha(animAlphaShow);
				animState = State.Shown;
				OnAnimEnd();
			}
			else
			{
				base.gameObject.transform.localPosition = animPositionHide;
				base.gameObject.transform.localScale = animScaleHide;
				SetAlpha(animAlphaHide);
				animState = State.Hidden;
				OnAnimEnd();
			}
		}
		else if (animState == State.Showing)
		{
			animProgress = Apply(animProgress, showInterpolation);
			base.gameObject.transform.localPosition = Vector2.Lerp(animPositionHide, animPositionShow, animProgress);
			base.gameObject.transform.localScale = Vector2.Lerp(animScaleHide, animScaleShow, animProgress);
			SetAlpha(Mathf.Lerp(animAlphaHide, animAlphaShow, animProgress));
		}
		else
		{
			animProgress = Apply(1f - animProgress, hideInterpolation);
			base.gameObject.transform.localPosition = Vector2.Lerp(animPositionHide, animPositionShow, animProgress);
			base.gameObject.transform.localScale = Vector2.Lerp(animScaleHide, animScaleShow, animProgress);
			SetAlpha(Mathf.Lerp(animAlphaHide, animAlphaShow, animProgress));
		}
	}

	private float GetAnimProgress()
	{
		if (animDurr > 0f)
		{
			return animTime / animDurr;
		}
		return 1f;
	}

	private void SetAlpha(float alpha)
	{
		CanvasGroup component = GetComponent<CanvasGroup>();
		if ((bool)component)
		{
			component.alpha = alpha;
		}
	}

	private float GetAlpha()
	{
		CanvasGroup component = GetComponent<CanvasGroup>();
		if ((bool)component)
		{
			return component.alpha;
		}
		return 1f;
	}

	private void OnAnimStart()
	{
	}

	private void OnAnimEnd()
	{
		if (onAnimEnd != null)
		{
			onAnimEnd();
			onAnimEnd = null;
		}
	}
}
