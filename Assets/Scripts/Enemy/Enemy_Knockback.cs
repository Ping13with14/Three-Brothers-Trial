using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Knockback : MonoBehaviour, IKnockbackable
{
    private Rigidbody2D rb;
    private Enemy_Movemont enemy_Movemont;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemy_Movemont = GetComponent<Enemy_Movemont>();
    }

    public void Knockback(Transform forceTransform,float knockbackForce,float knockbackTime,float stunTime)
    {
        enemy_Movemont.ChangeState(EnemyState.Knockback);
        StartCoroutine(StunTimer(knockbackTime,stunTime));

        Vector2 direction = (transform.position - forceTransform.position).normalized;
        rb.velocity = direction * knockbackForce;
    }

    IEnumerator StunTimer(float knockbackTime, float stunTime)
    {
        yield return new WaitForSeconds(knockbackTime);
        rb.velocity=Vector2.zero;
        yield return new WaitForSeconds(stunTime);
        enemy_Movemont.ChangeState(EnemyState.Idle);
    }



}
