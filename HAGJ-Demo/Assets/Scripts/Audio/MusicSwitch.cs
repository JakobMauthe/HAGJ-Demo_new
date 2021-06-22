using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSwitch : MonoBehaviour
{
    public AudioSource[] tracks;
    public float crossfadeDuration = 3;
    public int currentMusicTrack = 0;

    public float extVol;

    public void SwitchTrack(int trackIndex)
    {
        for (int i = 0; i < tracks.Length; ++i)
        {
            if (i == trackIndex)
            {
                if (!CheckIfPlaying(tracks[i]))
                {
                    tracks[i].GetComponent<AudioSourceController>().PlayLoop();
                }                    
                tracks[i].GetComponent<AudioSourceController>().FadeTo(extVol, crossfadeDuration, 1.0f, false);
            }
            else
            {
                tracks[i].GetComponent<AudioSourceController>().FadeTo(AudioUtility.MinSoundLevel(), crossfadeDuration * 3f, 0.5f, false);
            }
        }
        currentMusicTrack = trackIndex;
    }

    private static bool CheckIfPlaying(AudioSource source)
    {
        if (source.isPlaying || source.GetComponent<AudioSourceController>().looperIsLooping)
            return true;
        else
            return false;
    }

}