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
    public KeyCode jumpKey = KeyCode.Space; // if there's time, link to events but it's probably fine without.
    public GameObject playerDamageGruntContainer;
    public KeyCode takeDamageKey = KeyCode.O;



    private bool hasInitialised = false;


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

        if (Input.GetKeyDown(jumpKey))
        {
            jumpGruntContainer.GetComponent<AudioSourceController>().PlayRandom(-6, 2, 0.9f, 1.0f);
        }
        if (Input.GetKeyDown(takeDamageKey))
        {
            playerDamageGruntContainer.GetComponent<AudioSourceController>().PlayRandom(-3, 3, 0.9f, 1.1f);
        }



        if (Input.GetKeyDown(switchKey))
        {
            currentTrackIndex = (currentTrackIndex + 1) % (musicSwitch.tracks.Length - 1);
            musicSwitch.extVol = musicVolume;
            musicSwitch.SwitchTrack(currentTrackIndex);

        }
    }

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
