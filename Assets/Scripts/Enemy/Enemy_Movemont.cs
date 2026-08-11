using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人移动与状态机：根据玩家距离切换 Idle/Chasing/Attacking 状态
/// </summary>
public class Enemy_Movemont : MonoBehaviour
{
    [Header("移动与战斗参数")]
    public int speed;                     // 移动速度
    public float attackRange =1f;         // 攻击判定距离
    public float attackCooldown = 2;      // 攻击冷却时间（秒）
    public float playerDetectRange = 5;   // 玩家检测范围
    public Transform detectionPoint;      // 检测范围圆心位置
    public LayerMask playerLayer;         // 玩家所在图层

    private float attackCooldownTimer;    // 攻击冷却计时器
    private int facingDirection=1;        // 朝向：1=右，-1=左
    private EnemyState enemyState;        // 当前状态

    // 组件引用
    private Rigidbody2D rb;
    private Transform player;             // 检测到的玩家 Transform
    private Animator anim;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim=GetComponent<Animator>();
        ChangeState(EnemyState.Idle);
    }

    void Update()
    {
        if (enemyState != EnemyState.Knockback)
        {
            CheckForPlayer();

            if (attackCooldownTimer > 0)
            {
                attackCooldownTimer -= Time.deltaTime;
            }

            if (enemyState == EnemyState.Chasing)
            {
                Chase();
            }
            else if (enemyState == EnemyState.Attacking)
            {
                // 攻击状态：原地不动，攻击动作由动画事件 Enemy_Combat.Attack() 触发伤害
                rb.velocity = Vector2.zero;

            }
        }
    }

    /// <summary>
    /// 追逐玩家：面向玩家方向移动
    /// </summary>
    void Chase()
    {
        if (player.position.x > transform.position.x && facingDirection == -1 ||
              player.position.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * speed;
    }

    /// <summary>
    /// 翻转朝向
    /// </summary>
    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    /// <summary>
    /// 检测玩家：范围内发现玩家则切换到追逐或攻击状态，丢失玩家则回到 Idle
    /// </summary>
    private void CheckForPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPoint.position,playerDetectRange,playerLayer);

        if (hits.Length > 0)
        {
            player = hits[0].transform;

            if (Vector2.Distance(transform.position, player.position) <= attackRange && attackCooldownTimer <= 0)
            {
                attackCooldownTimer = attackCooldown;
                ChangeState(EnemyState.Attacking);
            }
            else if (Vector2.Distance(transform.position, player.position) > attackRange && enemyState!=EnemyState.Attacking)
            {
                ChangeState(EnemyState.Chasing);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            ChangeState(EnemyState.Idle);
        }
    }

    /// <summary>
    /// 切换状态：先关闭旧状态的动画参数，再开启新状态的动画参数
    /// </summary>
    public void ChangeState(EnemyState newState)
    {
        // 退出当前状态的动画
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle",false);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", false);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", false);

        // 切换至新状态
        enemyState = newState;

        // 开启新状态的动画
        if (enemyState == EnemyState.Idle)
            anim.SetBool("isIdle", true);
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", true);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", true);
    }

    /// <summary>
    /// 编辑器可视化：绘制玩家检测范围
    /// </summary>
     private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);
    }
}

/// <summary>
/// 敌人行为状态枚举
/// </summary>
public enum EnemyState
{
    Idle,       // 待机
    Chasing,    // 追逐玩家
    Attacking,  // 攻击中
    Knockback   // 被击退/眩晕
}
