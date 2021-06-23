using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSwitch : MonoBehaviour
{
    public AudioSource[] tracks;
    public float crossfadeDuration = 3;
    public int currentMusicTrack = -1;

    [Range(-81, 24), SerializeField] float fadeMaxVolume = 0.0f; // probably don't need this

    public void SwitchTrack(int trackIndex)
    {
        Debug.Log("Switching to track #" + trackIndex + " from #" + currentMusicTrack);
        if (trackIndex == currentMusicTrack) return;

        for (int i = 0; i < tracks.Length; ++i)
        {
            if (i == trackIndex)
            {
                if (!CheckIfPlaying(tracks[i]))
                {
                    tracks[i].GetComponent<AudioSourceController>().PlayLoop();
                }                    
                tracks[i].GetComponent<AudioSourceController>().FadeTo(fadeMaxVolume, crossfadeDuration, 1.0f, false);
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