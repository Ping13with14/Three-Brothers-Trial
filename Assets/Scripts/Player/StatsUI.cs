using TMPro;
using UnityEngine;

/// <summary>
/// 属性面板UI：按 Tab 切换显示，展示伤害、速度等玩家属性
/// </summary>
public class StatsUI : MonoBehaviour
{
    public GameObject[] statsSlots;          // 属性槽位数组（0=伤害, 1=速度）
    public CanvasGroup statsCanvas;          // 属性面板 CanvasGroup

    private bool statsOpen = false;          // 面板是否已打开

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

    /// <summary>
    /// 刷新伤害属性显示
    /// </summary>
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

    /// <summary>
    /// 刷新速度属性显示
    /// </summary>
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

    /// <summary>
    /// 刷新全部属性显示
    /// </summary>
    public void UpdateAllStates()
    {
        UpdateDamage();
        UpdateSpeed();
    }
}
