using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Utils
{
	public static class Helpers
	{
		private const float PI = (float)Math.PI;

		private const float PI_DIV_TWO = (float)Math.PI / 2f;

		private const float TWO_PI = (float)Math.PI * 2f;

		public static float simple_hermite(float t)
		{
			return t * t * (3f - 2f * t);
		}

		public static float simple_sinerp(float t)
		{
			return Mathf.Sin(t * ((float)Math.PI / 2f));
		}

		public static float simple_coserp(float t)
		{
			return 1f - Mathf.Cos(t * ((float)Math.PI / 2f));
		}

		public static float simple_berp(float t)
		{
			return (Mathf.Sin(t * (float)Math.PI * (0.2f + 2.5f * t * t * t)) * Mathf.Pow(1f - t, 2.2f) + t) * (1f + 1.2f * (1f - t));
		}

		public static float simple_bounce6(float t)
		{
			return Mathf.Abs(Mathf.Sin((float)Math.PI * 2f * (t + 1f) * (t + 1f)) * (1f - t));
		}

		public static float simple_bounce3(float t)
		{
			return Mathf.Abs(Mathf.Sin((float)Math.PI * (t + 1f) * (t + 1f)) * (1f - t));
		}

		public static float simple_smooth_step(float t)
		{
			return t * t * (3f - 2f * t);
		}

		public static float simple_quad_in(float t)
		{
			return t * t;
		}

		public static float simple_quad_out(float t)
		{
			return (0f - t) * (t - 2f);
		}

		public static float simple_quad_inout(float t)
		{
			t *= 2f;
			if (t < 1f)
			{
				return 0.5f * t * t;
			}
			t -= 1f;
			return -0.5f * (t * (t - 2f) - 1f);
		}

		public static float simple_sin_in(float t)
		{
			return 0f - Mathf.Cos(t * ((float)Math.PI / 2f));
		}

		public static float simple_sin_out(float t)
		{
			return Mathf.Sin(t * ((float)Math.PI / 2f));
		}

		public static float simple_sin_inout(float t)
		{
			return -0.5f * (Mathf.Cos((float)Math.PI * t) - 1f);
		}

		public static float simple_exp_in(float t)
		{
			return Mathf.Pow(2f, 10f * (t - 1f));
		}

		public static float simple_exp_out(float t)
		{
			return 0f - Mathf.Pow(2f, -10f * t) + 1f;
		}

		public static float simple_exp_inout(float t)
		{
			t *= 2f;
			if (t < 1f)
			{
				return 0.5f * Mathf.Pow(2f, 10f * (t - 1f));
			}
			t -= 1f;
			return 0.5f * (0f - Mathf.Pow(2f, -10f * t) + 2f);
		}

		public static float simple_circ_in(float t)
		{
			return 0f - (Mathf.Sqrt(1f - t * t) - 1f);
		}

		public static float simple_circ_out(float t)
		{
			t -= 1f;
			return Mathf.Sqrt(1f - t * t);
		}

		public static float simple_circ_inout(float t)
		{
			t *= 2f;
			if (t < 1f)
			{
				return -0.5f * (Mathf.Sqrt(1f - t * t) - 1f);
			}
			t -= 2f;
			return 0.5f * (Mathf.Sqrt(1f - t * t) + 1f);
		}

		public static float simple_cube_in(float t)
		{
			return t * t * t;
		}

		public static float simple_cube_out(float t)
		{
			t -= 1f;
			return t * t * t + 1f;
		}

		public static float simple_cube_inout(float t)
		{
			t *= 2f;
			if (t < 1f)
			{
				return 0.5f * t * t * t;
			}
			t -= 2f;
			return 0.5f * (t * t * t + 2f);
		}

		public static float simple_quartic_in(float t)
		{
			return t * t * t * t;
		}

		public static float simple_quartic_out(float t)
		{
			t -= 1f;
			return 1f - t * t * t * t;
		}

		public static float simple_quartic_inout(float t)
		{
			t *= 2f;
			if (t < 1f)
			{
				return 0.5f * t * t * t * t;
			}
			t -= 2f;
			return -0.5f * (t * t * t * t - 2f);
		}

		public static float simple_quintic_in(float t)
		{
			return t * t * t * t * t;
		}

		public static float simple_quintic_out(float t)
		{
			t -= 1f;
			return t * t * t * t * t + 1f;
		}

		public static float simple_quintic_inout(float t)
		{
			t *= 2f;
			if (t < 1f)
			{
				return 0.5f * t * t * t * t * t;
			}
			t -= 2f;
			return 0.5f * (t * t * t * t * t + 2f);
		}

		public static float rubber_band_response(float x, float d, float c)
		{
			return (1f - 1f / (x * c / d + 1f)) * d;
		}

		public static bool IntInRange(int value, int min, int max)
		{
			return value >= min && value < max;
		}

		public static Color SmoothStepColor(Color from, Color to, float time)
		{
			Color result = default(Color);
			result.r = Mathf.SmoothStep(from.r, to.r, time);
			result.g = Mathf.SmoothStep(from.g, to.g, time);
			result.b = Mathf.SmoothStep(from.b, to.b, time);
			result.a = Mathf.SmoothStep(from.a, to.a, time);
			return result;
		}

		public static void SetText(Text target, string value)
		{
			if (!(target == null))
			{
				target.text = value;
			}
		}

		public static void SetLocalScale(Component target, Vector3 value)
		{
			if (!(target == null))
			{
				target.transform.localScale = value;
			}
		}

		public static void SetLocalScale(GameObject target, Vector3 value)
		{
			if (!(target == null))
			{
				target.transform.localScale = value;
			}
		}

		public static void SetLocalPosition(Component target, Vector3 value)
		{
			if (!(target == null))
			{
				target.transform.localPosition = value;
			}
		}

		public static void SetTextWithQuads(Text target, string value)
		{
			if (!(target == null))
			{
				target.enabled = false;
				target.text = value;
				target.enabled = true;
			}
		}

		public static void SetImage(Image target, Sprite value)
		{
			if (!(target == null))
			{
				target.sprite = value;
			}
		}

		public static void SetRGB(Component target, Color color)
		{
			Text text = target as Text;
			if ((bool)text)
			{
				Color color2 = text.color;
				color.a = color2.a;
				text.color = color;
				return;
			}
			Image image = target as Image;
			if ((bool)image)
			{
				Color color3 = image.color;
				color.a = color3.a;
				image.color = color;
			}
		}

		public static void SetSpriteColor(SpriteRenderer target, Color value)
		{
			if (!(target == null))
			{
				target.color = value;
			}
		}

		public static void SetImageColor(Image target, Color value)
		{
			if (!(target == null))
			{
				target.color = value;
			}
		}

		public static void SetTextColor(Text target, Color value)
		{
			if (!(target == null))
			{
				target.color = value;
			}
		}

		public static void SetSpriteAlpha(SpriteRenderer target, float alpha)
		{
			if (!(target == null))
			{
				Color color = target.color;
				float r = color.r;
				Color color2 = target.color;
				float g = color2.g;
				Color color3 = target.color;
				target.color = new Color(r, g, color3.b, alpha);
			}
		}

		public static void SetImageAlpha(Image target, float alpha)
		{
			if (!(target == null))
			{
				Color color = target.color;
				float r = color.r;
				Color color2 = target.color;
				float g = color2.g;
				Color color3 = target.color;
				target.color = new Color(r, g, color3.b, alpha);
			}
		}

		public static void SetTextAlpha(Text target, float alpha)
		{
			if (!(target == null))
			{
				Color color = target.color;
				float r = color.r;
				Color color2 = target.color;
				float g = color2.g;
				Color color3 = target.color;
				target.color = new Color(r, g, color3.b, alpha);
			}
		}

		public static void SetActive(GameObject target, bool value)
		{
			if (!(target == null))
			{
				target.SetActive(value);
			}
		}

		public static void SetActiveGameObject(Component target, bool value)
		{
			if (!(target == null))
			{
				target.gameObject.SetActive(value);
			}
		}

		public static void SetActiveMonoBehaviour(MonoBehaviour target, bool value)
		{
			if (!(target == null))
			{
				target.enabled = value;
			}
		}

		public static void SetImageMaterial(Image target, Material material)
		{
			if (!(target == null))
			{
				target.material = material;
			}
		}

		public static void SetTime(TimeText target, DateTime? value, bool countdown = false)
		{
			if (!(target == null))
			{
				target.time = value;
				target.countdown = countdown;
			}
		}

		public static void AddListenerOnClick(Button target, UnityAction call)
		{
			if (!(target == null))
			{
				target.onClick.AddListener(call);
			}
		}

		public static T[] Shuffle<T>(T[] array)
		{
			for (int num = array.Length; num > 0; num--)
			{
				int num2 = UnityEngine.Random.Range(0, num);
				T val = array[num2];
				array[num2] = array[num - 1];
				array[num - 1] = val;
			}
			return array;
		}
	}
}
