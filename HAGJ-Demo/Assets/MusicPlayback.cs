using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayback : MonoBehaviour
{
    MusicSwitch mswitch;

    float trackIndexLower = 1;
    float trackIndexUpper = 3;

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
            Debug.Log("MusicPlayback: choosing new audio track. Track #" + newTrackIndex +" selected.");
            SwitchTrack(newTrackIndex);
            yield return new WaitForSeconds(10);
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
