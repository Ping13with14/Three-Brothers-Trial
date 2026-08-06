using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpManager : MonoBehaviour
{
    public int level;
    public int currentExp;
    public int expToLevel=10;
    public float expGrowthMultiplier = 1.2f; // 经验成长倍率，每级所需经验为上一级所需经验的1.2倍
    public Slider expSlider;
    public TMP_Text currentLeveText;

    // 升级事件已迁移至 GameEvents.OnLevelUp

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if(InputManager.Provider.IsDebugKeyPressed)
        {
            GainExperience(2);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnMonsterDefeated += GainExperience;
        GameEvents.OnExperienceGained += GainExperience;
    }

    private void OnDisable()
    {
        GameEvents.OnMonsterDefeated -= GainExperience;
        GameEvents.OnExperienceGained -= GainExperience;
    }


    public void GainExperience(int amount)
    {
        currentExp += amount;
        if (currentExp >= expToLevel)
        {
            LevelUp();
        }
        UpdateUI();
    }

    private void LevelUp()
    {
        level++;
        currentExp -= expToLevel;
        expToLevel=Mathf.RoundToInt(expToLevel*expGrowthMultiplier);
        GameEvents.OnLevelUp?.Invoke(1);

    }

    public void UpdateUI()
    {
        expSlider.maxValue = expToLevel;
        expSlider.value= currentExp;
        currentLeveText.text = "Level:" + level;
    }

}
