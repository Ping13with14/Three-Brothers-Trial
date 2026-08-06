using UnityEngine;
using System;

/// <summary>
/// 统一事件中心：集中管理所有跨系统事件，解耦各模块之间的通信
/// </summary>
public static class GameEvents
{
    #region 任务事件
    // NPC/任务板请求展示任务
    public static Action<QuestSO> OnQuestOfferRequested;
    // NPC/任务板请求提交已完成的任务
    public static Action<QuestSO> OnQuestTurnInRequested;
    // 玩家接取任务后触发
    public static Action<QuestSO> OnQuestAccepted;
    // 任务进度变化时触发（刷新UI）
    public static Action OnQuestProgressChanged;
    // 查询任务是否已完成
    public static Func<QuestSO, bool> IsQuestComplete;
    #endregion

    #region 战斗事件
    // 怪物被击败，参数为经验值
    public static Action<int> OnMonsterDefeated;
    #endregion

    #region 物品事件
    // 物品被拾取
    public static Action<ItemSo, int> OnItemLooted;
    // 获得经验（来自物品使用）
    public static Action<int> OnExperienceGained;
    #endregion

    #region 技能事件
    // 技能点被消耗
    public static Action<SkillSlot> OnAbilityPointSpent;
    // 技能升至满级
    public static Action<SkillSlot> OnSkillMaxed;
    // 技能点被退还
    public static Action<SkillSlot> OnAbilityPointRefunded;
    #endregion

    #region 玩家事件
    // 玩家升级，参数为升级获得技能点数
    public static Action<int> OnLevelUp;
    #endregion
}
