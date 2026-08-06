using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBow : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject arrowPrefab;

    public PlayerMovement playerMovement;

    private Vector2 aimDtrection = Vector2.right;

    public float shootCooldown = .5f;
    private float shootTimer;

    public Animator anim;

    // Update is called once per frame
    void Update()
    {
        shootTimer -= Time.deltaTime;
        HandleAiming();

        if (InputManager.Provider.IsShootPressed && shootTimer<=0)
        {
            playerMovement.isShooting = true;
            anim.SetBool("isShooting", true);
        }
    }

    private void OnEnable()
    {
        anim.SetLayerWeight(0, 0);
        anim.SetLayerWeight(1, 1);
    }

    private void OnDisable()
    {
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 0);
    }


    private void HandleAiming()
    {
        float horizontal = InputManager.Provider.HorizontalRaw;
        float vertical = InputManager.Provider.VerticalRaw;
        if (horizontal != 0 || vertical != 0)
        {
            aimDtrection = new Vector2(horizontal, vertical).normalized;
        }
    }

    public void Shoot()
    {
        if (shootTimer <= 0)
        {
            Arrow arrow = Instantiate(arrowPrefab, launchPoint.position, Quaternion.identity).GetComponent<Arrow>();
            arrow.direction = aimDtrection;
            shootTimer = shootCooldown;
        } 
        anim.SetBool("isShooting", false);
        playerMovement.isShooting = false;
    }
}
