using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class SkillSlot : MonoBehaviour
{
    public List<SkillSlot> prerequisiteSkillSlots;

    public SkillSo skillSo;

    public int currentLevel;
    public bool isUnlocked;

    public Image skillIcon;
    public Button skillButton;
    public TMP_Text skillLevelText;

    // 技能事件已迁移至 GameEvents（OnAbilityPointSpent / OnSkillMaxed / OnAbilityPointRefunded）

    private void OnValidate()
    {
        if (skillSo != null && skillLevelText != null)
            UpdateUI();
    }

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

    // 在Button物体上挂EventTrigger监听右键点击，避免Button组件拦截冒泡
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
    /// 右键回收技能点：当前等级>0且未锁定时可退还1点
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

    public bool CanUnlockSkill()
    {
        foreach (SkillSlot slot in prerequisiteSkillSlots)
        {
            if (!slot.isUnlocked||slot.currentLevel<slot.skillSo.maxlevel)
                return false;
        }
        return true;
    }

    public void Unlock()
    {
        isUnlocked = true;
        UpdateUI();
    }

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
