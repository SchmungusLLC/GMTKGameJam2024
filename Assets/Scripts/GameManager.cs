using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class GameManager : MonoBehaviour
{
    public static GameManager _gameManager { get; private set; }

    public int scalesValue;
    public float lightUltCharge;
    public float heavyUltCharge;

    // Called when the script instance is being loaded
    private void Awake()
    {
        // Check if there is already an instance of GameManager
        if (_gameManager != null && _gameManager != this)
        {
            // If there is, destroy this one since it's a duplicate
            Destroy(gameObject);
        }
        else
        {
            // If this is the first instance, make it the singleton instance
            _gameManager = this;

            // Optionally, prevent this GameManager from being destroyed when changing scenes
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetStatsOnPlayer()
    {
        Debug.Log("Adding values " + scalesValue + ", " + lightUltCharge + ", " + heavyUltCharge);
        player.heavyUltimateCharge = heavyUltCharge;
        player.lightUltimateCharge = lightUltCharge;
        player.currentScalesValue = scalesValue;
    }

    public void SetCarryOverStats(int scales, float light, float heavy)
    {
        scalesValue = scales;
        lightUltCharge = light;
        heavyUltCharge = heavy;
    }

    public void Reset()
    {
        scalesValue=12;
        lightUltCharge=0;
        heavyUltCharge=0;
    }
}
