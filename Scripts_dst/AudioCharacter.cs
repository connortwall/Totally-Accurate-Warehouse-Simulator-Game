using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCharacter : MonoBehaviour
{
    [Header("Sounds")]
    public List<string> startMoveSounds = new List<string>();
    public List<string> startSwingSounds = new List<string>();
    
    public List<string> moveSounds = new List<string>();
    public List<string> deathSounds = new List<string>();

    [Header("Debug")] 
    public bool isDebug = false;

    // Start is called before the first frame update
    void Awake()
    {
        AkSoundEngine.RegisterGameObj(gameObject); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateSounds(List<string> sounds)
    {
        if (isDebug)
        {
            Debug.Log(sounds.Count);
        }
        
        sounds.ForEach(sound => AkSoundEngine.PostEvent(sound, gameObject));
    }
}
