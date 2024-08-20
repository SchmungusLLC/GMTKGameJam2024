using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static GameManager;

public partial class Player : MonoBehaviour
{
    public static Player player;

    [Header("Components")]
    [Tooltip("Player's RigidBody component")]
    public Rigidbody rb3D;
    [Tooltip("Player's Animator component")]
    public Animator animator;
    [Tooltip("Main Camera GameObject")]
    public GameObject mainCamera;

    // Vector to hold camera-facing direction
    public Vector3 cameraFaceDir;

    [Header("Collision")]
    public float collisionDamageMultiplier;
    public float collisionDamageThreshold;

    public LayerMask damagingColliders;

    [Header("UI")]
    [Tooltip("Canvas GameObject holding player's in-world UI elements")]
    public GameObject playerWorldCanvas;
    public List<Sprite> scalesSprites;
    public Image scalesImage;

    public List<Vector2> flamePositions;
    public List<Vector2> heavyFlamePositions;
    public RectTransform lightFlameTransform;
    public RectTransform heavyFlameTransform;

    public Animator UIAnimator;

    //public Slider SPBar;
    //public Transform SPBarTransform;

    void Awake()
    {
        if (player != null && player != this)
        {
            Destroy(this);
        }
        else
        {
            player = this;
        }
        
        if (rb3D == null)
        {
            rb3D = GetComponent<Rigidbody>();
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (mainCamera == null)
        {
            mainCamera = Camera.main.gameObject;
        }
    }

    void Start()
    {
        _gameManager.SetStatsOnPlayer();
        player.AddHeavyUltimateCharge(0);
        player.AddLightUltimateCharge(0);
        player.AddScalesValue(0);

        currentAttackState = AttackState.None;

        isDashing = false;
        canDash = true;

        currentHP = maxHP;
        HPBar.maxValue = maxHP;
        HPBar.value = currentHP;
        cameraFaceDir = mainCamera.transform.eulerAngles;
    }

    void Update()
    {
        //Debug.Log("CurrentHP = " + currentHP);

        if (!canDash) { DashTimer(); }
        if (!isStunned)
        {
            Turn();
        }

        if (waitingForRecoveryMinimum)
        {
            RecoveryMinimumTimer();
        }

        if (comboCounter > 0)
        {
            comboResetTime += Time.deltaTime;
            if (comboResetTime > comboResetThreshold)
            {
                comboCounter = 0;
                comboResetTime = 0;
            }
        }
    }

    Vector3 lastPosition;

    void FixedUpdate()
    {
        if (isStunned)
        {
            if (!waitingForRecoveryMinimum && rb3D.velocity.magnitude < 0.1f)
            {
                //Debug.Log("Velocity = " + rb3D.velocity.magnitude);
                EndHitStun();
            }
        }
        else
        {
            Move();
        }

        Vector3 deltaPosition = transform.position - lastPosition;
        float speedX = deltaPosition.x / Time.deltaTime;
        float speedZ = deltaPosition.z / Time.deltaTime;
        float speedTotal = Mathf.Sqrt(speedX * speedX + speedZ * speedZ);

        if (speedTotal < 0.01f)
        {
            speedX = 0f;
            speedZ = 0f;
            speedTotal = 0f;
        }

        animator.SetFloat("XVelocity", speedX);
        animator.SetFloat("ZVelocity", speedZ);
        animator.SetFloat("TotalVelocity", speedTotal);

        lastPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (damagingColliders.ContainsLayer(collision.gameObject.layer))
        {
            // Get the magnitude of the collision
            float collisionMagnitude = collision.relativeVelocity.magnitude;

            if (collisionMagnitude >= collisionDamageThreshold)
            {
                //Debug.Log("Taking collision damage.  Velocity: " + collisionMagnitude);
                //rb3D.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                TakeDamage(collisionMagnitude * collisionDamageMultiplier);
            }
        }
    }

    void OnDeviceLost()
    {
        Debug.LogWarning("Controller Disconnected!  Please reconnect.");
        Time.timeScale = 0f;
    }

    void OnDeviceRegained()
    {
        Debug.Log("Controller Reconnected!");
        Time.timeScale = 1f;
    }

    void OnMoveInput(InputValue value)
    {
        //if (isStunned) { return; }

        moveInput = value.Get<Vector2>();
        targetMoveDirection = new Vector3(moveInput.x, 0, moveInput.y).IsoRotation();

        if (isStunned || targetMoveDirection == Vector3.zero) { return; }
        targetLookDirection = Quaternion.LookRotation(targetMoveDirection, Vector3.up);
    }

    void OnDashInput()
    {
        if (!canDash || isStunned) { return; }

        isDashing = true;
        canDash = false;
        dashTime = 0f;
        animator.SetTrigger("Sheathe");
    }

    void OnSmallAttackInput()
    {
        if (isStunned) { return; }

        if (comboCounter == 2)
        {
            StartAttack(AttackState.ComboEndAttack);
        }
        else
        {
            StartAttack(AttackState.SmallAttack);
        }
    }

    void OnBigAttackInput()
    {
        if (isStunned) { return; }

        StartAttack(AttackState.BigAttack);
    }

    void OnUseLightUltimateInput()
    {
        if (isStunned) { return; }

        UseLightUltimate();
    }

    void OnUseHeavyUltimateInput()
    {
        if (isStunned) { return; }

        UseHeavyUltimate();
    }
}
