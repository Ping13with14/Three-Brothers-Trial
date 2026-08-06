using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour, IDamageable
{
    public int expReward = 3;

    // 怪物被击败时通过 GameEvents 通知各系统

    public int currentHealth;
    public int maxHealth;

    int IDamageable.CurrentHealth => currentHealth;
    int IDamageable.MaxHealth => maxHealth;

    /// <summary>
    /// 全局配置引用，用于获取敌人默认数值（挂载后自动查找）
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

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else if (currentHealth <= 0)
        {
            GameEvents.OnMonsterDefeated?.Invoke(expReward);
            Destroy(gameObject);
        }
    }
}
