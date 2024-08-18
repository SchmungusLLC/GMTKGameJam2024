using UnityEngine;

public partial class Player
// ============================================================
//                    PLAYER HEALTH
// ============================================================
{
    [Space]
    [Header("Health Stats")]
    private float _maxHP;
    public float maxHP
    {
        get { return _maxHP; }
        set
        {
            _maxHP = value;
            if (currentHP > _maxHP)
            {
                currentHP = _maxHP;
            }
            else if (currentHP < 0)
            {
                PlayerDies();
            }
        }
    }

    public float currentHP;

    public void PlayerDies()
    {
        Debug.Log("Player died");
    }
}
