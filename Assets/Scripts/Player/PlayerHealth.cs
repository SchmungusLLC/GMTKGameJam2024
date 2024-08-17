using UnityEngine;

public partial class Player
// ============================================================
//                    PLAYER HEALTH
// ============================================================
{
    [Space]
    [Header("Health Stats")]
    private float _scaledMaxHP;
    public float scaledMaxHP
    {
        get { return _scaledMaxHP; }
        set
        {
            _scaledMaxHP = value;
            if (currentHP > _scaledMaxHP)
            {
                currentHP = _scaledMaxHP;
            }
        }
    }

    public float defaultMaxHP;
    public float currentHP;
}
