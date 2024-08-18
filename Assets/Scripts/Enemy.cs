using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Player;
using static HelperMethods;
using UnityEngine.AI;
using System.IO;

public partial class Enemy : MonoBehaviour
{
    [Header("Components")]
    // components
    public Rigidbody rb3D;
    public Animator animator;
    public NavMeshAgent agent;

    [Header("Movement")]
    public float moveSpeed = 3;
    public float aggroDistance = 10;

    public EnemyState currentEnemyState;

    public enum EnemyState
    {
        Idle,
        MovingToPlayer,
        Attacking,
        Stunned,
        Recovering
    }

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
        agent = GetComponent<NavMeshAgent>();

        agent.updatePosition = false;
        agent.updateRotation = true;
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
        HPBarTransform.eulerAngles = player.cameraFaceDir;
    }

    private void FixedUpdate()
    {
        switch (currentEnemyState)
        {
            case EnemyState.Idle:
                SearchForPlayer();
                break;
            case EnemyState.MovingToPlayer:
                MoveToPlayer();
                break;
            case EnemyState.Attacking:
                Attack();
                break;
            case EnemyState.Stunned:
                WaitToRecover();
                break;
        }
    }

    public void SearchForPlayer()
    {
        if (Vector3.Distance(rb3D.position, player.rb3D.position) < aggroDistance)
        {
            //Debug.Log("Enemy " + gameObject.name + " aggroed");
            currentEnemyState = EnemyState.MovingToPlayer;
        }
        else
        {
            // wander around?
        }
    }

    public void WaitToRecover()
    {
        if (rb3D.velocity.magnitude < 0.1f)
        {
            // Trigger recovery animation
            //Debug.Log("Recovering");
            animator.SetTrigger("Recover");
            currentEnemyState = EnemyState.Recovering;
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
                rb3D.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                TakeDamage(collisionMagnitude * collisionDamageMultiplier);
            }
        }
    }

    public void MoveToPlayer()
    {
        // Calculate the direction to the player
        //Vector3 direction = (player.transform.position - transform.position).normalized;
        // Move the Rigidbody towards the player
        //rb3D.velocity = direction * moveSpeed;

        agent.nextPosition = transform.position;
        agent.SetDestination(player.transform.position);

        rb3D.MovePosition(transform.position + agent.velocity * Time.fixedDeltaTime);

        if (Vector3.Distance(rb3D.position, player.rb3D.position) <= agent.stoppingDistance)
        {
            currentEnemyState = EnemyState.Attacking;
        }
    }

    public void Attack()
    {
        //Debug.Log("Enemy Attacking");
        animator.Play("Attack");

        // if player is now too far away, start moving toward them again
        if (Vector3.Distance(rb3D.position, player.rb3D.position) > agent.stoppingDistance)
        {
            currentEnemyState = EnemyState.MovingToPlayer;
        }
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
        currentEnemyState = EnemyState.Stunned;
        //rb3D.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        //agent.updatePosition = false;
        //agent.updateRotation = true;
    }

    public void EndHitStun()
    {
        currentEnemyState = EnemyState.MovingToPlayer;
        rb3D.constraints = RigidbodyConstraints.FreezeRotation;
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
