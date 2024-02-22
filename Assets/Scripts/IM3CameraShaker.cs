using UnityEngine;

public interface IM3CameraShaker
{
	void UpdateState(float deltaTime, ref Vector3 outPos, ref Quaternion outRot);

	bool IsIdle();

	int GetPriority();
}
