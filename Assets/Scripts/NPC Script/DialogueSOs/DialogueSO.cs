using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话脚本对象：定义对话内容、选项及任务关联
/// </summary>
///
[CreateAssetMenu(fileName = "DialogueSO", menuName = "DialogueSo/DialogueNode")]
public class DialogueSO : ScriptableObject
{
    public DialogueLine[] lines;
    public DialogueOption[] options;

    [Header("提供任务（可选）")]
    public QuestSO offerquestOnEnd;

    [Header("需要已完成任务（目标达成但未提交）")]
    public QuestSO[] requiredCompleteQuests;

    [Header("需要已提交任务（已完成并已领取奖励）")]
    public QuestSO[] requiredCompletedQuests;

    [Header("可提交任务（可选）")]
    public QuestSO turnInQuestOnEnd;

    [Header("对话条件(可选)")]
    public ActorSO[] requiredNPCs;
    public LocationSO[] requiredLocatons;
    public ItemSo[] requiredItems;

    [Header("控制标志")]
    public bool removeAfterPlay;
    public List<DialogueSO> removeTheseOnPlay;


    public bool IsConditionMet()
    {
        // 检查是否已与指定NPC对话
        if (requiredNPCs != null && requiredNPCs.Length > 0)
        {
            foreach (var npc in requiredNPCs)
            {
                if (!GameManager.Instance.DialogueHistoryTracker.HasSpokenWith(npc))
                    return false;
            }
        }
        // 检查是否已访问指定地点
        if (requiredLocatons != null && requiredLocatons.Length > 0)
        {
            foreach(var location in requiredLocatons)
            {
                if (!GameManager.Instance.LocationHistoryTracker.HasVisited(location))
                    return false;
            }
        }
        // 检查是否持有指定道具
        if (requiredItems != null && requiredItems.Length > 0)
        {
            foreach (var item in requiredItems)
            {
                if(!InventoryManger.Instance.HasItem(item))
                    return false;
            }
        }

        if(requiredCompleteQuests != null && requiredCompleteQuests.Length > 0)
        {
            foreach ( var quest in requiredCompleteQuests)
            {
                if (!GameManager.Instance.QuestManager.IsQuestComplete(quest))
                    return false;
            }
        }

        // 检查是否已完成指定任务（已提交并领取奖励）
        if(requiredCompletedQuests != null && requiredCompletedQuests.Length > 0)
        {
            foreach (var quest in requiredCompletedQuests)
            {
                if (!GameManager.Instance.QuestManager.GetCompleteQuest(quest))
                    return false;
            }
        }


        return true;
    }
}

[System.Serializable]
public class DialogueLine
{
    public ActorSO speaker;
    [TextArea(3,5)] public string text;
}

// 对话选项：玩家可选择的对话分支
[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public DialogueSO nextDialogue;
    public QuestSO offerQuest;
}

