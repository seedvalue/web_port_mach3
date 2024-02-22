using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSample", menuName = "Audio Sample", order = 220)]
public class AudioSample : ScriptableObject
{
	public bool mute;

	public AudioClip[] sources;

	public float minPitch = 0.95f;

	public float maxPitch = 1.05f;

	public AudioClip PickClip()
	{
		return (from s in sources
			where s != null
			orderby Rand.uniform
			select s).FirstOrDefault();
	}

	public float PickPitch()
	{
		return Mathf.Lerp(minPitch, maxPitch, Rand.uniform);
	}
}
