using UnityEngine;
using UnityEngine.Pool;

public class PoolObject : MonoBehaviour
{
    public IObjectPool<GameObject> Pool { get; set; }

    public void TakeObjectFromPool()
    {
        Pool.Get();
    }

    public void ReleaseObject()
    {
        Pool.Release(gameObject);
    }
}