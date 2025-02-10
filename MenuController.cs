using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public string newGameLevel;

    public void StartGame()
    {
        SceneManager.LoadScene(newGameLevel);
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
