using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    public GameObject listener;
    public PlayerController player;

    [Header("Game Variables")]
    [SerializeField, Range(0, 100)] int lowHealthThreshold = 25;
    [SerializeField, Range(0, 100)] int lowStaminaThreshold = 33;

    [Header("Music")]
    public MusicSwitch musicSwitch;
    [SerializeField, Range(-81, 24)] float musicVolume;
    GameObject[] musicObjects;
    public KeyCode switchKey = KeyCode.X;
    public int currentTrackIndex = -1;

    [Header("Environment")]
    [SerializeField, Range(-81, 24)] float environmentVolume;
    GameObject[] environmentObjects;

    [Header("Player")]
    [SerializeField, Range(-81, 24)] float playerVolume;
    public AudioSourceController playerJumpGrunt, playerDamageGrunt, playerDieGroan, playerAttackGrunt, playerStaminaBreath, playerHealthHeartbeat;
    

    public KeyCode jumpSoundKey = KeyCode.Space; // if there's time, link to events but it's probably fine without.
    public KeyCode takeDamageSoundKey = KeyCode.O; // TODO: link to events
    public KeyCode playerDieSoundKey = KeyCode.Q;
    public KeyCode playerAttackSoundKey = KeyCode.A;






    private bool hasInitialised = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        if (!listener) listener = Camera.main.gameObject;
        if (!player) player = PlayerController.Instance;

        UpdateVolumes();
    }

    private void OnValidate()
    {
        /*   FOR TESTING ONLY    */
        if (Application.isPlaying)
        {
            if (!hasInitialised) return;
            UpdateVolumes();
        }
       
    }

    void Update()
    {


        /*   FOR TESTING ONLY    */
        
        if (Input.GetKeyDown(takeDamageSoundKey))
        {
            TriggerPlayerTakesDamageAudio(10);
        }
        if (Input.GetKeyDown(playerDieSoundKey))
        {
            TriggerPlayerDeathAudio();
        }
        if (Input.GetKeyDown(switchKey))
        {
            currentTrackIndex = (currentTrackIndex + 1) % (musicSwitch.tracks.Length - 1);
            musicSwitch.SwitchTrack(currentTrackIndex);

        }
    }
    #region Player Audio


    /* MOVE */

    public void TriggerPlayerWalk()
    {

    }
    public void TriggerPlayerJump()
    {
        playerJumpGrunt.PlayRandom(-6, 2, 0.9f, 1.0f);
    }
    



    /* ATTACK */
    // Comes directly from PlayerAudioEventHandler on the model.
    public void TriggerPlayerAttackAudio(AttackType attackType)
    {
        if (attackType == AttackType.light)
        {
            playerAttackGrunt.PlayRandom(-9, -3, 0.95f, 1.15f);
        }
        else if (attackType == AttackType.heavy)
        {
            playerAttackGrunt.PlayRandom(-3, 3, 0.85f, 1.0f);
        }
    }
    /* STAMINA */
    // trigger stamina called from the audioeventshandler - coroutine runs audio and checks for stamina to regenerate //
    public void TriggerStamina()
    {
        StartCoroutine(CheckStamina());
    }
    bool isStaminaAudioPlaying = false;
    IEnumerator CheckStamina()
    {
        while (true)
        {
            Debug.Log("AUDIO: running CheckStamina coroutine..");
            if (player.Stamina < lowStaminaThreshold)
            {
                if (!isStaminaAudioPlaying)
                {
                    isStaminaAudioPlaying = true;
                    playerStaminaBreath.FadeTo(0, 1, 0.5f, false);
                    playerStaminaBreath.PlayLoop();
                    
                }
            }
            else
            { 
                if (isStaminaAudioPlaying) playerStaminaBreath.FadeTo(AudioUtility.MinSoundLevel(), 2, 0.5f, true);
                isStaminaAudioPlaying = false;
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
            yield return null;
        }
    }

    /* TAKE DAMAGE */
    public void TriggerPlayerTakesDamageAudio(float damage)
    {
        damage = AudioUtility.ScaleValue(damage, 0, 20, -6, 6);
        playerDamageGrunt.PlayRandom(-3 + damage, 3 + damage, 0.95f, 1.0f);
        StartCoroutine(CheckHealth());
    }

    bool isHealthAudioPlaying = false;
    IEnumerator CheckHealth()
    {
        while (true)
        {
            Debug.Log("AUDIO: running CheckHealth coroutine..");
            if (player.Health < lowHealthThreshold)
            {
                if (!isHealthAudioPlaying)
                {
                    isHealthAudioPlaying = true;
                    playerHealthHeartbeat.FadeTo(0, 0.1f, 0.5f, false);
                    playerHealthHeartbeat.PlayLoop();

                }
            }
            else
            {
                playerHealthHeartbeat.FadeTo(AudioUtility.MinSoundLevel(), 1f, 0.5f, true);
                isHealthAudioPlaying = false;
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
            yield return null;
        }
    }

    /* DIE */

    public void TriggerPlayerDeathAudio()
    {
        playerDieGroan.PlayRandom(-3, 0, 0.95f, 1.05f);
    }


    #endregion

    void UpdateVolumes()
    {
        hasInitialised = true;

        musicObjects = GameObject.FindGameObjectsWithTag("snd_music");
        for (int i = 0; i < musicObjects.Length; ++i)
        {            
            musicObjects[i].GetComponent<AudioSourceController>().SetInputGain(musicVolume);
        }


        environmentObjects = GameObject.FindGameObjectsWithTag("snd_environment");
        for (int i = 0; i < environmentObjects.Length; ++i)
        {
            environmentObjects[i].GetComponent<AudioSourceController>().SetInputGain(environmentVolume);
        }

        // Player //
        playerAttackGrunt.SetInputGain(playerVolume);
        playerDamageGrunt.SetInputGain(playerVolume);
        playerDieGroan.SetInputGain(playerVolume);
        playerHealthHeartbeat.SetInputGain(playerVolume);
        playerStaminaBreath.SetInputGain(playerVolume);
        playerJumpGrunt.SetInputGain(playerVolume);

    }





}
