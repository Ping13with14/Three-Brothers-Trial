using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务奖励槽位：展示单个奖励道具的图标和数量
/// </summary>
public class QuestRewardSlot : MonoBehaviour
{
    public Image rewardImage;              // 奖励物品图标
    public TMP_Text rewardQuantity;       // 奖励数量文本

    /// <summary>
    /// 显示奖励：设置图标和数量
    /// </summary>
    public void DiaplayReward(Sprite sprite, int quantity)
    {
        rewardImage.sprite = sprite;
        rewardQuantity.text = quantity.ToString();
    }
}
