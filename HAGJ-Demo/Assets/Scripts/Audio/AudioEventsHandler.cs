using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
/// <summary>
/// A bit easier to keep the event subscription and the functions separate.
/// </summary>
public class AudioEventsHandler : MonoBehaviour
{
    
    AudioManager AudioMan;
    MusicSwitch switcher;
    EventManager EventMan;

    Scene currentScene;
    

    private void Start()
    {
        EventMan = EventManager.Instance;
        AudioMan = GetComponent<AudioManager>();
        currentScene = SceneManager.GetActiveScene();
        
        switcher = AudioMan.musicSwitch;
        

        EventMan.OnJumpInitiated += SendJump;
        EventMan.OnPlayerGetsHit += SendPlayerHit;
        EventMan.OnPlayerDeath += SendPlayerDeath;

        SceneManager.activeSceneChanged += SelectMusicByScene;

        SelectMusicByScene(currentScene, currentScene);
    }
    private void SelectMusicByScene(Scene oldScene, Scene newScene)
    {
        Debug.Log("MUSIC: Scene change detected. New scene: " + newScene.name + ". Switching to new audio container.");
        string sceneName = newScene.name;

        if (sceneName.StartsWith("Main Menu"))
        {
            SwitchMusic(0);
        }
        else if (sceneName.StartsWith("Audio Lab"))
        {
            SwitchMusic(1);
        }
        else Debug.LogError(this.name + ": Unknown scene triggered, music cue not set up.");
    }

    private void SwitchMusic(int trackIndex)
    {
        switcher.SwitchTrack(trackIndex);
    }


    private void SendPlayerDeath(object sender, System.EventArgs e)
    {
        AudioMan.TriggerPlayerDeathAudio();
    }

    private void SendPlayerHit(object sender, System.EventArgs e)
    {
        AudioMan.TriggerPlayerTakesDamageAudio(10);
    }

    private void SendJump(object sender, System.EventArgs e)
    {
        AudioMan.TriggerPlayerJump();
    }

    public void TriggerPlayerOutOfBreathAudio()
    {
        AudioMan.TriggerStamina();
    }








    private void OnDisable()
    {
        EventMan.OnJumpInitiated -= SendJump;
        EventMan.OnPlayerGetsHit -= SendPlayerHit;
        EventMan.OnPlayerDeath -= SendPlayerDeath;
        SceneManager.activeSceneChanged -= SelectMusicByScene;
    }
}
