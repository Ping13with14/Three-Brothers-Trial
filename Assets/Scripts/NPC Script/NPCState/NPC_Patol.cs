using System.Collections;
using UnityEngine;

/// <summary>
/// NPC巡逻脚本：按预设路径点循环移动
/// </summary>
public class NPC_Patol : MonoBehaviour
{
    public Vector2[] patrolPoints;
    public float speed = 2;
    // 到达每个巡逻点后的停顿时间
    public float pauseDuration = 1.5f;

    private bool isPaused;
    private int currentPatrolIndex;
    private Vector2 target;

    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        StartCoroutine(SetPatrolPoint());
    }

    void Update()
    {
        if(isPaused)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 direction = (target - (Vector2)transform.position).normalized;
        // 根据移动方向翻转朝向
        if (direction.x < 0 && transform.localScale.x > 0 || direction.x > 0 && transform.localScale.x < 0)
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

        rb.velocity = direction * speed;

        // 到达目标点后切换到下一个巡逻点
        if(Vector2.Distance(transform.position, target) < 0.1f)
        {
            StartCoroutine(SetPatrolPoint());
        }
    }

    // 在当前巡逻点停顿后切换到下一个
    IEnumerator SetPatrolPoint()
    {
        isPaused = true;
        anim.Play("Idle");
        yield return new WaitForSeconds(pauseDuration);

        // 先取当前索引的目标点，再推进索引（确保首次使用index 0）
        target = patrolPoints[currentPatrolIndex];
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        anim.Play("Walk");
        isPaused = false;
    }
}
