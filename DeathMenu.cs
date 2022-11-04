using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.EventSystems;

public class DeathMenu : MonoBehaviour
{
    //set first scene of the game within Unitys
    public string firstLevelScene = "Level_one";
    public string mainMenuScene = "MainMenu";


    //create buttons for controller nagivations
    public GameObject gameRestartButton, levelRestartButton, exitButton;

    // Start is called before the first frame update
    void Start()
    {
        //clear controller selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set beginning selection to Start Game
        EventSystem.current.SetSelectedGameObject(exitButton);
    }

    public void ExitToMenu()
    {
        Debug.Log("Exiting game...");
        SceneManager.LoadScene(mainMenuScene);
    }

    public void LevelRestart()
    {
        Debug.Log("Level restarting...");

        SceneManager.LoadScene(PlayerPrefs.GetString("savedLevel"));
    }

    public void GameRestart()
    {
        Debug.Log("Game restarting...");
        SceneManager.LoadScene(firstLevelScene);
    }
}
