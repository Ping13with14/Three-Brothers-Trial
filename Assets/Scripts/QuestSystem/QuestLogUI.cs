using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 任务UI更新脚本
/// </summary>
public class QuestLogUI : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text questDescriptionText;
    [SerializeField] private QuestObjectiveSlot[] objectiveSlots;
    [SerializeField] private QuestRewardSlot[] rewardSlots;

    // 任务日志列表槽位（在Inspector中拖入QuestLogSlot）
    [SerializeField] private QuestLogSlot[] questSlots;

    //任务从哪里来
    private QuestSO questSO;

    //按钮画布引用
    [SerializeField] private CanvasGroup questCanvas;

    [SerializeField] private CanvasGroup acceptCanvasGroup;
    [SerializeField] private CanvasGroup declineCanvasGroup;
    [SerializeField] private CanvasGroup completeCanvasGroup;


    //监听任务事件
    private void OnEnable()
    {
        GameEvents.OnQuestOfferRequested += ShowQuestOffer;
        GameEvents.OnQuestTurnInRequested += ShowQuestTurnIn;
        GameEvents.OnQuestProgressChanged += RefreshObjectives;
        GameEvents.OnQuestAccepted += OnQuestAccepted_AddToLog;
    }

    private void OnDisable()
    {
        GameEvents.OnQuestOfferRequested -= ShowQuestOffer;
        GameEvents.OnQuestTurnInRequested -= ShowQuestTurnIn;
        GameEvents.OnQuestProgressChanged -= RefreshObjectives;
        GameEvents.OnQuestAccepted -= OnQuestAccepted_AddToLog;
    }

    private void Start()
    {
        ClearDetails();
    }

    // 任务被接取后，在日志列表中找一个空槽位加入
    private void OnQuestAccepted_AddToLog(QuestSO acceptedQuest)
    {
        if (questSlots == null || questSlots.Length == 0)
            return;

        // 先检查该任务是否已在槽位中（预配置的任务），避免重复添加
        foreach (var slot in questSlots)
        {
            if (slot == null) continue;
            if (slot.currentQuest == acceptedQuest)
                return;
        }

        // 找一个空槽位放入新接取的任务
        foreach (var slot in questSlots)
        {
            if (slot == null) continue;
            if (slot.currentQuest == null)
            {
                if (slot.questLogUI == null)
                    slot.questLogUI = this;
                slot.SetQuest(acceptedQuest);
                return;
            }
        }
    }

    // 从日志列表中移除已完成的任务
    private void RemoveQuestFromLog(QuestSO completedQuest)
    {
        if (questSlots == null)
            return;

        foreach (var slot in questSlots)
        {
            if (slot == null) continue;
            if (slot.currentQuest == completedQuest)
            {
                slot.ClearSlot();
                return;
            }
        }
    }

    private void ClearDetails()
    {
        questNameText.text = string.Empty;
        questDescriptionText.text = string.Empty;

        foreach (var slot in objectiveSlots)
            slot.gameObject.SetActive(false);

        foreach (var slot in rewardSlots)
            slot.gameObject.SetActive(false);

        SetCanvasState(acceptCanvasGroup, false);
        SetCanvasState(declineCanvasGroup, false);
        SetCanvasState(completeCanvasGroup, false);
    }


    // NPC或任务板提供任务时调用
    public void ShowQuestOffer(QuestSO incomingQuestSO)
    {
        HandleQuestClicked(incomingQuestSO);

        GameManager.ShowPanel(questCanvas);

        // 未接取时显示接取按钮，已接取且完成时显示完成按钮
        bool isAccepted = questManager.IsQuestAccepted(incomingQuestSO);
        bool canComplete = isAccepted && questManager.IsQuestComplete(incomingQuestSO);
        SetCanvasState(acceptCanvasGroup, !isAccepted);
        SetCanvasState(declineCanvasGroup, true);
        SetCanvasState(completeCanvasGroup, canComplete);
    }

    // NPC或任务板提交任务时调用
    public void ShowQuestTurnIn(QuestSO incomingQuestSO)
    {
        HandleQuestClicked(incomingQuestSO);

        GameManager.ShowPanel(questCanvas);

        // 提交界面永不显示接取按钮，仅当任务确实完成时才显示完成按钮
        bool canComplete = questManager.IsQuestAccepted(incomingQuestSO) && questManager.IsQuestComplete(incomingQuestSO);
        SetCanvasState(acceptCanvasGroup, false);
        SetCanvasState(declineCanvasGroup, true);
        SetCanvasState(completeCanvasGroup, canComplete);
    }

    // 点击接取任务按钮：将任务加入进行中列表
    public void OnAcceptQuestClicked()
    {
        if (questSO != null)
        {
            questManager.AcceptQuest(questSO);
            GameEvents.OnQuestAccepted?.Invoke(questSO);
        }
        ClosePanel();
    }

    // 点击拒绝任务按钮：关闭面板，任务仍可重新接取
    public void OnDeclineQuestClicked()
    {
        ClosePanel();
    }

    // 点击完成任务按钮：销毁所需道具、发放奖励、移除任务
    public void OnCompleteQuestClicked()
    {
        if (questSO != null)
        {
            questManager.CompleteQuest(questSO);
            // 通知其他系统任务进度已变更
            GameEvents.OnQuestProgressChanged?.Invoke();
            // 从日志列表中移除已完成的任务
            RemoveQuestFromLog(questSO);
        }
        ClosePanel();
    }

    private void ClosePanel()
    {
        GameManager.HidePanel(questCanvas);
        SetCanvasState(acceptCanvasGroup, false);
        SetCanvasState(declineCanvasGroup, false);
        SetCanvasState(completeCanvasGroup, false);
        questSO = null;
        ClearDetails();
    }

    private void SetCanvasState(CanvasGroup group, bool activate)
    {
        group.alpha = activate ? 1 : 0;
        group.blocksRaycasts = activate;
        group.interactable = activate;
    }


    public void HandleQuestClicked(QuestSO questSO)
    {
        this.questSO = questSO;

        questNameText.text = questSO.questName;
        questDescriptionText.text = questSO.questDescription;

        DisplayObjective();
        DisplayRewards();
    }

    // 从任务列表点击任务时调用（可能未接取也可能已接取）
    public void ShowQuestLogEntry(QuestSO questSO)
    {
        HandleQuestClicked(questSO);

        GameManager.ShowPanel(questCanvas);

        // 根据任务状态决定按钮：未接取→显示接取，已接取且完成→显示完成
        bool isAccepted = questManager.IsQuestAccepted(questSO);
        bool canComplete = isAccepted && questManager.IsQuestComplete(questSO);
        SetCanvasState(acceptCanvasGroup, !isAccepted);
        SetCanvasState(declineCanvasGroup, true);
        SetCanvasState(completeCanvasGroup, canComplete);
    }

    // 任务进度变化时实时刷新目标显示和按钮状态
    private void RefreshObjectives()
    {
        if (questSO != null && questCanvas.alpha > 0)
        {
            DisplayObjective();

            // 目标全部完成时自动切换为可提交状态
            if (questManager.IsQuestAccepted(questSO) && questManager.IsQuestComplete(questSO))
            {
                SetCanvasState(acceptCanvasGroup, false);
                SetCanvasState(declineCanvasGroup, true);
                SetCanvasState(completeCanvasGroup, true);
            }
        }
    }

    private void DisplayObjective()
    {
        for (int i = 0; i < objectiveSlots.Length; i++)
        {
            if( i < questSO.objectives.Count)
            {
                var objective = questSO.objectives[i];
                questManager.UpdateObjectiveProgress(questSO, objective);

                int currentAmount = questManager.GetCurrentAmount(questSO,objective);
                string progress = questManager.GetProgressText(questSO, objective);
                bool isComplete = currentAmount >= objective.requiredAmount;


                objectiveSlots[i].RefreshObjectives(objective.description, progress,isComplete);
                objectiveSlots[i].gameObject.SetActive(true);

            }
            else
            {
                objectiveSlots[i].gameObject.SetActive(false);
            }
        }
    }

    private void DisplayRewards()
    {
        for (int i = 0; i < rewardSlots.Length; i++)
        {
            if( i < questSO.rewards.Count)
            {
                var reward = questSO.rewards[i];
                rewardSlots[i].DiaplayReward(reward.itemSo.icon, reward.quantity);


                rewardSlots[i].gameObject.SetActive(true);
            }
            else
            {
                rewardSlots[i].gameObject.SetActive(false);
            }
        }
    }
}
