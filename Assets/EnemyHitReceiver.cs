using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitReceiver : MonoBehaviour
{
    public Enemy enemy;

    public void AttackHitBox()
    {
        enemy.AttackHitBox();
    }

    public void EndAttack()
    {
        enemy.EndAttack();
    }

    public void EndHitStun()
    {
        enemy.EndHitStun();
    }

    public void StartHitStun()
    {
        enemy.StartHitStun();
    }
}
