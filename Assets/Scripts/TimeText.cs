using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class TimeText : Text
{
	public int maxParts = 2;

	public bool leadingZero;

	public bool middleZero = true;

	public bool skipZeroValues = true;

	public string separator = " ";

	public string daySymbol = "d";

	public string hourSymbol = "h";

	public string minuteSymbol = "m";

	public string secondSymbol = "s";

	public string defaultValue = "0s";

	private string formatText;

	private bool _countdown;

	private DateTime? _time;

	private MetaTimeManager timeManager;

	private bool dirty;

	private const string leadingZeroFormat = "d2";

	private const string defaultFormat = "d";

	public bool countdown
	{
		get
		{
			return _countdown;
		}
		set
		{
			_countdown = value;
			dirty = true;
		}
	}

	public DateTime? time
	{
		get
		{
			return _time;
		}
		set
		{
			_time = value;
			dirty = true;
		}
	}

	public TimeSpan? currentTime
	{
		get
		{
			if (!this.time.HasValue)
			{
				return null;
			}
			if (countdown)
			{
				DateTime utcNow = timeManager.utcNow;
				if (utcNow >= this.time.Value)
				{
					return null;
				}
				return this.time.Value - utcNow;
			}
			DateTime? time = this.time;
			return (!time.HasValue) ? null : new TimeSpan?(time.GetValueOrDefault() - DateTime.MinValue);
		}
	}

	protected override void Start()
	{
		base.Start();
		if (Application.isPlaying)
		{
			formatText = text;
			timeManager = SingletonComponent<Meta, MetaTimeManager>.Instance;
		}
	}

	protected virtual void Update()
	{
		if (Application.isPlaying && countdown)
		{
			dirty = true;
		}
		if (dirty)
		{
			UpdateText();
		}
	}

	protected void UpdateText()
	{
		dirty = false;
		TimeSpan? currentTime = this.currentTime;
		string text = (!currentTime.HasValue) ? null : FormatTimeSpan(currentTime.Value);
		if (text == null)
		{
			this.text = null;
		}
		else
		{
			this.text = string.Format(formatText, text);
		}
	}

	protected string FormatTimeSpan(TimeSpan ts)
	{
		int[] array = new int[4]
		{
			ts.Days,
			ts.Hours,
			ts.Minutes,
			ts.Seconds
		};
		string[] array2 = new string[4]
		{
			daySymbol,
			hourSymbol,
			minuteSymbol,
			secondSymbol
		};
		int num = Array.FindIndex(array, (int v) => v > 0);
		if (num < 0)
		{
			return defaultValue;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(array[num].ToString((!leadingZero) ? "d" : "d2"));
		stringBuilder.Append(array2[num]);
		for (int i = num + 1; i < array.Length && i < num + maxParts; i++)
		{
			if (!skipZeroValues || array[i] > 0)
			{
				stringBuilder.Append(separator);
				stringBuilder.Append(array[i].ToString((!middleZero) ? "d" : "d2"));
				stringBuilder.Append(array2[i]);
			}
		}
		return stringBuilder.ToString();
	}
}
