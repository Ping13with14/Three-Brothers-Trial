using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 商品信息面板：鼠标悬停商品时显示物品名称、描述和属性
/// </summary>
public class ShopInfo : MonoBehaviour
{
    [Header("面板")]
    public CanvasGroup infoPanel;              // 信息面板 CanvasGroup

    [Header("基础信息")]
    public TMP_Text itemNameText;              // 物品名称文本
    public TMP_Text itemDescriptionText;       // 物品描述文本

    [Header("属性字段")]
    public TMP_Text[] statTexts;               // 属性文本数组（生命值/伤害/速度/时长）

    private RectTransform infoPanelRect;       // 面板 RectTransform（用于跟随鼠标定位）

    private void Awake()
    {
        infoPanelRect = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 显示物品详细信息：名称、描述、属性（生命值/伤害/速度/时长）
    /// </summary>
    public void ShowItemInfo(ItemSo itemSo)
    {
        infoPanel.alpha = 1;

        itemNameText.text = itemSo.itemName;
        itemDescriptionText.text = itemSo.itemDescription;

        List<string> stats = new List<string>();

        if(itemSo.currentHealth > 0 ) stats.Add("生命值: " +  itemSo.currentHealth.ToString());
        if(itemSo.damage > 0 ) stats.Add("伤害: " +  itemSo.damage.ToString());
        if(itemSo.speed > 0 ) stats.Add("速度: " +  itemSo.speed.ToString());
        if(itemSo.duration > 0 ) stats.Add("时长: " +  itemSo.duration.ToString());

        if (stats.Count <= 0)
            return;
        for(int i = 0;  i < statTexts.Length; i++)
        {
            if (i < stats.Count)
            {
                statTexts[i].text = stats[i];
                statTexts[i].gameObject.SetActive(true);
            }
            else
            {
                statTexts[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 隐藏物品信息面板
    /// </summary>
    public void HideItemInfo()
    {
        infoPanel.alpha = 0;

        itemNameText.text = "";
        itemDescriptionText.text = "";
    }

    /// <summary>
    /// 信息面板跟随鼠标位置（右下偏移10像素）
    /// </summary>
    public void FollowMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 offset = new Vector3(10, -10, 0);

        infoPanelRect.position = mousePosition + offset;
    }

}
