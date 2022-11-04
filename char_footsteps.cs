using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class char_footsteps : MonoBehaviour

{
    public bool Debug_Enabled = false;
    public string LeftFootWalk = "fs_leftfoot";
    public string RightFootWalk = "fs_rightfoot";


    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.RegisterGameObj(gameObject);
    }

    void fs_leftfoot()
    {
        if (Debug_Enabled) { Debug.Log("Left Foot Triggered"); }
        AkSoundEngine.PostEvent(LeftFootWalk, gameObject);
    }

    void fs_rightfoot()
    {
        if (Debug_Enabled) { Debug.Log("Right Foot Triggered"); }
        AkSoundEngine.PostEvent(RightFootWalk, gameObject);
    }
}
