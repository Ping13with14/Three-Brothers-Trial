using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人对象池：接收被击杀的敌人并回收复用，按 EnemySO 分组预创建
/// </summary>
public class EnemyPool
{
    private readonly Dictionary<EnemySO, Queue<Enemy>> pools = new();
    private Transform poolRoot;                              // 池中对象的父节点

    public EnemyPool(Transform root = null)
    {
        poolRoot = root;
    }

    /// <summary>
    /// 预创建指定数量的敌人实例（非活跃）
    /// </summary>
    public void Prewarm(EnemySO data, int count)
    {
        if (!pools.TryGetValue(data, out var queue))
        {
            queue = new Queue<Enemy>();
            pools[data] = queue;
        }

        for (int i = 0; i < count; i++)
        {
            var instance = CreateInstance(data);
            queue.Enqueue(instance);
        }
    }

    /// <summary>
    /// 从池中取一个敌人（池空时动态创建），初始化属性并返回
    /// </summary>
    public Enemy Get(EnemySO data)
    {
        if (!pools.TryGetValue(data, out var queue))
        {
            queue = new Queue<Enemy>();
            pools[data] = queue;
        }

        Enemy enemy = null;

        // 从队列取出可用对象，跳过已被意外销毁的
        while (queue.Count > 0)
        {
            var dequeued = queue.Dequeue();
            if (dequeued != null && dequeued.gameObject != null)
            {
                enemy = dequeued;
                break;
            }
        }

        if (enemy == null)
            enemy = CreateInstance(data);

        enemy.gameObject.SetActive(true);
        enemy.Initialize(data);
        return enemy;
    }

    /// <summary>
    /// 归还敌人到池中（停用）
    /// </summary>
    public void Return(Enemy enemy)
    {
        if (enemy == null || enemy.gameObject == null)
            return;

        enemy.gameObject.SetActive(false);

        if (enemy.data == null)
        {
            Object.Destroy(enemy.gameObject);
            return;
        }

        if (!pools.TryGetValue(enemy.data, out var queue))
        {
            queue = new Queue<Enemy>();
            pools[enemy.data] = queue;
        }
        queue.Enqueue(enemy);
    }

    private Enemy CreateInstance(EnemySO data)
    {
        GameObject go = Object.Instantiate(data.prefab, poolRoot);
        go.name = data.enemyName;

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy == null)
            enemy = go.AddComponent<Enemy>();

        go.SetActive(false);
        return enemy;
    }
}
