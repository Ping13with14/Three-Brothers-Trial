using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Transform attackPoint;
    public LayerMask enemyLayer;


    public Animator anim;

    public float cooldown = 1;
    private float timer;



    private void Update()
    {
        if(timer>0)
        {
            timer-= Time.deltaTime;
        }
    }
    public void Attack()
    {
        if (timer <= 0)
        {

            anim.SetBool("isAttacking", true);
            timer = cooldown;
        }
    }

    public void DealDamage()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, StatsManager.Instance.weaponRange, enemyLayer);
        if (enemies.Length > 0)
        {
            var damageable = enemies[0].GetComponent<IDamageable>();
            if (damageable != null)
                damageable.ChangeHealth(-StatsManager.Instance.damage);

            var knockbackable = enemies[0].GetComponent<IKnockbackable>();
            if (knockbackable != null)
                knockbackable.Knockback(transform, StatsManager.Instance.knockbackForce, StatsManager.Instance.knockbackTime, StatsManager.Instance.stunTime);
        }
    }


    public void FinishAttacking()
    {
        anim.SetBool("isAttacking", false);
    }

    private void OnDrawGizmosSelected()
    {
        // 编辑器模式下字段或单例可能尚未初始化，需要空检查
        if (attackPoint == null || StatsManager.Instance == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, StatsManager.Instance.weaponRange);
    }
}
