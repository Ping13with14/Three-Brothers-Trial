using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Talk : MonoBehaviour
{
    private Rigidbody2D rb;
    public Animator anim;
    public Animator interactAnim;

    public List<DialogueSO> conversations;
    public DialogueSO currentConversation;


    private void Awake()
    {

        rb = GetComponent<Rigidbody2D>();

    }
    private void Start()
    {
        GameEvents.OnQuestAccepted += OnQuestAccepted_RemoveOfferings;
    }

    private void OnDestroy()
    {
        GameEvents.OnQuestAccepted -= OnQuestAccepted_RemoveOfferings;
    }

    private void OnEnable()
    {

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        anim.Play("Idle");
        interactAnim.Play("Open");

    }

    private void OnDisable()
    {
        rb.isKinematic = false;
        interactAnim.Play("Close");
    }


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

    // 遍历对话列表，找到第一个满足条件的对话作为当前对话
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

    // 当任务被接取后，从对话列表中移除提供该任务的对话
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
