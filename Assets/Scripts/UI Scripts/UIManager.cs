using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 底部菜单栏管理：切换 Stats、Skills、Quests 子面板，菜单栏本身不触发暂停
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("菜单栏")]
    [SerializeField] private CanvasGroup menuBar;          // 底部菜单栏 CanvasGroup
    private bool isMenuActive;                             // 菜单栏是否展开

    [Header("子面板")]
    [SerializeField] private CanvasGroup statsMenu;        // 属性面板
    [SerializeField] private CanvasGroup skillsMenu;       // 技能树面板
    [SerializeField] private CanvasGroup questsMenu;       // 任务面板

    [Header("菜单按钮图标")]
    [SerializeField] private Image menuToggleImage;        // 菜单按钮 Image（用于切换图标）
    [SerializeField] private Sprite openSprite;            // 菜单关闭时的图标（点击可打开）
    [SerializeField] private Sprite closeSprite;           // 菜单开启时的图标（点击可关闭）

    /// <summary>
    /// 打开目标子面板，关闭其余子面板并收起菜单栏（通过 GameManager 统一管理暂停）
    /// </summary>
    public void ToggleMenu(CanvasGroup target)
    {
        // 先关闭所有子面板，再打开目标
        GameManager.HidePanel(statsMenu);
        GameManager.HidePanel(skillsMenu);
        GameManager.HidePanel(questsMenu);
        GameManager.ShowPanel(target);

        // 打开子面板后自动收起菜单栏，防止菜单栏遮挡子面板的交互
        isMenuActive = false;
        SetMenuBarState(menuBar, false);
        menuToggleImage.sprite = openSprite;

        // 技能树打开时禁用 Quest 画布组射线，防止任务槽位遮挡技能按钮点击
        if (target == skillsMenu)
            SetQuestCanvasRaycasts(false);
        else
            SetQuestCanvasRaycasts(true);
    }

    /// <summary>
    /// 禁用/启用所有名称含"Quest"的 CanvasGroup 射线阻挡
    /// </summary>
    private void SetQuestCanvasRaycasts(bool enable)
    {
        foreach (CanvasGroup g in FindObjectsOfType<CanvasGroup>())
        {
            if (g.gameObject.name.Contains("Quest"))
                g.blocksRaycasts = enable;
        }
    }

    /// <summary>
    /// 切换底部菜单栏的显示（菜单栏本身不暂停游戏）
    /// </summary>
    public void ToggleMainMenu()
    {
        // 播放UI点击音效
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("UI点击_BlipSelect");

        isMenuActive = !isMenuActive;
        SetMenuBarState(menuBar, isMenuActive);
        menuToggleImage.sprite = isMenuActive ? closeSprite : openSprite;

        // 收起菜单栏时同时关闭所有子面板
        GameManager.HidePanel(statsMenu);
        GameManager.HidePanel(skillsMenu);
        GameManager.HidePanel(questsMenu);

        EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// 仅设置菜单栏本身的 CanvasGroup 状态（不触发暂停）
    /// </summary>
    private void SetMenuBarState(CanvasGroup group, bool isActive)
    {
        group.alpha = isActive ? 1 : 0;
        group.interactable = isActive;
        group.blocksRaycasts = isActive;
    }
}
