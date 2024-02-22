using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PCResolutionSetter : MonoBehaviour
{
	private class ExceptionInfo
	{
		public float time;

		public string condition;

		public string trace;

		public ExceptionInfo(float t, string cond, string trac)
		{
			time = t;
			condition = cond;
			trace = trac;
		}
	}

	public Text text;

	private bool hasExceptions;

	private List<ExceptionInfo> exceptions = new List<ExceptionInfo>();

	private void Awake()
	{
		Application.logMessageReceivedThreaded += delegate(string cnd, string trace, LogType type)
		{
			if (type == LogType.Exception)
			{
				lock (exceptions)
				{
					exceptions.Add(new ExceptionInfo(Time.time, cnd, trace));
				}
				hasExceptions = true;
			}
		};
	}

	private void Update()
	{
		if (hasExceptions)
		{
			lock (exceptions)
			{
				hasExceptions = false;
				text.text = "error{0}\n{1}" + exceptions[0].condition + ", " + exceptions[0].trace;
				exceptions.Clear();
			}
		}
	}
}
