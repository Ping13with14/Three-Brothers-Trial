using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC 对话状态：处理玩家交互、对话条件检查、任务接取后移除对话
/// </summary>
public class NPC_Talk : MonoBehaviour
{
    private Rigidbody2D rb;                        // NPC 刚体
    public Animator anim;                          // NPC 动画控制器
    public Animator interactAnim;                  // 交互提示动画（如头顶气泡）

    public List<DialogueSO> conversations;         // 可用对话列表
    public DialogueSO currentConversation;         // 当前匹配到的对话

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 初始化时订阅任务接取事件：接取任务后自动移除提供该任务的对话
    /// </summary>
    private void Start()
    {
        GameEvents.OnQuestAccepted += OnQuestAccepted_RemoveOfferings;
    }

    /// <summary>
    /// 销毁时退订事件
    /// </summary>
    private void OnDestroy()
    {
        GameEvents.OnQuestAccepted -= OnQuestAccepted_RemoveOfferings;
    }

    /// <summary>
    /// 进入对话状态：停止物理移动、播放待机动画、显示交互提示
    /// </summary>
    private void OnEnable()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        anim.Play("Idle");
        interactAnim.Play("Open");
    }

    /// <summary>
    /// 退出对话状态：恢复物理运动、关闭交互提示
    /// </summary>
    private void OnDisable()
    {
        rb.isKinematic = false;
        //interactAnim.Play("Close");
    }

    /// <summary>
    /// 每帧检测交互按键：对话进行中则推进对话，否则尝试开启新对话
    /// </summary>
    private void Update()
    {
        if (InputManager.Provider.IsInteractionPressed)
        {
            if (GameManager.Instance == null || GameManager.Instance.DialogueManger == null)
                return;

            if (GameManager.Instance.DialogueManger.isDialogueActive)
                GameManager.Instance.DialogueManger.AdvanceDialogue();
            else
            {
                if (GameManager.Instance.DialogueManger.CanStartDialogue())
                {
                    CheckForNewConversation();
                    // 无匹配对话时不启动对话，避免传入null导致空异常
                    if (currentConversation != null)
                        GameManager.Instance.DialogueManger.StartDialogue(currentConversation);
                }
            }
        }
    }

    /// <summary>
    /// 遍历对话列表，找到第一个满足条件的对话作为当前对话
    /// </summary>
    private void CheckForNewConversation()
    {
        // 先置空，避免无匹配时保留旧对话引用导致空异常
        currentConversation = null;

        for (int i = 0; i < conversations.Count; i++)
        {
            var convo = conversations[i];
            if (convo != null && convo.IsConditionMet())
            {
                currentConversation = convo;

                // 移除一次性对话
                if(convo.removeAfterPlay)
                    conversations.RemoveAt(i);
                // 移除关联的对话（如任务接取后移除多个相关对话）
                if(convo.removeTheseOnPlay != null && convo.removeTheseOnPlay.Count > 0)
                {
                    foreach(var toRemove in convo.removeTheseOnPlay)
                    {
                        conversations.Remove(toRemove);
                    }
                }
                break;
            }
        }
    }

    /// <summary>
    /// 当任务被接取后，从对话列表中移除提供该任务的对话（由 GameEvents.OnQuestAccepted 回调）
    /// </summary>
    private void OnQuestAccepted_RemoveOfferings(QuestSO acceptedQuest)
    {
        for (int i = conversations.Count - 1; i >= 0; i--)
        {
            var convo = conversations[i];
            if(convo == null) continue;
            if (convo.offerquestOnEnd == acceptedQuest)
                conversations.RemoveAt(i);
        }
    }
}
