using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public partial class Player
// ============================================================
//                      PLAYER COMBAT
// ============================================================
{
    [Header("Scaled Stats")]
    public float lightMoveSpeed;
    public float heavyMoveSpeed;
    public float lightBigAttackDuration;
    public float heavyBigAttackDuration = 3f;
    public float lightSmallAttackDuration = 0.5f;
    public float heavySmallAttackDuration = 1.5f;
    public float lightBigAttackForce = 5f;
    public float heavyBigAttackForce = 20f;
    public float lightSmallAttackForce = 2f;
    public float heavySmallAttackForce = 10f;

    public float scaledMoveSpeed;
    public float scaledBigAttackDuration;
    public float scaledSmallAttackDuration;
    public float scaledBigAttackForce;
    public float scaledSmallAttackForce;

    [Header("Scales")]
    public float currentScalesValue = 0.5f;
    public Slider scalesSlider;
    public Slider lightSlider;
    public Slider heavySlider;
    public TextMeshProUGUI scaledStatsTB;

    [Header("Ultimates")]
    public PlayableDirector director;
    public TimelineAsset lightUltimateTimeline;
    public TimelineAsset heavyUltimateTimeline;

    public float ultimateChargeThreshold;
    public float ultimateRange;
    public float lightUltimateCharge;
    public float heavyUltimateCharge;

    [Header("Attack")]
    public float smallAttackDamage;
    public float bigAttackDamage;

    public bool isStunned;

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

    public bool waitingForRecoveryMinimum;
    public float minimumRecoveryThreshold;
    public float minimumRecoveryTimer;

    void StartAttack(AttackState state)
    {
        if (currentAttackState != AttackState.None) { return; }

        currentAttackState = state;
        animator.Play(state.ToString());
    }

    public void AttackHitBox()
    {
        //Debug.Log("Player attack hitbox");
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
                    enemy.IncomingAttack(bigAttackDamage, currentAttackState);
                    enemy.rb3D.AddForce(direction * scaledBigAttackForce, ForceMode.Impulse);
                    enemy.rb3D.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                }
                else
                {
                    enemy.IncomingAttack(smallAttackDamage, currentAttackState);
                    enemy.rb3D.AddForce(direction * scaledSmallAttackForce, ForceMode.Impulse);
                    enemy.rb3D.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
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
                    destructible.Struck(bigAttackDamage);
                    destructible.rb3D.AddForce(direction * scaledBigAttackForce, ForceMode.Impulse);
                }
                else
                {
                    destructible.Struck(smallAttackDamage);
                    destructible.rb3D.AddForce(direction * scaledSmallAttackForce, ForceMode.Impulse);
                }
            }
        }
    }

    public void UseLightUltimate()
    {
        if (lightUltimateCharge < 1) return;

        lightUltimateCharge = 0;
        UIAnimator.Play("LightUlt0", 1);
        //director.Play(lightUltimateTimeline);
        UltimateKillEnemies(); // temp - kill 5 enemies
    }

    public void UseHeavyUltimate()
    {
        if (heavyUltimateCharge < 1) return;

        heavyUltimateCharge = -0.5f;
        UIAnimator.Play("HeavyUlt0", 1);
        //director.Play(heavyUltimateTimeline);
        UltimateKillEnemies(); // temp - kill 5 enemies
    }

    void UltimateKillEnemies()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ultimateRange, hitLayers);

        var closestEnemies = hitColliders
            .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
            .Take(5); // Take the 5 closest enemies

        foreach (var enemyCollider in closestEnemies)
        {
            enemyCollider.GetComponent<Enemy>().TakeDamage(999);
        }
    }

    public void AddLightUltimateCharge()
    {
        lightUltimateCharge += 0.1f;
        switch (lightUltimateCharge)
        {
            case < 0.1f:
                UIAnimator.Play("LightUlt0", 1);
                break;
            case >= 0.1f and < 0.4f:
                UIAnimator.Play("LightUlt1", 1);
                break;
            case >= 0.4f and < 0.7f:
                UIAnimator.Play("LightUlt2", 1);
                break;
            case >= 0.7f and < 1:
                UIAnimator.Play("LightUlt3", 1);
                break;
            case >= 1:
                lightUltimateCharge = 1;
                UIAnimator.Play("LightUltReady", 1);
                break;
        }

        lightSlider.value = lightUltimateCharge;
    }

    public void AddHeavyUltimateCharge()
    {
        heavyUltimateCharge += 0.1f;
        switch (heavyUltimateCharge)
        {
            case < 0.1f:
                UIAnimator.Play("HeavyUlt0", 0);
                break;
            case >= 0.1f and < 0.5f:
                UIAnimator.Play("HeavyUlt1", 0);
                break;
            case >= 0.4f and < 0.7f:
                UIAnimator.Play("HeavyUlt2", 0);
                break;
            case >= 0.7f and < 1:
                UIAnimator.Play("HeavyUlt3", 0);
                break;
            case >= 1:
                heavyUltimateCharge = 1;
                UIAnimator.Play("HeavyUltReady", 0);
                break;
        }

        heavySlider.value = heavyUltimateCharge;
    }

    public void AddScalesValue(float value)
    {
        // edit scales value
        currentScalesValue += value;
        currentScalesValue = Mathf.Clamp01(currentScalesValue);
        scalesSlider.value = currentScalesValue;

        // update scaled stats
        scaledMoveSpeed = Mathf.Lerp(lightMoveSpeed, heavyMoveSpeed, currentScalesValue);
        scaledBigAttackDuration = Mathf.Lerp(lightBigAttackDuration, heavyBigAttackDuration, currentScalesValue);
        scaledSmallAttackDuration = Mathf.Lerp(lightSmallAttackDuration, heavySmallAttackDuration, currentScalesValue);
        scaledBigAttackForce = Mathf.Lerp(lightBigAttackForce, heavyBigAttackForce, currentScalesValue);
        scaledSmallAttackForce = Mathf.Lerp(lightSmallAttackForce, heavySmallAttackForce, currentScalesValue);

        animator.SetFloat("SmallAttackSpeed", 1 / scaledSmallAttackDuration);
        animator.SetFloat("BigAttackSpeed", 1 / scaledBigAttackDuration);

        scaledStatsTB.text = $"Move: {scaledMoveSpeed}\n" +
            $"BigDuration: {scaledBigAttackDuration}\n" +
            $"SmallDuration: {scaledSmallAttackDuration}\n" +
            $"BigForce: {scaledBigAttackForce}\n" +
            $"SmallForce: {scaledSmallAttackForce}\n";
    }

    void EndAttack()
    {
        currentAttackState = AttackState.None;
    }

    void TakeDamage(float damage)
    {
        animator.Play("Hit");

        //Debug.Log("Player took Damage: " + damage);
        currentHP -= damage;
    }

    public void IncomingAttack(Enemy attacker, float damage)
    {
        //Debug.Log("Player was hit by " + attackState);
        TakeDamage(damage);
    }

    public void StartHitStun()
    {
        minimumRecoveryTimer = 0;
        waitingForRecoveryMinimum = true;
        Debug.Log("Recovery waiting...");

        isStunned = true;
        currentAttackState = AttackState.None;
        //rb3D.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        //agent.updatePosition = false;
        //agent.updateRotation = true;
    }

    public void EndHitStun()
    {
        Debug.Log("End hit stun");
        isStunned = false;
        //rb3D.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void RecoveryMinimumTimer()
    {
        minimumRecoveryTimer += Time.deltaTime;
        if (minimumRecoveryTimer >= minimumRecoveryThreshold)
        {
            waitingForRecoveryMinimum = false;
        }
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
