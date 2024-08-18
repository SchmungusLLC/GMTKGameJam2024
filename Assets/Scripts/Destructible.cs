using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class Destructible : MonoBehaviour
{
    public Animator animator;
    public Rigidbody rb3D;

    public float HP;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb3D = GetComponent<Rigidbody>();
    }

    public void Struck(AttackState playerAttackState)
    {
        switch (playerAttackState)
        {
            case AttackState.BigAttack:
                TakeDamage(20);
                break;
            case AttackState.SmallAttack:
                TakeDamage(10);
                break;
            default:
                Debug.LogWarning("WARNING: no attack state");
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        //animator.Play("Hit");
        HP -= damage;
        //HPBar.value = currentHP;
        if (HP < 0)
        {
            Destroyed();
        }
    }

    public void Destroyed()
    {
        gameObject.SetActive(false);
    }
}
