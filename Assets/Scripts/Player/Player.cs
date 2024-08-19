using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Enemy;

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
    public GameObject ScalesUI;

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

        rb3D = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main.gameObject;
    }

    void Start()
    {
        currentAttackState = AttackState.None;
        AddScalesValue(0);

        isDashing = false;
        canDash = true;

        HPBar.maxValue = currentHP = maxHP;
        HPBar.value = currentHP;
        cameraFaceDir = mainCamera.transform.eulerAngles;
    }

    void Update()
    {
        if (!canDash) { DashTimer(); }
        if (!isStunned)
        {
            Turn();
        }

        if (waitingForRecoveryMinimum)
        {
            RecoveryMinimumTimer();
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

        if (targetMoveDirection == Vector3.zero) { return; }
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

    void OnUpAttackInput()
    {
        if (isStunned) { return; }

        StartAttack(AttackState.SmallAttack);
    }

    void OnDownAttackInput()
    {
        if (isStunned) { return; }

        StartAttack(AttackState.SmallAttack);
    }

    void OnLeftAttackInput()
    {
        if (isStunned) { return; }

        StartAttack(AttackState.BigAttack);
    }

    void OnRightAttackInput()
    {
        if (isStunned) { return; }

        StartAttack(AttackState.BigAttack);
    }
}
