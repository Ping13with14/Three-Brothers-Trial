using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 商店分类切换按钮：点击按钮时切换到对应的商品分类（物品/武器/防具）
/// </summary>
public class ShopButtonToggles : MonoBehaviour
{
    /// <summary>
    /// 切换到物品商店
    /// </summary>
    public void OpenItemShop()
    {
        if(ShopKeeper.currentShopKeeper != null )
        {
            ShopKeeper.currentShopKeeper.OpenItemShop();
        }
    }

    /// <summary>
    /// 切换到武器商店
    /// </summary>
    public void OpenWeaponShop()
    {
        if (ShopKeeper.currentShopKeeper != null)
        {
            ShopKeeper.currentShopKeeper.OpenWeaponShop();
        }
    }

    /// <summary>
    /// 切换到防具商店
    /// </summary>
    public void OpenArmourShop()
    {
        if (ShopKeeper.currentShopKeeper != null)
        {
            ShopKeeper.currentShopKeeper.OpenArmourShop();
        }
    }
}
