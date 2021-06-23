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
            float clipLength = mswitch.tracks[mswitch.currentMusicTrack].clip.length;
            float clipTime = mswitch.tracks[mswitch.currentMusicTrack].time;
            float timeRemaining = clipLength - clipTime;
            float offsetInSecs = 5;
            float waitTime = timeRemaining - offsetInSecs;
            Debug.Log("Waiting " + waitTime + "secs. (clipLength = " + clipLength + ", clipTime = " + clipTime + " timeremaining = " + timeRemaining);



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
