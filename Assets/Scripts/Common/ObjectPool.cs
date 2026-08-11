using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 泛型 MonoBehaviour 对象池：减少频繁 Instantiate/Destroy 带来的 GC 压力
/// </summary>
public class ObjectPool<T> where T : MonoBehaviour
{
    private readonly Queue<T> availableObjects = new();  // 空闲对象队列

    private T prefab;                                    // 对象预制体
    private Transform parent;                            // 池中对象的父级 Transform

    /// <summary>
    /// 初始化对象池：预创建指定数量的实例并设为非活跃
    /// </summary>
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

    /// <summary>
    /// 从池中获取一个可用对象（池空时动态创建），设为活跃并返回
    /// </summary>
    public T Get()
    {
        T obj = availableObjects.Count > 0
            ? availableObjects.Dequeue()
            : Object.Instantiate(prefab, parent);

        obj.gameObject.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 将对象归还到池中，设为非活跃
    /// </summary>
    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        availableObjects.Enqueue(obj);
    }
}
