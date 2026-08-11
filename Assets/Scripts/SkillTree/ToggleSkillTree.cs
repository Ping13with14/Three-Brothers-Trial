using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能树面板切换：按 ToggleSkillTree 键开关技能树面板
/// </summary>
public class ToggleSkillTree : MonoBehaviour
{
    public CanvasGroup statsCanvas;           // 技能树面板 CanvasGroup（命名源自 Stats，实际为技能树）
    private bool skillTreeOpen = false;       // 技能树是否已打开

    void Update()
    {
        if (InputManager.Provider.IsToggleSkillTreePressed)
        {
            if (skillTreeOpen)
            {
                GameManager.HidePanel(statsCanvas);
                SetQuestCanvasRaycasts(true);
                skillTreeOpen = false;
            }
            else
            {
                GameManager.ShowPanel(statsCanvas);
                SetQuestCanvasRaycasts(false);
                skillTreeOpen = true;
            }
        }
    }

    /// <summary>
    /// 禁用/启用所有名称含"Quest"的 CanvasGroup 射线阻挡：打开技能树时禁用 Quest 画布射线，防止任务槽位遮挡技能按钮点击
    /// </summary>
    private void SetQuestCanvasRaycasts(bool enable)
    {
        foreach (CanvasGroup g in FindObjectsOfType<CanvasGroup>())
        {
            if (g.gameObject.name.Contains("Quest"))
                g.blocksRaycasts = enable;
        }
    }
}
