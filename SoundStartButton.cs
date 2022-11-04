using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundStartButton : MonoBehaviour
{

    public bool Debug_Enabled = false;
    public string StartButtonPress = "Stop_menumusic";



    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.RegisterGameObj(gameObject);
    }

    void Destroy()
    {
        if (Debug_Enabled) { Debug.Log("triggered sound on start button"); }
        AkSoundEngine.PostEvent(StartButtonPress, gameObject);
    }

}