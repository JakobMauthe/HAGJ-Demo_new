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
    public List<AudioClip> musicClips = new List<AudioClip>();

    void Start()
    {
        listener = Camera.main.gameObject;
    }

}
