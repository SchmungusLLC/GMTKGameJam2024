using UnityEngine;
using UnityEngine.UI;
using static AudioManager;

public partial class Player
// ============================================================
//                    PLAYER HEALTH
// ============================================================
{
    [Space]
    [Header("Health")]
    public float maxHP;
    private float _currentHP;
    public float currentHP
    {
        get { return _currentHP; }
        set
        {
            _currentHP = value;
            HPBar.value = currentHP;
            if (currentHP > maxHP)
            {
                currentHP = maxHP;
            }
            else if (currentHP < 0)
            {
                PlayerDies();
            }
        }
    }

    public Slider HPBar;

    public void PlayerDies()
    {
        Debug.Log("Player died");
        animator.Play("Death");
        _AudioManger.PlayRandomSoundFromArray(_AudioManger.GoonCelebrate);
    }
}
