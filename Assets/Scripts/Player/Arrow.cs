using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 箭矢：发射后沿方向飞行，碰撞敌人造成伤害并嵌入，碰撞障碍物直接嵌入
/// </summary>
public class Arrow : MonoBehaviour
{
    [Header("飞行参数")]
    public Rigidbody2D rb;
    public Vector2 direction = Vector2.right;  // 飞行方向（由 PlayerBow 设置）
    public float lifeSpawn = 2;                // 存活时间（秒），到期自动销毁
    public float speed;                         // 飞行速度

    [Header("碰撞图层")]
    public LayerMask enemyLayer;                // 敌人图层
    public LayerMask obstacleLayer;             // 障碍物图层

    [Header("嵌入表现")]
    public SpriteRenderer sr;                   // 箭矢精灵渲染器
    public Sprite buriedSprite;                 // 嵌入物体后的精灵（卡在表面的视觉效果）

    [Header("伤害参数")]
    public int damage;                          // 伤害值
    public float knockbackForce;                // 击退力度
    public float knockbackTime;                 // 击退持续时间
    public float stunTime;                      // 眩晕/硬直时间

    void Start()
    {
        rb.velocity = direction * speed;
        RotateArrow();
        Destroy(gameObject, lifeSpawn);
    }

    /// <summary>
    /// 根据飞行方向旋转箭矢朝向
    /// </summary>
    private void RotateArrow()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    /// <summary>
    /// 碰撞处理：命中敌人→造成伤害+击退+嵌入，命中障碍物→直接嵌入
    /// </summary>
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if((enemyLayer.value & ( 1 << collision.gameObject.layer)) > 0 )
        {
            var health = collision.gameObject.GetComponent<Enemy_Health>();
            health.ChangeHealth(-damage);
            // 敌人死亡后会被回收/销毁并设为非活跃，此时再启动击退协程会报错，故仅对存活敌人击退
            if (health.currentHealth > 0)
                collision.gameObject.GetComponent<Enemy_Knockback>().Knockback(transform, knockbackForce, knockbackTime, stunTime);
            AttachToTarget(collision.gameObject.transform);
        }
        else if ((obstacleLayer.value & (1 << collision.gameObject.layer)) > 0)
        {
            AttachToTarget(collision.gameObject.transform);
        }
    }

    /// <summary>
    /// 嵌入目标：替换为嵌入精灵、停止物理运动、挂载到目标父级
    /// </summary>
    private void AttachToTarget(Transform target)
    {
        sr.sprite = buriedSprite;

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;

        transform.SetParent(target);
    }
}
