using TMPro;
using UnityEngine;

/// <summary>
/// 任务目标槽位：展示单个目标的描述和完成进度
/// </summary>
public class QuestObjectiveSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text objectiveText;   // 目标描述文本
    [SerializeField] private TMP_Text trackingText;    // 进度追踪文本（如 "3/5"）

    /// <summary>
    /// 刷新目标显示：更新描述、进度文本，已完成目标变灰色
    /// </summary>
    public void RefreshObjectives(string description,string progressText,bool isComplete)
    {
        objectiveText.text = description;
        trackingText.text = progressText;

        Color color = isComplete ? Color.gray : Color.white;
        objectiveText.color = color;
        trackingText.color = color;
    }
}
