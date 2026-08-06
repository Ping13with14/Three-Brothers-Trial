using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManger : MonoBehaviour
{
    [Header("UI组件")]

    public CanvasGroup canvasGroup;
    public Image portrait;
    public TMP_Text actorName;
    public TMP_Text dialogueText;
    public Button[] choiceButtons;

    public bool isDialogueActive;

    private DialogueSO currentDialogue;
    private int dialogueIndex;

    private float lastDialogueEndTime;
    private float dialogueCooldown = .1f;

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

    // 检查对话冷却时间，防止快速重复对话
    public bool CanStartDialogue()
    {
        return Time.unscaledTime - lastDialogueEndTime >= dialogueCooldown;

    }
    // 开始一段新对话
    public void StartDialogue(DialogueSO dialogueSO)
    {
        currentDialogue = dialogueSO;
        dialogueIndex = 0;
        isDialogueActive = true;
        ShowDialogue();
    }

    // 推进对话到下一句或显示选项
    public void AdvanceDialogue()
    {
        if (dialogueIndex < currentDialogue.lines.Length)
        {
            ShowDialogue();
        }
        else
            ShowChoices();
    }

    // 显示当前对话行的内容
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

    // 显示玩家可选的对话选项
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
            if(currentDialogue.turnInQuestOnEnd != null && GameManager.Instance.QuestManager.IsQuestComplete(currentDialogue.turnInQuestOnEnd))
            {
                GameEvents.OnQuestTurnInRequested?.Invoke(currentDialogue.turnInQuestOnEnd);
                EndDialogue();
            }

            else if (currentDialogue.offerquestOnEnd != null)
            {
                EndDialogue();
                GameEvents.OnQuestOfferRequested?.Invoke(currentDialogue.offerquestOnEnd);
            }
            else
            // 无选项无任务时提供结束对话按钮
            {
                choiceButtons[0].GetComponentInChildren<TMP_Text>().text = "结束";
                choiceButtons[0].onClick.AddListener(EndDialogue);
                choiceButtons[0].gameObject.SetActive(true);
                EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject);
            }
        }
    }

    // 玩家选择对话分支
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




    // 结束对话，隐藏面板并记录冷却时间
    public void EndDialogue()
    {
        dialogueIndex = 0;
        isDialogueActive = false;
        ClearChoices();
        GameManager.HidePanel(canvasGroup);

        lastDialogueEndTime = Time.unscaledTime;
    }

    // 清空所有选项按钮的监听和显示
    private void ClearChoices()
    {
        foreach (var button in choiceButtons)
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }
    }

}
