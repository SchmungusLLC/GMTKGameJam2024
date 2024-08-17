using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public partial class Enemy
// ============================================================
//                      ENEMY COMBAT
// ============================================================
{
    [Header("Combat Stats")]
    [Tooltip("Total duration of attacks in seconds")]
    [SerializeField] float attackDuration;

    [Header("Hitbox Settings")]
    [Tooltip("Layers which this attack's hitbox should check against")]
    [SerializeField] LayerMask hitLayers;
    [Tooltip("Half-extents for horizontal hitbox")]
    [SerializeField] Vector3 hbSizeH;
    [Tooltip("Half-extents for vertical hitbox")]
    [SerializeField] Vector3 hbSizeV;
    [Tooltip("Forward offset for hitboxes")]
    [SerializeField] float hbOffset;

    // list of targets struck by attack hitbox
    private Collider[] targetsStruck;

    public void IncomingAttack(char playerAttackDir)
    {
        Debug.Log($"Enemy was hit from {playerAttackDir}");
    }
}
