using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

public class MainMenu : MonoBehaviour
{
    //set first scene of the game within Unity
    public string firstScene;

    private GameObject settingsScreen;
    private GameObject controlsScreen;
    public GameObject creditsScreen;

    //create buttons for controller nagivations
    public GameObject startButton, controlsButton, settingsButton, exitButton, controlsBackButton;
    public GameObject creditsBackButton, creditsButton;
    
    public Button loadButton;


    // Start is called before the first frame update
    void Start()
    {
        //find the settings menu and verify found, then disable
		settingsScreen = GameObject.Find("SettingsScreen");
		if (settingsScreen == null) throw new Exception("Settings screen could not be loaded");
        settingsScreen.SetActive(false);

        //find the controls page and verify found, then disable
		controlsScreen = GameObject.Find("ControlsScreen");
		if (controlsScreen == null) throw new Exception("Controls screen could not be loaded");
        CloseControls();
        CloseCredits();

        //clear controller selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set beginning selection to Start Game
        EventSystem.current.SetSelectedGameObject(startButton);

        //create user preferences if they don't exist
        CreateUserPrefs();

        //disable load button if there is no loaded game
        if (PlayerPrefs.GetInt("newGame") == 1 || PlayerPrefs.GetInt("savedLives") == -1){
            loadButton.interactable = false;
        }
        else loadButton.interactable = true;
    }


    public void StartGame(){
        //start a new game/overwrite all old data.
        PlayerPrefs.SetString("savedLevel", firstScene);
        PlayerPrefs.SetInt("savedLives", -1);
        PlayerPrefs.SetInt("savedScore", 0);

        //this var tells the game manager to reset info when it loads
        PlayerPrefs.SetInt("newGame", 1);

        //load the first level
        SceneManager.LoadScene(firstScene);
    }

    public void LoadGame(){
        //make sure there is no required new game
        Assert.IsFalse(PlayerPrefs.GetInt("newGame") == 1);
        SceneManager.LoadScene(PlayerPrefs.GetString("savedLevel"));
    }

    public void OpenControls(){
        controlsScreen.SetActive(true);
        //set selection to controls back button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsBackButton);
    }

    public void CloseControls(){
        controlsScreen.SetActive(false);
        //set selection to controls button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsButton);
    }

    public void OpenCredits(){
        creditsScreen.SetActive(true);
        //set selection to controls back button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(creditsBackButton);
    }

    public void CloseCredits(){
        creditsScreen.SetActive(false);
        //set selection to controls button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(creditsButton);
    }

    public void OpenSettings(){
        settingsScreen.SetActive(true);
    }

    public void CloseSettings(){
        settingsScreen.SetActive(false);

        //set selection to settings
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingsButton);
    }

    public void ExitGame(){
        print("Exiting game...");
        //save player preferences before quitting
        PlayerPrefs.Save();
        Application.Quit();
    }

    //checks if the user preferences have been created; else creates them
    private void CreateUserPrefs(){
        //settings
        if (!PlayerPrefs.HasKey("language")) PlayerPrefs.SetInt("language", 0);
        if (!PlayerPrefs.HasKey("sfxVolume")) PlayerPrefs.SetFloat("masterVolume", 0.75f);
        if (!PlayerPrefs.HasKey("sfxVolume")) PlayerPrefs.SetFloat("sfxVolume", 0.75f);
        if (!PlayerPrefs.HasKey("musicVolume")) PlayerPrefs.SetFloat("musicVolume", 0.75f);

        //save game
        if (!PlayerPrefs.HasKey("savedLevel")) PlayerPrefs.SetString("savedLevel", firstScene);
        if (!PlayerPrefs.HasKey("savedLives")) PlayerPrefs.SetInt("savedLives", -1);
        if (!PlayerPrefs.HasKey("savedScore")) PlayerPrefs.SetInt("savedScore", 0);
        if (!PlayerPrefs.HasKey("newGame")) PlayerPrefs.SetInt("newGame", 1);
    }
}
