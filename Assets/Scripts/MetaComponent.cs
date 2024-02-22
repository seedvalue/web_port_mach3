using UnityEngine;
using Utils;

public abstract class MetaComponent<T> : SingletonComponent<Meta, T>, IMetaComponent where T : MonoBehaviour
{
	public string metaID => GetType().Name;
}
