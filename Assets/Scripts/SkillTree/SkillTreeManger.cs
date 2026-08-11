using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 技能树管理器：管理技能点分配、技能解锁/升级，监听 GameEvents 处理技能系统的各种操作
/// </summary>
public class SkillTreeManger : MonoBehaviour
{
    [Header("技能树配置")]
    public SkillSlot[] skillSlots;            // 技能槽位数组
    public TMP_Text pointsText;               // 技能点文本显示
    public int availablePoints;               // 当前可用技能点数

    /// <summary>
    /// 启用时订阅技能系统事件
    /// </summary>
    private void OnEnable()
    {
        GameEvents.OnAbilityPointSpent += HandleAbilityPointsSpent;      // 技能点被消耗
        GameEvents.OnSkillMaxed += HandleSkillMaxed;                     // 技能升至满级
        GameEvents.OnAbilityPointRefunded += HandleAbilityPointRefunded; // 技能点被退还
        GameEvents.OnLevelUp += UpdateAbilityPoints;                     // 玩家升级获得技能点
    }

    /// <summary>
    /// 禁用时退订所有事件，防止内存泄漏
    /// </summary>
    private void OnDisable()
    {
        GameEvents.OnAbilityPointSpent -= HandleAbilityPointsSpent;
        GameEvents.OnSkillMaxed -= HandleSkillMaxed;
        GameEvents.OnAbilityPointRefunded -= HandleAbilityPointRefunded;
        GameEvents.OnLevelUp -= UpdateAbilityPoints;
    }

    /// <summary>
    /// 初始化：绑定技能槽按钮点击事件
    /// </summary>
    private void Start()
    {
        foreach (SkillSlot slot in skillSlots)
        {
            if (slot == null || slot.skillButton == null)
                continue;

            SkillSlot capturedSlot = slot;
            slot.skillButton.onClick.AddListener(() =>
            {
                CheckAvailablePoints(capturedSlot);
            });
        }

        UpdateAbilityPoints(0);
    }

    /// <summary>
    /// 检查可用技能点并尝试升级：由技能槽按钮点击事件回调
    /// </summary>
    private void CheckAvailablePoints(SkillSlot slot)
    {
        if (availablePoints > 0)
        {
            // 播放UI点击音效
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("UI点击_BlipSelect");
            slot.TryUpgradeSkill();
        }
    }

    /// <summary>
    /// 处理技能点消耗：由 GameEvents.OnAbilityPointSpent 回调
    /// </summary>
    private void HandleAbilityPointsSpent(SkillSlot skillSlot)
    {
        if (availablePoints > 0)
            UpdateAbilityPoints(-1);
    }

    /// <summary>
    /// 处理技能点退还：由 GameEvents.OnAbilityPointRefunded 回调
    /// </summary>
    private void HandleAbilityPointRefunded(SkillSlot skillSlot)
    {
        UpdateAbilityPoints(1);
    }

    /// <summary>
    /// 处理技能满级：由 GameEvents.OnSkillMaxed 回调，解锁所有可解锁的前置技能
    /// </summary>
    private void HandleSkillMaxed(SkillSlot skillSlot)
    {
        foreach (SkillSlot slot in skillSlots)
        {
            if (!slot.isUnlocked && slot.CanUnlockSkill())
                slot.Unlock();
        }
    }

    /// <summary>
    /// 更新可用技能点数：由 OnLevelUp / HandleAbilityPointsSpent / HandleAbilityPointRefunded 调用
    /// </summary>
    public void UpdateAbilityPoints(int amount)
    {
        availablePoints += amount;
        pointsText.text = "技能点: " + availablePoints;
    }
}
