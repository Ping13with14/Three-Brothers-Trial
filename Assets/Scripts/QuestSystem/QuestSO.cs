using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任务配置脚本对象：定义任务名称、描述、目标与奖励
/// </summary>
///
[CreateAssetMenu(fileName ="QuestSO",menuName ="QuestSO")]
public class QuestSO : ScriptableObject
{

    // 任务名称
    public string questName;
    // 任务描述
    [TextArea] public string questDescription;
    // 任务等级
    public int questLevel;


    // 任务目标列表
    public List<QuestObjective> objectives;
    // 任务奖励列表
    public List<QuestReward> rewards;
}

// 任务目标类型枚举
public enum ObjectiveType
{
    Collect,   // 收集道具
    Talk,      // 与NPC对话
    Visit,     // 访问地点
    Kill       // 消灭敌人
}

// 任务目标：收集道具/访问地点/与NPC对话/消灭敌人
[System.Serializable]
public class QuestObjective
{
    public string description;
    // 目标类型（决定如何追踪进度）
    public ObjectiveType objectiveType = ObjectiveType.Collect;
    // 目标对象（Collect类型为ItemSo，Talk类型为ActorSO，Visit类型为LocationSO，Kill类型可留空）
    public Object target;
    public ItemSo targetItem => target as ItemSo;
    public ActorSO targetNPC => target as ActorSO;
    public LocationSO targetLocation => target as LocationSO;

    // 目标需求数量（收集/杀敌类为具体数量，地点/NPC类固定为1）
    public int requiredAmount;
}

// 任务奖励：道具及数量
[System.Serializable]
public class QuestReward
{
    public ItemSo itemSo;
    public int quantity;
}

