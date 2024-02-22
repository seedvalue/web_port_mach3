using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

[SingletonInitializeOnLoad]
[SingletonPrefab]
public class AudioManager : Singleton<AudioManager>
{
	private const string MusicAllowedPref = "AudioManager.musicAllowed";

	private const string SoundAllowedPref = "AudioManager.soundAllowed";

	[Range(0f, 255f)]
	public int musicPriority = 50;

	[Range(0f, 255f)]
	public int soundPriority = 200;

	[Range(1f, 20f)]
	public int maxSounds = 10;

	[Range(0f, 4f)]
	public float fadeTime = 1f;

	public AudioClip defaultSound;

	private AudioSource[] soundSources;

	private AudioSource[] musicSources;

	private AudioSource currentMusicSource;

	private AudioSource waitingMusicSource;

	private AudioSample currentMusicSample;

	private bool inScene;

	private bool musicChanged;

	private static readonly List<AudioScene> currentScenes = new List<AudioScene>();

	public static bool musicAllowed
	{
		get
		{
			return PlayerPrefs.GetInt("AudioManager.musicAllowed", 1) > 0;
		}
		set
		{
			PlayerPrefs.SetInt("AudioManager.musicAllowed", value ? 1 : 0);
			if (Singleton<AudioManager>.HasInstance)
			{
				Singleton<AudioManager>.Instance.SetMusicAllowed(value);
			}
		}
	}

	public static bool soundAllowed
	{
		get
		{
			return PlayerPrefs.GetInt("AudioManager.soundAllowed", 1) > 0;
		}
		set
		{
			PlayerPrefs.SetInt("AudioManager.soundAllowed", value ? 1 : 0);
			if (Singleton<AudioManager>.HasInstance)
			{
				Singleton<AudioManager>.Instance.SetSoundAllowed(value);
			}
		}
	}

	public static void Play(AudioMechanics mechanics)
	{
		PlaySafe(GetSceneSound(mechanics));
	}

	public static void Play(AudioSample sample)
	{
		if (!sample.mute)
		{
			Singleton<AudioManager>.Instance.PlaySound(sample.PickClip(), sample.PickPitch());
		}
	}

	public static void Play(AudioSample sample, float pitch)
	{
		if (!sample.mute)
		{
			Singleton<AudioManager>.Instance.PlaySound(sample.PickClip(), pitch);
		}
	}

	public static void PlaySafe(AudioSample sample)
	{
		if ((bool)sample)
		{
			Play(sample);
		}
	}

	public static void PlaySafe(AudioSample sample, float pitch)
	{
		if ((bool)sample)
		{
			Play(sample, pitch);
		}
	}

	public static void AddScene(AudioScene scene)
	{
		int num = currentScenes.BinarySearch(scene);
		if (num < 0)
		{
			num = ~num;
		}
		else
		{
			UnityEngine.Debug.LogErrorFormat("AudioScene with priority {0} already registered!", scene.priority);
		}
		currentScenes.Insert(num, scene);
		if (Singleton<AudioManager>.HasInstance)
		{
			Singleton<AudioManager>.Instance.SceneChanged();
		}
	}

	public static void RemScene(AudioScene scene)
	{
		currentScenes.Remove(scene);
		if (Singleton<AudioManager>.HasInstance)
		{
			Singleton<AudioManager>.Instance.SceneChanged();
		}
	}

	public static AudioSample GetSceneMusic()
	{
		return (from s in currentScenes
			select s.music into s
			where s != null
			select s).LastOrDefault();
	}

	public static AudioSample GetSceneSound(AudioMechanics mechanics)
	{
		return (from s in currentScenes
			select s.mechanics.GetSample(mechanics) into s
			where s != null
			select s).LastOrDefault();
	}

