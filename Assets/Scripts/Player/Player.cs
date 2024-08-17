using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public partial class Player : MonoBehaviour
{
    public static Player player;

    [Header("Components")]
    [Tooltip("Player's Transform component")]
    [SerializeField] Transform _transform;
    [Tooltip("Player's RigidBody component")]
    [SerializeField] Rigidbody _rigidbody;
    [Tooltip("Player's Animator component")]
    [SerializeField] Animator _animator;
    [Tooltip("Main Camera GameObject")]
    [SerializeField] GameObject _camera;

    // Vector to hold camera-facing direction
    private Vector3 cameraFaceDir;

    [Header("UI")]
    [Tooltip("Canvas GameObject holding player's in-world UI elements")]
    [SerializeField] GameObject playerWorldCanvas;
    [Tooltip("UI Slider for SP Points")]
    [SerializeField] Slider SPBar;
    [Tooltip("Transform of the player's SP bar (used to lock rotation)")]
    [SerializeField] Transform SPBarTransform;

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
    }

    void Start()
    {
        attackState = 'N';
        _animator.SetFloat("AttackSpeed", 1 / attackDuration);

        SPBar.maxValue = currentSP = maxSP;
        SPBar.value = currentSP;
        isRegenSP = false;

        isDashing = false;
        canDash = true;

        cameraFaceDir = _camera.transform.eulerAngles;
    }

    void Update()
    {
        if (!canDash) { DashTimer(); }
        Turn();

        if (isRegenSP) { RegenSP(); }
        else if (currentSP != maxSP) { SPTimer(); }
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
        _animator.SetTrigger("Sheathe");
    }

    void OnUpAttackInput()
    {
        StartAttack('U');
    }

    void OnDownAttackInput()
    {
        StartAttack('D');
    }

    void OnLeftAttackInput()
    {
        StartAttack('L');
    }

    void OnRightAttackInput()
    {
        StartAttack('R');
    }
}
