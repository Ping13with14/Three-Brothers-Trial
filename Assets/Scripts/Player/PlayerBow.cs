using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家弓箭：处理瞄准输入，由动画事件在射箭帧回调 Shoot() 生成箭矢
/// </summary>
public class PlayerBow : MonoBehaviour
{
    public Transform launchPoint;          // 箭矢生成位置
    public GameObject arrowPrefab;         // 箭矢预制体

    public PlayerMovement playerMovement;  // 玩家移动组件引用（射击时锁定移动）

    private Vector2 aimDtrection = Vector2.right;  // 当前瞄准方向

    public float shootCooldown = .5f;      // 射击冷却时间（秒）
    private float shootTimer;              // 当前冷却计时器

    public Animator anim;                  // 玩家动画控制器

    void Update()
    {
        shootTimer -= Time.deltaTime;
        HandleAiming();

        if (InputManager.Provider.IsShootPressed && shootTimer<=0)
        {
            playerMovement.isShooting = true;
            anim.SetBool("isShooting", true);
        }
    }

    /// <summary>
    /// 启用弓箭时：将动画层权重从近战层(0)切换到弓箭层(1)
    /// </summary>
    private void OnEnable()
    {
        anim.SetLayerWeight(0, 0);
        anim.SetLayerWeight(1, 1);
    }

    /// <summary>
    /// 禁用弓箭时：将动画层权重从弓箭层(1)切回近战层(0)
    /// </summary>
    private void OnDisable()
    {
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 0);
    }

    /// <summary>
    /// 根据原始输入更新瞄准方向（方向键/摇杆）
    /// </summary>
    private void HandleAiming()
    {
        float horizontal = InputManager.Provider.HorizontalRaw;
        float vertical = InputManager.Provider.VerticalRaw;
        if (horizontal != 0 || vertical != 0)
        {
            aimDtrection = new Vector2(horizontal, vertical).normalized;
        }
    }

    /// <summary>
    /// 射箭：由动画事件（Animation Event）在射击动画的放箭帧回调，生成箭矢并进入冷却，复位射击状态
    /// </summary>
    public void Shoot()
    {
        if (shootTimer <= 0)
        {
            Arrow arrow = Instantiate(arrowPrefab, launchPoint.position, Quaternion.identity).GetComponent<Arrow>();
            arrow.direction = aimDtrection;
            shootTimer = shootCooldown;

            // 播放弓箭射击音效
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("弓箭射击_BowShoot");
        }
        anim.SetBool("isShooting", false);
        playerMovement.isShooting = false;
    }
}
