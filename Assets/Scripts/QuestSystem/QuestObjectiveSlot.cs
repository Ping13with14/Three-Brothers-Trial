using TMPro;
using UnityEngine;

/// <summary>
/// 任务目标槽位脚本：展示单个目标的描述和完成进度
/// </summary>
public class QuestObjectiveSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text objectiveText;
    [SerializeField] private TMP_Text trackingText;

    public void RefreshObjectives(string description,string progressText,bool isComplete)
    {
        objectiveText.text = description;
        trackingText.text = progressText;

        Color color = isComplete ? Color.gray : Color.white;
        objectiveText.color = color;
        trackingText.color = color;
    }
}
