using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour
{
    public ItemSo itemSo;
    public int quantity;

    public Image itemImage;
    public TMP_Text quantityText;

    private InventoryManger inventoryManger;
    private static ShopManager activeShop;


    private void Start()
    {
        inventoryManger = GetComponentInParent<InventoryManger>();

        // 挂载EventTrigger处理左右键点击，避免子物体Button拦截冒泡
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
                    if (itemSo.currentHealth > 0 && StatsManager.Instance != null
                        && StatsManager.Instance.currentHealth >= StatsManager.Instance.maxHealth)
                        return;
                    inventoryManger.UserItem(this);
                }
            }
            else if (pointerData.button == PointerEventData.InputButton.Right)
            {
                inventoryManger.DropItem(this);
            }
        });
        trigger.triggers.Add(entry);
    }

    private void OnEnable()
    {
        ShopKeeper.OnShopStateChange += HandleShopStateChanged;
    }

    private void OnDisable()
    {
        ShopKeeper.OnShopStateChange -= HandleShopStateChanged;
    }

    private void HandleShopStateChanged(ShopManager shopManager, bool isOpen)
    {
        activeShop = isOpen ? shopManager : null;
    }

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
