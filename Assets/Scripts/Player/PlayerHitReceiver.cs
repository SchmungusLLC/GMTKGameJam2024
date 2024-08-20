using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class PlayerHitReceiver : MonoBehaviour
{
    public void AttackHitBox()
    {
        player.AttackHitBox();
    }

    public void EndAttack()
    {
        player.EndAttack();
    }

    public void StartHitStun()
    {
        player.StartHitStun();
    }

    public void EnableSwordVFX()
    {
        player.EnableSwordVFX();
    }

    public void EnableScalesVFX()
    {
        //player.EnableScalesVFX();
    }
}
