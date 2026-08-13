using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家近战战斗：检测攻击输入，由动画事件在关键帧触发伤害判定
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;       // 攻击判定点位置
    public LayerMask enemyLayer;        // 敌人所在图层

    public Animator anim;               // 玩家动画控制器

    public float cooldown = 1;          // 攻击冷却时间（秒）
    private float timer;                // 当前冷却计时器


    private void Update()
    {
        if(timer>0)
        {
            timer-= Time.deltaTime;
        }
    }

    /// <summary>
    /// 发起攻击：由 PlayerMovement.Update() 检测攻击按键后调用，进入攻击动画并进入冷却
    /// </summary>
    public void Attack()
    {
        if (timer <= 0)
        {
            // 播放攻击音效
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("攻击_LaserShoot");

            anim.SetBool("isAttacking", true);
            timer = cooldown;
        }
    }

    /// <summary>
    /// 伤害判定：由动画事件（Animation Event）在攻击动画的挥砍关键帧回调，对范围内敌人造成伤害、击退并播放受击音效
    /// </summary>
    public void DealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, StatsManager.Instance.weaponRange, enemyLayer);
        if (enemies.Length > 0)
        {
            var damageable = enemies[0].GetComponent<IDamageable>();
            if (damageable != null)
                damageable.ChangeHealth(-StatsManager.Instance.damage);

            var knockbackable = enemies[0].GetComponent<IKnockbackable>();
            // 敌人死亡后会被回收/销毁并设为非活跃，此时再启动击退协程会报错，故仅对存活敌人击退
            if (knockbackable != null && damageable != null && damageable.CurrentHealth > 0)
                knockbackable.Knockback(transform, StatsManager.Instance.knockbackForce, StatsManager.Instance.knockbackTime, StatsManager.Instance.stunTime);

            // 播放受击音效
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("受击_HitHurt");
        }
    }

    /// <summary>
    /// 结束攻击：由动画事件（Animation Event）在攻击动画末尾回调，复位 isAttacking 状态
    /// </summary>
    public void FinishAttacking()
    {
        anim.SetBool("isAttacking", false);
    }

    /// <summary>
    /// 编辑器可视化：绘制攻击判定范围
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // 编辑器模式下字段或单例可能尚未初始化，需要空检查
        if (attackPoint == null || StatsManager.Instance == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, StatsManager.Instance.weaponRange);
    }
}
