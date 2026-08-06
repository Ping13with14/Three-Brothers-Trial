using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopInfo : MonoBehaviour
{
    public CanvasGroup infoPanel;

    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;

    [Header ("Stat Fields")]
    public TMP_Text[] statTexts;

    private RectTransform infoPanelRect;

    private void Awake()
    {
        infoPanelRect = GetComponent<RectTransform>();
    }


    public void ShowItemInfo(ItemSo itemSo)
    {
        infoPanel.alpha = 1;

        itemNameText.text = itemSo.itemName;
        itemDescriptionText.text = itemSo.itemDescription;
        
        List<string> stats = new List<string>();

        if(itemSo.currentHealth > 0 ) stats.Add("生命值: " +  itemSo.currentHealth.ToString());
        if(itemSo.damage > 0 ) stats.Add("攻击: " +  itemSo.damage.ToString());
        if(itemSo.speed > 0 ) stats.Add("速度: " +  itemSo.speed.ToString());
        if(itemSo.duration > 0 ) stats.Add("时间: " +  itemSo.duration.ToString());

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

    public void HideItemInfo()
    {
        infoPanel.alpha = 0;

        itemNameText.text = "";
        itemDescriptionText.text = "";
    }

    public void FollowMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 offset = new Vector3(10, -10, 0);

        infoPanelRect.position = mousePosition + offset;
    }

}
