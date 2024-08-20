using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;
using static Player;

public class NextScene : MonoBehaviour
{
 public string sceneName;

 void OnTriggerEnter(Collider other)
 {
    if(other.CompareTag("Player"))
    {
        // set the stats to carry over
        _gameManager.SetCarryOverStats(player.currentScalesValue, player.lightUltimateCharge, player.heavyUltimateCharge);

        SceneManager.LoadScene(sceneName);
    }
 } 
}
