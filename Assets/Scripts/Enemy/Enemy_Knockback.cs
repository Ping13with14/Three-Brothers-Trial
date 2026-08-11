using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人击退：实现 IKnockbackable，由玩家攻击/箭矢命中时调用
/// </summary>
public class Enemy_Knockback : MonoBehaviour, IKnockbackable
{
    private Rigidbody2D rb;                        // 敌人刚体
    private Enemy_Movemont enemy_Movemont;         // 敌人状态机引用

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy_Movemont = GetComponent<Enemy_Movemont>();
    }

    /// <summary>
    /// 击退处理：由玩家攻击/箭矢命中时调用，切换到击退状态，先击退、再硬直、最后恢复 Idle
    /// </summary>
    public void Knockback(Transform forceTransform,float knockbackForce,float knockbackTime,float stunTime)
    {
        enemy_Movemont.ChangeState(EnemyState.Knockback);
        StartCoroutine(StunTimer(knockbackTime,stunTime));

        Vector2 direction = (transform.position - forceTransform.position).normalized;
        rb.velocity = direction * knockbackForce;
    }

    /// <summary>
    /// 击退计时协程：等待击退时间→停止移动→等待硬直时间→恢复 Idle
    /// </summary>
    IEnumerator StunTimer(float knockbackTime, float stunTime)
    {
        yield return new WaitForSeconds(knockbackTime);
        rb.velocity=Vector2.zero;
        yield return new WaitForSeconds(stunTime);
        enemy_Movemont.ChangeState(EnemyState.Idle);
    }


}
