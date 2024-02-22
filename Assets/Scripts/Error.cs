using UnityEngine;
using UnityEngine.UI;

public class Error : MonoBehaviour
{
	public Text leaveThisEmpty;

	public void CauseError()
	{
		leaveThisEmpty.text = "Nie da się zmienić tekstu, bo null";
	}
}
