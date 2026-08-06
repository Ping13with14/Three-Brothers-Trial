using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    int IDamageable.CurrentHealth => StatsManager.Instance != null ? StatsManager.Instance.currentHealth : 0;
    int IDamageable.MaxHealth => StatsManager.Instance != null ? StatsManager.Instance.maxHealth : 0;

    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Animator healthTextAnim;

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
