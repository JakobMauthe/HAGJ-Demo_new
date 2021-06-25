using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
/// <summary>
/// A bit easier to keep the event subscription and the functions separate.
/// </summary>
/// 


public enum AttackType
{
    light,
    heavy
}

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
        EventMan.OnPlayerLittleAttack += EventMan_OnPlayerLittleAttack;
        EventMan.OnPlayerHeavyAttack += EventMan_OnPlayerHeavyAttack;
        EventMan.OnEnemyAttack += EventMan_OnEnemyAttack;


        //EventMan.OnBlockInitiated += EventMan_OnBlockInitiated;

        SceneManager.activeSceneChanged += SelectMusicByScene;

        //SelectMusicByScene(currentScene, currentScene);
    }

    
    private void EventMan_OnBlockInitiated(object sender, System.EventArgs e)
    {
        
    }


    private void EventMan_OnPlayerHeavyAttack(object sender, System.EventArgs e)
    {
        AudioManager.Instance.TriggerPlayerAttackAudio(AttackType.heavy);
        SendSwordSwish(AttackType.heavy);
    }

    private void EventMan_OnPlayerLittleAttack(object sender, System.EventArgs e)
    {
        AudioManager.Instance.TriggerPlayerAttackAudio(AttackType.light);
        SendSwordSwish(AttackType.light);
    }
    private void EventMan_OnEnemyAttack(object sender, System.EventArgs e)
    {
        SendSwordSwish(AttackType.light);
    }



    void SelectMusicByScene(Scene oldScene, Scene newScene)
    {
        Debug.Log("MUSIC: Scene change detected. New scene: " + newScene.name + ". Switching to new audio container.");
        string sceneName = newScene.name;

        if (sceneName == Loader.Scene.MainMenu.ToString() || sceneName.StartsWith("MainMenu (Audio)"))
        {
            SwitchMusic(0);
        }
        if (sceneName == Loader.Scene.TestingLevel.ToString() || sceneName.StartsWith("TestingLevel (Audio)"))
        {
            SwitchMusic(1);
        }
        else Debug.LogError(this.name + ": Unknown scene triggered, music cue not set up.");
    }


    void SendSwordSwish(AttackType type)
    {
        AudioMan.TriggerSwordSwish(type);
    }

    void SendArmourHit()
    {
        AudioMan.TriggerArmourHit();
    }
    void SendFleshHit()
    {
        AudioMan.TriggerFleshHit();
    }

    void SwitchMusic(int trackIndex)
    {
        switcher.SwitchTrack(trackIndex);
    }

    void SendPlayerDeath(object sender, System.EventArgs e)
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
        EventMan.OnPlayerLittleAttack -= EventMan_OnPlayerLittleAttack;
        EventMan.OnPlayerHeavyAttack -= EventMan_OnPlayerHeavyAttack;
        //EventMan.OnBlockInitiated -= EventMan_OnBlockInitiated;
    }
}
