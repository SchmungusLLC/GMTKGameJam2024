using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using static AudioManager;

public partial class Player
// ============================================================
//                      PLAYER COMBAT
// ============================================================
{
    [Header("VFX")]
    public ParticleSystem SwordTrailVFXSystem;
    public ParticleSystem SwordParticleVFXSystem;
    public ParticleSystem DashTrailVFXSystem;
    public ParticleSystem DashParticleVFXSystem;

    public ParticleSystem.EmissionModule SwordTrailVFX;
    public ParticleSystem.EmissionModule SwordParticleVFX;
    public ParticleSystem.EmissionModule DashTrailVFX;
    public ParticleSystem.EmissionModule DashParticleVFX;

    [Header("Scaled Stats")]
    public float lightMoveSpeed;
    public float heavyMoveSpeed;
    public float lightPlayerMass;
    public float heavyPlayerMass;
    public float lightBigAttackDuration;
    public float heavyBigAttackDuration;
    public float lightSmallAttackDuration;
    public float heavySmallAttackDuration;
    public float lightComboEndAttackDuration;
    public float heavyComboEndAttackDuration;
    public float lightBigAttackForce;
    public float heavyBigAttackForce;
    public float lightComboEndAttackForce;
    public float heavyComboEndAttackForce;

    public float scaledMoveSpeed;
    public float scaledPlayerMass;
    public float scaledComboEndAttackDuration;
    public float scaledBigAttackDuration;
    public float scaledSmallAttackDuration;
    public float scaledBigAttackForce;
    public float scaledComboEndAttackForce;

    [Header("Scales")]
    public int currentScalesValue = 12;
    public Slider lightSlider;
    public Slider heavySlider;

    [Header("Ultimates")]
    public PlayableDirector ultimateDirector;
    public TimelineAsset lightUltimateTimeline;
    public TimelineAsset heavyUltimateTimeline;
    public GameObject mainVcam;

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
        ComboEndAttack
    }

    public int comboCounter;
    public float comboResetTime;
    public float comboResetThreshold;

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

    public bool debugUseTestAnimations;
    void StartAttack(AttackState state)
    {
        if (currentAttackState != AttackState.None) { return; }

        //Debug.Log("Setting state to " + state.ToString());
        currentAttackState = state;
        comboCounter++;
        comboResetTime = 0;

        if (debugUseTestAnimations)
        {
            animator.Play(state.ToString());
            return;
        }

        if (state == AttackState.BigAttack)
        {
            _AudioManger.PlayRandomSoundFromArray(_AudioManger.LJHeavyAttack);
        }
        else
        {
            _AudioManger.PlayRandomSoundFromArray(_AudioManger.LJLightAttack);
        }

        animator.Play(state.ToString(), 1);
        //animator.Play(state.ToString(), 2);
    }

    public void EnableSwordVFX()
    {
        SwordTrailVFX.enabled = true;
        SwordParticleVFX.enabled = true;
    }

    public void AttackHitBox()
    {
        //Debug.Log("Player attack hitbox");
        if (currentAttackState == AttackState.SmallAttack || currentAttackState == AttackState.ComboEndAttack)
        {
            if (comboCounter >= 3)
            {
                comboCounter = 0;
                comboResetTime = 0;
            }
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
                    enemy.EnemyVFXEnable();
                    enemy.rb3D.AddForce(direction * scaledBigAttackForce, ForceMode.Impulse);
                    //enemy.rb3D.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                }
                else if (currentAttackState == AttackState.SmallAttack)
                {
                    enemy.IncomingAttack(smallAttackDamage, currentAttackState);
                    //enemy.rb3D.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                }
                else if (currentAttackState == AttackState.ComboEndAttack)
                {
                    enemy.IncomingAttack(smallAttackDamage, currentAttackState);
                    //enemy.rb3D.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                    enemy.rb3D.AddForce(direction * scaledComboEndAttackForce, ForceMode.Impulse);
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
                else if (currentAttackState == AttackState.SmallAttack)
                {
                    destructible.Struck(smallAttackDamage);
                    destructible.rb3D.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                }
                else if (currentAttackState == AttackState.ComboEndAttack)
                {
                    destructible.Struck(smallAttackDamage);
                    destructible.rb3D.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                    destructible.rb3D.AddForce(direction * scaledComboEndAttackForce, ForceMode.Impulse);
                }
            }
        }
    }

    public void UseLightUltimate()
    {
        if (lightUltimateCharge < 1) return;

        lightUltimateCharge = -0.5f;
        UIAnimator.Play("LightUlt0", 1);
        mainVcam.SetActive(false);
        ultimateDirector.Play(lightUltimateTimeline);
        Time.timeScale = 0;
        //UltimateKillEnemies(); // temp - kill 5 enemies
    }

    public void UseHeavyUltimate()
    {
        if (heavyUltimateCharge < 1) return;

        heavyUltimateCharge = -0.5f;
        UIAnimator.Play("HeavyUlt0", 0);
        Time.timeScale = 0;
        mainVcam.SetActive(false);
        ultimateDirector.Play(heavyUltimateTimeline);
        //UltimateKillEnemies(); // temp - kill 5 enemies
    }

    public void UltimateKillEnemies()
    {
        // Find all objects with the tag "Enemy"
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // Sort enemies by distance to the player
        var sortedEnemies = enemies
            .Select(enemy => new { EnemyObject = enemy, Distance = Vector3.Distance(transform.position, enemy.transform.position) })
            .OrderBy(e => e.Distance)
            .Take(5) // Take the 5 closest enemies
            .ToList();

        // Deal damage to each of the closest enemies
        foreach (var enemy in sortedEnemies)
        {
            // Assuming your enemies have a script with a TakeDamage method
            Enemy enemyScript = enemy.EnemyObject.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(999);
            }
        }
        mainVcam.SetActive(true);
        Time.timeScale = 1;
    }

    public void AddLightUltimateCharge(float value)
    {
        lightUltimateCharge += value;
        switch (lightUltimateCharge)
        {
            case < 0.1f:
                UIAnimator.Play("LightUlt0", 1);
                break;
            case >= 0.1f and < 0.5f:
                UIAnimator.Play("LightUlt1", 1);
                break;
            case >= 0.5f and < 1:
                UIAnimator.Play("LightUlt2", 1);
                break;
            case >= 1:
                lightUltimateCharge = 1;
                UIAnimator.Play("LightUltReady", 1);
                break;
        }

        lightSlider.value = lightUltimateCharge;
    }

    public void AddHeavyUltimateCharge(float value)
    {
        heavyUltimateCharge += value;
        switch (heavyUltimateCharge)
        {
            case < 0.1f:
                UIAnimator.Play("HeavyUlt0", 0);
                break;
            case >= 0.1f and < 0.5f:
                UIAnimator.Play("HeavyUlt1", 0);
                break;
            case >= 0.5f and < 1:
                UIAnimator.Play("HeavyUlt2", 0);
                break;
            case >= 1:
                heavyUltimateCharge = 1;
                UIAnimator.Play("HeavyUltReady", 0);
                break;
        }

        heavySlider.value = heavyUltimateCharge;
    }
    public TextMeshProUGUI weightLabel;

    public void AddScalesValue(int value)
    {
        // edit scales value
        //Debug.Log("Setting scales value to " + value);
        currentScalesValue += value;
        currentScalesValue = Mathf.Clamp(currentScalesValue, 0, 24);

        // Normalize t to the range [0, 1]
        float normalizedT = currentScalesValue / 24f;

        // update scaled stats
        scaledMoveSpeed = Mathf.Lerp(lightMoveSpeed, heavyMoveSpeed, normalizedT);
        scaledPlayerMass = Mathf.Lerp(lightPlayerMass, heavyPlayerMass, normalizedT);
        scaledBigAttackDuration = Mathf.Lerp(lightBigAttackDuration, heavyBigAttackDuration, normalizedT);
        scaledSmallAttackDuration = Mathf.Lerp(lightSmallAttackDuration, heavySmallAttackDuration, normalizedT);
        scaledComboEndAttackDuration = Mathf.Lerp(lightSmallAttackDuration, heavySmallAttackDuration, normalizedT);
        scaledBigAttackForce = Mathf.Lerp(lightBigAttackForce, heavyBigAttackForce, normalizedT);
        scaledComboEndAttackForce = Mathf.Lerp(lightComboEndAttackForce, heavyComboEndAttackForce, normalizedT);

        rb3D.mass = scaledPlayerMass;

        animator.SetFloat("SmallAttackSpeed", 1 / scaledSmallAttackDuration);
        animator.SetFloat("ComboEndAttackSpeed", 1 / scaledComboEndAttackDuration);
        animator.SetFloat("BigAttackSpeed", 1 / scaledBigAttackDuration);

        int weight = 300 + (currentScalesValue * 100);
        weightLabel.text = weight + " lbs.";

        UpdateScalesUI();
    }

    public void UpdateScalesUI()
    {
        scalesImage.sprite = scalesSprites[currentScalesValue];

        if (lightUltimateCharge > 0)
        {
            lightFlameTransform.anchoredPosition = flamePositions[currentScalesValue];
        }

        if (heavyUltimateCharge > 0)
        {
            heavyFlameTransform.anchoredPosition = heavyFlamePositions[currentScalesValue];
        }
    }

    public void EndAttack()
    {
        currentAttackState = AttackState.None;
        SwordTrailVFX.enabled = false;
        SwordParticleVFX.enabled = false;
    }

    void TakeDamage(float damage)
    {
        animator.Play("Hit");
        _AudioManger.PlayRandomSoundFromArray(_AudioManger.LJHit);
        DashParticleVFX.enabled = false;
        DashTrailVFX.enabled = false;
        SwordParticleVFX.enabled = false;
        SwordTrailVFX.enabled = false;

        //Debug.Log("Player took Damage: " + damage);
        currentHP -= damage;
    }

    public void IncomingAttack(Enemy attacker, float damage)
    {
        //Debug.Log("Player was hit by " + attackState);
        TakeDamage(damage);
        SwordParticleVFX.enabled = false;
        SwordTrailVFX.enabled = false;
        DashParticleVFX.enabled = false;
        DashTrailVFX.enabled = false;
    }

    public void StartHitStun()
    {
        minimumRecoveryTimer = 0;
        waitingForRecoveryMinimum = true;
        //Debug.Log("Recovery waiting...");

        isStunned = true;
        currentAttackState = AttackState.None;
        //rb3D.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        //agent.updatePosition = false;
        //agent.updateRotation = true;
    }

    public void EndHitStun()
    {
        //Debug.Log("End hit stun");
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
