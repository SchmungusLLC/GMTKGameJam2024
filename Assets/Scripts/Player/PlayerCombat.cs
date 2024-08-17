using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class Player
// ============================================================
//                      PLAYER COMBAT
// ============================================================
{
    [Header("Attack Stats")]
    [Tooltip("Total duration of small attacks in seconds")]
    public float defaultSmallAttackDuration;
    public float scaledSmallAttackDuration;
    [Tooltip("Total duration of big attacks in seconds")]
    public float scaledBigAttackDuration;
    public float defaultBigAttackDuration;

    // force applied by attack types
    public float defaultSmallAttackForce;
    public float defaultBigAttackForce;
    public float scaledSmallAttackForce;
    public float scaledBigAttackForce;

    // scales
    public float currentScalesValue = 1;
    public Slider scalesSlider;

    // Current attack being executed
    public AttackState currentAttackState;

    public enum AttackState
    {
        None,
        BigAttack,
        SmallAttack,
    }

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

    void StartAttack(AttackState state)
    {
        if (currentAttackState != AttackState.None) { return; }

        currentAttackState = state;
        animator.Play(state.ToString());
    }

    void AttackHitBox()
    {
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

            if (target.gameObject.TryGetComponent(out Enemy enemy))
            {
                // Calculate direction from the attack source to the enemy
                Vector3 direction = enemy.rb3D.position - rb3D.position;
                direction.y = 0; // Ensure force is applied on the horizontal plane
                direction.Normalize();

                // Apply force to the Rigidbody
                if (currentAttackState == AttackState.BigAttack)
                {
                    enemy.rb3D.AddForce(direction * scaledBigAttackForce, ForceMode.Impulse);
                    AddScalesValue(0.1f);
                }
                else
                {
                    enemy.rb3D.AddForce(direction * scaledSmallAttackForce, ForceMode.Impulse);
                    AddScalesValue(-0.1f);
                }

                enemy.IncomingAttack(currentAttackState);
            }
            else if (target.gameObject.TryGetComponent(out Destructible destructible))
            {
                destructible.Struck(rb3D);
            }
        }
    }

    public void AddScalesValue(float value)
    {
        // edit scales value
        currentScalesValue += value;
        if (currentScalesValue < 0.5)
        {
            currentScalesValue = 0.5f;
        }
        else if (currentScalesValue > 1.5)
        {
            currentScalesValue = 1.5f;
        }
        scalesSlider.value = currentScalesValue;

        // update scaled stats
        scaledMoveSpeed = defaultMoveSpeed * currentScalesValue;
        scaledBigAttackDuration = defaultBigAttackDuration * currentScalesValue;
        scaledSmallAttackDuration = defaultSmallAttackDuration * currentScalesValue;
        scaledBigAttackForce = defaultBigAttackForce * currentScalesValue;
        scaledSmallAttackForce = defaultSmallAttackForce * currentScalesValue;
        scaledMaxHP = defaultMaxHP * currentScalesValue;

        animator.SetFloat("SmallAttackSpeed", 1 / scaledSmallAttackDuration);
        animator.SetFloat("BigAttackSpeed", 1 / scaledBigAttackDuration);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(transform.position + transform.forward * hbOffset, transform.rotation, transform.localScale);
        Gizmos.DrawCube(Vector3.zero, new Vector3(hbSizeH.x * 2, hbSizeH.y * 2, hbSizeH.z * 2));
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(Vector3.zero, new Vector3(hbSizeV.x * 2, hbSizeV.y * 2, hbSizeV.z * 2));
    }

    void EndAttack()
    {
        currentAttackState = AttackState.None;
    }

    void TakeDamage()
    {
        Debug.Log("Player took Damage");
    }

    public void IncomingAttack(char attackDir)
    {
        Debug.Log("Player was hit from " + attackDir);
    }
}
