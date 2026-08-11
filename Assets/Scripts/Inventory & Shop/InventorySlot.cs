using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 背包槽位：处理左键使用/右键丢弃物品，商店模式下左键为出售
/// </summary>
public class InventorySlot : MonoBehaviour
{
    [Header("物品数据")]
    public ItemSo itemSo;                      // 槽位中的物品
    public int quantity;                       // 当前数量

    [Header("UI 组件")]
    public Image itemImage;                    // 物品图标
    public TMP_Text quantityText;             // 数量文本

    private InventoryManger inventoryManger;  // 库存管理器引用
    private static ShopManager activeShop;    // 当前打开的商店（非商店模式为 null）

    /// <summary>
    /// 初始化：添加左右键事件监听（左键使用/出售，右键丢弃）
    /// </summary>
    private void Start()
    {
        inventoryManger = GetComponentInParent<InventoryManger>();

        // 挂载 EventTrigger 处理左右键点击，避免子物体 Button 拦截冒泡
        var trigger = gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = gameObject.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) =>
        {
            var pointerData = data as PointerEventData;
            if (pointerData == null || quantity <= 0) return;

            if (pointerData.button == PointerEventData.InputButton.Left)
            {
                // 商店模式下左键为出售，否则为使用物品
                if (activeShop != null)
                {
                    if (activeShop.SellItem(itemSo))
                    {
                        quantity--;
                        UpdateUI();
                    }
                }
                else
                {
                    // 生命值已满时不使用回血物品
                    if (itemSo.currentHealth > 0 && StatsManager.Instance != null
                        && StatsManager.Instance.currentHealth >= StatsManager.Instance.maxHealth)
                        return;
                    inventoryManger.UserItem(this);
                }
            }
            else if (pointerData.button == PointerEventData.InputButton.Right)
            {
                // 右键丢弃1个物品
                inventoryManger.DropItem(this);
            }
        });
        trigger.triggers.Add(entry);
    }

    /// <summary>
    /// 启用时订阅商店状态变化事件
    /// </summary>
    private void OnEnable()
    {
        ShopKeeper.OnShopStateChange += HandleShopStateChanged;
    }

    /// <summary>
    /// 禁用时退订事件
    /// </summary>
    private void OnDisable()
    {
        ShopKeeper.OnShopStateChange -= HandleShopStateChanged;
    }

    /// <summary>
    /// 商店开关时更新 activeShop 引用（由 ShopKeeper.OnShopStateChange 回调）
    /// </summary>
    private void HandleShopStateChanged(ShopManager shopManager, bool isOpen)
    {
        activeShop = isOpen ? shopManager : null;
    }

    /// <summary>
    /// 刷新图标、数量文本、空槽位隐藏
    /// </summary>
    public void UpdateUI()
    {
        if (quantity<=0)
            itemSo = null;

        if(itemSo != null)
        {
            itemImage.sprite = itemSo.icon;
            itemImage.gameObject.SetActive(true);
            quantityText.text = quantity.ToString();
        }
        else
        {
            itemImage.gameObject.SetActive(false);
            quantityText.text = "";
        }
    }
}
