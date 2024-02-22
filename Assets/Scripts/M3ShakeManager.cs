using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class M3ShakeManager : MonoBehaviour
{
	public static int priorHigh = 100;

	public static int priorMedium = 50;

	public static int priorLow = 10;

	public static int priorZero;

	private List<IM3CameraShaker> shakers = new List<IM3CameraShaker>();

	public void UpdateState(ref Vector3 camPos, ref Quaternion camRot, float deltaTime)
	{
		int num = 0;
		int num2 = (shakers.Count > 0) ? shakers[shakers.Count - 1].GetPriority() : 0;
		for (int num3 = shakers.Count - 1; num3 >= 0; num3--)
		{
			if (shakers[num3].IsIdle())
			{
				shakers.RemoveAt(num3);
				num++;
			}
			else
			{
				Vector3 outPos = Vector3.zero;
				Quaternion outRot = Quaternion.identity;
				if (shakers[num3].GetPriority() == num2)
				{
					shakers[num3].UpdateState(Time.deltaTime, ref outPos, ref outRot);
				}
				else
				{
					shakers[num3].UpdateState(0f, ref outPos, ref outRot);
				}
				camPos += outPos;
				camRot *= outRot;
			}
		}
	}

	public void StartShake(IM3CameraShaker shake)
	{
		shakers.Add(shake);
		shakers = (from sh in shakers
			orderby sh.GetPriority()
			select sh).ToList();
	}
}
