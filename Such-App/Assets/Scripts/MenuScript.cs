using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script for the main menu. Scene management
/// </summary>
public class MenuScript : MonoBehaviour
{
    public void startGame()
    {
        SceneManager.LoadScene(1);
    }

    public void openSettings()
    {
        SceneManager.LoadScene(2);
    }
}
