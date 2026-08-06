using TMPro;
using UnityEngine;
using System;

public class InventoryManger : Singleton<InventoryManger>
{
    public InventorySlot[] itemSlots;
    public UserItem userItem;
    public int gold;
    public TMP_Text goldText;
    public GameObject lootPrefab;
    public Transform player;

    // 经验获取事件已迁移至 GameEvents.OnExperienceGained

    private ObjectPool<Loot> lootPool;

    protected override void Awake()
    {
        transform.SetParent(null);
        base.Awake();
        if (Instance != this) return;

        // 初始化 Loot 对象池，预创建10个实例
        var lootComponent = lootPrefab.GetComponent<Loot>();
        if (lootComponent != null)
        {
            lootPool = new ObjectPool<Loot>();
            lootPool.Initialize(lootComponent, 10, transform);
            Loot.Pool = lootPool;
        }
    }

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

    private void OnEnable()
    {
        GameEvents.OnItemLooted += AddItem;
    }
    private void OnDisable()
    {
        GameEvents.OnItemLooted -= AddItem;
    }

    public void AddItem(ItemSo itemSo,int quantity)
    {

        //物品是金币时
        if(itemSo.isGold)
        {
            gold += quantity;
            goldText.text = gold.ToString();
            // 金币变化也需要通知任务进度刷新（如金币收集型任务目标）
            GameEvents.OnQuestProgressChanged?.Invoke();
            return;
        }

        //物品有经验时
        if (itemSo.isEXP)
        {
            GameEvents.OnExperienceGained?.Invoke(quantity);
            return;
        }

        //检查背包是否有相同物品
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

        if (quantity > 0)
        {
            DropLoot(itemSo, quantity);
        }

    }


    //移除物品
    public void RemoveItem(ItemSo itemSo,int quantity)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            var slot = itemSlots[i];

            //检查是否是要移除的物品
            if (slot.itemSo != itemSo)
                continue;
            if(slot.quantity > quantity)
            {
                //移除超过需要的数量
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


    private void DropLoot(ItemSo itemSo, int quantity)
    {
        if (lootPool == null)
            return;
        Loot loot = lootPool.Get();
        loot.transform.position = player.position;
        loot.Initialize(itemSo, quantity);
    }

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

    //是否拥有某物品
    public bool HasItem(ItemSo itemSo)
    {
        foreach(var slot in itemSlots)
        {
            if(slot.itemSo == itemSo && slot.quantity > 0)
                return true;
        }
        return false;
    }


    //获取物品数量（金币类物品从gold字段获取，普通物品从背包槽位获取）
    public int GetItemQuantity(ItemSo itemSo)
    {
        // 金币类物品不走背包槽位，直接从gold字段返回
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
