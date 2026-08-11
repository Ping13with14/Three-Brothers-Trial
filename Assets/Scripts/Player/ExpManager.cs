using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 经验与等级管理：监听怪物击败/经验获得事件，累积经验并触发升级
/// </summary>
public class ExpManager : MonoBehaviour
{
    [Header("等级与经验")]
    public int level;                          // 当前等级
    public int currentExp;                     // 当前经验值
    public int expToLevel=10;                  // 升级所需经验值
    public float expGrowthMultiplier = 1.2f;   // 经验成长倍率，每级所需经验为上一级所需经验的1.2倍

    [Header("UI 组件")]
    public Slider expSlider;                   // 经验条
    public TMP_Text currentLeveText;           // 等级文本显示

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        // 调试用：按回车键直接获得经验
        if(InputManager.Provider.IsDebugKeyPressed)
        {
            GainExperience(2);
        }
    }

    /// <summary>
    /// 启用时订阅事件：监听怪物击败和经验获得
    /// </summary>
    private void OnEnable()
    {
        GameEvents.OnMonsterDefeated += GainExperience;
        GameEvents.OnExperienceGained += GainExperience;
    }

    /// <summary>
    /// 禁用时退订事件，防止内存泄漏
    /// </summary>
    private void OnDisable()
    {
        GameEvents.OnMonsterDefeated -= GainExperience;
        GameEvents.OnExperienceGained -= GainExperience;
    }

    /// <summary>
    /// 获得经验：由 GameEvents.OnMonsterDefeated / OnExperienceGained 回调
    /// </summary>
    public void GainExperience(int amount)
    {
        currentExp += amount;
        if (currentExp >= expToLevel)
        {
            LevelUp();
        }
        UpdateUI();
    }

    /// <summary>
    /// 升级：提升等级、重置经验、增加升级所需经验，触发 OnLevelUp 事件
    /// </summary>
    private void LevelUp()
    {
        level++;
        currentExp -= expToLevel;
        expToLevel=Mathf.RoundToInt(expToLevel*expGrowthMultiplier);
        // 播放升级音效
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("升级_PowerUp");
        GameEvents.OnLevelUp?.Invoke(1);

    }

    /// <summary>
    /// 刷新经验条和等级文本
    /// </summary>
    public void UpdateUI()
    {
        expSlider.maxValue = expToLevel;
        expSlider.value= currentExp;
        currentLeveText.text = "Level:" + level;
    }

}
