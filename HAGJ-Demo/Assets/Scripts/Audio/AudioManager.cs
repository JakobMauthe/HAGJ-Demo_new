using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    public AudioListener listener;
    public PlayerController player;

    [Header("Music")]
    public MusicSwitch musicSwitch;
    public MusicShuffler shuffler;
    [SerializeField, Range(-81, 24)] float musicVolume;
    GameObject[] musicObjects;

    [Header("Environment")]
    [SerializeField, Range(-81, 24)] float environmentVolume;
    public GameObject[] environmentObjects;

    [Header("Audio Objects")]
    [SerializeField, Range(-81, 24)] float playerVolume;
    public AudioSourceController playerJumpGrunt, playerDamageGrunt, playerDieGroan, playerAttackGrunt, playerStaminaBreath, playerHealthHeartbeat;
    public GameObject swordSwish, swordClash, swordOnArmour, swordOnFlesh, enemyAttackGrunt, enemyChargeGrunt;
    private GameObject oneshotContainer;

    // private:
    private bool hasInitialised = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }



    void Start()
    {

        if (!player) player = PlayerController.Instance;

        // If the scene contains a player character, make sure it's the only listener in the scene, otherwise defaults to main camera.
        if (player)
        {
            if (Camera.main.GetComponent<AudioListener>())
            {
                Camera.main.GetComponent<AudioListener>().enabled = false;
            }
            listener = player.GetComponent<AudioListener>();
        } 
        else
        {
            listener = Camera.main.GetComponent<AudioListener>();
        }

       
       

        UpdateVolumes();

        oneshotContainer = new GameObject();
        oneshotContainer.name = "OneShot Sounds";
        oneshotContainer.transform.parent = gameObject.transform;
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
    private void Update()
    {
        if (player)
        {
            transform.position = listener.transform.position;
        }
        else transform.position = Camera.main.transform.position;
    }

    #region Trigger Audio
    /* MOVE */

    public void TriggerPlayerWalk()
    {

    }
    public void TriggerPlayerJump()
    {
        playerJumpGrunt.PlayRandom(-6, 2, 0.9f, 1.0f);
    }

    /* ATTACK */
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

    public void TriggerSwordSwish(Vector2 position, AttackType attackType)
    {
        if (attackType == AttackType.light)
        {
            CreateOneShotSoundObject(swordSwish, position, -6, 0, 0.9f, 1.2f);
            //swordSwish.PlayRandom(-6, 0, 0.9f, 1.2f);
        }
        else if (attackType == AttackType.heavy)
        {
            CreateOneShotSoundObject(swordSwish, position, -3, 0, 0.75f, 0.95f);
            //swordSwish.PlayRandom();
        }
    }

    private void CreateOneShotSoundObject(GameObject obj, Vector2 position, float volMin, float volMax, float pitchMin, float pitchMax)
    {
        var newObj = Instantiate(obj, position, Quaternion.identity);
        newObj.transform.parent = oneshotContainer.transform;

        var asc = newObj.GetComponent<AudioSourceController>();
        asc.PlayRandom(volMin, volMax, pitchMin, pitchMax);

        StartCoroutine(WaitThenDestroy(newObj, asc.audioSource.clip, asc.pitch));

    }
    IEnumerator WaitThenDestroy(GameObject obj, AudioClip clip, float pitch)
    {
        
        float duration = clip.length / pitch;
        yield return new WaitForSeconds(duration);        
        Destroy(obj);
    }

    public void TriggerEnemyAttackSound(Vector2 position)
    {
        CreateOneShotSoundObject(enemyAttackGrunt, position, -3, 3, 0.9f, 1f);
        //enemyAttackGrunt.PlayRandom(-3, 3, 0.9f, 1f);
    }
    public void TriggerEnemyChargeSound(Vector2 position)
    {
        CreateOneShotSoundObject(enemyChargeGrunt, position, -6, 0, 0.9f, 1f);
        //enemyChargeGrunt.PlayRandom(-6, 0, 0.9f, 1f);
    }


    /* ENEMY HIT */
    // regular hit => armour; killing blow => flesh.
    public void TriggerArmourHit(Vector2 position)
    {
        CreateOneShotSoundObject(swordOnArmour, position, -3, 3, 0.8f, 1.2f);
        //swordOnArmour.PlayRandom(-3, 3, 0.8f, 1.2f);
    }
    public void TriggerFleshHit(Vector2 position)
    {
        CreateOneShotSoundObject(swordOnFlesh, position, -3, 0, 0.9f, 1.1f);
        //swordOnFlesh.PlayRandom(-3, 0, 0.9f, 1.1f);
    }

    /* BLOCK */
    public void TriggerBlockSound(Vector2 position)
    {
        CreateOneShotSoundObject(swordClash, position, -9, 0, 0.9f, 1.1f);
        //swordClash.PlayRandom(-9, 0, 0.9f, 1.1f);
    }



    /* STAMINA */
    
    bool isLowStaminaAudioPlaying = false;
    public void TriggerLowStaminaSound()
    {
        if (!isLowStaminaAudioPlaying)
        {
            playerStaminaBreath.FadeTo(0, 1, 0.5f, false);
            playerStaminaBreath.PlayLoop();
            isLowStaminaAudioPlaying = true;
        }
    }
    public void StopLowStaminaSound()
    {
        if (isLowStaminaAudioPlaying)
        {
            playerStaminaBreath.FadeTo(AudioUtility.minimum, 1, 0.5f, true);
            isLowStaminaAudioPlaying = false;
        }
    }

    /* TAKE DAMAGE */
    public void TriggerPlayerTakesDamageSound(float damage)
    {
        damage = AudioUtility.ScaleValue(damage, 0, 20, -6, 6);
        playerDamageGrunt.PlayRandom(-3 + damage, 3 + damage, 0.95f, 1.0f);
    }

    bool isLowHealthAudioPlaying = false;
    public void TriggerLowHealthSound()
    {
        if (!isLowHealthAudioPlaying)
        {
            playerHealthHeartbeat.FadeTo(0, 0.1f, 0.5f, false);
            playerHealthHeartbeat.PlayLoop();
        }
    }
    public void StopLowHealthSound()
    {
        if (isLowHealthAudioPlaying)
        {
            playerHealthHeartbeat.FadeTo(AudioUtility.minimum, 1, 0.5f, true);
            isLowHealthAudioPlaying = false;
        }
    }

   
        /* DIE */

    public void TriggerPlayerDeathSound() 
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
