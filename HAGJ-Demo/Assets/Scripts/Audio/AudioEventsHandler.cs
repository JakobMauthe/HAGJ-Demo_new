using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEventsHandler : MonoBehaviour
{
    
    AudioManager manager;
    MusicSwitch switcher;

    private void OnEnable()
    {
        // subscribe to events
    }


    private void Start()
    {
        manager = GetComponent<AudioManager>();
        switcher = manager.musicSwitch;
    }

    private void SwitchMusic(int trackIndex)
    {
        switcher.SwitchTrack(trackIndex);
    }



    private void OnDisable()
    {
        // unsubscribe to events
    }
}
