using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能效果管理器：监听技能点消耗事件，按技能名称应用对应的属性/功能效果
/// </summary>
public class SkillManger : MonoBehaviour
{
    public PlayerCombat combat;                // 玩家近战组件引用（"挥砍"技能启用）

    private void Awake()
    {
        if (combat == null)
            combat = FindObjectOfType<PlayerCombat>();
    }

    /// <summary>
    /// 启用时订阅技能点消耗事件
    /// </summary>
    private void OnEnable()
    {
        GameEvents.OnAbilityPointSpent += HandleAbilityPointSpent;
    }

    /// <summary>
    /// 禁用时退订事件
    /// </summary>
    private void OnDisable()
    {
        GameEvents.OnAbilityPointSpent -= HandleAbilityPointSpent;
    }

    /// <summary>
    /// 处理技能效果：由 GameEvents.OnAbilityPointSpent 回调，根据技能名称应用对应的效果
    /// </summary>
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
