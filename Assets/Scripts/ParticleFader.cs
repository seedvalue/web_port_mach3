using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFader : MonoBehaviour
{
	public float fadeDuration = 0.25f;

	public bool autoStart;

	public float autoStartDelay = 0.5f;

	private List<ParticleSystem> pSystems = new List<ParticleSystem>();

	private M3InterpolatedFloat time;

	private int fadedSystems;

	private void Start()
	{
		GetComponentsInChildren(pSystems);
		time = new M3InterpolatedFloat(this);
		if (autoStart)
		{
			foreach (ParticleSystem pSystem in pSystems)
			{
				StartCoroutine(CoFadeOut(pSystem, autoStarted: true));
			}
		}
	}

	public void FadeOut()
	{
		foreach (ParticleSystem pSystem in pSystems)
		{
			StartCoroutine(CoFadeOut(pSystem, autoStarted: false));
		}
	}

    private IEnumerator CoFadeOut(ParticleSystem pSystem, bool autoStarted)
    {
        if (autoStarted && autoStartDelay > float.Epsilon)
        {
            yield return new WaitForSeconds(autoStartDelay);
        }

        pSystem.Stop();

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[pSystem.main.maxParticles];
        int pCount = pSystem.GetParticles(particles);
        Color[] alpha = new Color[pCount];

        for (int i = 0; i < pCount; i++)
        {
            alpha[i] = particles[i].GetCurrentColor(pSystem);
        }

        var colorOverLifetimeModule = pSystem.colorOverLifetime;
        colorOverLifetimeModule.enabled = false;

        var colorBySpeedModule = pSystem.colorBySpeed;
        colorBySpeedModule.enabled = false;

        time.SlideDown(fadeDuration, adjustTime: false, forceFullCycle: true);

        while (time.Value > 0f)
        {
            for (int j = 0; j < pCount; j++)
            {
                Color c = alpha[j];
                c.a *= time.Value;
                particles[j].startColor = c;
            }

            pSystem.SetParticles(particles, pCount);
            yield return null;
        }

        fadedSystems++;
    }


    private void Update()
	{
		if (fadedSystems == pSystems.Count)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
