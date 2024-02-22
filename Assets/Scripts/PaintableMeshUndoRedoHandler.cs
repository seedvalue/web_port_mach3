using UnityEngine;

public class PaintableMeshUndoRedoHandler : MonoBehaviour
{
	[SerializeField]
	private int _counter;

	private int _nonSerializableCounter;

	private PaintableMesh _target;
}
