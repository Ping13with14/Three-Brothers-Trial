using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public ItemSo itemSo;
    public SpriteRenderer sr;
    public Animator anim;

    public bool canBePickUp = true;
    public int quantity;
    // 物品拾取事件已迁移至 GameEvents.OnItemLooted

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


    public void Initialize(ItemSo itemSo,int quantity)
    {
        this.itemSo = itemSo;
        this.quantity = quantity;

        canBePickUp = false;
        UpdateAppearance();
    }


    private void UpdateAppearance()
    {
        sr.sprite = itemSo.icon;
        this.name = itemSo.itemName;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && canBePickUp == true )
        {
            anim.Play("PickWood");
            GameEvents.OnItemLooted?.Invoke(itemSo, quantity);
            StartCoroutine(ReturnToPoolAfterDelay(.5f));
        }
    }


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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canBePickUp = true;
        }
    }
}
