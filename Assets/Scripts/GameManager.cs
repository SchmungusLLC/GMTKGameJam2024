using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _gameManager;
    public static GameManager gameManager
    {
        get
        {
            if (!_gameManager)
            {
                _gameManager = new GameObject().AddComponent<GameManager>();
                DontDestroyOnLoad(_gameManager.gameObject);
            }
            return _gameManager;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
