using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 玩家属性管理器：单例，管理战斗/移动/生命属性，从 GameConfig 加载默认值，由物品使用时更新
/// </summary>
public class StatsManager : Singleton<StatsManager>
{
    public StatsUI statsUI;                // 属性面板UI引用
    public TMP_Text healthText;            // 血量文本显示

    [Header("全局配置")]
    public GameConfig config;              // 游戏配置 ScriptableObject

    [Header("战斗属性")]
    public int damage;                     // 攻击力
    public float weaponRange;              // 武器攻击范围半径
    public float knockbackForce;           // 击退力度
    public float knockbackTime;            // 击退持续时间（秒）
    public float stunTime;                 // 眩晕/硬直时间（秒）

    [Header("移动属性")]
    public int speed;                      // 移动速度

    [Header("生命属性")]
    public int maxHealth;                  // 最大生命值
    public int currentHealth;              // 当前生命值


    protected override void Awake()
    {
        transform.SetParent(null);
        base.Awake();
        if (Instance != this) return;

        // 从配置中加载默认值
        if (config != null)
        {
            damage = config.player.defaultDamage;
            weaponRange = config.player.defaultWeaponRange;
            knockbackForce = config.player.defaultKnockbackForce;
            knockbackTime = config.player.defaultKnockbackTime;
            stunTime = config.player.defaultStunTime;
            speed = config.player.defaultSpeed;
            maxHealth = config.player.defaultMaxHealth;
            currentHealth = maxHealth;
        }

        if (healthText == null)
        {
            var hpObj = GameObject.Find("HP Text");
            if (hpObj != null) healthText = hpObj.GetComponent<TMP_Text>();
        }
        if (statsUI == null)
            statsUI = FindObjectOfType<StatsUI>();
    }

    /// <summary>
    /// 更新最大生命值上限（由使用物品时调用）
    /// </summary>
    public void UpdateMaxHealth(int amount)
    {
        maxHealth += amount;
        healthText.text="HP:" + currentHealth + "/" + maxHealth;
    }

    /// <summary>
    /// 更新当前生命值（由使用物品时调用，不超出上限）
    /// </summary>
    public void UpdateHealth(int amount)
    {
        currentHealth += amount;
        if(currentHealth >= maxHealth)
            currentHealth = maxHealth;

        healthText.text = "HP:" + currentHealth + "/" + maxHealth;
    }

    /// <summary>
    /// 更新速度值并刷新属性面板（由使用物品时调用）
    /// </summary>
    public void UpdateSpeed(int amount)
    {
        speed += amount;
        if (statsUI != null)
            statsUI.UpdateAllStates();
    }

    /// <summary>
    /// 更新攻击力并刷新属性面板（由使用物品时调用）
    /// </summary>
    public void UpdateDamage(int amount)
    {
        damage += amount;
        if (statsUI != null)
            statsUI.UpdateAllStates();
    }
}
