using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ѷ��ʵص㴥����
/// </summary>
public class LocationVisitedTrigger : MonoBehaviour
{
    [SerializeField] private LocationSO locationVisited;
    [SerializeField] private bool destoryOnTouch = true;

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
