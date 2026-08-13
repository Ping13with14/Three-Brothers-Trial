using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人移动与状态机：根据玩家距离切换 Idle/Patrol/Chasing/Attacking/Knockback 状态
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

    [Header("巡逻参数")]
    public bool canPatrol = false;        // 是否具备巡逻能力
    public float patrolRadius = 3f;       // 巡逻范围半径（从出生点算起）
    public float patrolPauseTime = 1.5f;  // 到达巡逻点后停顿时间（秒）

    private float attackCooldownTimer;    // 攻击冷却计时器
    private int facingDirection=1;        // 朝向：1=右，-1=左
    private EnemyState enemyState;        // 当前状态

    // 巡逻
    private Vector2 patrolOrigin;         // 巡逻原点（出生位置）
    private Vector2 patrolTarget;         // 当前巡逻目标点
    private bool isPatrolPaused;          // 巡逻是否在停顿中
    private Coroutine patrolPauseCoroutine;

    // 组件引用
    private Rigidbody2D rb;
    private Transform player;             // 检测到的玩家 Transform
    private Animator anim;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim=GetComponent<Animator>();
        patrolOrigin = transform.position;
        ChangeState(canPatrol ? EnemyState.Patrol : EnemyState.Idle);
    }

    /// <summary>
    /// 由 Enemy.Initialize() 调用：设置巡逻原点并切换到初始状态
    /// </summary>
    public void Initialize()
    {
        patrolOrigin = transform.position;
        ChangeState(canPatrol ? EnemyState.Patrol : EnemyState.Idle);
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
            else if (enemyState == EnemyState.Patrol && !isPatrolPaused)
            {
                PatrolBehavior();
            }
            else if (enemyState == EnemyState.Attacking)
            {
                // 攻击状态：原地不动，攻击动作由动画事件 Enemy_Combat.Attack() 触发伤害
                rb.velocity = Vector2.zero;
            }
        }
    }

    /// <summary>
    /// 巡逻行为：向目标点移动，到达后停顿再选新目标
    /// </summary>
    private void PatrolBehavior()
    {
        if (Vector2.Distance(transform.position, patrolTarget) < 0.1f)
        {
            if (patrolPauseCoroutine == null)
                patrolPauseCoroutine = StartCoroutine(PatrolPause());
            return;
        }

        // 面向目标方向
        if (patrolTarget.x > transform.position.x && facingDirection == -1 ||
            patrolTarget.x < transform.position.x && facingDirection == 1)
        {
            Flip();
        }

        Vector2 direction = (patrolTarget - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;
    }

    /// <summary>
    /// 巡逻停顿：到达目标点后停顿并选取下一个随机目标点
    /// </summary>
    private IEnumerator PatrolPause()
    {
        isPatrolPaused = true;
        rb.velocity = Vector2.zero;
        anim.SetBool("isIdle", true);
        yield return new WaitForSeconds(patrolPauseTime);
        anim.SetBool("isIdle", false);
        PickNewPatrolTarget();
        isPatrolPaused = false;
        patrolPauseCoroutine = null;
    }

    /// <summary>
    /// 在巡逻半径内随机选一个目标点
    /// </summary>
    private void PickNewPatrolTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * patrolRadius;
        patrolTarget = patrolOrigin + randomOffset;
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
    /// 检测玩家：范围内发现玩家则切换到追逐或攻击，丢失玩家则回到巡逻或待机
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
            // 丢失玩家后：有巡逻能力的回到 Patrol，否则回 Idle
            if (enemyState == EnemyState.Chasing || enemyState == EnemyState.Attacking)
            {
                rb.velocity = Vector2.zero;
                ChangeState(canPatrol ? EnemyState.Patrol : EnemyState.Idle);
            }
        }
    }

    /// <summary>
    /// 切换状态：先关闭旧状态的动画参数，再开启新状态的动画参数
    /// </summary>
    public void ChangeState(EnemyState newState)
    {
        if (enemyState == newState)
            return;

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
        else if (enemyState == EnemyState.Patrol)
        {
            isPatrolPaused = false;
            PickNewPatrolTarget();
        }
        else if (enemyState == EnemyState.Chasing)
            anim.SetBool("isChasing", true);
        else if (enemyState == EnemyState.Attacking)
            anim.SetBool("isAttacking", true);
    }

    /// <summary>
    /// 编辑器可视化：绘制玩家检测范围和巡逻范围
    /// </summary>
     private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, playerDetectRange);

        if (canPatrol)
        {
            Gizmos.color = Color.green;
            Vector2 origin = Application.isPlaying ? patrolOrigin : (Vector2)transform.position;
            Gizmos.DrawWireCube(origin, new Vector2(patrolRadius * 2, patrolRadius * 2));
        }
    }
}

/// <summary>
/// 敌人行为状态枚举
/// </summary>
public enum EnemyState
{
    Idle,       // 待机
    Patrol,     // 巡逻（在出生点附近游荡）
    Chasing,    // 追逐玩家
    Attacking,  // 攻击中
    Knockback   // 被击退/眩晕
}
