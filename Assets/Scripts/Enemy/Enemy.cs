using UnityEngine;

/// <summary>
/// 敌人根组件：挂在敌人预制体根部，管理数据初始化、池引用
/// </summary>
public class Enemy : MonoBehaviour
{
    public EnemySO data;                   // 当前使用的数据预设
    public static EnemyPool Pool { get; set; }  // 对象池引用（由 Spawner 设置）

    // 组件引用缓存
    private Enemy_Movemont movement;
    private Enemy_Health health;
    private Enemy_Combat combat;

    private void Awake()
    {
        movement = GetComponent<Enemy_Movemont>();
        health = GetComponent<Enemy_Health>();
        combat = GetComponent<Enemy_Combat>();
    }

    /// <summary>
    /// 由池取出或 Spawner 生成时调用，将 EnemySO 数据同步到各子组件并初始化状态
    /// </summary>
    public void Initialize(EnemySO enemyData)
    {
        data = enemyData;

        // 同步属性到各子组件
        if (health != null)
        {
            health.maxHealth = enemyData.maxHealth;
            health.expReward = enemyData.expReward;
            health.currentHealth = enemyData.maxHealth;
        }

        if (combat != null)
        {
            combat.damage = enemyData.damage;
            combat.weaponRange = enemyData.attackRange;
            combat.knockBackForce = enemyData.knockBackForce;
            combat.stunTime = enemyData.stunTime;
        }

        if (movement != null)
        {
            movement.speed = enemyData.speed;
            movement.attackRange = enemyData.attackRange;
            movement.attackCooldown = enemyData.attackCooldown;
            movement.playerDetectRange = enemyData.detectRange;
            movement.canPatrol = enemyData.canPatrol;
            movement.patrolRadius = enemyData.patrolRadius;
            movement.patrolPauseTime = enemyData.patrolPauseTime;
            movement.Initialize();
        }
    }
}
