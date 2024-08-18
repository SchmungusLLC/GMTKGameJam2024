using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Player;
using static HelperMethods;

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
    public bool isRecovering;
    public float fallThreshold;

    [Header("Combat Stats")]
    [Tooltip("Total duration of attacks in seconds")]
    public float attackDuration;

    [Tooltip("Total Number of Health Points (HP)")]
    public float maxHP;

    public float collisionDamageMultiplier;
    public float collisionDamageThreshold;

    public LayerMask damagingColliders;

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
        HPBarTransform.eulerAngles = player.cameraFaceDir;
    }

    private void FixedUpdate()
    {
        if (isHitStunned)
        {
            if (!isRecovering && rb3D.velocity.magnitude < 0.01f)
            {
                // Trigger recovery animation
                //Debug.Log("Recovering");
                animator.SetTrigger("Recover");
                isRecovering = true;
            }
        }
        else
        {
            CombatActions();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (damagingColliders.ContainsLayer(collision.gameObject.layer))
        {
            // Get the magnitude of the collision
            float collisionMagnitude = collision.relativeVelocity.magnitude;

            if (collisionMagnitude >= collisionDamageThreshold)
            {
                Debug.Log("Taking collision damage.  Velocity: " + collisionMagnitude);
                TakeDamage(collisionMagnitude * collisionDamageMultiplier);
            }
        }
    }

    public void CombatActions()
    {
        if (aggroPlayer)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < fightDistance)
            {
                Attack();
            }
            else
            {
                // Calculate the direction to the player
                Vector3 direction = (player.transform.position - transform.position).normalized;

                // Move the Rigidbody towards the player
                rb3D.velocity = direction * moveSpeed;
            }
        }
        else
        {
            if (Vector3.Distance(rb3D.position, player.rb3D.position) < aggroDistance)
            {
                //Debug.Log("Enemy " + gameObject.name + " aggroed");
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
        //Debug.Log("Enemy Attacking");
        animator.Play("Attack");
    }

    public void IncomingAttack(AttackState playerAttackState)
    {
        //Debug.Log($"Enemy was hit from {playerAttackDir}");

        switch (playerAttackState)
        {
            case AttackState.BigAttack:
                TakeDamage(player.bigAttackDamage);
                break;
            case AttackState.SmallAttack:
                TakeDamage(player.smallAttackDamage);
                break;
            default:
                Debug.LogWarning("WARNING: no attack state");
                break;
        }
    }

    public void StartHitStun()
    {
        isHitStunned = true;
        isRecovering = false;
    }

    public void EndHitStun()
    {
        isHitStunned = false;
        isRecovering = false;
    }

    public void TakeDamage(float damage)
    {
        animator.Play("Hit");

        //Debug.Log("Taking damage: " + damage);
        currentHP -= damage;
        HPBar.value = currentHP;
        if (currentHP < 0)
        {
            EnemyDies();
        }
    }

    public void EnemyDies()
    {
        gameObject.SetActive(false);
    }
}
