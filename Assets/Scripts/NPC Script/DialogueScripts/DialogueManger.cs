using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 对话管理器：处理对话的显示、推进、选项分支和结束，管理对话冷却
/// </summary>
public class DialogueManger : MonoBehaviour
{
    [Header("UI 组件")]
    public CanvasGroup canvasGroup;          // 对话面板 CanvasGroup
    public Image portrait;                   // 说话者头像
    public TMP_Text actorName;               // 说话者名称文本
    public TMP_Text dialogueText;            // 对话内容文本
    public Button[] choiceButtons;           // 选项按钮数组

    [Header("对话状态")]
    public bool isDialogueActive;            // 当前是否正在进行对话

    private DialogueSO currentDialogue;      // 当前对话数据
    private int dialogueIndex;               // 当前对话行索引

    private float lastDialogueEndTime;       // 上次对话结束时间（用于冷却）
    private float dialogueCooldown = .1f;    // 对话冷却时间（秒），防止快速重复触发

    /// <summary>
    /// 初始化：隐藏对话面板和所有选项按钮
    /// </summary>
    private void Awake()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        foreach (var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 检查对话冷却时间，防止快速重复对话
    /// </summary>
    public bool CanStartDialogue()
    {
        return Time.unscaledTime - lastDialogueEndTime >= dialogueCooldown;

    }

    /// <summary>
    /// 开始一段新对话：设置当前对话、重置索引、显示第一行
    /// </summary>
    public void StartDialogue(DialogueSO dialogueSO)
    {
        currentDialogue = dialogueSO;
        dialogueIndex = 0;
        isDialogueActive = true;
        ShowDialogue();
    }

    /// <summary>
    /// 推进对话到下一句或显示选项（由玩家按交互键触发）
    /// </summary>
    public void AdvanceDialogue()
    {
        if (dialogueIndex < currentDialogue.lines.Length)
        {
            ShowDialogue();
        }
        else
            ShowChoices();
    }

    /// <summary>
    /// 显示当前对话行的内容：更新头像、名称、文本，记录说话者历史
    /// </summary>
    private void ShowDialogue()
    {
        DialogueLine line = currentDialogue.lines[dialogueIndex];

        GameManager.Instance.DialogueHistoryTracker.RecordNPC(line.speaker);

        portrait.sprite = line.speaker.portratait;
        actorName.text = line.speaker.actorName;

        dialogueText.text = line.text;

        GameManager.ShowPanel(canvasGroup);

        dialogueIndex++;
    }

    /// <summary>
    /// 显示玩家可选的对话选项：有选项时绑定分支跳转，无选项时判断任务提交/提供/结束
    /// </summary>
    private void ShowChoices()
    {
        ClearChoices();
        if (currentDialogue.options.Length > 0)
        {
            // 遍历选项并绑定到按钮
            for (int i = 0; i < currentDialogue.options.Length;i++)
            {
                var option= currentDialogue.options[i];

                choiceButtons[i].GetComponentInChildren<TMP_Text>().text = option.optionText;
                choiceButtons[i].gameObject.SetActive(true);


                choiceButtons[i].onClick.AddListener(()=>ChooseOption(option.nextDialogue));

            }
            EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject);
        }
        // 无选项时提供默认行为
        else
        {
            // 有可提交的任务时触发提交
            if(currentDialogue.turnInQuestOnEnd != null && GameManager.Instance.QuestManager.IsQuestComplete(currentDialogue.turnInQuestOnEnd))
            {
                GameEvents.OnQuestTurnInRequested?.Invoke(currentDialogue.turnInQuestOnEnd);
                EndDialogue();
            }
            // 有可提供的新任务时触发任务提供
            else if (currentDialogue.offerquestOnEnd != null)
            {
                EndDialogue();
                GameEvents.OnQuestOfferRequested?.Invoke(currentDialogue.offerquestOnEnd);
            }
            else
            // 无选项无任务时提供"结束"按钮
            {
                choiceButtons[0].GetComponentInChildren<TMP_Text>().text = "结束";
                choiceButtons[0].onClick.AddListener(EndDialogue);
                choiceButtons[0].gameObject.SetActive(true);
                EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject);
            }
        }
    }

    /// <summary>
    /// 玩家选择对话分支：有后续对话则跳转，无则结束对话
    /// </summary>
    private void ChooseOption(DialogueSO dialogueSO)
    {
        if (dialogueSO == null)
            EndDialogue();
        else
        {
            ClearChoices();
            StartDialogue(dialogueSO);
        }
    }

    /// <summary>
    /// 结束对话：隐藏面板、清除选项、记录冷却时间
    /// </summary>
    public void EndDialogue()
    {
        dialogueIndex = 0;
        isDialogueActive = false;
        ClearChoices();
        GameManager.HidePanel(canvasGroup);

        lastDialogueEndTime = Time.unscaledTime;
    }

    /// <summary>
    /// 清空所有选项按钮的监听和显示
    /// </summary>
    private void ClearChoices()
    {
        foreach (var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }
    }

}
