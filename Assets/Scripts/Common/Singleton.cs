using UnityEngine;

/// <summary>
/// 泛型 MonoBehaviour 单例基类，消除各 Manager 中重复的单例创建逻辑
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // DontDestroyOnLoad 要求对象必须是根 GameObject，先解除父子关系
        transform.SetParent(null);
        Instance = this as T;
        DontDestroyOnLoad(gameObject);
    }
}
