using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // control system
    public GameObject pauseScreen, settingsScreen, controlsScreen, saveButton;
    //access to event system + buttons
    public GameObject[] pauseButtons = new GameObject[6];

    public TMP_Text saveText;

    [Header("Player Input GameObject")]
    public PlayerInput control;

    private InputAction pauseAction = null;

    void Awake(){

    }

    // Start is called before the first frame update
    void Start()
    {
        //set up control menu
        if (control == null) throw new Exception("PlayerInput object could not be loaded.");
        //set up pause action
        pauseAction = control.actions["Pause"];
        //hide all menus
        CloseSettings();
        CloseControls();
        ResumeGame();
    }

    // Update is called once per frame
    void Update()
    {
        // controls.MenuActions.Pause.ReadValue<float>();
        // if (controls.MenuActions.Pause.triggered){
        //     print("pause button pressed");
        // }
        if (Input.GetKeyDown(KeyCode.P) || pauseAction.triggered)
        {
            print("Opening Pause Menu...");
            PauseButtonPress();
        }
    }

    public void PauseButtonPress(){
        if (pauseScreen.activeSelf) ResumeGame();
        else PauseGame();
    }

    public void PauseGame(){
        Time.timeScale = 0;
        //select resume game button as starting button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseButtons[0]);
        //open menu
        pauseScreen.SetActive(true);
    }

    public void ResumeGame(){
        print("Resuming Game...");
        Time.timeScale = 1;
        //deselect menu buttons to prevent accidental clicks
        EventSystem.current.SetSelectedGameObject(null);
        //close menu
        pauseScreen.SetActive(false);
    }

    public void OpenControls(){
        controlsScreen.SetActive(true);
        //select close controls button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseButtons[5]);
    }

    public void CloseControls(){
        controlsScreen.SetActive(false);
        //select open controls button
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseButtons[1]);
    }

    public void OpenSettings(){
        settingsScreen.SetActive(true);
    }

    public void CloseSettings(){
        settingsScreen.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseButtons[2]);
    }

    public void SaveGame(){
        //game is already saved! This is all for show
        saveButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "[Game Saved!]"; //Set text on button
    }

    public void ExitMainMenu(){
        // ResumeGame();
        print("Returning to Main Menu...");
        // get name of current scene
        Scene gameScene = SceneManager.GetActiveScene();
        //load main menu
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainMenu"));
        //unload game scene
        SceneManager.UnloadSceneAsync(gameScene);
    }

    public void ExitDesktop(){
        print("Exiting game...");
        //save player preferences before quitting
        PlayerPrefs.Save();
        Application.Quit();
    }
}
