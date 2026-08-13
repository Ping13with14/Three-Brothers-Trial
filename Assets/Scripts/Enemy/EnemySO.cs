using UnityEngine;

/// <summary>
/// 敌人数据预设：ScriptableObject，统一管理敌人属性，Spawner/Enemy组件从此读取
/// </summary>
[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemy/EnemySO")]
public class EnemySO : ScriptableObject
{
    [Header("基本信息")]
    public string enemyName;               // 敌人名称
    public GameObject prefab;              // 敌人预制体（需挂载 Enemy 根组件）

    [Header("战斗属性")]
    public int maxHealth = 30;             // 最大生命值
    public int damage = 10;                // 攻击伤害
    public float attackRange = 1.5f;       // 攻击判定范围半径
    public float attackCooldown = 2f;      // 攻击冷却时间（秒）
    public float knockBackForce = 5f;      // 对玩家的击退力度
    public float stunTime = 0.3f;          // 对玩家的眩晕时间（秒）

    [Header("移动属性")]
    public int speed = 3;                  // 移动速度
    public float detectRange = 5f;         // 玩家检测范围

    [Header("奖励")]
    public int expReward = 3;              // 击败经验奖励

    [Header("巡逻配置")]
    public bool canPatrol = false;         // 是否具备巡逻能力
    public float patrolRadius = 3f;        // 巡逻范围半径（从出生点算起）
    public float patrolPauseTime = 1.5f;   // 巡逻途中停顿时间（秒）
}
