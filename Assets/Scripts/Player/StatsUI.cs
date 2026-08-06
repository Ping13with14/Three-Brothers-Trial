using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour
{
    public GameObject[] statsSlots;
    public CanvasGroup statsCanvas;

    private bool statsOpen = false;

    private void Start()
    {
        UpdateAllStates();
    }

    private void Update()
    {
        if (InputManager.Provider.IsToggleStatsPressed)
        {
            if (statsOpen)
            {
                GameManager.HidePanel(statsCanvas);
                statsOpen = false;
            }
            else
            {
                GameManager.ShowPanel(statsCanvas);
                statsOpen = true;
                UpdateAllStates();
            }
        }
    }

    public void UpdateDamage()
    {
        if (statsSlots == null || statsSlots.Length < 1 || statsSlots[0] == null) return;
        if (StatsManager.Instance == null) return;

        int dmg = StatsManager.Instance.damage;
        var text = statsSlots[0].GetComponentInChildren<TMP_Text>();
        if (text != null)
            text.text = "伤害：" + dmg;
        statsSlots[0].SetActive(dmg > 0);
    }

    public void UpdateSpeed()
    {
        if (statsSlots == null || statsSlots.Length < 2 || statsSlots[1] == null) return;
        if (StatsManager.Instance == null) return;

        int spd = StatsManager.Instance.speed;
        var text = statsSlots[1].GetComponentInChildren<TMP_Text>();
        if (text != null)
            text.text = "速度：" + spd;
        statsSlots[1].SetActive(spd > 0);
    }

    public void UpdateAllStates()
    {
        UpdateDamage();
        UpdateSpeed();
    }
}
