using UnityEngine;

public class M3FaceStageCamera : MonoBehaviour
{
	public bool lookAtCamera;

	private M3StageCamera stageCamera;

	private void Start()
	{
		stageCamera = UnityEngine.Object.FindObjectOfType<M3StageCamera>();
	}

	private void Update()
	{
		if ((bool)stageCamera)
		{
			if (lookAtCamera)
			{
				Vector3 position = stageCamera.transform.position;
				float x = position.x;
				Vector3 position2 = base.transform.position;
				float x2 = x - position2.x;
				Vector3 position3 = stageCamera.transform.position;
				float z = position3.z;
				Vector3 position4 = base.transform.position;
				Vector3 forward = new Vector3(x2, 0f, z - position4.z);
				base.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
			}
			else
			{
				base.transform.rotation = stageCamera.transform.rotation;
			}
		}
	}
}
