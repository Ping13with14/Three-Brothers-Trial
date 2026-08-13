using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

/// <summary>
/// Cinemachine 边界查找器：初始场景与场景切换后，按名称查找并绑定 Confiner 边界碰撞体
/// </summary>
public class ConfinerFinder : MonoBehaviour
{
    private CinemachineConfiner2D confiner;

    /// <summary>
    /// 初始场景绑定：进入 Play 时已打开的场景不会触发 sceneLoaded，需在 Awake 手动绑定一次
    /// </summary>
    private void Awake()
    {
        confiner = GetComponent<CinemachineConfiner2D>();
        BindConfiner();
    }

    /// <summary>
    /// 启用时订阅场景加载事件
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// 禁用时退订事件
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 场景加载完成后重新绑定边界（游戏内切换场景时由 SceneManager.sceneLoaded 回调）
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BindConfiner();
    }

    /// <summary>
    /// 按名称查找 Confiner 物体，将其 PolygonCollider2D 赋给 Confiner2D 并刷新缓存
    /// </summary>
    private void BindConfiner()
    {
        if (confiner == null)
        {
            Debug.LogWarning("ConfinerFinder：未找到 CinemachineConfiner2D 组件，请确认脚本挂在 CM vcam1 上");
            return;
        }

        GameObject confinerGO = GameObject.Find("Confiner");
        if (confinerGO == null)
        {
            Debug.LogWarning("ConfinerFinder：未找到名为 Confiner 的物体，请确认场景中存在且处于激活状态");
            return;
        }

        confiner.m_BoundingShape2D = confinerGO.GetComponent<PolygonCollider2D>();
        confiner.InvalidateCache();   // 运行时赋值后需让 Confiner 重新计算缓存
    }
}
