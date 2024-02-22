using UnityEngine;

public class SequenceInvoker : MonoBehaviour
{
	public void Play()
	{
		SequenceManager.Play();
	}

	public void Skip()
	{
		SequenceManager.Skip();
	}

	private void Start()
	{
		FullScreenCanvas component = GetComponent<FullScreenCanvas>();
		if ((bool)component)
		{
			component.onGainFocus.AddListener(Play);
		}
	}
}
