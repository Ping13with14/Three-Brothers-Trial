using TMPro;
using UnityEngine;
using System;

/// <summary>
/// 库存管理器：单例，管理物品添加/移除/丢弃/使用，处理金币和经验物品的特殊逻辑，维护 Loot 对象池
/// </summary>
public class InventoryManger : Singleton<InventoryManger>
{
    [Header("背包槽位")]
    public InventorySlot[] itemSlots;          // 背包槽位数组
    public UserItem userItem;                  // 物品使用组件引用
    public int gold;                           // 金币数量
    public TMP_Text goldText;                  // 金币文本显示

    [Header("掉落物")]
    public GameObject lootPrefab;              // 掉落物预制体
    public Transform player;                   // 玩家 Transform（掉落物生成位置）

    private ObjectPool<Loot> lootPool;         // Loot 对象池

    /// <summary>
    /// Awake：初始化 Loot 对象池，预创建10个实例
    /// </summary>
    protected override void Awake()
    {
        transform.SetParent(null);
        base.Awake();
        if (Instance != this) return;

        var lootComponent = lootPrefab.GetComponent<Loot>();
        if (lootComponent != null)
        {
            lootPool = new ObjectPool<Loot>();
            lootPool.Initialize(lootComponent, 10, transform);
            Loot.Pool = lootPool;
        }
    }

    /// <summary>
    /// 初始化：加载金币显示和槽位UI
    /// </summary>
    private void Start()
    {
        if(gold!=0)
        {
            goldText.text=gold.ToString();
        }

        foreach (var slot in itemSlots)
        {
            slot.UpdateUI();
        }
    }

    /// <summary>
    /// 启用时订阅物品拾取事件
    /// </summary>
    private void OnEnable()
    {
        GameEvents.OnItemLooted += AddItem;
    }

    /// <summary>
    /// 禁用时退订事件
    /// </summary>
    private void OnDisable()
    {
        GameEvents.OnItemLooted -= AddItem;
    }

    /// <summary>
    /// 添加物品到背包：由 GameEvents.OnItemLooted 回调，优先堆叠到现有槽位，背包满则生成掉落物
    /// </summary>
    public void AddItem(ItemSo itemSo,int quantity)
    {
        //金币类物品直接加金币
        if(itemSo.isGold)
        {
            gold += quantity;
            goldText.text = gold.ToString();
            // 金币变化也需要通知任务进度刷新（如金币收集型任务目标）
            GameEvents.OnQuestProgressChanged?.Invoke();
            return;
        }

        //经验类物品直接加经验
        if (itemSo.isEXP)
        {
            GameEvents.OnExperienceGained?.Invoke(quantity);
            return;
        }

        //优先堆叠到已有相同物品且未满的槽位
        foreach (var slot in itemSlots)
        {
            if(slot.itemSo == itemSo && slot.quantity < itemSo.stackSize)
            {
                int availableSpace = itemSo.stackSize - slot.quantity;
                int amountToAdd = Mathf.Min(availableSpace, quantity);

                slot.quantity += amountToAdd;
                quantity -= amountToAdd;

                slot.UpdateUI();

                if (quantity <= 0)
                {
                    GameEvents.OnQuestProgressChanged?.Invoke();
                    return;
                }
            }

        }

        //放入空槽位
        foreach (var slot in itemSlots)
        {
            if(slot.itemSo == null)
            {
                slot.itemSo = itemSo;
                slot.quantity = quantity;
                slot.UpdateUI();
                GameEvents.OnQuestProgressChanged?.Invoke();
                return;
            }
        }

        //背包满，生成掉落物
        if (quantity > 0)
        {
            DropLoot(itemSo, quantity);
        }

    }

    /// <summary>
    /// 移除指定数量的物品（用于任务提交消耗）
    /// </summary>
    public void RemoveItem(ItemSo itemSo,int quantity)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            var slot = itemSlots[i];

            if (slot.itemSo != itemSo)
                continue;
            if(slot.quantity > quantity)
            {
                //槽位数量多于移除数量，只减不删
                slot.quantity -= quantity;
                slot.UpdateUI();
                GameEvents.OnQuestProgressChanged?.Invoke();
                quantity = 0;
            }
            else
            {
                //移除槽位中的所有数量
                quantity -= slot.quantity;
                slot.itemSo = null;
                slot.quantity = 0;
                slot.UpdateUI();
                GameEvents.OnQuestProgressChanged?.Invoke();
            }

        }
    }

    /// <summary>
    /// 通过对象池生成掉落物
    /// </summary>
    private void DropLoot(ItemSo itemSo, int quantity)
    {
        if (lootPool == null)
            return;
        Loot loot = lootPool.Get();
        loot.transform.position = player.position;
        loot.Initialize(itemSo, quantity);
    }

    /// <summary>
    /// 丢弃槽位中的1个物品为掉落物（右键丢弃）
    /// </summary>
    public void DropItem(InventorySlot slot)
    {
        DropLoot(slot.itemSo,1);
        slot.quantity --;
        if (slot.quantity <= 0)
        {
            slot.itemSo = null;
        }
        slot.UpdateUI();
        GameEvents.OnQuestProgressChanged?.Invoke();
    }

    /// <summary>
    /// 使用槽位中的物品（左键使用），应用物品效果后减1
    /// </summary>
    public void UserItem(InventorySlot slot)
    {
        if(slot.itemSo != null && slot.quantity>0)
        {
            userItem.ApplyItemEffects(slot.itemSo);

            slot.quantity--;
            if(slot.quantity <= 0)
            {
                slot.itemSo = null;
            }
            slot.UpdateUI();
            GameEvents.OnQuestProgressChanged?.Invoke();
        }
    }

    /// <summary>
    /// 是否拥有某物品
    /// </summary>
    public bool HasItem(ItemSo itemSo)
    {
        foreach(var slot in itemSlots)
        {
            if(slot.itemSo == itemSo && slot.quantity > 0)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 获取物品数量：金币类从 gold 字段获取，普通物品遍历槽位累加
    /// </summary>
    public int GetItemQuantity(ItemSo itemSo)
    {
        if (itemSo.isGold)
            return gold;

        int total = 0;
        foreach (var slot in itemSlots)
        {
            if (slot.itemSo == itemSo)
                total += slot.quantity;
        }
        return total;
    }
}
