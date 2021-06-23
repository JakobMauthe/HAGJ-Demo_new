using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    public GameObject listener;

    [Header("Game Variables")]
    public int playerHealth;
    public int playerStamina;

    [Header("Music")]
    public MusicSwitch musicSwitch;
    [Range(-81, 24)] public float musicVolume;
    GameObject[] musicObjects;
    public KeyCode switchKey = KeyCode.X;
    public int currentTrackIndex = -1;

    [Header("Environment")]
    [Range(-81, 24)] public float environmentVolume;
    GameObject[] environmentObjects;

    [Header("Player")]
    public GameObject jumpGruntContainer;
    public GameObject playerDamageGruntContainer;
    public GameObject playerDieGroanContainer;
    public GameObject playerAttackGruntContainer;

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
        listener = Camera.main.gameObject;
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

        if (Input.GetKeyDown(jumpSoundKey))
        {
            jumpGruntContainer.GetComponent<AudioSourceController>().PlayRandom(-6, 2, 0.9f, 1.0f);
        }
        if (Input.GetKeyDown(takeDamageSoundKey))
        {
            playerDamageGruntContainer.GetComponent<AudioSourceController>().PlayRandom(-3, 3, 0.9f, 1.1f);
        }
        if (Input.GetKeyDown(playerDieSoundKey))
        {
            playerDieGroanContainer.GetComponent<AudioSourceController>().PlayRandom(-2, 2, 0.95f, 1.0f);
        }



        if (Input.GetKeyDown(switchKey))
        {
            currentTrackIndex = (currentTrackIndex + 1) % (musicSwitch.tracks.Length - 1);
            musicSwitch.SwitchTrack(currentTrackIndex);

        }
    }
    #region Player Audio
    public void TriggerPlayerAttackAudio(string attackType)
    {
        if (attackType == "light")
        {
            playerDamageGruntContainer.GetComponent<AudioSourceController>().PlayRandom(-9, -3, 0.95f, 1.15f);
        }
        else if (attackType == "heavy")
        {
            playerDamageGruntContainer.GetComponent<AudioSourceController>().PlayRandom(-3, 3, 0.85f, 1.0f);
        }

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
    }





}