	protected void Awake()
	{
		SceneLoader.BeginLoading += OnBeginLoad;
		SceneLoader.EndLoading += OnEndLoad;
		musicSources = new AudioSource[2];
		for (int i = 0; i < musicSources.Length; i++)
		{
			musicSources[i] = base.gameObject.AddComponent<AudioSource>();
			musicSources[i].priority = musicPriority;
			musicSources[i].loop = true;
		}
		currentMusicSource = musicSources.First();
		waitingMusicSource = musicSources.Last();
		soundSources = new AudioSource[maxSounds];
		for (int j = 0; j < soundSources.Length; j++)
		{
			soundSources[j] = base.gameObject.AddComponent<AudioSource>();
			soundSources[j].priority = soundPriority;
		}
		SetSoundAllowed(soundAllowed);
		SetMusicAllowed(musicAllowed);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		SceneLoader.BeginLoading -= OnBeginLoad;
		SceneLoader.EndLoading -= OnEndLoad;
	}

	protected void PlaySound(AudioClip clip, float pitch)
	{
		if (clip == null)
		{
			clip = defaultSound;
			pitch = 1f;
		}
		if (!(clip == null))
		{
			AudioSource audioSource = FindFreeAudioSource();
			if (audioSource == null)
			{
				UnityEngine.Debug.LogWarning("Too many sounds playing simultaneously!", this);
				return;
			}
			audioSource.clip = clip;
			audioSource.pitch = pitch;
			audioSource.Play();
		}
	}

	protected AudioSource FindFreeAudioSource()
	{
		AudioSource[] array = soundSources;
		foreach (AudioSource audioSource in array)
		{
			if (!audioSource.isPlaying)
			{
				return audioSource;
			}
		}
		return null;
	}

	protected void OnBeginLoad()
	{
		inScene = false;
	}

	protected void OnEndLoad()
	{
		inScene = true;
		if (musicChanged)
		{
			musicChanged = false;
			SwitchMusic(GetSceneMusic());
		}
	}

	protected void SceneChanged()
	{
		if (inScene)
		{
			SwitchMusic(GetSceneMusic());
		}
		else if (currentMusicSample != GetSceneMusic())
		{
			SwitchMusic(null);
			musicChanged = true;
		}
	}

	protected void SwitchMusic(AudioSample sample, bool instant = false)
	{
		if (!(currentMusicSample == sample))
		{
			float time = (!instant) ? fadeTime : 0f;
			StopMusic(time);
			PlayMusic(sample, time);
		}
	}

	private void StopMusic(float time)
	{
		if (!(currentMusicSample == null))
		{
			currentMusicSample = null;
			if (time <= 0f)
			{
				currentMusicSource.Stop();
			}
			else
			{
				StartCoroutine(FadeOutSource(currentMusicSource, time));
			}
		}
	}

	private IEnumerator FadeOutSource(AudioSource source, float time)
	{
		float startTime = Time.time;
		source.volume = 1f;
		yield return null;
		while (Time.time < startTime + time)
		{
			float t = (Time.time - startTime) / time;
			source.volume = Mathf.SmoothStep(1f, 0f, t);
			yield return null;
		}
		source.volume = 0f;
		source.Stop();
	}

	private void PlayMusic(AudioSample sample, float time)
	{
		if (!(sample == null))
		{
			currentMusicSample = sample;
			waitingMusicSource.volume = 1f;
			waitingMusicSource.clip = sample.PickClip();
			if (time <= 0f)
			{
				waitingMusicSource.Play();
			}
			else
			{
				StartCoroutine(FadeInSource(waitingMusicSource, time));
			}
			AudioSource audioSource = waitingMusicSource;
			waitingMusicSource = currentMusicSource;
			currentMusicSource = audioSource;
		}
	}

	private IEnumerator FadeInSource(AudioSource source, float time)
	{
		float startTime = Time.time;
		source.volume = 0f;
		source.Play();
		yield return null;
		while (Time.time < startTime + time)
		{
			float t = (Time.time - startTime) / time;
			source.volume = Mathf.SmoothStep(0f, 1f, t);
			yield return null;
		}
		source.volume = 1f;
	}

	private void SetSoundAllowed(bool value)
	{
		AudioSource[] array = soundSources;
		foreach (AudioSource audioSource in array)
		{
			audioSource.mute = !value;
		}
	}

	private void SetMusicAllowed(bool value)
	{
		AudioSource[] array = musicSources;
		foreach (AudioSource audioSource in array)
		{
			audioSource.mute = !value;
		}
	}
}
