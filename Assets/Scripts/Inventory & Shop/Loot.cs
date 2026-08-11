using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 掉落物：玩家靠近后触发拾取，支持对象池复用
/// </summary>
public class Loot : MonoBehaviour
{
    [Header("物品数据")]
    public ItemSo itemSo;                  // 关联的物品数据
    public int quantity;                   // 物品数量

    [Header("组件引用")]
    public SpriteRenderer sr;              // 物品图标渲染器
    public Animator anim;                  // 拾取动画控制器

    [Header("拾取状态")]
    public bool canBePickUp = true;        // 是否可被拾取

    /// <summary>
    /// 对象池引用，由 InventoryManger 在初始化时设置
    /// </summary>
    public static ObjectPool<Loot> Pool { get; set; }


    private void OnValidate()
    {
        if (itemSo == null)
            return;

       UpdateAppearance();
    }

    /// <summary>
    /// 初始化掉落物：设置物品数据并延迟可拾取
    /// </summary>
    public void Initialize(ItemSo itemSo,int quantity)
    {
        this.itemSo = itemSo;
        this.quantity = quantity;

        canBePickUp = false;
        UpdateAppearance();
    }

    /// <summary>
    /// 同步外观：图标和名称
    /// </summary>
    private void UpdateAppearance()
    {
        sr.sprite = itemSo.icon;
        this.name = itemSo.itemName;
    }

    /// <summary>
    /// 玩家进入触发器时拾取：播放动画、触发物品事件、延迟返回对象池
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && canBePickUp == true )
        {
            anim.Play("PickWood");
            // 播放拾取音效
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX("拾取金币_PickupCoin");
            GameEvents.OnItemLooted?.Invoke(itemSo, quantity);
            StartCoroutine(ReturnToPoolAfterDelay(.5f));
        }
    }

    /// <summary>
    /// 延迟后返回对象池（等待拾取动画播放完毕）
    /// </summary>
    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canBePickUp = true;
        itemSo = null;
        if (Pool != null)
            Pool.Return(this);
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 玩家离开触发器后重置可拾取状态
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canBePickUp = true;
        }
    }
}
