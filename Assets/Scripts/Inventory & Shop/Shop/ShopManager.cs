using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    

    [SerializeField] private ShopSlot[] shopSlots;

    [SerializeField] private InventoryManger inventoryManger;

    private void Awake()
    {
        if (inventoryManger == null)
            inventoryManger = FindObjectOfType<InventoryManger>();
    }

    public void PopulateShopItems(List<ShopItems> shopItems)
    {
        for (int i = 0; i < shopItems.Count && i < shopSlots.Length; i++)
        {
            ShopItems shopItem = shopItems[i];
            shopSlots[i].Initialize(shopItem.itemSo, shopItem.price);
            shopSlots[i].gameObject.SetActive(true);
        }

        for (int i = shopItems.Count;i < shopSlots.Length;i++)
        {
            shopSlots[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 尝试购买道具：检查金币和背包空间后执行购买
    /// </summary>
    /// <param name="itemSo">要购买的道具</param>
    /// <param name="price">道具价格</param>
    public void TryBuyItem(ItemSo itemSo,int price)
    {
        if(itemSo != null && inventoryManger.gold>=price)
        {
            if(HasSpaceForItem(itemSo))
            {
                inventoryManger.gold -= price;
                inventoryManger.goldText.text = inventoryManger.gold.ToString();
                inventoryManger.AddItem(itemSo, 1);
            }
        }
    }


    private bool HasSpaceForItem(ItemSo itemSo)
    {
        foreach (var slot in inventoryManger.itemSlots)
        {
            if(slot.itemSo == itemSo && slot.quantity<itemSo.stackSize)
                return true;
            else if(slot.itemSo == null)
                return true;
        }
        return false;
    }


    // 出售道具：若该道具不在本商店售卖列表中则返回false
    public bool SellItem(ItemSo itemSo)
    {
        if(itemSo == null)
            return false;

        foreach (var slot in shopSlots)
        {
            if(slot.itemSo == itemSo)
            {
                inventoryManger.gold += slot.price;
                inventoryManger.goldText.text = inventoryManger.gold.ToString();
                return true;
            }
        }
        return false;
    }
}

[System.Serializable]
public class ShopItems
{
    public ItemSo itemSo;
    public int price;
}
