using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    private Vector3 cameraFaceDir;

    [Header("UI")]
    [Tooltip("Canvas GameObject holding player's in-world UI elements")]
    public GameObject playerWorldCanvas;
    [Tooltip("UI Slider for SP Points")]
    public Slider SPBar;
    [Tooltip("Transform of the player's SP bar (used to lock rotation)")]
    public Transform SPBarTransform;

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
        DontDestroyOnLoad(gameObject);

        rb3D = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main.gameObject;
    }

    void Start()
    {
        currentAttackState = AttackState.None;
        animator.SetFloat("SmallAttackSpeed", 1 / smallAttackDuration);
        animator.SetFloat("BigAttackSpeed", 1 / bigAttackDuration);

        SPBar.maxValue = currentSP = maxSP;
        SPBar.value = currentSP;
        isRegenSP = false;

        isDashing = false;
        canDash = true;

        cameraFaceDir = mainCamera.transform.eulerAngles;
    }

    void Update()
    {
        if (!canDash) { DashTimer(); }
        Turn();

        if (isRegenSP) { RegenSP(); }
        else if (currentSP != maxSP) { SPTimer(); }
    }

    private void LateUpdate()
    {
        animator.SetFloat("XVelocity", rb3D.velocity.x);
        animator.SetFloat("ZVelocity", rb3D.velocity.z);
        animator.SetFloat("TotalVelocity", rb3D.velocity.magnitude);
    }

    void FixedUpdate()
    {
        Move();
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
        moveInput = value.Get<Vector2>();
        targetMoveDirection = new Vector3(moveInput.x, 0, moveInput.y).IsoRotation();

        if (targetMoveDirection == Vector3.zero) { return; }
        targetLookDirection = Quaternion.LookRotation(targetMoveDirection, Vector3.up);
    }

    void OnDashInput()
    {
        if (!canDash || !SpendSP(dashSPCost)) { return; }

        isDashing = true;
        canDash = false;
        dashTime = 0f;
        animator.SetTrigger("Sheathe");
    }

    void OnUpAttackInput()
    {
        StartAttack(AttackState.SmallAttack);
    }

    void OnDownAttackInput()
    {
        StartAttack(AttackState.SmallAttack);
    }

    void OnLeftAttackInput()
    {
        StartAttack(AttackState.BigAttack);
    }

    void OnRightAttackInput()
    {
        StartAttack(AttackState.BigAttack);
    }
}
