using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品数据定义：ScriptableObject，在编辑器中创建和配置物品属性
/// </summary>
[CreateAssetMenu(fileName ="New Item", menuName = "Inventory/Item")]
public class ItemSo : ScriptableObject
{
    [Header("基本信息")]
    public string itemName;                    // 物品名称
    [TextArea] public string itemDescription;  // 物品描述（支持多行文本）
    public Sprite icon;                        // 物品图标

    [Header("物品类型")]
    public bool isGold;                        // 是否为金币（拾取后直接加金币）
    public bool isEXP;                         // 是否为经验值（拾取后直接加经验）
    public int stackSize = 3;                  // 最大堆叠数量

    [Header("使用效果（Stats）")]
    public int currentHealth;                  // 使用后恢复的当前生命值
    public int maxHealth;                      // 使用后增加的最大生命值上限
    public int speed;                          // 使用后增加的速度值
    public int damage;                         // 使用后增加的攻击力值

    [Header("时效物品")]
    public float duration;                     // 时效物品持续时间（秒），0 表示永久/非时效物品
}
