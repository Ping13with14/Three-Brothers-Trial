using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话历史追踪器：记录玩家已对话过的 NPC 列表，用于任务条件判断
/// </summary>
public class DialogueHistoryTracker : MonoBehaviour
{
    /// <summary>
    /// 已对话的 NPC 集合（HashSet 自动去重）
    /// </summary>
    private readonly HashSet<ActorSO> spokenNPCs = new HashSet<ActorSO>();

    /// <summary>
    /// 记录 NPC 对话：首次对话时触发任务进度更新
    /// </summary>
    public void RecordNPC(ActorSO actorSO)
    {
        if (spokenNPCs.Add(actorSO))
        {
            GameEvents.OnQuestProgressChanged?.Invoke();
        }
    }

    /// <summary>
    /// 查询是否已与指定 NPC 对话过
    /// </summary>
    public bool HasSpokenWith(ActorSO actorSO)
    {
        return spokenNPCs.Contains(actorSO);
    }

}
