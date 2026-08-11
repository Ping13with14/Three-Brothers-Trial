using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人生命值：实现 IDamageable，管理受击、死亡、经验奖励
/// </summary>
public class Enemy_Health : MonoBehaviour, IDamageable
{
    [Header("属性")]
    public int expReward = 3;          // 击败后奖励的经验值
    public int currentHealth;          // 当前生命值
    public int maxHealth;              // 最大生命值

    int IDamageable.CurrentHealth => currentHealth;
    int IDamageable.MaxHealth => maxHealth;

    /// <summary>
    /// 从 GameConfig 加载默认数值（如未在 Inspector 中设置）
    /// </summary>
    private void Awake()
    {
        if (maxHealth <= 0 || expReward <= 0)
        {
            var config = Resources.Load<GameConfig>("Configs/GameConfig");
            if (config != null)
            {
                if (maxHealth <= 0)
                    maxHealth = config.enemy.defaultMaxHealth;
                if (expReward <= 0)
                    expReward = config.enemy.defaultExpReward;
            }
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// 生命值变更：由玩家攻击/箭矢命中时调用，血量≤0 时触发死亡流程（播放音效、触发事件、销毁对象）
    /// </summary>
    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            // 播放死亡音效
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("死亡_Death");
            GameEvents.OnMonsterDefeated?.Invoke(expReward);
            Destroy(gameObject);
        }
    }
}
