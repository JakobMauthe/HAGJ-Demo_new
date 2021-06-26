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
    
    private static AudioEventsHandler _instance;
    public static AudioEventsHandler Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        au = GetComponent<AudioManager>();
        currentScene = SceneManager.GetActiveScene();
    }


    AudioManager au;
    MusicSwitch switcher;
    MusicShuffler shuffler;
    Scene currentScene;



    private void Start()
    {        
        switcher = au.musicSwitch;
        shuffler = au.shuffler;        

        EventManager.Instance.OnJumpInitiated += SendJump;
        EventManager.Instance.OnPlayerGetsHit += SendPlayerHit;
        EventManager.Instance.OnPlayerDeath += SendPlayerDeath;
        EventManager.Instance.OnPlayerLittleAttack += SendPlayerLittleAttack;
        EventManager.Instance.OnPlayerHeavyAttack += SendPlayerHeavyAttack;
        EventManager.Instance.OnEnemyAttack += SendEnemyAttack;
        EventManager.Instance.OnBlockInitiated += SendBlockStart;
        EventManager.Instance.OnEnemyGetsHit += SendEnemyHit;
        EventManager.Instance.OnEnemyDie += SendEnemyDie;
        EventManager.Instance.OnHealthLow += SendHealthLow;
        EventManager.Instance.OnHealthNotLow += SendHealthRecovered;
        EventManager.Instance.OnStaminaLow += SendStaminaLow;
        EventManager.Instance.OnStaminaNotLow += SendStaminaRecovered;
        EventManager.Instance.OnEnemyStartChase += SendChaseStart;

        SceneManager.activeSceneChanged += CueMusicByScene;

        CueMusicByScene(currentScene, currentScene);
    }

    private void SendChaseStart(Vector2 position)
    {
        au.TriggerEnemyChargeSound(position);
    }

    private void CueMusicByScene(Scene oldScene, Scene newScene)
    {
        //Debug.Log("MUSIC: Scene change detected. New scene: " + newScene.name + ". Switching to new audio container.");
        currentScene = newScene;
        string sceneName = newScene.name;

        au.StopHeartbeat(); // if we have more levels, remove this

        if (sceneName.StartsWith(Loader.Scene.MainMenu.ToString()))
        {
            
            SwitchMusic(0);
            shuffler.StopShuffling();

            for (int i = 0; i < au.environmentObjects.Length; ++i)
            {                
                au.environmentObjects[i].GetComponent<AudioSourceController>().FadeTo(AudioUtility.minimum, 3, 0.5f, true);
            }
         

        }
        else if (sceneName.StartsWith(Loader.Scene.Loading.ToString()))
        {
            shuffler.StopShuffling();
            SwitchMusic(0);
        }
        else if (sceneName.StartsWith(Loader.Scene.Intro.ToString()))
        {
            shuffler.StopShuffling();
            SwitchMusic(0);
            for (int i = 0; i < au.environmentObjects.Length; ++i)
            {
                au.environmentObjects[i].GetComponent<AudioSourceController>().FadeTo(-6, 5, 0.5f, false);
            }
            for (int i = 0; i < au.introFireCrackle.Length; ++i)
            {
                au.introFireCrackle[i].PlayLoop();
                au.introFireCrackle[i].FadeTo(0, 1, 0.5f, false);
            }
        }
        else if (sceneName.StartsWith(Loader.Scene.Level1.ToString()) || sceneName.StartsWith(Loader.Scene.TestingLevel.ToString()))
        {
            shuffler.BeginShuffling(1, 3);
            for (int i = 0; i < au.environmentObjects.Length; ++i)
            {
                au.environmentObjects[i].GetComponent<AudioSourceController>().FadeTo(0, 5, 0.5f, false);
            }
            for (int i = 0; i < au.introFireCrackle.Length; ++i)
            {
                au.introFireCrackle[i].FadeTo(AudioUtility.minimum, 5, 0.5f, true);
            }
               
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
        au.StopLowHealthSound();
    }

    private void SendHealthLow(object sender, System.EventArgs e)
    {
        au.TriggerLowHealthSound();
    }

    private void SendEnemyDie(Vector2 position)
    {
        au.TriggerFleshHit(position);
    }

    private void SendEnemyHit(Vector2 position)
    {
        au.TriggerArmourHit(position);
    }

    private void SendBlockStart(Vector2 position)
    {
        au.TriggerBlockSound(position);
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
    private void SendEnemyAttack(Vector2 position)
    {
        au.TriggerSwordSwish(transform.position, AttackType.light);// todo: update with enemy pos
        au.TriggerEnemyAttackSound(position);
    }

    private void SwitchMusic(int trackIndex)
    {
        switcher.SwitchTrack(trackIndex);
    }


    private void SendPlayerDeath(object sender, System.EventArgs e)
    {
        au.TriggerPlayerDeathSound();
        au.TriggerFleshHit(transform.position);

        au.StopLowHealthSound();
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
        EventManager.Instance.OnJumpInitiated -= SendJump;
        EventManager.Instance.OnPlayerGetsHit -= SendPlayerHit;
        EventManager.Instance.OnPlayerDeath -= SendPlayerDeath;
        EventManager.Instance.OnPlayerLittleAttack -= SendPlayerLittleAttack;
        EventManager.Instance.OnPlayerHeavyAttack -= SendPlayerHeavyAttack;
        EventManager.Instance.OnEnemyAttack -= SendEnemyAttack;
        EventManager.Instance.OnBlockInitiated -= SendBlockStart;
        EventManager.Instance.OnEnemyGetsHit -= SendEnemyHit;
        EventManager.Instance.OnEnemyDie -= SendEnemyDie;
        EventManager.Instance.OnHealthLow -= SendHealthLow;
        EventManager.Instance.OnHealthNotLow -= SendHealthRecovered;
        EventManager.Instance.OnStaminaLow -= SendStaminaLow;
        EventManager.Instance.OnStaminaNotLow -= SendStaminaRecovered;
        EventManager.Instance.OnEnemyStartChase -= SendChaseStart;

        SceneManager.activeSceneChanged -= CueMusicByScene;
       
    }
}
