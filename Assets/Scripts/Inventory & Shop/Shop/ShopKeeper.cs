using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    public static ShopKeeper currentShopKeeper;

    public Animator anim;
    public CanvasGroup shopCanvasGroup;
    public ShopManager shopManager;

    [SerializeField] private List<ShopItems> shopItems;
    [SerializeField] private List<ShopItems> shopWeapons;
    [SerializeField] private List<ShopItems> shopArmour;

    [SerializeField] private Camera shopkeeperCam;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, -1);

    public static event Action<ShopManager, bool> OnShopStateChange;

    private bool playerInRange;


    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            if (InputManager.Provider.IsInteractionPressed)
            {
                
                currentShopKeeper = this;
                OnShopStateChange?.Invoke(shopManager, true);
                GameManager.ShowPanel(shopCanvasGroup);

                shopkeeperCam.transform.position = transform.position + cameraOffset;
                shopkeeperCam.gameObject.SetActive(true);

                OpenItemShop();
               
            }
            else if (InputManager.Provider.IsCancelPressed)
            {
                currentShopKeeper = null;
                OnShopStateChange?.Invoke(shopManager, false);
                GameManager.HidePanel(shopCanvasGroup);

                shopkeeperCam.gameObject.SetActive(false);

            }
        }
    }

    public void OpenItemShop()
    {
        shopManager.PopulateShopItems(shopItems);
    }

    public void OpenWeaponShop()
    {
        shopManager.PopulateShopItems(shopWeapons);
    }

    public void OpenArmourShop()
    {
        shopManager.PopulateShopItems(shopArmour);
    }
    



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", true);
            playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            anim.SetBool("playerInRange", false);
            playerInRange = false;
        }
    }
}
