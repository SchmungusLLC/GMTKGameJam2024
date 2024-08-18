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
    public Vector3 cameraFaceDir;

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
        AddScalesValue(0);

        isDashing = false;
        canDash = true;

        cameraFaceDir = mainCamera.transform.eulerAngles;
    }

    void Update()
    {
        if (!canDash) { DashTimer(); }
        Turn();
    }

    Vector3 lastPosition;

    void FixedUpdate()
    {
        Move();

        Vector3 deltaPosition = transform.position - lastPosition;
        float speedX = deltaPosition.x / Time.deltaTime;
        float speedZ = deltaPosition.z / Time.deltaTime;
        float speedTotal = Mathf.Sqrt(speedX * speedX + speedZ * speedZ);

        animator.SetFloat("XVelocity", speedX);
        animator.SetFloat("ZVelocity", speedZ);
        animator.SetFloat("TotalVelocity", speedTotal);

        lastPosition = transform.position;
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
        if (!canDash) { return; }

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
