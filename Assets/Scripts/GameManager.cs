using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private static int pauseCount = 0;

    /// <summary>
    /// 显示面板并暂停游戏
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
    /// 隐藏面板，全部关闭后恢复游戏
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

    public DialogueManger DialogueManger;
    public DialogueHistoryTracker DialogueHistoryTracker;
    public LocationHistoryTracker LocationHistoryTracker;
    public QuestManager QuestManager;


    [Header("Persitent Object")]
    public GameObject[] persistentObject;

    protected override void Awake()
    {
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

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindSceneReferences();
    }

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

    private void CleanUpAndDestroy()
    {
        foreach (GameObject obj in persistentObject)
        {
            if (obj != null && ShouldDestroy(obj))
                DestroyImmediate(obj);
        }
        DestroyImmediate(gameObject);
    }

    private bool ShouldDestroy(GameObject obj)
    {
        if (DialogueManger != null && DialogueManger.gameObject == obj) return false;
        if (DialogueHistoryTracker != null && DialogueHistoryTracker.gameObject == obj) return false;
        if (LocationHistoryTracker != null && LocationHistoryTracker.gameObject == obj) return false;
        return true;
    }
}
