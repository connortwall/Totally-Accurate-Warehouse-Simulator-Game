using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public string initWwise = "PlayLevelSound";

    public bool sceneAudioEnabled = true;
    public string sceneAudio = "";
    public bool stopAllAudioOnSceneExit = true;
    public string stopAllAudioEvent = "";

    public bool timedEventsEnabled = true;

    public List<string> audioEvents = new List<string>();

    private List<TimedAudioEvent> timedEvents = new List<TimedAudioEvent>();
    [HideInInspector] 
    public float timeRemaining = -1;


    void Awake(){
        if (instance != null)
        {
            Debug.LogError("More than one AudioManager in scene!");
            return;
        }
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //wise stuff
        AkSoundEngine.RegisterGameObj(gameObject);
        SendEvent(initWwise);

        if (sceneAudioEnabled) SendEvent(sceneAudio);
    }

    // Update is called once per frame
    void Update()
    {
        //send all audio events
        if (audioEvents.Count > 0){
            //send all events to wise
            foreach (var eventName in audioEvents){
                SendEvent(eventName);
            } 
            //delete them from the list
            audioEvents.Clear();
        }

        //if timed events are not being used
        if (timeRemaining == -1 || !timedEventsEnabled) return;

        List<int> remList = new List<int>();
        for (int i = 0; i < timedEvents.Count; i++){
            if (timeRemaining <= timedEvents[i].timer){
                //add event to queue
                audioEvents.Add(timedEvents[i].eventName);
                //queue for removal
                remList.Add(i);
            }
        }

        //remove any timed events that were added to queue
        foreach (int index in remList){
            if (index >= timedEvents.Count) break;
            timedEvents.RemoveAt(index);
        }

        
    }

    void OnDestroy() {
        if (stopAllAudioOnSceneExit){
            SendEvent(stopAllAudioEvent);
        }
    }

    public void SendEvent(string eventName){
        //do not send event if the event is empty
        if (string.Equals("", eventName)) return;
        AkSoundEngine.PostEvent(eventName, gameObject);
    }

    public void AddTimedEvent(TimedAudioEvent timedEvent){
        timedEvents.Add(timedEvent);
    }

}
