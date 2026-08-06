using TMPro;
using UnityEngine;

/// <summary>
/// 任务日志槽位脚本：显示单个已接取任务的名称和等级
/// </summary>
///

public class QuestLogSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text questLevelText;

    public QuestSO currentQuest;
    // 点击时通知QuestLogUI展示任务详情
    public QuestLogUI questLogUI;

    private void OnValidate()
    {
        if (currentQuest != null)
            SetQuest(currentQuest);
        else
            gameObject.SetActive(false);
    }

    // 绑定任务数据并显示
    public void SetQuest(QuestSO questSO)
    {
        currentQuest = questSO;

        // 显示任务显示名称（中文），而非资产文件名
        questNameText.text = questSO.questName;
        questLevelText.text = "Lv." + questSO.questLevel.ToString();

        gameObject.SetActive(true);
    }

    // 清空槽位（任务完成或被移除时调用）
    public void ClearSlot()
    {
        currentQuest = null;
        gameObject.SetActive(false);
    }

    // 点击槽位时打开任务详情面板
    public void OnSlotCliked()
    {
        questLogUI.ShowQuestLogEntry(currentQuest);
    }

}
