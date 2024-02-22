using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
	public Text labelFPS;

	public Text labelFPSAvg;

	public int avgSamples = 30;

	private List<float> framesCumulated = new List<float>();

	private int FrameCount => framesCumulated.Count;

	private void Start()
	{
	}

	private void Update()
	{
		framesCumulated.Add(Time.time);
		if (FrameCount > avgSamples)
		{
			framesCumulated.RemoveAt(0);
		}
		ShowOnLabels();
	}

	private void ShowOnLabels()
	{
		if (FrameCount > 1)
		{
			float num = framesCumulated[FrameCount - 1] - framesCumulated[FrameCount - 2];
			if ((bool)labelFPS && num > float.Epsilon)
			{
				labelFPS.text = "FPS: " + (1f / num).ToString("0.00");
			}
		}
		if ((bool)labelFPSAvg && FrameCount > 1)
		{
			float num2 = framesCumulated[FrameCount - 1] - framesCumulated[0];
			labelFPSAvg.text = "FPS (avg): " + ((float)(FrameCount - 1) / num2).ToString("0.00");
		}
	}
}
