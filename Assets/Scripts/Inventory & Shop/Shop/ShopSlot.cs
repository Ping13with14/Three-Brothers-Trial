using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerMoveHandler
{
    public ItemSo itemSo;
    public TMP_Text itemNameText;
    public TMP_Text priceText;
    public Image itemImage;

    [SerializeField] private ShopManager shopManager;
    [SerializeField] private ShopInfo shopInfo;

    private void Awake()
    {
        if (shopManager == null)
            shopManager = GetComponentInParent<ShopManager>();
        if (shopInfo == null)
            shopInfo = GetComponentInParent<ShopInfo>();
    }

    public int price;

    /// <summary>
    /// ʵ�����̵��е���Ʒ
    /// </summary>
    /// <param name="itemSo">��Ʒ��Ŀ</param>
    /// <param name="price">��Ʒ�۸�</param>
     public void Initialize(ItemSo newItemSo,int price)
    {
        //�����ȷ�����ƣ��۸����ƷͼƬ
        itemSo = newItemSo;
        itemImage.sprite = itemSo.icon;
        itemNameText.text = itemSo.itemName;
        this.price = price;
        priceText.text = price.ToString();

    }


    public void OnBuyButtonClicked()
    {
        shopManager.TryBuyItem(itemSo, price);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        shopInfo.ShowItemInfo(itemSo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shopInfo.HideItemInfo();
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if(itemSo != null)
        { 
            shopInfo.FollowMouse();
        }
    }
}
