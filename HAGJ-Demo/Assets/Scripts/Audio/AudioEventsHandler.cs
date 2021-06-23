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
    EventManager em;

    private void Start()
    {
        em = EventManager.Instance;
        manager = GetComponent<AudioManager>();
        switcher = manager.musicSwitch;

        em.OnJumpInitiated += SendJump;
        em.OnPlayerGetsHit += SendPlayerHit;
        em.OnPlayerDeath += SendPlayerDeath;
    }

    private void SendPlayerDeath(object sender, System.EventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void SendPlayerHit(object sender, System.EventArgs e)
    {
        manager.TriggerPlayerTakesDamageAudio(10);
    }

    private void SendJump(object sender, System.EventArgs e)
    {
        manager.TriggerPlayerJump();
    }

    public void TriggerPlayerOutOfBreathAudio()
    {
        AudioManager.Instance.TriggerStamina();
    }





private void SwitchMusic(int trackIndex)
    {
        switcher.SwitchTrack(trackIndex);
    }





    private void OnDisable()
    {
        em.OnJumpInitiated -= SendJump;
        em.OnPlayerGetsHit -= SendPlayerHit;
        em.OnPlayerDeath -= SendPlayerDeath;
    }
}
