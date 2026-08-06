using System.Collections;
using UnityEngine;

/// <summary>
/// NPC漫游脚本：在矩形范围内随机选择边缘点移动
/// </summary>
public class NPC_Wander : MonoBehaviour
{
    [Header("漫游范围（范围不宜过大，建议5以内）")]
    public float wanderWidth = 5;
    public float wanderHeight = 5;
    // 漫游区域的中心点
    public Vector2 startingPoint;

    // 到达目标后的停顿时间
    public float pauseDuration = 1;
    public float speed = 2;
    public Vector2 target;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isPaused;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        StartCoroutine(PauseAndPickNewDestination());
    }

    private void Update()
    {
        if (isPaused)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        if (Vector2.Distance(transform.position, target) < .1f)
            StartCoroutine(PauseAndPickNewDestination());

        Move();
    }

    private void Move()
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        // 根据移动方向翻转朝向
        if(direction.x > 0 && transform.localScale.x < 0 || direction.x < 0 && transform.localScale.x > 0)
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

        rb.velocity = direction * speed;
    }

    // 停顿后随机选取矩形边界上的新目标点
    IEnumerator PauseAndPickNewDestination()
    {
        isPaused = true;
        anim.Play("Idle");
        yield return new WaitForSeconds(pauseDuration);

        target = GetRandomTarget();
        isPaused = false;
        anim.Play("Walk");
    }

    // 碰撞到障碍物时重新选择目标
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(!enabled) return;
        StartCoroutine(PauseAndPickNewDestination());
    }

    // 在矩形区域的四条边上随机选一个点
    private Vector2 GetRandomTarget()
    {
        float halfWidth = wanderWidth / 2;
        float halfHeight = wanderHeight / 2;
        int edge = Random.Range(0, 4);

        return edge switch
        {
            0 => new Vector2(startingPoint.x - halfWidth,  Random.Range(startingPoint.y - halfHeight, startingPoint.y + halfHeight)),   //左边界
            1 => new Vector2(startingPoint.x + halfWidth,  Random.Range(startingPoint.y - halfHeight, startingPoint.y + halfHeight)),   //右边界
            2 => new Vector2(Random.Range(startingPoint.x - halfWidth, startingPoint.x + halfWidth), startingPoint.y - halfHeight),    //下边界
            _ => new Vector2(Random.Range(startingPoint.x - halfWidth, startingPoint.x + halfWidth), startingPoint.y + halfHeight),    //上边界
        };
    }

    // 编辑器中可视化漫游范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(startingPoint, new Vector3(wanderWidth, wanderHeight, 0));
    }
}
