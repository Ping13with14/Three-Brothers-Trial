using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家移动：处理移动输入、朝向翻转、攻击按键检测、击退状态
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public int facingDirecation = 1;           // 当前朝向：1=右，-1=左

    public Rigidbody2D rb;                     // 玩家刚体
    public Animator anim;                      // 玩家动画控制器

    private bool isKnockedBack;                // 是否处于击退状态（击退期间锁定移动）
    public bool isShooting;                    // 是否处于射击状态（射击期间锁定移动）

    public PlayerCombat playerCombat;          // 近战战斗组件引用

    /// <summary>
    /// 每帧检测攻击按键：如果近战组件启用，则触发攻击
    /// </summary>
    private void Update()
    {
        if(InputManager.Provider.IsAttackPressed && playerCombat.enabled == true)
        {
            playerCombat.Attack();
        }
    }

    /// <summary>
    /// 固定帧率更新：处理移动、朝向翻转（射击/击退期间锁定移动）
    /// </summary>
    void FixedUpdate()
    {
        if(isShooting == true)
        {
            rb.velocity = Vector2.zero;
        }
        else if (isKnockedBack == false)
        {
            float horizontal = InputManager.Provider.Horizontal;
            float vertical = InputManager.Provider.Vertical;

            if (horizontal > 0 && transform.localScale.x < 0 ||
                horizontal < 0 && transform.localScale.x > 0)
            {
                Flip();
            }

            anim.SetFloat("horizontal", Mathf.Abs(horizontal));
            anim.SetFloat("vertical", Mathf.Abs(vertical));

            rb.velocity = new Vector2(horizontal, vertical) * StatsManager.Instance.speed;
        }
    }

    /// <summary>
    /// 翻转朝向
    /// </summary>
    void Flip()
    {
        facingDirecation *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    /// <summary>
    /// 受击击退：由敌人攻击或箭矢命中时调用，向远离攻击者的方向弹开
    /// </summary>
    public void Knockback(Transform enemy,float force,float stunTime)
    {
        isKnockedBack=true;

        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.velocity = direction * force;
        StartCoroutine(KnockbackCounter(stunTime));
    }

    /// <summary>
    /// 击退恢复协程：硬直时间结束后复位移动状态
    /// </summary>
    IEnumerator KnockbackCounter(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.velocity = Vector2.zero;
        isKnockedBack = false;
    }
}
