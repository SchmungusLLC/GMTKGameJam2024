using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Player;

public partial class Enemy : MonoBehaviour
{
    // components
    public Rigidbody rb3D;
    public Animator animator;

    [Header("Movement")]
    public float moveSpeed = 3;
    public bool aggroPlayer = false;

    public float aggroDistance = 10;
    public float fightDistance = 1;

    public bool isHitStunned;

    [Header("Combat Stats")]
    [Tooltip("Total duration of attacks in seconds")]
    public float attackDuration;

    [Tooltip("Total Number of Health Points (HP)")]
    public float maxHP;
    [Tooltip("Number of HP regenerated per second")]
    public float HPRegenRate;
    [Tooltip("Time in seconds after last expending HP before regen starts")]
    public float HPRegenDelay;

    // current HP value
    private float currentHP;

    [Header("Hitbox Settings")]
    [Tooltip("Layers which this attack's hitbox should check against")]
    public LayerMask hitLayers;
    [Tooltip("Half-extents for horizontal hitbox")]
    public Vector3 hbSizeH;
    [Tooltip("Half-extents for vertical hitbox")]
    public Vector3 hbSizeV;
    [Tooltip("Forward offset for hitboxes")]
    public float hbOffset;

    // list of targets struck by attack hitbox
    private Collider[] targetsStruck;

    [Header("UI")]
    [Tooltip("Canvas GameObject holding player's in-world UI elements")]
    public GameObject playerWorldCanvas;
    [Tooltip("UI Slider for HP Points")]
    public Slider HPBar;
    [Tooltip("Transform of the player's HP bar (used to lock rotation)")]
    public Transform HPBarTransform;

    private void Awake()
    {
        rb3D = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        HPBar.maxValue = currentHP = maxHP;
        HPBar.value = currentHP;
        HPBarTransform = HPBar.gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        //if (isRegenHP) { RegenHP(); }
        //else if (currentHP != maxHP) { HPTimer(); }
        CombatActions();
    }

    public void CombatActions()
    {
        if (isHitStunned) { return; }

        if (aggroPlayer)
        {
            if (Vector3.Distance(rb3D.position, player.rb3D.position) < fightDistance)
            {
                Attack();
            }
            else
            {
                // Calculate the direction to the player
                Vector3 direction = (player.rb3D.position - rb3D.position).normalized;

                // Move the Rigidbody towards the player
                rb3D.velocity = direction * moveSpeed;
            }
        }
        else
        {
            if (Vector3.Distance(rb3D.position, player.rb3D.position) < aggroDistance)
            {
                Debug.Log("Enemy " + gameObject.name + " aggroed");
                aggroPlayer = true;
            }
            else
            {
                // wander around?
            }
        }
    }

    public void Attack()
    {
        Debug.Log("Enemy Attacking");
        animator.Play("Attack");
    }

    public void IncomingAttack(char playerAttackDir)
    {
        Debug.Log($"Enemy was hit from {playerAttackDir}");
        switch (playerAttackDir)
        {
            case 'U':
                TakeDamage(2);
                break;
            case 'D':
                TakeDamage(1);
                break;
            case 'L':
                TakeDamage(1);
                break;
            case 'R':
                TakeDamage(1);
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        HPBar.value = currentHP;
        if (currentHP < 0)
        {
            EnemyDies();
        }
        else
        {
            animator.Play("Hit");
        }
    }

    public void EnemyDies()
    {
        gameObject.SetActive(false);
    }
}
