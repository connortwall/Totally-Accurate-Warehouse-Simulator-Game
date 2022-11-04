using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Audio;
using UnityEngine.EventSystems;


//for behaviors of the settings menu allowed in all areas of the game
public class SettingsMenu : MonoBehaviour
{
    //label for language toggle in Unity
    public TMP_Text langLabel; 
    public Button leftLangButton;
    public Button rightLangButton;

    //access to music mixer and audio channels
    public AudioMixer audMixer;
    public Slider[] audSliders = new Slider[3];
    public TMP_Text[] audLabels = new TMP_Text[3];

    //access to event system + buttons
    public EventSystem mainEventSystem;
    public GameObject[] settingsButtons = new GameObject[4];
    
    //list of languages to swap between
    private List<string> languages = new List<string>();
    //index of current language 
    private int currLanguageIndex;
    //music settings
    private float masterVolume;
    private float sfxVolume;
    private float musicVolume;
    private GameObject settingsScreen;



    // Start is called before the first frame update
    void Start()
    {
        //make sure buttons loaded
        if (leftLangButton == null || rightLangButton == null) throw new Exception("Language buttons could not be loaded.");
        //load settings screen
        //find the settings menu and verify found
		settingsScreen = GameObject.Find("SettingsScreen");
		if (settingsScreen == null) throw new Exception("Settings screen could not be loaded");

        //populate langs
        languages.Add("English");
        languages.Add("Danish");
        //reflect current user prefs in menu
        RefreshSettings();
    }

    // Update is called once per frame
    void Update()
    {
        //make sure you are not on a disabled language button
        if (EventSystem.current.currentSelectedGameObject == leftLangButton && !leftLangButton.interactable){
            //select right button because left is disabled
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(settingsButtons[1]);
        }
        
    }

    // changing the language (left)
    public void LanguageLeft(){
        //make sure there are options left
        if (currLanguageIndex <= 0) {
            UpdateLangLabel();
            return;
        }

        //update index
        currLanguageIndex -= 1;
        UpdateLangLabel();
    }

    // changing the language (right)
    public void LanguageRight(){

        //make sure there are options left
        if (currLanguageIndex >= languages.Count-1) {
            UpdateLangLabel();
            return;
        }

        //update index
        currLanguageIndex += 1;
        UpdateLangLabel();
    }

    //Changes the label between danish and english (includes support for more langs)
    public void UpdateLangLabel(){
        langLabel.text = languages[currLanguageIndex];

        //enable/disable buttons if all options exhausted
        if (currLanguageIndex <= 0 ){
            leftLangButton.interactable = false;
            //select right button because left is disabled
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(settingsButtons[1]);
        } 
        else {
            leftLangButton.interactable = true;
        }

        if (currLanguageIndex >= languages.Count-1) {
            rightLangButton.interactable = false;
            //select left button because right is disabled
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(settingsButtons[0]);
        }
        else {
            rightLangButton.interactable = true;
        }
    }

    /*
    * The following 3 functions are called when sliders
    * are changed by the user in the GUI to keep vars up 
    * to date
    */
    public void SetMasterVolume(){
        masterVolume = audSliders[0].value;
        SetAudioVisuals();
    }

    public void SetSFXVolume(){
        sfxVolume = audSliders[1].value;
        SetAudioVisuals();
    }

    public void SetMusicVolume(){
        musicVolume = audSliders[2].value;
        SetAudioVisuals();
    }

    //change GUI sliders to reflect audio level number
    public void SetAudioVisuals(){
        //update text labels
        audLabels[0].text = (masterVolume * 100).ToString("N0");
        audLabels[1].text = (sfxVolume * 100).ToString("N0");
        audLabels[2].text = (musicVolume * 100).ToString("N0");

        //update sliders
        audSliders[0].value = masterVolume;
        audSliders[1].value = sfxVolume;
        audSliders[2].value = musicVolume;
    }

    //apply changes
    public void ApplyChanges() {
        ApplyVolume();
        ApplyLanguage();
        //save settings to player preferences
        PlayerPrefs.Save();

    }

    //actually change volume with wwise
    public void ApplyVolume(){
        //check for changes in each of the volume sections, then apply if change detected
        if (PlayerPrefs.GetFloat("masterVolume") != masterVolume){
            //save to prefs
            PlayerPrefs.SetFloat("masterVolume", masterVolume);
            //send update to wwise
            AkSoundEngine.SetRTPCValue("MasterVolume", masterVolume);
        }

        if (PlayerPrefs.GetFloat("sfxVolume") != sfxVolume){
            //save to prefs
            PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
            //send update to wwise
            AkSoundEngine.SetRTPCValue("SFXVolume", sfxVolume);
            
        }

        if (PlayerPrefs.GetFloat("musicVolume") != musicVolume){
            //save to prefs
            PlayerPrefs.SetFloat("musicVolume", musicVolume);
            //send update to wwise
            AkSoundEngine.SetRTPCValue("MusicVolume", musicVolume);
        }
    }

    //CHANGE LANGUAGE HERE
    public void ApplyLanguage(){
        if (PlayerPrefs.GetInt("language") != currLanguageIndex){
            PlayerPrefs.SetInt("language", currLanguageIndex);
        }
    }

    /*
     * Called to change the visuals of the settings menu
     * (Useful on chance of user exiting the menu without
     * applying changes)
     */
    public void RefreshSettings(){
        //update language to saved setting
        currLanguageIndex = PlayerPrefs.GetInt("language");
        UpdateLangLabel();

        //update volume controls to saved settings
        masterVolume = PlayerPrefs.GetFloat("masterVolume");
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        musicVolume = PlayerPrefs.GetFloat("musicVolume");

        //update sliders
        SetAudioVisuals();
    }

    //start controller selection at language (top) of settings menu
    public void OpenSettings(){
        EventSystem.current.SetSelectedGameObject(null);
        if (currLanguageIndex > 0 ){
            EventSystem.current.SetSelectedGameObject(settingsButtons[0]);
        }
        else {
            EventSystem.current.SetSelectedGameObject(settingsButtons[1]);
        }
    }

    public void CloseSettings(){
        //replace any settings that were not applied
        RefreshSettings();
    }
}
