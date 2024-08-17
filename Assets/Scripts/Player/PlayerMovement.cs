using UnityEngine;

public partial class Player
// ============================================================
//                     PLAYER MOVEMENT
// ============================================================
{
    [Header("Movement")]
    [Tooltip("Movement speed in m/s")]
    [SerializeField] float moveSpeed;
    [Tooltip("Rotation speed in degrees/s")]
    [SerializeField] float turnSpeed;
    [Tooltip("Movement speed when dashing")]
    [SerializeField] float dashSpeed;
    [Tooltip("Time in seconds from dash start to stop")]
    [SerializeField] float dashDuration;
    [Tooltip("Time in seconds between dash uses")]
    [SerializeField] float dashCooldown;
    [Tooltip("SP cost of dashing")]
    [SerializeField] float dashSPCost;

    // bool to check if dash is currently being performed
    [HideInInspector] public bool isDashing;
    // bool to check if dash is ready to be activated
    private bool canDash;
    // float for current dash time
    private float dashTime;

    // vectors for player movement input
    Vector2 moveInput;
    Vector3 targetMoveDirection;
    Vector3 currentPos;
    Vector3 targetPos;
    Quaternion targetLookDirection;

    // layers by which the player will be obstructed
    [SerializeField] LayerMask obstructionLayers;

    void Turn()
    {
        if (_transform.rotation == targetLookDirection) { return; }
        _transform.rotation = Quaternion.RotateTowards(_transform.rotation, targetLookDirection, turnSpeed * Time.deltaTime);
        SPBarTransform.eulerAngles = cameraFaceDir;
    }

    void Move()
    {
        currentPos = _rigidbody.position;
        if (!isDashing)
        {
            if (targetMoveDirection == Vector3.zero) { return; }
            targetPos = currentPos + targetMoveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            if (targetMoveDirection == Vector3.zero)
            {
                targetPos = currentPos + _transform.TransformDirection(Vector3.forward) * dashSpeed * Time.deltaTime;
            }
            else
            {
                targetPos = currentPos + targetMoveDirection * dashSpeed * Time.deltaTime;
            }
        }

        if (Physics.CheckSphere(targetPos, 0.5f, obstructionLayers)) { return; }

        _rigidbody.MovePosition(targetPos);
    }

    void DashTimer()
    {
        dashTime += Time.deltaTime;
        if (dashTime > dashDuration)
        {
            isDashing = false;
        }
        if (dashTime > dashCooldown)
        {
            canDash = true;
        }
    }
}
