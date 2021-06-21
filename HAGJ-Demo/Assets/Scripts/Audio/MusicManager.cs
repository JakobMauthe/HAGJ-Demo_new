using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
   public MusicSwitch musicSwitch;

    public KeyCode switchKey = KeyCode.X;
    public int currentTrackIndex = -1;
    void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            currentTrackIndex = (currentTrackIndex + 1) % (musicSwitch.tracks.Length - 1);
            musicSwitch.SwitchTrack(currentTrackIndex);
            
        }
    }


}
