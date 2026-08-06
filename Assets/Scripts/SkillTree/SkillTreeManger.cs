using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillTreeManger : MonoBehaviour
{
    public SkillSlot[] skillSlots;
    public TMP_Text pointsText;
    public int availablePoints;

    private void OnEnable()
    {
        GameEvents.OnAbilityPointSpent += HandleAbilityPointsSpent;
        GameEvents.OnSkillMaxed += HandleSkillMaxed;
        GameEvents.OnAbilityPointRefunded += HandleAbilityPointRefunded;
        GameEvents.OnLevelUp += UpdateAbilityPoints;
    }

    private void OnDisable()
    {
        GameEvents.OnAbilityPointSpent -= HandleAbilityPointsSpent;
        GameEvents.OnSkillMaxed -= HandleSkillMaxed;
        GameEvents.OnAbilityPointRefunded -= HandleAbilityPointRefunded;
        GameEvents.OnLevelUp -= UpdateAbilityPoints;
    }

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

    private void CheckAvailablePoints(SkillSlot slot)
    {
        if (availablePoints > 0)
            slot.TryUpgradeSkill();
    }

    private void HandleAbilityPointsSpent(SkillSlot skillSlot)
    {
        if (availablePoints > 0)
            UpdateAbilityPoints(-1);
    }

    private void HandleAbilityPointRefunded(SkillSlot skillSlot)
    {
        UpdateAbilityPoints(1);
    }

    private void HandleSkillMaxed(SkillSlot skillSlot)
    {
        foreach (SkillSlot slot in skillSlots)
        {
            if (!slot.isUnlocked && slot.CanUnlockSkill())
                slot.Unlock();
        }
    }

    public void UpdateAbilityPoints(int amount)
    {
        availablePoints += amount;
        pointsText.text = "技能点: " + availablePoints;
    }
}
