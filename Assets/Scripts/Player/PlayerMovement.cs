using UnityEngine;

public partial class Player
// ============================================================
//                     PLAYER MOVEMENT
// ============================================================
{
    [Header("Movement")]
    [Tooltip("Rotation speed in degrees/s")]
    public float turnSpeed;
    [Tooltip("Movement speed when dashing")]
    public float dashSpeed;
    [Tooltip("Time in seconds from dash start to stop")]
    public float dashDuration;
    [Tooltip("Time in seconds between dash uses")]
    public float dashCooldown;
    [Tooltip("SP cost of dashing")]
    public float dashSPCost;

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
    public LayerMask obstructionLayers;

    void Turn()
    {
        if (isStunned || transform.rotation == targetLookDirection) { return; }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetLookDirection, turnSpeed * Time.deltaTime);
        //SPBarTransform.eulerAngles = cameraFaceDir;
    }

    void Move()
    {
        currentPos = rb3D.position;
        if (!isDashing)
        {
            if (targetMoveDirection == Vector3.zero) { return; }
            targetPos = currentPos + targetMoveDirection * scaledMoveSpeed * Time.deltaTime;
        }
        else
        {
            if (targetMoveDirection == Vector3.zero)
            {
                targetPos = currentPos + transform.TransformDirection(Vector3.forward) * dashSpeed * Time.deltaTime;
            }
            else
            {
                targetPos = currentPos + targetMoveDirection * dashSpeed * Time.deltaTime;
            }
        }

        if (Physics.CheckSphere(targetPos, 0.5f, obstructionLayers)) { return; }

        rb3D.MovePosition(targetPos);
    }

    void DashTimer()
    {
        dashTime += Time.deltaTime;
        if (dashTime > dashDuration)
        {
            isDashing = false;
            DashTrailVFX.enabled = false;
            DashParticleVFX.enabled = false;
        }
        if (dashTime > dashCooldown)
        {
            canDash = true;
        }
    }
}
