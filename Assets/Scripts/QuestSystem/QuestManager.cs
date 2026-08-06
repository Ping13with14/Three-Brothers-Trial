using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestManager : MonoBehaviour
{

    // 任务进度字典：键为已接取任务，值为各目标当前进度
    private Dictionary<QuestSO, Dictionary<QuestObjective, int>> questProgress = new();
    // 已完成任务列表
    private List<QuestSO> completedQuests = new();

    // 注册任务完成判断事件与杀敌事件
    private void OnEnable()
    {
        GameEvents.IsQuestComplete += IsQuestComplete;
        GameEvents.OnMonsterDefeated += OnMonsterDefeated;
    }
    private void OnDisable()
    {
        GameEvents.IsQuestComplete -= IsQuestComplete;
        GameEvents.OnMonsterDefeated -= OnMonsterDefeated;
    }

    // 怪物被击败时，为所有进行中的杀敌目标累加进度
    private void OnMonsterDefeated(int exp)
    {
        foreach (var kvp in questProgress)
        {
            foreach (var objective in kvp.Key.objectives)
            {
                if (objective.objectiveType == ObjectiveType.Kill)
                {
                    kvp.Value[objective]++;
                }
            }
        }
        GameEvents.OnQuestProgressChanged?.Invoke();
    }

    #region 任务接取逻辑
    // 判断任务是否已接取
    public bool IsQuestAccepted(QuestSO questSO)
    {
        return questProgress.ContainsKey(questSO);
    }

    // 获取所有进行中的任务
    public List<QuestSO> GetActiveQuests()
    {
        return new List<QuestSO>(questProgress.Keys);
    }

    // 接取任务：加入进度字典并初始化各目标进度
    public void AcceptQuest(QuestSO questSO)
    {
        questProgress[questSO] = new Dictionary<QuestObjective, int>();

        foreach(var objective in questSO.objectives)
        {
            UpdateObjectiveProgress(questSO, objective);
        }
    }
    #endregion

    #region 任务完成逻辑
    // 判断任务是否已完成（所有目标均达到要求数量）
    public bool IsQuestComplete(QuestSO questSO)
    {
        if(!questProgress.TryGetValue(questSO,out var progressDict))
            return false;

        foreach (var objective in questSO.objectives)
        {
            UpdateObjectiveProgress(questSO,objective);
        }

        foreach (var objective in questSO.objectives)
        {
            if (progressDict[objective] < objective.requiredAmount)
            {
                return false;
            }
        }
        return true;
    }

    // 完成任务：移除进行中状态、销毁目标道具、发放奖励
    public void CompleteQuest(QuestSO questSO)
    {
        questProgress.Remove(questSO);
        completedQuests.Add(questSO);

        // 消耗任务目标道具
        foreach (var objective in questSO.objectives)
        {
            if(objective.requiredAmount > 0 && objective.targetItem != null)
            {
                InventoryManger.Instance.RemoveItem(objective.targetItem,objective.requiredAmount);
            }
        }

        // 发放任务奖励
        foreach(var reward in questSO.rewards)
        {
            InventoryManger.Instance.AddItem(reward.itemSo, reward.quantity);
        }


    }
    // 判断任务是否已在已完成列表中
    public bool GetCompleteQuest(QuestSO questSO)
    {
        return completedQuests.Contains(questSO);
    }
    #endregion

    // 更新单个目标的当前进度（从背包/位置/对话记录/杀敌计数中获取）
    public void UpdateObjectiveProgress(QuestSO questSO,QuestObjective objective)
    {
        if(!questProgress.ContainsKey(questSO))
            return;

        var progressDictionary = questProgress[questSO];
        int newAmount = 0;

        if (objective.objectiveType == ObjectiveType.Kill)
        {
            // 杀敌目标保持现有进度（由OnMonsterDefeated累加）
            if (progressDictionary.TryGetValue(objective, out int currentKills))
                newAmount = currentKills;
        }
        else if (objective.targetItem != null && InventoryManger.Instance != null)
            newAmount = InventoryManger.Instance.GetItemQuantity(objective.targetItem);
        else if (objective.targetLocation != null && GameManager.Instance.LocationHistoryTracker != null && GameManager.Instance.LocationHistoryTracker.HasVisited(objective.targetLocation))
            newAmount = objective.requiredAmount;
        else if (objective.targetNPC != null && GameManager.Instance.DialogueHistoryTracker != null && GameManager.Instance.DialogueHistoryTracker.HasSpokenWith(objective.targetNPC))
            newAmount = objective.requiredAmount;

        progressDictionary[objective] = newAmount;
    }
    // 获取进度的显示文本（如"3/5"、"已完成"或"进行中"）
    public string GetProgressText(QuestSO questSO,QuestObjective objective)
    {
        int currentAmount = GetCurrentAmount(questSO,objective);

        if (currentAmount >= objective.requiredAmount)
            return "已完成";
        else if (objective.targetItem != null || objective.objectiveType == ObjectiveType.Kill)
            return $"{currentAmount}/{objective.requiredAmount}";
        else
            return "进行中";
    }

    // 获取目标当前进度数值
    public int GetCurrentAmount(QuestSO questSO,QuestObjective objective)
    {
        if (questProgress.TryGetValue(questSO, out var objectiveDictionary))
            if (objectiveDictionary.TryGetValue(objective, out int amount))
                return amount;
        return 0;
    }



}
