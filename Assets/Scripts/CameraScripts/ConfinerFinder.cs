using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

/// <summary>
/// Cinemachine 边界查找器：场景加载后自动为 CinemachineConfiner2D 绑定 Confiner 碰撞体
/// </summary>
public class ConfinerFinder : MonoBehaviour
{
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
    /// 场景加载完成后自动查找并绑定 Confiner 边界碰撞体（由 SceneManager.sceneLoaded 回调）
    /// </summary>
    private void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        CinemachineConfiner2D confiner = GetComponent<CinemachineConfiner2D>();
        confiner.m_BoundingShape2D = GameObject.FindWithTag("Confiner").GetComponent<PolygonCollider2D>();
    }
}
