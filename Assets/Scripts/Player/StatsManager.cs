using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatsManager : Singleton<StatsManager>
{
    public StatsUI statsUI;
    public TMP_Text healthText;

    [Header("全局配置")]
    public GameConfig config;

    [Header("Combat Stats")]
    public int damage;
    public float weaponRange;
    public float knockbackForce;
    public float knockbackTime;
    public float stunTime;

    [Header("Movament Stats")]
    public int speed;

    [Header("Health Stats")]
    public int maxHealth;
    public int currentHealth;


    protected override void Awake()
    {
        transform.SetParent(null);
        base.Awake();
        if (Instance != this) return;

        // 从配置中加载默认值（如有配置）
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

    public void UpdateMaxHealth(int amount)
    {
        maxHealth += amount;
        healthText.text="HP:" + currentHealth + "/" + maxHealth;
    }
    public void UpdateHealth(int amount)
    {
        currentHealth += amount;
        if(currentHealth >= maxHealth)
            currentHealth = maxHealth;

        healthText.text = "HP:" + currentHealth + "/" + maxHealth;
    }
    public void UpdateSpeed(int amount)
    {
        speed += amount;
        if (statsUI != null)
            statsUI.UpdateAllStates();
    }
    public void UpdateDamage(int amount)
    {
        damage += amount;
        if (statsUI != null)
            statsUI.UpdateAllStates();
    }
}
