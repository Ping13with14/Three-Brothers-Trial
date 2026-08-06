using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManger : MonoBehaviour
{
    public PlayerCombat combat;

    private void Awake()
    {
        if (combat == null)
            combat = FindObjectOfType<PlayerCombat>();
    }

    private void OnEnable()
    {
        GameEvents.OnAbilityPointSpent += HandleAbilityPointSpent;
    }

    private void OnDisable()
    {
        GameEvents.OnAbilityPointSpent -= HandleAbilityPointSpent;
    }


    private void HandleAbilityPointSpent(SkillSlot slot)
    {
        string skillName = slot.skillSo.skillName;

        switch(skillName)
        {
            case "最大血量":
                StatsManager.Instance.UpdateMaxHealth(1);
                break;

            case "挥砍":
                combat.enabled = true;
                break;

            default:
                Debug.LogWarning("正在点击技能" + skillName);
                break;
        }
    }
}
