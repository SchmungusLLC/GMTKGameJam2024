using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static AudioManager;
using static GameManager;

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
        //Debug.Log("Player died");
        PlayerInput input = GetComponent<PlayerInput>();
        input.enabled = false;
        animator.Play("Death", 3);
        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            enemy.enabled = false;
        }
        _AudioManger.PlayRandomSoundFromArray(_AudioManger.GoonCelebrate);        
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSecondsRealtime(2);
        _gameManager.Reset();
        SceneManager.LoadScene("MainMenu");
    }
}
