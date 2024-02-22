using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(ParticleSystem))]
public class CanvasParticleSystem : MaskableGraphic
{
	[SerializeField]
	private Vector2 scale = Vector2.one;

	private ParticleSystem ps;

	private ParticleSystem.Particle[] tempParticles;

	private readonly UIVertex[] tempQuad = new UIVertex[4];

	protected override void Awake()
	{
		base.Awake();
		if (!Init())
		{
			base.enabled = false;
		}
	}

	protected virtual void Update()
	{
		if (Application.isPlaying && ps.IsAlive())
		{
			SetAllDirty();
		}
	}

	protected bool Init()
	{
		ps = GetComponent<ParticleSystem>();
		return true;
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (tempParticles == null || tempParticles.Length < ps.main.maxParticles)
		{
			tempParticles = new ParticleSystem.Particle[ps.main.maxParticles];
		}
		Transform transform = null;
		if (ps.main.simulationSpace == ParticleSystemSimulationSpace.World)
		{
			transform = base.transform;
		}
		else if (ps.main.simulationSpace == ParticleSystemSimulationSpace.Custom)
		{
			transform = ps.main.customSimulationSpace;
		}
		float d = 1f;
		if (ps.main.scalingMode == ParticleSystemScalingMode.Shape)
		{
			d = 1f / base.canvas.scaleFactor;
		}
		int num = 0;
		Vector2 vector = Vector2.zero;
		if (ps.textureSheetAnimation.enabled)
		{
			num = ps.textureSheetAnimation.numTilesX * ps.textureSheetAnimation.numTilesY;
			vector = new Vector2(1f / (float)ps.textureSheetAnimation.numTilesX, 1f / (float)ps.textureSheetAnimation.numTilesY);
		}
		int particles = ps.GetParticles(tempParticles);
		for (int i = 0; i < particles; i++)
		{
			ParticleSystem.Particle particle = tempParticles[i];
			Vector2 a = ((!(transform == null)) ? transform.InverseTransformPoint(particle.position) : particle.position) * d;
			a.Scale(scale);
			float num2 = particle.rotation * ((float)Math.PI / 180f);
			Color32 color = particle.GetCurrentColor(ps) * this.color;
			Vector2 b = scale * particle.GetCurrentSize(ps) * 0.5f;
			float time = 1f - particle.remainingLifetime / particle.startLifetime;
			float num3 = 0f;
			float x = 1f;
			float num4 = 0f;
			float y = 1f;
			if (num > 0)
			{
				float num5 = ps.textureSheetAnimation.frameOverTime.Evaluate(time);
				num5 = Mathf.Repeat(num5 * (float)ps.textureSheetAnimation.cycleCount, 1f);
				int num6 = 0;
				switch (ps.textureSheetAnimation.animation)
				{
				case ParticleSystemAnimationType.WholeSheet:
					num6 = Mathf.FloorToInt(num5 * (float)num);
					break;
				case ParticleSystemAnimationType.SingleRow:
				{
					num6 = Mathf.FloorToInt(num5 * (float)ps.textureSheetAnimation.numTilesX);
					int num7 = ps.textureSheetAnimation.rowIndex;
					if (ps.textureSheetAnimation.useRandomRow)
					{
						num7 = (int)(particle.randomSeed % (uint)ps.textureSheetAnimation.numTilesY);
					}
					num6 += num7 * ps.textureSheetAnimation.numTilesX;
					break;
				}
				}
				num6 %= num;
				num3 = (float)(num6 % ps.textureSheetAnimation.numTilesX) * vector.x;
				num4 = (float)Mathf.FloorToInt(num6 / ps.textureSheetAnimation.numTilesX) * vector.y;
				x = num3 + vector.x;
				y = num4 + vector.y;
			}
			tempQuad[0] = UIVertex.simpleVert;
			tempQuad[0].color = color;
			tempQuad[0].uv0 = new Vector2(num3, num4);
			tempQuad[1] = UIVertex.simpleVert;
			tempQuad[1].color = color;
			tempQuad[1].uv0 = new Vector2(num3, y);
			tempQuad[2] = UIVertex.simpleVert;
			tempQuad[2].color = color;
			tempQuad[2].uv0 = new Vector2(x, y);
			tempQuad[3] = UIVertex.simpleVert;
			tempQuad[3].color = color;
			tempQuad[3].uv0 = new Vector2(x, num4);
			if (Mathf.Approximately(num2, 0f))
			{
				Vector2 vector2 = a - b;
				Vector2 vector3 = a + b;
				tempQuad[0].position = new Vector2(vector2.x, vector2.y);
				tempQuad[1].position = new Vector2(vector2.x, vector3.y);
				tempQuad[2].position = new Vector2(vector3.x, vector3.y);
				tempQuad[3].position = new Vector2(vector3.x, vector2.y);
			}
			else
			{
				float num8 = Mathf.Cos(num2);
				float num9 = Mathf.Sin(num2);
				Vector2 b2 = new Vector2(num9, num8);
				Vector2 b3 = new Vector2(num8, 0f - num9);
				b2.Scale(b);
				b3.Scale(b);
				tempQuad[0].position = a - b3 - b2;
				tempQuad[1].position = a - b3 + b2;
				tempQuad[2].position = a + b3 + b2;
				tempQuad[3].position = a + b3 - b2;
			}
			vh.AddUIVertexQuad(tempQuad);
		}
	}
}
