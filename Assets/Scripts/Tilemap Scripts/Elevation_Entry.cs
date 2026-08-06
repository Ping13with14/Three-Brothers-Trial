using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevation_Entry : MonoBehaviour
{

    public Collider2D[] mountainColliders;
    public Collider2D[] boundaryColliders;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 切换山体碰撞器的启用状态
            if (mountainColliders != null)
            {
                foreach (Collider2D mountain in mountainColliders)
                {
                    if (mountain != null)
                        mountain.enabled = !mountain.enabled;
                }
            }

            // 切换边界碰撞器的启用状态
            if (boundaryColliders != null)
            {
                foreach (Collider2D boundary in boundaryColliders)
                {
                    if (boundary != null)
                        boundary.enabled = !boundary.enabled;
                }

                // 根据第一个边界碰撞器状态调整玩家层级
                if (boundaryColliders.Length > 0 && boundaryColliders[0] != null)
                    collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = boundaryColliders[0].enabled ? 15 : 10;
            }
        }
    }
}
