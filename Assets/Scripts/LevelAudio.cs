using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAudio : MonoBehaviour
{
    public static LevelAudio instance;
    /** tutorial **/
    /*
    //add event to the queue (will execute every few ms)
    AddEvent("Event Name");

    //VERY TIME URGENT!!! BEFORE EVERYTHING ELSE!!!
    audioManager.SendEvent("EventName");

    */
    public string OnStartEvent = "";
    public string OnStartTutorial = "";
    public string WrongItemEvent = "";
    public string shipBoxEvent = "";

    public TimedAudioEvent[] timedEvents = new TimedAudioEvent[1];

    private AudioManager audioManager;
    // Start is called before the first frame update
    void Awake(){
        if (instance != null)
        {
            Debug.LogError("More than one LevelAudio in scene!");
            return;
        }
        instance = this;
    }
    
    void Start()
    {
        while (audioManager == null){
            audioManager = AudioManager.instance;
        } 
    }

    //audio at beginning of level
    public void StartGame(bool newGame){
        if (newGame){
            AddEvent(OnStartTutorial);
        }
        else {
            AddEvent(OnStartEvent);
        }
    }

    public void WrongItem(){
        AddEvent(WrongItemEvent);
    }

    public void ShipBoxAudio(){
        AddEvent(shipBoxEvent);
    }

    public void CalculateTimerEvents(float levelTime){
        //calc times and add to audio manager
        foreach (var timedEvent in timedEvents){
            if (timedEvent == null) continue;
            timedEvent.timer = (timedEvent.timer/100) * levelTime;    
            audioManager.AddTimedEvent(timedEvent);
        }
    }

    //helper function
    private void AddEvent(string eventName){
        if (audioManager == null) return;
        audioManager.audioEvents.Add(eventName);
    }

}

[System.Serializable]
public class TimedAudioEvent {
    public string eventName;
    //0 to 100, 100 being the end of the level
    public float timer;
}
