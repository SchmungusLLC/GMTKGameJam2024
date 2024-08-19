using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Player;
using static HelperMethods;
using UnityEngine.AI;
using System.IO;
using TMPro;

public partial class Enemy : MonoBehaviour
{
    public bool debugDisableEnemyAttacks;

    [Header("Components")]
    // components
    public Rigidbody rb3D;
    public Animator animator;
    public NavMeshAgent agent;

    [Header("Combat Stats")]
    [Tooltip("Total Number of Health Points (HP)")]
    public float maxHP;
    // current HP value
    private float currentHP;

    public float bigAttackDuration;
    public float smallAttackDuration;
    public float bigAttackForce;
    public float smallAttackForce;
    public float bigAttackDamage;
    public float smallAttackDamage;

    [Header("States")]
    public float aggroDistance = 10;

    public EnemyState currentEnemyState;
    public AttackState currentAttackState;
    public AttackState lastAttackTaken;

    public enum EnemyState
    {
        Idle,
        MovingToPlayer,
        Attacking,
        Stunned,
        Recovering,
        Dead
    }

    public float fallThreshold;

    public float collisionDamageMultiplier;
    public float collisionDamageThreshold;

    public LayerMask damagingColliders;

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

    public GameObject soulObject;
    public bool isSoulMoving;

    public bool waitingForRecoveryMinimum;
    public float minimumRecoveryThreshold;
    public float minimumRecoveryTimer;

    private void Awake()
    {
        rb3D = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.updatePosition = false;
        agent.updateRotation = true;

        currentAttackState = AttackState.None;
        currentEnemyState = EnemyState.Idle;
    }

    // Start is called before the first frame update
    void Start()
    {
        HPBar.maxValue = currentHP = maxHP;
        HPBar.value = currentHP;
        HPBarTransform = HPBar.gameObject.transform;

        animator.SetFloat("SmallAttackSpeed", 1 / smallAttackDuration);
        animator.SetFloat("BigAttackSpeed", 1 / bigAttackDuration);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentEnemyState == EnemyState.Dead)
        {
            if (isSoulMoving)
            {
                MoveSoulToTarget();
            }

            return;
        }

        HPBarTransform.eulerAngles = player.cameraFaceDir;

