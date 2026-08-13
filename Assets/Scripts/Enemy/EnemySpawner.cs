using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人生成器：管理敌人的生成、回收和重生
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    /// <summary>
    /// 生成模式
    /// </summary>
    public enum SpawnMode
    {
        AlwaysActive,      // 场景加载时立即生成，死亡后重生
        QuestActivated     // 接取关联任务后生成，任务完成回收
    }

    [Header("敌人配置")]
    public EnemySO enemyData;                // 敌人类型
    public int spawnCount = 3;               // 同时存在的最大数量
    public Vector2 spawnArea = Vector2.one;  // 生成区域（以 Spawner 位置为中心）

    [Header("激活策略")]
    public SpawnMode mode = SpawnMode.AlwaysActive;
    public QuestSO requiredQuest;            // 任务激活模式下关联的任务（QuestActivated模式必填）
    public float spawnInterval = 0.5f;       // 生成间隔（秒），避免同帧生成过多

    [Header("重生")]
    public float respawnDelay = 10f;         // 死亡后延迟重生时间（秒），0=不重生

    private EnemyPool pool;                  // 对象池引用
    private List<Enemy> activeEnemies = new(); // 当前活跃的敌人列表
    private bool isSpawning;                 // 是否正在生成中

    private void Awake()
    {
        pool = new EnemyPool(transform);
        Enemy.Pool = pool;                   // 设置静态池引用，供 Enemy_Health 死亡时归还
        pool.Prewarm(enemyData, spawnCount);
    }

    private void Start()
    {
        if (mode == SpawnMode.AlwaysActive)
        {
            StartCoroutine(SpawnAll());
        }
        else if (mode == SpawnMode.QuestActivated)
        {
            // 监听任务接取和完成事件
            GameEvents.OnQuestAccepted += OnQuestAccepted;
            GameEvents.OnQuestTurnInRequested += OnQuestCompleted;
            // 如果任务已经接取，立即生成
            if (QuestManagerExists() && IsQuestAlreadyAccepted())
            {
                StartCoroutine(SpawnAll());
            }
        }

        // 监听敌人死亡事件以处理重生
        GameEvents.OnMonsterDefeated += OnEnemyDefeated;
    }

    private void OnDestroy()
    {
        GameEvents.OnQuestAccepted -= OnQuestAccepted;
        GameEvents.OnQuestTurnInRequested -= OnQuestCompleted;
        GameEvents.OnMonsterDefeated -= OnEnemyDefeated;
    }

    /// <summary>
    /// 分批生成所有敌人
    /// </summary>
    private IEnumerator SpawnAll()
    {
        isSpawning = true;
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnOne();
            yield return new WaitForSeconds(spawnInterval);
        }
        isSpawning = false;
    }

    /// <summary>
    /// 生成一个敌人在随机位置
    /// </summary>
    private Enemy SpawnOne()
    {
        Enemy enemy = pool.Get(enemyData);
        enemy.transform.position = GetRandomSpawnPosition();
        enemy.transform.SetParent(null); // 从池父节点移出，放入场景根
        activeEnemies.Add(enemy);
        return enemy;
    }

    /// <summary>
    /// 在矩形区域内随机取一个生成位置
    /// </summary>
    private Vector2 GetRandomSpawnPosition()
    {
        float x = Random.Range(-spawnArea.x / 2, spawnArea.x / 2);
        float y = Random.Range(-spawnArea.y / 2, spawnArea.y / 2);
        return (Vector2)transform.position + new Vector2(x, y);
    }

    /// <summary>
    /// 敌人被击杀时：从活跃列表移除，延迟后重生
    /// </summary>
    private void OnEnemyDefeated(int exp)
    {
        // 传入的 exp 无法直接识别具体敌人实例，改为每帧检查活跃列表中的 null 对象
    }

    private void Update()
    {
        // 清理活跃列表中已被回收的敌人，并处理重生
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            var enemy = activeEnemies[i];
            if (enemy == null || enemy.gameObject == null || !enemy.gameObject.activeSelf)
            {
                activeEnemies.RemoveAt(i);

                // AlwaysActive 模式下，非生成期间补一个
                if (mode == SpawnMode.AlwaysActive && respawnDelay > 0 && !isSpawning)
                    StartCoroutine(RespawnAfterDelay());
            }
        }
    }

    /// <summary>
    /// 延迟后补充一个敌人
    /// </summary>
    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        if (activeEnemies.Count < spawnCount)
            SpawnOne();
    }

    /// <summary>
    /// 任务被接取时：生成这批敌人
    /// </summary>
    private void OnQuestAccepted(QuestSO quest)
    {
        if (mode == SpawnMode.QuestActivated && requiredQuest == quest)
        {
            if (activeEnemies.Count == 0)
                StartCoroutine(SpawnAll());
        }
    }

    /// <summary>
    /// 任务被提交时：回收所有活跃敌人
    /// </summary>
    private void OnQuestCompleted(QuestSO quest)
    {
        if (mode == SpawnMode.QuestActivated && requiredQuest == quest)
        {
            ReturnAllToPool();
        }
    }

    /// <summary>
    /// 回收所有活跃敌人到池中
    /// </summary>
    private void ReturnAllToPool()
    {
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            var enemy = activeEnemies[i];
            if (enemy != null)
                pool.Return(enemy);
        }
        activeEnemies.Clear();
    }

    private bool QuestManagerExists()
    {
        return GameManager.Instance != null && GameManager.Instance.QuestManager != null;
    }

    private bool IsQuestAlreadyAccepted()
    {
        return GameManager.Instance.QuestManager.IsQuestAccepted(requiredQuest);
    }

    /// <summary>
    /// 编辑器可视化：绘制生成区域
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = mode == SpawnMode.AlwaysActive ? Color.red : Color.yellow;
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }
}
