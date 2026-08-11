using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地点访问触发器：玩家进入时记录该地点已访问，可选触碰后销毁
/// </summary>
public class LocationVisitedTrigger : MonoBehaviour
{
    [SerializeField] private LocationSO locationVisited;   // 被访问的地点数据
    [SerializeField] private bool destoryOnTouch = true;    // 触碰后是否销毁触发器

    /// <summary>
    /// 玩家进入触发器时记录地点访问历史
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance != null && GameManager.Instance.LocationHistoryTracker != null)
            {
                GameManager.Instance.LocationHistoryTracker.RecordLocation(locationVisited);
            }
            if (destoryOnTouch)
            {
                Destroy(gameObject);
            }
        }
    }
}
