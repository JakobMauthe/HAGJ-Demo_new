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
    MusicShuffler shuffler;
    Scene currentScene;

    private void Start()
    {
        ev = EventManager.Instance;
        au = GetComponent<AudioManager>();
        currentScene = SceneManager.GetActiveScene();
        
        switcher = au.musicSwitch;
        shuffler = au.shuffler;
        

        ev.OnJumpInitiated += SendJump;
        ev.OnPlayerGetsHit += SendPlayerHit;
        ev.OnPlayerDeath += SendPlayerDeath;
        ev.OnPlayerLittleAttack += SendPlayerLittleAttack;
        ev.OnPlayerHeavyAttack += SendPlayerHeavyAttack;
        ev.OnEnemyAttack += SendEnemyAttack;
        ev.OnBlockInitiated += SendBlockStart;
        ev.OnEnemyGetsHit += SendEnemyHit;
        ev.OnEnemyDie += SendEnemyDie;
        ev.OnHealthLow += SendHealthLow;
        ev.OnHealthNotLow += SendHealthRecovered;
        ev.OnStaminaLow += SendStaminaLow;
        ev.OnStaminaNotLow += SendStaminaRecovered;       

        SceneManager.activeSceneChanged += CueMusicByScene;

        CueMusicByScene(currentScene, currentScene);
    }


    private void CueMusicByScene(Scene oldScene, Scene newScene)
    {
        Debug.Log("MUSIC: Scene change detected. New scene: " + newScene.name + ". Switching to new audio container.");
        currentScene = newScene;
        string sceneName = newScene.name;


        if (sceneName.StartsWith(Loader.Scene.MainMenu.ToString()))
        {
            SwitchMusic(0);
            shuffler.StopShuffling();
            for (int i = 0; i < au.environmentObjects.Length; ++i)
            {
                au.environmentObjects[i].GetComponent<AudioSourceController>().FadeTo(AudioUtility.minimum, 3, 0.5f, true);
            }

        }
        else if (sceneName.StartsWith(Loader.Scene.TestingLevel.ToString()))
        {
            shuffler.BeginShuffling(1, 3);
            for (int i = 0; i < au.environmentObjects.Length; ++i)
            {
                au.environmentObjects[i].GetComponent<AudioSourceController>().FadeTo(0, 5, 0.5f, false);
            }
        }
        else if (sceneName.StartsWith(Loader.Scene.Loading.ToString()))
        {
            shuffler.StopShuffling();
            SwitchMusic(0);
        }
        else
        {
            shuffler.StopShuffling();
            SwitchMusic(-1);
            Debug.LogError(this.name + ": Unknown scene triggered (name: " + sceneName + "), music cue not fired.");
        }
        
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
        // CONNECT TO POSITION
        au.TriggerFleshHit(transform.position);
    }

    private void SendEnemyHit(object sender, System.EventArgs e)
    {
        // CONNECT TO POSITION
        au.TriggerArmourHit(transform.position);
    }

    private void SendBlockStart(object sender, System.EventArgs e)
    {
        // CONNECT TO POSITION
        au.TriggerBlockSound(transform.position);
    }


    private void SendPlayerHeavyAttack(object sender, System.EventArgs e)
    {
        AudioManager.Instance.TriggerPlayerAttackAudio(AttackType.heavy);
        au.TriggerSwordSwish(transform.position, AttackType.heavy);
    }

    private void SendPlayerLittleAttack(object sender, System.EventArgs e)
    {
        AudioManager.Instance.TriggerPlayerAttackAudio(AttackType.light);
        au.TriggerSwordSwish(transform.position, AttackType.light);

    }
    private void SendEnemyAttack(object sender, System.EventArgs e)
    {
        au.TriggerSwordSwish(transform.position, AttackType.light);// todo: update with enemy pos
        au.TriggerEnemyAttackSound(transform.position);
    }

    private void SwitchMusic(int trackIndex)
    {
        switcher.SwitchTrack(trackIndex);
    }


    private void SendPlayerDeath(object sender, System.EventArgs e)
    {
        au.TriggerPlayerDeathSound();
        au.TriggerFleshHit(transform.position);
    }

    private void SendPlayerHit(object sender, System.EventArgs e)
    {
        au.TriggerPlayerTakesDamageSound(10);
        au.TriggerArmourHit(transform.position);
    }

    private void SendJump(object sender, System.EventArgs e)
    {
        au.TriggerPlayerJump();
    }









    private void OnDestroy()
    {
        ev.OnJumpInitiated -= SendJump;
        ev.OnPlayerGetsHit -= SendPlayerHit;
        ev.OnPlayerDeath -= SendPlayerDeath;
        ev.OnPlayerLittleAttack -= SendPlayerLittleAttack;
        ev.OnPlayerHeavyAttack -= SendPlayerHeavyAttack;
        ev.OnEnemyAttack -= SendEnemyAttack;
        ev.OnBlockInitiated -= SendBlockStart;
        ev.OnEnemyGetsHit -= SendEnemyHit;
        ev.OnEnemyDie -= SendEnemyDie;
        ev.OnHealthLow -= SendHealthLow;
        ev.OnHealthNotLow -= SendHealthRecovered;
        ev.OnStaminaLow -= SendStaminaLow;
        ev.OnStaminaNotLow -= SendStaminaRecovered;

        SceneManager.activeSceneChanged -= CueMusicByScene;
       
    }
}
