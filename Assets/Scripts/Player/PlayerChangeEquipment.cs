using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 装备切换：按切换键在近战(Combat)和远程(Bow)之间切换
/// </summary>
public class PlayerChangeEquipment : MonoBehaviour
{
    public PlayerCombat combat;    // 近战战斗组件
    public PlayerBow bow;          // 弓箭组件

    void Update()
    {
        // 按下切换装备键时，交替启用/禁用近战和远程组件
        if(InputManager.Provider.IsChangeEquipmentPressed)
        {
            combat.enabled = !combat.enabled;
            bow.enabled = !bow.enabled;
        }
    }
}
