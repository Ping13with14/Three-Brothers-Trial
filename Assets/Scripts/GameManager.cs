using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏全局管理器：单例，处理场景切换时的跨场景引用保留、面板暂停/恢复
/// </summary>
public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// 暂停计数：多个面板叠加打开时累加，全部关闭后才恢复 timeScale
    /// </summary>
    private static int pauseCount = 0;

    /// <summary>
    /// 显示面板并暂停游戏（支持多层叠加，仅第一层触发暂停）
    /// </summary>
    public static void ShowPanel(CanvasGroup group)
    {
        if (group == null || group.alpha > 0) return;

        group.alpha = 1;
        group.interactable = true;
        group.blocksRaycasts = true;

        if (pauseCount == 0)
            Time.timeScale = 0;
        pauseCount++;
    }

    /// <summary>
    /// 隐藏面板，全部关闭后恢复游戏时间
    /// </summary>
    public static void HidePanel(CanvasGroup group)
    {
        if (group == null || group.alpha == 0) return;

        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;

        pauseCount--;
        if (pauseCount <= 0)
        {
            pauseCount = 0;
            Time.timeScale = 1;
        }
    }

    [Header("场景引用（运行时自动查找）")]
    public DialogueManger DialogueManger;                // 对话系统
    public DialogueHistoryTracker DialogueHistoryTracker; // 对话历史追踪
    public LocationHistoryTracker LocationHistoryTracker; // 位置历史追踪
    public QuestManager QuestManager;                     // 任务系统

    [Header("跨场景持久化对象")]
    public GameObject[] persistentObject;                  // 需要 DontDestroyOnLoad 保留的对象


    /// <summary>
    /// Awake：单例去重、标记持久化对象、订阅场景加载事件、查找场景引用
    /// </summary>
    protected override void Awake()
    {
        // 单例去重：如果实例已存在（场景重载时），转移引用后销毁自身
        if (Instance != null && Instance != this)
        {
            TransferReferencesTo(Instance);
            CleanUpAndDestroy();
            return;
        }

        base.Awake();
        if (Instance != this) return;

        MarkPersistentObjects();
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindSceneReferences();
    }

    /// <summary>
    /// 销毁时退订场景加载事件
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 场景加载后重新查找场景内的系统引用（由 SceneManager.sceneLoaded 回调）
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindSceneReferences();
    }

    /// <summary>
    /// 在场景中查找各系统组件引用（DialogueManger 等）
    /// </summary>
    private void FindSceneReferences()
    {
        if (DialogueManger == null)
            DialogueManger = FindObjectOfType<DialogueManger>();
        if (DialogueHistoryTracker == null)
            DialogueHistoryTracker = FindObjectOfType<DialogueHistoryTracker>();
        if (LocationHistoryTracker == null)
            LocationHistoryTracker = FindObjectOfType<LocationHistoryTracker>();
        if (QuestManager == null)
            QuestManager = FindObjectOfType<QuestManager>();
    }

    /// <summary>
    /// 将当前实例的引用转移给目标实例（单例去重时防止引用丢失）
    /// </summary>
    private void TransferReferencesTo(GameManager target)
    {
        if (target.DialogueManger == null && DialogueManger != null)
            target.DialogueManger = DialogueManger;
        if (target.DialogueHistoryTracker == null && DialogueHistoryTracker != null)
            target.DialogueHistoryTracker = DialogueHistoryTracker;
        if (target.LocationHistoryTracker == null && LocationHistoryTracker != null)
            target.LocationHistoryTracker = LocationHistoryTracker;
        if (target.QuestManager == null && QuestManager != null)
            target.QuestManager = QuestManager;
    }

    /// <summary>
    /// 将持久化对象数组中的对象标记为 DontDestroyOnLoad
    /// </summary>
    private void MarkPersistentObjects()
    {
        foreach(GameObject obj in persistentObject)
        {
            if(obj != null)
            {
                DontDestroyOnLoad(obj);
            }
        }
    }

    /// <summary>
    /// 清理不属于新实例的持久化对象，然后销毁自身 GameObject（单例去重时调用）
    /// </summary>
    private void CleanUpAndDestroy()
    {
        foreach (GameObject obj in persistentObject)
        {
            if (obj != null && ShouldDestroy(obj))
                DestroyImmediate(obj);
        }
        DestroyImmediate(gameObject);
    }

    /// <summary>
    /// 判断对象是否应被销毁：已在目标实例中保留的对象不应销毁
    /// </summary>
    private bool ShouldDestroy(GameObject obj)
    {
        if (DialogueManger != null && DialogueManger.gameObject == obj) return false;
        if (DialogueHistoryTracker != null && DialogueHistoryTracker.gameObject == obj) return false;
        if (LocationHistoryTracker != null && LocationHistoryTracker.gameObject == obj) return false;
        return true;
    }
}
