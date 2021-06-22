using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Applies simple PerlinNoise to an attached AudioSourceController's Output Gain.
/// </summary>

[RequireComponent(typeof(AudioSourceController))]
public class AudioJitterGain : MonoBehaviour
{
    public float volumeJitter;
    public float panJitter;
    [Range(0.01f, 2)] public float jitterSpeed = 0.2f;

    private float gainOffset;

    private AudioSourceController asc;
    private AudioSource source;
    private float startGain;

    void Start()
    {
        asc = GetComponent<AudioSourceController>();
        source = GetComponent<AudioSource>();
        startGain = asc.outputGain;
    }

    void Update()
    {
        if (volumeJitter > 0.0f)
        {
            gainOffset = (volumeJitter * Mathf.PerlinNoise(Time.time * jitterSpeed, 0.0f)) - volumeJitter / 2;
            asc.SetOutputGain(gainOffset + startGain);

        }

        if (panJitter > 0.0f)
        {
            source.panStereo = (panJitter * Mathf.PerlinNoise(Time.time * jitterSpeed, 0.0f)) - panJitter / 2;
        }
        
    }



}
