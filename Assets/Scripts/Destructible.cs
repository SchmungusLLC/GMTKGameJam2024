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

    public void Struck(float damage)
    {
        TakeDamage(damage);
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
