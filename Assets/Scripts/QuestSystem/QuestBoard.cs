using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 任务看板：玩家靠近后按交互键可接取或提交任务
/// 未配置任务时自动使用兜底提示，避免空引用静默失败
/// </summary>
public class QuestBoard : MonoBehaviour
{
    [SerializeField] private QuestSO questToOffer;
    [SerializeField] private QuestSO questToTurnIn;

    [Header("兜底提示（所有任务都未配置时使用）")]
    [SerializeField] private QuestSO fallbackNoQuest;

    private bool playerInRange;


    private void Update()
    {
        if(playerInRange && InputManager.Provider.IsInteractionPressed)
        {
            // 优先判断是否可以提交已完成的任务
            bool canTurnIn = questToTurnIn != null
                          && GameEvents.IsQuestComplete?.Invoke(questToTurnIn) == true;

            if (canTurnIn)
            {
                GameEvents.OnQuestTurnInRequested?.Invoke(questToTurnIn);
                return;
            }

            // 尝试弹出任务接取界面，无配置任务时使用兜底提示
            QuestSO offer = questToOffer;
            if (offer == null)
            {
                if (fallbackNoQuest != null)
                {
                    GameEvents.OnQuestOfferRequested?.Invoke(fallbackNoQuest);
                }
                else
                {
                    Debug.LogWarning($"[QuestBoard] {gameObject.name} 未配置 questToOffer 也没有 fallbackNoQuest，交互无效");
                }
                return;
            }

            GameEvents.OnQuestOfferRequested?.Invoke(offer);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