        if (waitingForRecoveryMinimum)
        {
            RecoveryMinimumTimer();
        }
    }

    private void FixedUpdate()
    {
        switch (currentEnemyState)
        {
            case EnemyState.Dead:
                return;
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
        if (!waitingForRecoveryMinimum && rb3D.velocity.magnitude < 0.1f)
        {
            // Trigger recovery animation
            //Debug.Log("Recovering");
            animator.SetTrigger("Recover");
            currentEnemyState = EnemyState.Recovering;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentEnemyState == EnemyState.Dead) { return; }

        if (damagingColliders.ContainsLayer(collision.gameObject.layer))
        {
            // Get the magnitude of the collision
            float collisionMagnitude = collision.relativeVelocity.magnitude;

            if (collisionMagnitude >= collisionDamageThreshold)
            {
                //Debug.Log("Taking collision damage.  Velocity: " + collisionMagnitude);
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
        if (currentAttackState != AttackState.None)
        {
            // enemy is mid-attack, stop here
            return;
        }
        else if (Vector3.Distance(rb3D.position, player.rb3D.position) > agent.stoppingDistance)
        {
            // if player is now too far away, start moving toward them again
            currentEnemyState = EnemyState.MovingToPlayer;
        }
        else
        {
            // enemy is in-range but not attacking - start an attack
            //Debug.Log("Enemy Attacking");

            // stop now if debug enabled
            if (debugDisableEnemyAttacks) { return; }

            if (Random.Range(0, 2) == 0)
            {
                animator.Play("SmallAttack");
                currentAttackState = AttackState.SmallAttack;
            }
            else
            {
                animator.Play("BigAttack");
                currentAttackState = AttackState.BigAttack;
            }
        }
    }

    public void AttackHitBox()
    {
        //Debug.Log("Enemy AttackHitBox");
        if (currentAttackState == AttackState.SmallAttack)
        {
            targetsStruck = Physics.OverlapBox(transform.position + transform.forward * hbOffset, hbSizeV, transform.rotation, hitLayers);
        }
        else
        {
            targetsStruck = Physics.OverlapBox(transform.position + transform.forward * hbOffset, hbSizeH, transform.rotation, hitLayers);
        }

        foreach (Collider target in targetsStruck)
        {
            //Debug.Log("Hit target " + target.gameObject.name);

            if (target.gameObject.TryGetComponent(out Player player))
            {
                // Calculate direction from the attack source to the player
                Vector3 direction = player.rb3D.position - rb3D.position;
                direction.y = 0; // Ensure force is applied on the horizontal plane
                direction.Normalize();

                // Apply force to the Rigidbody
                if (currentAttackState == AttackState.BigAttack)
                {
                    player.rb3D.AddForce(direction * bigAttackForce, ForceMode.Impulse);
                    player.IncomingAttack(this, bigAttackDamage);
                    //player.rb3D.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                }
                else
                {
                    player.rb3D.AddForce(direction * smallAttackForce, ForceMode.Impulse);
                    player.IncomingAttack(this, smallAttackDamage);
                    //player.rb3D.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                }
            }
            else if (target.gameObject.TryGetComponent(out Destructible destructible))
            {
                // Calculate direction from the attack source to the destructible
                Vector3 direction = destructible.rb3D.position - rb3D.position;
                direction.y = 0; // Ensure force is applied on the horizontal plane
                direction.Normalize();

                // Apply force to the Rigidbody
                if (currentAttackState == AttackState.BigAttack)
                {
                    destructible.rb3D.AddForce(direction * bigAttackForce, ForceMode.Impulse);
                    destructible.Struck(bigAttackDamage);
                }
                else
                {
                    destructible.rb3D.AddForce(direction * smallAttackForce, ForceMode.Impulse);
                    destructible.Struck(smallAttackDamage);
                }
            }
        }
    }

    void EndAttack()
    {
        currentAttackState = AttackState.None;
    }

    public void IncomingAttack(float damage, AttackState attackState)
    {
        //Debug.Log($"Enemy was hit from {playerAttackDir}");
        TakeDamage(damage);

        lastAttackTaken = attackState;
    }

    public void StartHitStun()
    {
        minimumRecoveryTimer = 0;
        waitingForRecoveryMinimum = true;
        //Debug.Log("Recovery waiting...");

        currentEnemyState = EnemyState.Stunned;
        currentAttackState = AttackState.None;
        //rb3D.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public void EndHitStun()
    {
        currentEnemyState = EnemyState.MovingToPlayer;
        rb3D.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void RecoveryMinimumTimer()
    {
        minimumRecoveryTimer += Time.deltaTime;
        if (minimumRecoveryTimer >= minimumRecoveryThreshold)
        {
            waitingForRecoveryMinimum = false;
        }
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
        //gameObject.SetActive(false);
        rb3D.constraints = RigidbodyConstraints.FreezeAll;
        currentEnemyState = EnemyState.Dead;
        soulObject.SetActive(true);
        animator.Play("Death");
    }

    public void StartSoulMoving()
    {
        isSoulMoving = true;
    }

    public void MoveSoulToTarget()
    {
        // Get the bottom-right corner of the screen in world space
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(player.lightSlider.transform.position);
        //targetPosition.z = transform.position.z;

        soulObject.transform.position = Vector3.MoveTowards(soulObject.transform.position, targetPosition, 200 * Time.deltaTime);

        // Check if the sprite has reached the target position
        if (Vector3.Distance(soulObject.transform.position, targetPosition) < 0.1f)
        {
            isSoulMoving = false;
            // Destroy or deactivate the sprite
            soulObject.SetActive(false);
            if (lastAttackTaken == AttackState.SmallAttack)
            {
                ChargePlayerLight();
            }
            else
            {
                ChargePlayerHeavy();
            }

            //gameObject.SetActive(false);
        }
    }

    public void ChargePlayerLight()
    {
        player.AddLightUltimateCharge();
        player.AddScalesValue(-0.1f);
    }

    public void ChargePlayerHeavy()
    {
        player.AddHeavyUltimateCharge();
        player.AddScalesValue(0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(transform.position + transform.forward * hbOffset, transform.rotation, transform.localScale);
        Gizmos.DrawCube(Vector3.zero, new Vector3(hbSizeH.x * 2, hbSizeH.y * 2, hbSizeH.z * 2));
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(Vector3.zero, new Vector3(hbSizeV.x * 2, hbSizeV.y * 2, hbSizeV.z * 2));
    }
}
