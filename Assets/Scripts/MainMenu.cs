using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame () 
    {
        SceneManager.LoadScene("CutScene1");
    }

    public void QuitGame()
    {
        Debug.Log("The Case is Closed");
        Application.Quit();
    }

    public void PlayLevel(string name)
    {
        SceneManager.LoadScene(name);
    }
}
