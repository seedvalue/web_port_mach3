using System;
using UnityEngine;

public class AudioScene : MonoBehaviour, IComparable<AudioScene>
{
	public int priority;

	public AudioSample music;

	public AudioMechanicsSamples mechanics;

	protected virtual void OnEnable()
	{
		AudioManager.AddScene(this);
	}

	protected virtual void OnDisable()
	{
		AudioManager.RemScene(this);
	}

	public int CompareTo(AudioScene other)
	{
		return priority.CompareTo(other.priority);
	}
}
