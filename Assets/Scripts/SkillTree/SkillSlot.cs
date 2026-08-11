using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// 技能槽：处理单个技能的升级/退还/解锁逻辑，管理UI显示
/// </summary>
public class SkillSlot : MonoBehaviour
{
    [Header("前置技能")]
    public List<SkillSlot> prerequisiteSkillSlots;  // 解锁此前置条件的前置技能列表

    [Header("技能数据")]
    public SkillSo skillSo;                          // 技能数据 ScriptableObject

    [Header("当前状态")]
    public int currentLevel;                         // 当前技能等级
    public bool isUnlocked;                          // 是否已解锁

    [Header("UI 组件")]
    public Image skillIcon;                          // 技能图标
    public Button skillButton;                       // 技能按钮
    public TMP_Text skillLevelText;                 // 技能等级文本

    private void OnValidate()
    {
        if (skillSo != null && skillLevelText != null)
            UpdateUI();
    }

    /// <summary>
    /// 尝试升级技能：已解锁且未满级时可升级，触发 OnAbilityPointSpent，满级时触发 OnSkillMaxed
    /// </summary>
    public void TryUpgradeSkill()
    {
        if (skillSo == null)
            return;

        if(isUnlocked && currentLevel<skillSo.maxlevel)
        {
            currentLevel++;
            GameEvents.OnAbilityPointSpent?.Invoke(this);

            if(currentLevel>=skillSo.maxlevel)
                GameEvents.OnSkillMaxed?.Invoke(this);

            UpdateUI();
        }
    }

    /// <summary>
    /// 初始化：为技能按钮添加右键退还事件监听（通过 EventTrigger 避免 Button 拦截冒泡）
    /// </summary>
    private void Start()
    {
        if (skillButton == null) return;
        var trigger = skillButton.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = skillButton.gameObject.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) =>
        {
            var pointerData = data as PointerEventData;
            if (pointerData != null && pointerData.button == PointerEventData.InputButton.Right)
                TryRefundSkill();
        });
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// 右键退还技能点：当前等级>0且未锁定时可退还1点，但若被其他已解锁技能依赖则不允许退还
    /// </summary>
    public void TryRefundSkill()
    {
        if (skillSo == null || !isUnlocked || currentLevel <= 0)
            return;

        // 检查是否为其他技能的前置依赖
        foreach (var slot in FindObjectsOfType<SkillSlot>())
        {
            if (slot.prerequisiteSkillSlots.Contains(this) && slot.isUnlocked)
                return;
        }

        currentLevel--;
            GameEvents.OnAbilityPointRefunded?.Invoke(this);
        UpdateUI();
    }

    /// <summary>
    /// 检查是否满足解锁条件：所有前置技能均已解锁且满级
    /// </summary>
    public bool CanUnlockSkill()
    {
        foreach (SkillSlot slot in prerequisiteSkillSlots)
        {
            if (!slot.isUnlocked||slot.currentLevel<slot.skillSo.maxlevel)
                return false;
        }
        return true;
    }

    /// <summary>
    /// 解锁技能（由 HandleSkillMaxed 在满足条件时调用）
    /// </summary>
    public void Unlock()
    {
        isUnlocked = true;
        UpdateUI();
    }

    /// <summary>
    /// 刷新图标、按钮交互状态、等级文本
    /// </summary>
    private void UpdateUI()
    {
        if (skillIcon != null && skillSo != null)
            skillIcon.sprite = skillSo.skillIcon;

        if(isUnlocked)
        {
            if (skillButton != null)
                skillButton.interactable = true;
            if (skillLevelText != null && skillSo != null)
                skillLevelText.text = currentLevel.ToString() + "/" + skillSo.maxlevel.ToString();
            if (skillIcon != null)
                skillIcon.color = Color.white;
        }
        else
        {
            if (skillButton != null)
                skillButton.interactable = false;
            if (skillLevelText != null)
                skillLevelText.text = "Locked";
            if (skillIcon != null)
                skillIcon.color = Color.grey;
        }
    }
}
