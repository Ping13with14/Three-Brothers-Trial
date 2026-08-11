using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 商店商品槽位：显示单个商品信息，处理悬停提示和购买按钮
/// </summary>
public class ShopSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerMoveHandler
{
    [Header("商品数据")]
    public ItemSo itemSo;                      // 商品对应的物品数据
    public int price;                          // 商品价格

    [Header("UI 组件")]
    public TMP_Text itemNameText;              // 商品名称文本
    public TMP_Text priceText;                 // 价格文本
    public Image itemImage;                    // 商品图标

    [SerializeField] private ShopManager shopManager;  // 商店管理器引用
    [SerializeField] private ShopInfo shopInfo;        // 商品信息面板引用

    private void Awake()
    {
        if (shopManager == null)
            shopManager = GetComponentInParent<ShopManager>();
        if (shopInfo == null)
            shopInfo = GetComponentInParent<ShopInfo>();
    }

    /// <summary>
    /// 初始化商店槽位：设置物品名称、价格和图标
    /// </summary>
    /// <param name="newItemSo">物品数据</param>
    /// <param name="price">商品价格</param>
     public void Initialize(ItemSo newItemSo,int price)
    {
        itemSo = newItemSo;
        itemImage.sprite = itemSo.icon;
        itemNameText.text = itemSo.itemName;
        this.price = price;
        priceText.text = price.ToString();

    }

    /// <summary>
    /// 购买按钮点击回调
    /// </summary>
    public void OnBuyButtonClicked()
    {
        shopManager.TryBuyItem(itemSo, price);
    }

    /// <summary>
    /// 鼠标进入时显示物品信息面板
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        shopInfo.ShowItemInfo(itemSo);
    }

    /// <summary>
    /// 鼠标离开时隐藏物品信息面板
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        shopInfo.HideItemInfo();
    }

    /// <summary>
    /// 鼠标悬停时信息面板跟随鼠标移动
    /// </summary>
    public void OnPointerMove(PointerEventData eventData)
    {
        if(itemSo != null)
        {
            shopInfo.FollowMouse();
        }
    }
}
