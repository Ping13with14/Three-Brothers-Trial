using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 玩家生命值：实现 IDamageable，由敌人攻击/箭矢命中时调用 ChangeHealth
/// </summary>
public class PlayerHealth : MonoBehaviour, IDamageable
{
    int IDamageable.CurrentHealth => StatsManager.Instance != null ? StatsManager.Instance.currentHealth : 0;
    int IDamageable.MaxHealth => StatsManager.Instance != null ? StatsManager.Instance.maxHealth : 0;

    [SerializeField] private TMP_Text healthText;         // 血量文本显示
    [SerializeField] private Animator healthTextAnim;     // 血量变化时的文本动画

    private void Start()
    {
        if (StatsManager.Instance != null)
        {
            healthText = StatsManager.Instance.healthText;
            if (healthText != null)
            {
                healthTextAnim = healthText.GetComponent<Animator>();
                healthText.text = "Hp:" + StatsManager.Instance.currentHealth + "/" + StatsManager.Instance.maxHealth;
            }
        }
    }

    /// <summary>
    /// 生命值变更：由敌人攻击/箭矢命中时调用，更新 UI 并播放动画，血量≤0 时禁用玩家对象
    /// </summary>
    public void ChangeHealth(int amount)
    {
        StatsManager.Instance.currentHealth += amount;
        if (healthTextAnim != null)
            healthTextAnim.Play("TextUpdate");
        if (healthText != null)
            healthText.text = "Hp:" + StatsManager.Instance.currentHealth + "/" + StatsManager.Instance.maxHealth;

        if (StatsManager.Instance.currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
