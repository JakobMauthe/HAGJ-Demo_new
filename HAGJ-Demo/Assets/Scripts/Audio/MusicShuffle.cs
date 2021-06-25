using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicShuffle : MonoBehaviour
{
    MusicSwitch mswitch;

    [SerializeField, Min(0)] float trackIndexLower = 1;
    [SerializeField, Min(0)] float trackIndexUpper = 3;

    private void Start()
    {
        mswitch = GetComponent<MusicSwitch>();
        BeginMusicPlayback();
    }

    public void BeginMusicPlayback()
    {
        StartCoroutine(ShuffleMusic());
    }

    public void StopMusicPlayback()
    {
        StopAllCoroutines();
    }

    IEnumerator ShuffleMusic()
    {
        while (true)
        {

            int newTrackIndex = Mathf.RoundToInt(Random.Range(trackIndexLower, trackIndexUpper));            
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
            
            
            Debug.Log("MUSIC: Waiting " + waitTime + "secs until new music transition. (clipLength = " + newClipLength + ", clipTime = " + newClipPlaytime + " timeremaining = " + timeRemaining);

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
