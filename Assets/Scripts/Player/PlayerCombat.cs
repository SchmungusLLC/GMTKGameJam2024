using System.Collections.Generic;
using UnityEngine;

public partial class Player
// ============================================================
//                      PLAYER COMBAT
// ============================================================
{
    [Header("Attack Stats")]
    [Tooltip("Total duration of attacks in seconds")]
    [SerializeField] float attackDuration;
    // Current attack being exeuted (UDLR = Up, Down, Left, Right; N = none)
    [HideInInspector] public char attackState;

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

    void StartAttack(char direction)
    {
        if (attackState != 'N') { return; }

        attackState = direction;
        _animator.Play("Attack" + direction);
    }

    void AttackHitBox()
    {
        if (attackState == 'U' || attackState == 'D')
        {
            targetsStruck = Physics.OverlapBox(_transform.position + _transform.forward * hbOffset, hbSizeV, _transform.rotation, hitLayers);
        }
        else
        {
            targetsStruck = Physics.OverlapBox(_transform.position + _transform.forward * hbOffset, hbSizeH, _transform.rotation, hitLayers);
        }

        foreach (Collider target in targetsStruck)
        {
            Debug.Log("Hit target " + target.gameObject.name);
            target.gameObject.GetComponent<Enemy>()?.IncomingAttack(attackState);
            target.gameObject.GetComponent<Destructible>()?.Struck(_rigidbody, attackState);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.matrix = Matrix4x4.TRS(_transform.position + _transform.forward * hbOffset, _transform.rotation, transform.localScale);
        Gizmos.DrawCube(Vector3.zero, new Vector3(hbSizeH.x * 2, hbSizeH.y * 2, hbSizeH.z * 2));
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(Vector3.zero, new Vector3(hbSizeV.x * 2, hbSizeV.y * 2, hbSizeV.z * 2));
    }

    void EndAttack()
    {
        attackState = 'N';
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
