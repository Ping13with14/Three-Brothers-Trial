using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人战斗：由动画事件在攻击帧回调 Attack() 对玩家造成伤害和击退
/// </summary>
public class Enemy_Combat : MonoBehaviour
{
    public int damage = 1;                 // 攻击伤害值
    public Transform attackPoint;          // 攻击判定点位置
    public float weaponRange;              // 攻击判定范围半径
    public float knockBackForce;           // 击退力度
    public float stunTime;                 // 眩晕/硬直时间
    public LayerMask playerLayer;          // 玩家所在图层

    /// <summary>
    /// 攻击判定：由动画事件（Animation Event）在攻击动画的关键帧回调，检测玩家并造成伤害与击退
    /// </summary>
    public void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position,weaponRange,playerLayer);

        if(hits.Length > 0)
        {
            var damageable = hits[0].GetComponent<IDamageable>();
            damageable.ChangeHealth(-damage);

            // 玩家死亡后会被禁用，此时再启动击退协程会报错，故仅对存活玩家击退
            if (damageable.CurrentHealth > 0)
                hits[0].GetComponent<PlayerMovement>().Knockback(transform, knockBackForce, stunTime);
        }
    }
}
