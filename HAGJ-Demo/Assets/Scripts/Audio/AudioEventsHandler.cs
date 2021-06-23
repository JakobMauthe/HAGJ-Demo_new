using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A bit easier to keep the event subscription and the functions separate.
/// </summary>
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

    #region Public Methods
    public void TriggerPlayerTakesDamageAudio(float damage)
    {
        AudioManager.Instance.TriggerPlayerTakesDamageAudio(damage);
    }

    public void TriggerPlayerOutOfBreathAudio()
    {
        AudioManager.Instance.TriggerStamina();
    }



#endregion



private void SwitchMusic(int trackIndex)
    {
        switcher.SwitchTrack(trackIndex);
    }





    private void OnDisable()
    {
        // unsubscribe to events
    }
}
