using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 泛型 MonoBehaviour 对象池，减少频繁 Instantiate/Destroy 带来的 GC 压力
/// </summary>
public class ObjectPool<T> where T : MonoBehaviour
{
    private readonly Queue<T> availableObjects = new();
    private T prefab;
    private Transform parent;

    public void Initialize(T prefab, int initialSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            T obj = Object.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            availableObjects.Enqueue(obj);
        }
    }

    public T Get()
    {
        T obj = availableObjects.Count > 0
            ? availableObjects.Dequeue()
            : Object.Instantiate(prefab, parent);

        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        availableObjects.Enqueue(obj);
    }
}
