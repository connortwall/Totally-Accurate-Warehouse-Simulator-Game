using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [Header("Master Volume")]
    public Slider masterVolumeSlider;
    public string masterVolume = "MasterVolume";
    
    [Header("Music Volume")]
    public Slider musicVolumeSlider;
    public string musicVolume = "MusicVolume";
    
    [Header("SFX Volume")]
    public Slider sfxVolumeSlider;
    public string sfxVolume = "SFXVolume";
    
    [Header("Debug")]
    public bool isDebug = false;

    private void Awake()
    {
        AkSoundEngine.RegisterGameObj(gameObject);
        
        masterVolumeSlider.onValueChanged.AddListener(delegate { SetMasterVolume(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { SetSFXVolume(); });
    }

    private void SetMasterVolume()
    {
        if (isDebug)
        {
            Debug.Log("master volume " + (int)(masterVolumeSlider.value * 100));
        }
        
        AkSoundEngine.SetRTPCValue(masterVolume, (int)(masterVolumeSlider.value * 100), gameObject);
    }
    
    private void SetMusicVolume()
    {
        if (isDebug)
        {
            Debug.Log("music volume " + (int)(musicVolumeSlider.value * 100));
        }
        
        AkSoundEngine.SetRTPCValue(musicVolume, (int)(musicVolumeSlider.value * 100), gameObject);
    }
    
    private void SetSFXVolume()
    {
        if (isDebug)
        {
            Debug.Log("sfx volume " + (int)(sfxVolumeSlider.value * 100));
        }
        
        AkSoundEngine.SetRTPCValue(sfxVolume, (int)(sfxVolumeSlider.value * 100), gameObject);
    }
}
