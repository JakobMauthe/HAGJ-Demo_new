using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSwitch : MonoBehaviour
{
    public AudioSource[] tracks;
    public float crossfadeDuration = 3;
    public int currentMusicTrack = -1;

    public void SwitchTrack(int trackIndex)
    {
        if (trackIndex == -1)
        {
            for (int i = 0; i < tracks.Length; ++i)
            {
                AudioSourceController controller = tracks[i].GetComponent<AudioSourceController>();
                controller.FadeTo(AudioUtility.minimum, crossfadeDuration * 3f, 0.5f, false);
            }            
        }


        //Debug.Log("MUSIC: Crossfading from track #" + currentMusicTrack + " to #" + trackIndex);
        if (trackIndex == currentMusicTrack) return;

        for (int i = 0; i < tracks.Length; ++i)
        {
            AudioSourceController controller = tracks[i].GetComponent<AudioSourceController>();

            if (i == trackIndex)
            {
                if (!CheckIfPlaying(tracks[i]))
                {
                    controller.PlayLoop();
                }
                controller.FadeTo(0, crossfadeDuration, 1.0f, false);
            }
            else
            {
                controller.FadeTo(AudioUtility.minimum, crossfadeDuration * 3f, 0.5f, false);
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
