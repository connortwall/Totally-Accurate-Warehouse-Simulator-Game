using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundDoll : MonoBehaviour
{

        public bool Debug_Enabled = false;
        public string EnemyDestroy = "Destroy";



        // Start is called before the first frame update
        void Start()
        {
            AkSoundEngine.RegisterGameObj(gameObject);
        }

        public void Destroy()
        {
            if (Debug_Enabled) { Debug.Log("Triggered doll destroy"); }
            AkSoundEngine.PostEvent(EnemyDestroy, gameObject);
        }
   
}