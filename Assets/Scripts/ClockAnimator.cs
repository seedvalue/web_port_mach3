using System;
using UnityEngine;

public class ClockAnimator : MonoBehaviour
{
	private const float hoursToDegrees = 30f;

	private const float minutesToDegrees = 6f;

	private const float secondsToDegrees = 6f;

	public Transform hours;

	public Transform minutes;

	public Transform seconds;

	public bool analog;

	private void Update()
	{
		if (analog)
		{
			TimeSpan timeOfDay = DateTime.Now.TimeOfDay;
			hours.localRotation = Quaternion.Euler(0f, 0f, (float)timeOfDay.TotalHours * -30f);
			minutes.localRotation = Quaternion.Euler(0f, 0f, (float)timeOfDay.TotalMinutes * -6f);
			seconds.localRotation = Quaternion.Euler(0f, 0f, (float)timeOfDay.TotalSeconds * -6f);
		}
		else
		{
			DateTime now = DateTime.Now;
			hours.localRotation = Quaternion.Euler(0f, 0f, (float)now.Hour * -30f);
			minutes.localRotation = Quaternion.Euler(0f, 0f, (float)now.Minute * -6f);
			seconds.localRotation = Quaternion.Euler(0f, 0f, (float)now.Second * -6f);
		}
	}
}
