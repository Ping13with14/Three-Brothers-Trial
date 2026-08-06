using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSkillTree : MonoBehaviour
{
    public CanvasGroup statsCanvas;
    private bool skillTreeOpen = false;

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

    private void SetQuestCanvasRaycasts(bool enable)
    {
        foreach (CanvasGroup g in FindObjectsOfType<CanvasGroup>())
        {
            if (g.gameObject.name.Contains("Quest"))
                g.blocksRaycasts = enable;
        }
    }
}
