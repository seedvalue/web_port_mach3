using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class HeroLightManager : MonoBehaviour
{
	public enum Mode
	{
		Global,
		RegisteredRenderers
	}

	public enum FogMode
	{
		Disabled = 0,
		Kamis = 5
	}

	[Serializable]
	public struct KamisLightParams
	{
		[ColorUsage(false)]
		public Color HeroAmbient;

		public Color EnemyAmbient;

		public Color EnemyOutline;
	}

	[Serializable]
	public struct DestroParams
	{
		[ColorUsage(false)]
		public Color Color0;

		[ColorUsage(false)]
		public Color Color1;
	}

	[Serializable]
	public struct SelectionParams
	{
		public Color Selected;

		public Color Faded;
	}

	[Serializable]
	public struct RimParams
	{
		public Color Color;

		public float Hardness;

		public Vector3 Rotation;
	}

	[Serializable]
	public struct KamisFogParams
	{
		[Range(0f, 100f)]
		public float DepthFront;

		[Range(0f, 300f)]
		public float DepthBack;

		public float HeightBase;

		[Range(0.01f, 100f)]
		public float HeightFront;

		[Range(0.01f, 300f)]
		public float HeightBack;

		[Range(0f, 50f)]
		public float HeightPit;

		[Range(0f, 1f)]
		public float DepthIntensity;

		[Range(0f, 5f)]
		public float PitIntensity;

		public Color FrontUpper;

		public Color FrontLower;

		public Color BackUpper;

		public Color BackLower;

		public Color Pit;
	}

	[Serializable]
	public struct KamisShadowParams
	{
		public Color ColorBack;

		public Color ColorBottom;
	}

	[Serializable]
	public struct ShadowParams
	{
		public Color Color;

		public Color AmbientForNormalMapped;
	}

	[Serializable]
	public struct Unity5Workarounds
	{
		public float CharactersLightProbesScale;

		public float CharactersDynamicLightScale;

		public float BakeLightProbesScale;

		public float MaxDynamicLightMagnitudeLM;
	}

	private struct LightState
	{
		public Vector4 PosX;

		public Vector4 PosY;

		public Vector4 PosZ;

		public Vector4 Atten;

		public Color Color0;

		public Color Color1;

		public Color Color2;

		public Color Color3;

		public Color this[int index]
		{
			get
			{
				switch (index)
				{
				case 0:
					return Color0;
				case 1:
					return Color1;
				case 2:
					return Color2;
				case 3:
					return Color3;
				default:
					throw new IndexOutOfRangeException();
				}
			}
			set
			{
				switch (index)
				{
				case 0:
					Color0 = value;
					break;
				case 1:
					Color1 = value;
					break;
				case 2:
					Color2 = value;
					break;
				case 3:
					Color3 = value;
					break;
				default:
					throw new IndexOutOfRangeException();
				}
			}
		}
	}

	[Flags]
	private enum DirtyState
	{
		Nothing = 0x0,
		Fog = 0x1,
		KamisLight = 0x2,
		Rim = 0x4,
		FogKeywords = 0x8,
		Shadow = 0x10,
		ForcedVertexLightKeyword = 0x20,
		KamisShadow = 0x40,
		Workarounds = 0x80,
		Selection = 0x100,
		Destro = 0x200,
		All = 0x3FF
	}

	private struct LightInfo
	{
		public Light light;

		public float intensityMask;

		public bool outOfRange;

		public bool inavtive;
	}

	private static Dictionary<FogMode, string> s_keywords = new Dictionary<FogMode, string>
	{
		{
			FogMode.Disabled,
			"FANCY_FOG_DISABLED"
		},
		{
			FogMode.Kamis,
			"FANCY_FOG_KAMIS_ENABLED"
		}
	};

	public const int MaxDynamicLights = 4;

	[SerializeField]
	private Mode m_Mode;

	[SerializeField]
	private KamisLightParams m_kamisLight = new KamisLightParams
	{
		HeroAmbient = Color.black,
		EnemyAmbient = Color.black,
		EnemyOutline = Color.clear
	};

	[SerializeField]
	private KamisShadowParams m_kamisShadow = new KamisShadowParams
	{
		ColorBack = Color.red,
		ColorBottom = Color.blue
	};

	[SerializeField]
	private DestroParams m_destro = new DestroParams
	{
		Color0 = Color.blue,
		Color1 = Color.red
	};

	[SerializeField]
	private SelectionParams m_selection = new SelectionParams
	{
		Selected = Color.white,
		Faded = Color.black
	};

	[SerializeField]
	private RimParams m_rim = new RimParams
	{
		Color = Color.white,
		Hardness = 0.8f,
		Rotation = Vector3.zero
	};

	[SerializeField]
	private ShadowParams m_shadow = new ShadowParams
	{
		Color = Color.gray,
		AmbientForNormalMapped = Color.black
	};

	[SerializeField]
	private FogMode m_fogType;

	[SerializeField]
	private KamisFogParams m_kamisFog = new KamisFogParams
	{
		DepthFront = 5f,
		DepthBack = 15f,
		HeightBase = 0f,
		HeightFront = 3f,
		HeightBack = 15f,
		HeightPit = 2f,
		DepthIntensity = 0.01f,
		PitIntensity = 0.1f,
		FrontUpper = Color.red,
		FrontLower = Color.magenta,
		BackUpper = Color.green,
		BackLower = Color.blue,
		Pit = Color.white
	};

	[SerializeField]
	public bool DynamicVertexPointLightsEnabled;

	public const float DynamicLightsPartitionDistance = 7f;

	public const float DynamicLightsSwitchRate = 2f;

	[SerializeField]
	private Unity5Workarounds m_unity5Workarounds = new Unity5Workarounds
	{
		CharactersDynamicLightScale = 1f,
		CharactersLightProbesScale = 1f,
		BakeLightProbesScale = 1f,
		MaxDynamicLightMagnitudeLM = 1.73f
	};

	private List<HeroLightRenderer> m_affectedRenderers = new List<HeroLightRenderer>();

	private DirtyState m_dirtyState;

	private LightState m_forcedVertexLightState;

	private List<LightInfo> m_pointVertexLights = new List<LightInfo>();

	private bool m_forcedVertexLightOn;

	public static HeroLightManager Instance
	{
		get;
		private set;
	}

	public KamisLightParams KamisLight
	{
		get
		{
			return m_kamisLight;
		}
		set
		{
			UpdateState(ref m_kamisLight, value, DirtyState.KamisLight);
		}
	}

	public KamisShadowParams KamisShadow
	{
		get
		{
			return m_kamisShadow;
		}
		set
		{
			UpdateState(ref m_kamisShadow, value, DirtyState.KamisShadow);
		}
	}

	public DestroParams Destro
	{
		get
		{
			return m_destro;
		}
		set
		{
			UpdateState(ref m_destro, value, DirtyState.Destro);
		}
	}

	public SelectionParams Hit
	{
		get
		{
			return m_selection;
		}
		set
		{
			UpdateState(ref m_selection, value, DirtyState.Selection);
		}
	}

	public RimParams Rim
	{
		get
		{
			return m_rim;
		}
		set
		{
			UpdateState(ref m_rim, value, DirtyState.Rim);
		}
	}

	public FogMode FogType
	{
		get
		{
			return m_fogType;
		}
		set
		{
			UpdateState(ref m_fogType, value, DirtyState.Fog | DirtyState.FogKeywords);
		}
	}

	public KamisFogParams KamisFog
	{
		get
		{
			return m_kamisFog;
		}
		set
		{
			UpdateState(ref m_kamisFog, value, DirtyState.Fog);
		}
	}

	public ShadowParams Shadow
	{
		get
		{
			return m_shadow;
		}
		set
		{
			UpdateState(ref m_shadow, value, DirtyState.Shadow);
		}
	}

	private bool ForcedVertexLightOn
	{
		get
		{
			return m_forcedVertexLightOn;
		}
		set
		{
			UpdateStateFast(ref m_forcedVertexLightOn, value, DirtyState.ForcedVertexLightKeyword);
		}
	}

	public Unity5Workarounds Workarounds
	{
		get
		{
			return m_unity5Workarounds;
		}
		set
		{
			UpdateState(ref m_unity5Workarounds, value, DirtyState.Workarounds);
		}
	}

	public Transform GetHeroTransform()
	{
		HeroMesh heroMesh = UnityEngine.Object.FindObjectOfType<HeroMesh>();
		if ((bool)heroMesh)
		{
			return heroMesh.transform;
		}
		return base.transform;
	}

	public void AddPointLight(Light light)
	{
		if (light.type != LightType.Point)
		{
			UnityEngine.Debug.LogError("Only point lights are supported", light);
			return;
		}
		for (int i = 0; i < m_pointVertexLights.Count; i++)
		{
			LightInfo lightInfo = m_pointVertexLights[i];
			if (lightInfo.light == light)
			{
				UnityEngine.Debug.LogError("Light already added", light);
				return;
			}
		}
		m_pointVertexLights.Add(new LightInfo
		{
			light = light
		});
	}

	public void RemovePointLight(Light light)
	{
		for (int i = 0; i < m_pointVertexLights.Count; i++)
		{
			LightInfo lightInfo = m_pointVertexLights[i];
			if (lightInfo.light == light)
			{
				m_pointVertexLights.RemoveAt(i);
				return;
			}
		}
		UnityEngine.Debug.LogWarning("Light was not present", light);
	}

	public void AddRenderer(HeroLightRenderer renderer)
	{
		if (m_affectedRenderers.Contains(renderer))
		{
			UnityEngine.Debug.LogError("Light already added", renderer);
			return;
		}
		m_affectedRenderers.Add(renderer);
		m_dirtyState = DirtyState.All;
	}

	public void RemoveRenderer(HeroLightRenderer renderer)
	{
		if (!m_affectedRenderers.Remove(renderer))
		{
			UnityEngine.Debug.LogWarning("HeroLightRenderer was not present", renderer);
		}
	}

	private void OnDrawGizmos()
	{
		if (DynamicVertexPointLightsEnabled && m_pointVertexLights.Count > 4)
		{
			Vector3 position = GetHeroTransform().position;
			Gizmos.DrawWireSphere(position, 7f);
		}
	}

	private void Awake()
	{
		if (m_Mode == Mode.Global)
		{
			if (Instance != null)
			{
				throw new InvalidOperationException("Singleton " + Instance + " already exists");
			}
			Instance = this;
		}
		m_pointVertexLights.Clear();
		m_dirtyState = DirtyState.All;
	}

	private void Start()
	{
		float? depthOffsetForThisDevice = GetDepthOffsetForThisDevice();
		if (depthOffsetForThisDevice.HasValue)
		{
			UnityEngine.Debug.Log("Setting depth offset to " + depthOffsetForThisDevice.Value);
			SetGlobalFloat("_PostDepthPassDepthOffset", depthOffsetForThisDevice.Value);
		}
	}

	public void OnValidate()
	{
		if (DynamicVertexPointLightsEnabled)
		{
			IEnumerable<Light> source = from x in UnityEngine.Object.FindObjectsOfType<DynamicVertexPointLight>()
				where x.enabled
				select x.GetComponent<Light>();
			m_pointVertexLights = source.Select(delegate(Light x)
			{
				LightInfo result = default(LightInfo);
				result.light = x;
				return result;
			}).ToList();
		}
		m_dirtyState = DirtyState.All;
		Update();
	}

	private void Update()
	{
		if (DynamicVertexPointLightsEnabled)
		{
			bool forcedVertexLightOn = false;
			for (int i = 0; i < m_pointVertexLights.Count; i++)
			{
				LightInfo lightInfo = m_pointVertexLights[i];
				if (!lightInfo.light)
				{
					UnityEngine.Debug.LogError("Getting rid of a light at index " + i + " that has been destroyed or disabled, but not removed", this);
					m_pointVertexLights.RemoveAt(i);
					i--;
					continue;
				}
				LightInfo lightInfo2 = m_pointVertexLights[i];
				if (lightInfo2.light.intensity > 0f)
				{
					forcedVertexLightOn = true;
				}
			}
			ForcedVertexLightOn = forcedVertexLightOn;
		}
		else
		{
			ForcedVertexLightOn = false;
		}
		if ((m_dirtyState & DirtyState.FogKeywords) == DirtyState.FogKeywords)
		{
			UpdateFogShaderKeywords();
		}
		if ((m_dirtyState & DirtyState.Fog) == DirtyState.Fog)
		{
			UpdateFogShaderUniforms();
		}
		if ((m_dirtyState & DirtyState.Rim) == DirtyState.Rim)
		{
			UpdateRimUniforms();
		}
		if ((m_dirtyState & DirtyState.KamisLight) == DirtyState.KamisLight)
		{
			UpdateKamisLightUniforms();
		}
		if ((m_dirtyState & DirtyState.KamisShadow) == DirtyState.KamisShadow)
		{
			UpdateKamisShadowUniforms();
		}
		if ((m_dirtyState & DirtyState.Destro) == DirtyState.Destro)
		{
			UpdateDestroUniforms();
		}
		if ((m_dirtyState & DirtyState.Selection) == DirtyState.Selection)
		{
			UpdateSelectionUniforms();
		}
		if ((m_dirtyState & DirtyState.Shadow) == DirtyState.Shadow)
		{
			UpdateShadowUniforms();
		}
		if ((m_dirtyState & DirtyState.ForcedVertexLightKeyword) == DirtyState.ForcedVertexLightKeyword)
		{
			UpdateLightKeywords();
		}
		if ((m_dirtyState & DirtyState.Workarounds) == DirtyState.Workarounds)
		{
			UpdateUnity5Workarounds();
		}
		if (ForcedVertexLightOn)
		{
			LightState lightState = CalculateLightState();
			if (!m_forcedVertexLightState.Equals(lightState))
			{
				m_forcedVertexLightState = lightState;
				UpdateLightUniforms();
			}
		}
		m_dirtyState = DirtyState.Nothing;
	}

	private void UpdateFogShaderKeywords()
	{
		foreach (string value in s_keywords.Values)
		{
			DisableKeyword(value);
		}
		EnableKeyword(s_keywords[FogType]);
	}

	private void UpdateFogShaderUniforms()
	{
		FogMode fogType = FogType;
		if (fogType == FogMode.Kamis)
		{
			Matrix4x4 value = default(Matrix4x4);
			KamisFogParams kamisFog = KamisFog;
			value.SetRow(0, kamisFog.FrontLower);
			KamisFogParams kamisFog2 = KamisFog;
			value.SetRow(1, kamisFog2.FrontUpper);
			KamisFogParams kamisFog3 = KamisFog;
			value.SetRow(2, kamisFog3.BackLower);
			KamisFogParams kamisFog4 = KamisFog;
			value.SetRow(3, kamisFog4.BackUpper);
			SetGlobalMatrix("Kamis_FogColors", value);
			KamisFogParams kamisFog5 = KamisFog;
			SetGlobalColor("Kamis_FogPitColor", kamisFog5.Pit);
			KamisFogParams kamisFog6 = KamisFog;
			float depthFront = kamisFog6.DepthFront;
			KamisFogParams kamisFog7 = KamisFog;
			float y = 1f / kamisFog7.DepthBack;
			KamisFogParams kamisFog8 = KamisFog;
			float depthIntensity = kamisFog8.DepthIntensity;
			KamisFogParams kamisFog9 = KamisFog;
			float z = depthIntensity * kamisFog9.DepthIntensity;
			KamisFogParams kamisFog10 = KamisFog;
			float pitIntensity = kamisFog10.PitIntensity;
			KamisFogParams kamisFog11 = KamisFog;
			SetGlobalVector("Kamis_FogDepths", new Vector4(depthFront, y, z, pitIntensity * kamisFog11.PitIntensity));
			KamisFogParams kamisFog12 = KamisFog;
			float heightBase = kamisFog12.HeightBase;
			KamisFogParams kamisFog13 = KamisFog;
			float y2 = 1f / kamisFog13.HeightFront;
			KamisFogParams kamisFog14 = KamisFog;
			float z2 = 1f / kamisFog14.HeightBack;
			KamisFogParams kamisFog15 = KamisFog;
			SetGlobalVector("Kamis_FogHeights", new Vector4(heightBase, y2, z2, kamisFog15.HeightPit));
		}
	}

	private void UpdateShadowUniforms()
	{
		SetGlobalColor("Fancy_ShadowColor", m_shadow.Color);
		SetGlobalColor("Fancy_AmbientColor", m_shadow.AmbientForNormalMapped);
	}

	private void UpdateRimUniforms()
	{
		Vector3 zero = Vector3.zero;
		RimParams rim = Rim;
		Matrix4x4 value = Matrix4x4.TRS(zero, Quaternion.Euler(rim.Rotation), Vector3.one);
		RimParams rim2 = Rim;
		Vector4 a = rim2.Color;
		RimParams rim3 = Rim;
		Vector4 value2 = a * rim3.Color.a;
		RimParams rim4 = Rim;
		value2.w = rim4.Hardness;
		SetGlobalMatrix("Fancy_RimRotation", value);
		SetGlobalVector("Fancy_RimParams", value2);
	}

	private void UpdateKamisLightUniforms()
	{
		SetGlobalColor("Fancy_HeroAmbientLight", m_kamisLight.HeroAmbient);
		SetGlobalColor("Kamis_EnemyAmbientColor", m_kamisLight.EnemyAmbient);
		SetGlobalColor("Kamis_EnemyOutlineColor", m_kamisLight.EnemyOutline);
	}

	private void UpdateKamisShadowUniforms()
	{
		SetGlobalColor("Kamis_ShadowColor0", m_kamisShadow.ColorBack);
		SetGlobalColor("Kamis_ShadowColor1", m_kamisShadow.ColorBottom);
	}

	private void UpdateDestroUniforms()
	{
		SetGlobalColor("_destroColor0", m_destro.Color0);
		SetGlobalColor("_destroColor1", m_destro.Color1);
	}

	private void UpdateSelectionUniforms()
	{
		SetGlobalColor("Fancy_SelectionSelected", m_selection.Selected);
		SetGlobalColor("Fancy_SelectionFaded", m_selection.Faded);
	}

	private void UpdateLightKeywords()
	{
		if (ForcedVertexLightOn)
		{
			DisableKeyword("FANCY_FORCED_VERTEXLIGHT_OFF");
			EnableKeyword("FANCY_FORCED_VERTEXLIGHT_ON");
		}
		else
		{
			DisableKeyword("FANCY_FORCED_VERTEXLIGHT_ON");
			EnableKeyword("FANCY_FORCED_VERTEXLIGHT_OFF");
		}
	}

	private void UpdateUnity5Workarounds()
	{
		SetGlobalFloat("Fancy_pointLightIncrease", m_unity5Workarounds.CharactersDynamicLightScale - 1f);
		SetGlobalFloat("Fancy_lightProbesIncrease", m_unity5Workarounds.CharactersLightProbesScale - 1f);
		SetGlobalFloat("Fancy_maxDynamicLightMagnitude", m_unity5Workarounds.MaxDynamicLightMagnitudeLM);
	}

	private void UpdateLightUniforms()
	{
		for (int i = 0; i < 4; i++)
		{
			SetGlobalVector("Fancy_LightColor" + i, m_forcedVertexLightState[i]);
		}
		SetGlobalVector("Fancy_4LightPosX0", m_forcedVertexLightState.PosX);
		SetGlobalVector("Fancy_4LightPosY0", m_forcedVertexLightState.PosY);
		SetGlobalVector("Fancy_4LightPosZ0", m_forcedVertexLightState.PosZ);
		SetGlobalVector("Fancy_4LightAtten0", m_forcedVertexLightState.Atten);
	}

	private LightState CalculateLightState()
	{
		LightState result = default(LightState);
		Vector4 zero = Vector4.zero;
		Vector4 zero2 = Vector4.zero;
		Vector4 zero3 = Vector4.zero;
		Vector4 zero4 = Vector4.zero;
		if (m_pointVertexLights.Count > 4)
		{
			Vector2 b = GetHeroTransform().position.ToXZ();
			float num = 49f;
			int num2 = 0;
			for (int i = 0; i < m_pointVertexLights.Count; i++)
			{
				LightInfo value = m_pointVertexLights[i];
				Vector2 a = value.light.transform.position.ToXZ();
				float sqrMagnitude = (a - b).sqrMagnitude;
				if (sqrMagnitude > num)
				{
					value.outOfRange = true;
					value.inavtive = (value.intensityMask <= 0f);
				}
				else
				{
					num2++;
					value.outOfRange = false;
					value.inavtive = false;
				}
				m_pointVertexLights[i] = value;
			}
			m_pointVertexLights.InsertionSort((LightInfo x, LightInfo y) => (x.inavtive ? 1 : 0) - (y.inavtive ? 1 : 0));
			int num3 = 0;
			while (num2 < 4 && num3 < m_pointVertexLights.Count)
			{
				LightInfo value2 = m_pointVertexLights[num3];
				if (value2.outOfRange)
				{
					value2.outOfRange = false;
					m_pointVertexLights[num3] = value2;
					num2++;
				}
				num3++;
			}
		}
		else
		{
			for (int j = 0; j < m_pointVertexLights.Count; j++)
			{
				LightInfo value3 = m_pointVertexLights[j];
				value3.outOfRange = false;
				m_pointVertexLights[j] = value3;
			}
		}
		int num4 = 0;
		int k = 0;
		float num5 = 2f * Time.deltaTime;
		while (num4 < 4 && k < m_pointVertexLights.Count)
		{
			LightInfo value4 = m_pointVertexLights[k];
			Light light = value4.light;
			if (light.enabled && light.gameObject.activeInHierarchy)
			{
				Vector3 position = light.transform.position;
				zero[num4] = position.x;
				zero2[num4] = position.y;
				zero3[num4] = position.z;
				zero4[num4] = 1f / (light.range * light.range);
				result[num4] = light.color * light.intensity * value4.intensityMask;
				num4++;
				if (value4.outOfRange)
				{
					value4.intensityMask = Mathf.Clamp01(value4.intensityMask - num5);
				}
				else
				{
					value4.intensityMask = Mathf.Clamp01(value4.intensityMask + num5);
				}
				m_pointVertexLights[k] = value4;
			}
			k++;
		}
		for (; k < m_pointVertexLights.Count; k++)
		{
			LightInfo value5 = m_pointVertexLights[k];
			value5.intensityMask = 0f;
			m_pointVertexLights[k] = value5;
		}
		for (int l = num4; l < 4; l++)
		{
			result[l] = Color.black;
		}
		result.PosX = zero;
		result.PosY = zero2;
		result.PosZ = zero3;
		result.Atten = zero4;
		return result;
	}

	private void UpdateState<T>(ref T variable, T value, DirtyState flag) where T : struct
	{
		if (!variable.Equals(value))
		{
			variable = value;
			m_dirtyState |= flag;
		}
	}

	private void UpdateStateFast<T>(ref T variable, T value, DirtyState flag) where T : struct, IEquatable<T>
	{
		if (!variable.Equals(value))
		{
			variable = value;
			m_dirtyState |= flag;
		}
	}

	private void UpdateStateObj<T>(ref T variable, T value, DirtyState flag) where T : UnityEngine.Object
	{
		if (!object.Equals(variable, value))
		{
			variable = value;
			m_dirtyState |= flag;
		}
	}

	private static float? GetDepthOffsetForThisDevice()
	{
		if (SystemInfo.graphicsDeviceName == "PowerVR Rogue G6200" || SystemInfo.graphicsDeviceName == "PowerVR Rogue G6430")
		{
			return -1000f;
		}
		return null;
	}

	private void DisableKeyword(string keyword)
	{
		if (m_Mode == Mode.Global)
		{
			Shader.DisableKeyword(keyword);
		}
		else
		{
			foreach (HeroLightRenderer affectedRenderer in m_affectedRenderers)
			{
				Material[] materials = affectedRenderer.renderer.materials;
				foreach (Material material in materials)
				{
					material.DisableKeyword(keyword);
				}
			}
		}
	}

	private void EnableKeyword(string keyword)
	{
		if (m_Mode == Mode.Global)
		{
			Shader.EnableKeyword(keyword);
		}
		else
		{
			foreach (HeroLightRenderer affectedRenderer in m_affectedRenderers)
			{
				Material[] materials = affectedRenderer.renderer.materials;
				foreach (Material material in materials)
				{
					material.EnableKeyword(keyword);
				}
			}
		}
	}

	private void SetGlobalColor(string propertyName, Color value)
	{
		if (m_Mode == Mode.Global)
		{
			Shader.SetGlobalColor(propertyName, value);
		}
		else
		{
			foreach (HeroLightRenderer affectedRenderer in m_affectedRenderers)
			{
				Material[] materials = affectedRenderer.renderer.materials;
				foreach (Material material in materials)
				{
					material.SetColor(propertyName, value);
				}
			}
		}
	}

	private void SetGlobalFloat(string propertyName, float value)
	{
		if (m_Mode == Mode.Global)
		{
			Shader.SetGlobalFloat(propertyName, value);
		}
		else
		{
			foreach (HeroLightRenderer affectedRenderer in m_affectedRenderers)
			{
				Material[] materials = affectedRenderer.renderer.materials;
				foreach (Material material in materials)
				{
					material.SetFloat(propertyName, value);
				}
			}
		}
	}

	private void SetGlobalMatrix(string propertyName, Matrix4x4 value)
	{
		if (m_Mode == Mode.Global)
		{
			Shader.SetGlobalMatrix(propertyName, value);
		}
		else
		{
			foreach (HeroLightRenderer affectedRenderer in m_affectedRenderers)
			{
				Material[] materials = affectedRenderer.renderer.materials;
				foreach (Material material in materials)
				{
					material.SetMatrix(propertyName, value);
				}
			}
		}
	}

	private void SetGlobalTexture(string propertyName, Texture value)
	{
		if (m_Mode == Mode.Global)
		{
			Shader.SetGlobalTexture(propertyName, value);
		}
		else
		{
			foreach (HeroLightRenderer affectedRenderer in m_affectedRenderers)
			{
				Material[] materials = affectedRenderer.renderer.materials;
				foreach (Material material in materials)
				{
					material.SetTexture(propertyName, value);
				}
			}
		}
	}

	private void SetGlobalVector(string propertyName, Vector4 value)
	{
		if (m_Mode == Mode.Global)
		{
			Shader.SetGlobalVector(propertyName, value);
		}
		else
		{
			foreach (HeroLightRenderer affectedRenderer in m_affectedRenderers)
			{
				Material[] materials = affectedRenderer.renderer.materials;
				foreach (Material material in materials)
				{
					material.SetVector(propertyName, value);
				}
			}
		}
	}
}
