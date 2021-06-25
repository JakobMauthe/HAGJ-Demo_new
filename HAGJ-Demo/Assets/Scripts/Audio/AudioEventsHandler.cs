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
    EventManager ev;
    AudioManager au;
    MusicSwitch switcher;
    

    Scene currentScene;
    

    private void Start()
    {
        ev = EventManager.Instance;
        au = GetComponent<AudioManager>();
        currentScene = SceneManager.GetActiveScene();
        
        switcher = au.musicSwitch;
        

        ev.OnJumpInitiated += SendJump;
        ev.OnPlayerGetsHit += SendPlayerHit;
        ev.OnPlayerDeath += SendPlayerDeath;
        ev.OnPlayerLittleAttack += EventMan_OnPlayerLittleAttack;
        ev.OnPlayerHeavyAttack += EventMan_OnPlayerHeavyAttack;
        ev.OnEnemyAttack += EventMan_OnEnemyAttack;
        ev.OnBlockInitiated += EventMan_OnBlockInitiated;
        ev.OnEnemyGetsHit += SendEnemyHit;
        ev.OnEnemyDie += SendEnemyDie;
        ev.OnHealthLow += SendHealthLow;
        ev.OnHealthNotLow += SendHealthRecovered;
        ev.OnStaminaLow += SendStaminaLow;
        ev.OnStaminaNotLow += SendStaminaRecovered;
        

        SceneManager.activeSceneChanged += SelectMusicByScene;

        //SelectMusicByScene(currentScene, currentScene);
    }

    private void SendStaminaRecovered(object sender, System.EventArgs e)
    {
        au.StopLowStaminaSound();
    }

    private void SendStaminaLow(object sender, System.EventArgs e)
    {
        au.TriggerLowStaminaSound();
    }

    private void SendHealthRecovered(object sender, System.EventArgs e)
    {
        au.TriggerLowHealthSound();
    }

    private void SendHealthLow(object sender, System.EventArgs e)
    {
        au.StopLowHealthSound();
    }

    private void SendEnemyDie(object sender, System.EventArgs e)
    {
        au.TriggerFleshHit();
    }

    private void SendEnemyHit(object sender, System.EventArgs e)
    {
        au.TriggerArmourHit();
    }

    private void EventMan_OnBlockInitiated(object sender, System.EventArgs e)
    {
        Debug.Log(this.name + ": onblockintiiated");
        au.TriggerBlockSound();
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



    private void SelectMusicByScene(Scene oldScene, Scene newScene)
    {
        Debug.Log("MUSIC: Scene change detected. New scene: " + newScene.name + ". Switching to new audio container.");
        string sceneName = newScene.name;

        if (sceneName == Loader.Scene.MainMenu.ToString() || sceneName.StartsWith("MainMenu (Audio)"))
        {
            SwitchMusic(0);
        }
        else if (sceneName == Loader.Scene.TestingLevel.ToString() || sceneName.StartsWith("TestingLevel (Audio)"))
        {
            SwitchMusic(1);
        }
        else Debug.LogError(this.name + ": Unknown scene triggered, music cue not set up.");
    }

    void SendSwordSwish(AttackType type)
    {
        au.TriggerSwordSwish(type);
    }

    private void SwitchMusic(int trackIndex)
    {
        switcher.SwitchTrack(trackIndex);
    }


    private void SendPlayerDeath(object sender, System.EventArgs e)
    {
        au.TriggerPlayerDeathAudio();
    }

    private void SendPlayerHit(object sender, System.EventArgs e)
    {
        au.TriggerPlayerTakesDamageAudio(10);
    }

    private void SendJump(object sender, System.EventArgs e)
    {
        au.TriggerPlayerJump();
    }









    private void OnDisable()
    {
        ev.OnJumpInitiated -= SendJump;
        ev.OnPlayerGetsHit -= SendPlayerHit;
        ev.OnPlayerDeath -= SendPlayerDeath;
        ev.OnPlayerLittleAttack -= EventMan_OnPlayerLittleAttack;
        ev.OnPlayerHeavyAttack -= EventMan_OnPlayerHeavyAttack;
        ev.OnEnemyAttack -= EventMan_OnEnemyAttack;
        ev.OnBlockInitiated -= EventMan_OnBlockInitiated;
        ev.OnEnemyGetsHit -= SendEnemyHit;
        ev.OnEnemyDie -= SendEnemyDie;
        ev.OnHealthLow -= SendHealthLow;
        ev.OnHealthNotLow -= SendHealthRecovered;
        ev.OnStaminaLow -= SendStaminaLow;
        ev.OnStaminaNotLow -= SendStaminaRecovered;

        SceneManager.activeSceneChanged -= SelectMusicByScene;
       
    }
}
