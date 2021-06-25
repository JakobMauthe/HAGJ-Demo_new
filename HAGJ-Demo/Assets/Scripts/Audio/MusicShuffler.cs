using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicShuffler : MonoBehaviour
{
    MusicSwitch mswitch;

    private void Start()
    {
        mswitch = GetComponent<MusicSwitch>();
    }

    public void BeginShuffling(int lowerRange, int upperRange)
    {
        StartCoroutine(ShuffleMusic(lowerRange, upperRange));
    }

    public void StopShuffling()
    {
        StopAllCoroutines();
    }

    IEnumerator ShuffleMusic(int lowerRange, int upperRange)
    {
        while (true)
        {

            int newTrackIndex = Random.Range(lowerRange, upperRange + 1);
            SwitchTrack(newTrackIndex);
            
            yield return new WaitForEndOfFrame();
            AudioSource newSource = mswitch.tracks[mswitch.currentMusicTrack];
            float newClipLength = newSource.clip.length;
            float newClipPlaytime = newSource.time; 
            float timeRemaining = newClipLength - newClipPlaytime;
            float crossfadeOffset = mswitch.crossfadeDuration;
            float waitTime;
            if (timeRemaining > crossfadeOffset)
            {
                waitTime = timeRemaining - crossfadeOffset;
            }
            else waitTime = timeRemaining;
            
            
            //Debug.Log("MUSIC: Waiting " + waitTime + "secs until new music transition. (clipLength = " + newClipLength + ", clipTime = " + newClipPlaytime + " timeremaining = " + timeRemaining);

            yield return new WaitForSeconds(waitTime);
        }

    }

    private void SwitchTrack(int newTrackIndex)
    {
        mswitch.SwitchTrack(newTrackIndex);
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }


}
