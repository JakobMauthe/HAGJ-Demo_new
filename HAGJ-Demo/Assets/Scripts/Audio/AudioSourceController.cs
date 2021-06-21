// By blubberbaleen. Find more at https://github.com/xvelastin/unityaudioutility //
// v1.0 //
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class AudioSourceController : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Gain")] // Processing stage for audio.
    [Range(-81, 24)] public float outputGain = 0f;
    [Range(-81, 24)] public float inputGain;
    [Range(-81, 24)] public float startingVolume;

    [Header("Fader")]// Controls fades.
    [Min(0)] [SerializeField] float FadeInOnAwakeTime = 0f;    
    public float fadeVolume;
    public bool isFading;
    private IEnumerator fadeCoroutine;
    [HideInInspector] public float currentFadeTarget;
    [HideInInspector] public float currentFadeTime;


    [Header("Clip Player")] // Choosing and playing back clips with variations.
    [SerializeField] List<AudioClip> playlist = new List<AudioClip>();
    public bool playOnAwake = false;
    public bool loopClips = false;
    public bool newClipPerPlay = false;
    [Min(0)] public float intervalBetweenPlays = 0f;
    public float intervalRand = 0f;
    private IEnumerator loopCoroutine;

    [Range(-4f, 4f)] public float pitch = 1f;
    [Range(-1f, 1f)] public float pitchRand = 0f;
    public bool looperIsLooping;

    [Header("Audibility Check")]
    public bool enableAudibilityCheck = false;



    public void SetInputGain(float value)
    {
        inputGain = value;
        UpdateParams();

    }
    public void SetOutputGain(float value)
    {
        outputGain = value;
        UpdateParams();
    }

    #region Initialisation
    private void Awake()
    {
        // disables inspector fiddling
        inputGain = 0;

        if (!audioSource)
            audioSource = GetComponent<AudioSource>();

        // Takes over audiosource functions.
        if (audioSource.isPlaying)
            audioSource.Stop();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.volume = AudioUtility.ConvertDbtoA(startingVolume);
        fadeVolume = startingVolume;

    }


    private void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        // Chooses/plays clips as set.
        if (playlist.Count == 0)
        {
            if (audioSource.clip != null)
            {
                playlist.Add(audioSource.clip);
            }
            else
            {
                Debug.LogError(this + "on " + gameObject.name + ": You must attach at least one AudioClip to the AudioSource or the AudioSourceController");
            }
        }

        if (newClipPerPlay)
            audioSource.clip = AudioUtility.RandomClipFromList(playlist);
        else
            audioSource.clip = playlist[0];

        if (enableAudibilityCheck)
            StartCoroutine(CheckAudibility(1));
        else
            StartPlayback();

    }

    IEnumerator CheckAudibility(float checkInterval)
    {
        do
        {
            yield return new WaitForSeconds(checkInterval);
            if (CheckIfAudible())
            {
                StartPlayback();
                yield break;
            }
        }
        while (true);
    }

    private void StartPlayback()
    {

        if (playOnAwake)
        {
            audioSource.pitch = pitch + Random.Range(-pitchRand, pitchRand);
            audioSource.Play();
        }

        if (loopClips)
        {
            PlayLoopWithInterval();
        }


        if (FadeInOnAwakeTime > 0.0f)
        {
            FadeTo(0f, FadeInOnAwakeTime, 1.0f, false);
        }
            
        
        

    }



    #endregion

    public bool CheckIfAudible()
    {
        GameObject listener = AudioManager.Instance.listener;
        float distance = Vector3.Distance(transform.position, listener.transform.position);
        if (distance > audioSource.maxDistance) return false;
        else return true;

    }

    private void GetAudioSourceVolume()
    {
        if (!audioSource)
            audioSource = GetComponent<AudioSource>();

        fadeVolume = AudioUtility.ConvertAtoDb(audioSource.volume);
    }



    #region Player/Looper

    public void PlayNew(AudioClip clip)
    {
        StopLooping(0.01f);
        audioSource.clip = clip;
        audioSource.Play();
    }


    public void PlayLoopWithInterval()
    {
        loopClips = true;
        loopCoroutine = ClipLooper(intervalBetweenPlays);
        StartCoroutine(loopCoroutine);
    }
    public void StopLooping(float fadeOutTime)
    {
        if (loopCoroutine != null) StopCoroutine(loopCoroutine);
        StartCoroutine(StopSourceAfter(fadeOutTime));
        looperIsLooping = false;
    }

    private IEnumerator StopSourceAfter(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        audioSource.Stop();
        yield break;
    }

    private IEnumerator ClipLooper(float interval)
    {
        while (true)
        {
            if (!looperIsLooping)
            {
                AudioClip newClip;

                if (!newClipPerPlay)
                {
                    // ie if we're looping the same one
                    if (interval == 0.0f)
                    {
                        // if no interval (straight loop on one clip): use audiosource native looper, which is more precise than coroutine
                        audioSource.loop = true;
                        audioSource.pitch = pitch + Random.Range(-pitchRand, pitchRand);

                        if (!audioSource.isPlaying) audioSource.Play();

                        looperIsLooping = true;
                        yield break;

                    }
                    else
                    {
                        // if there is an interval
                        newClip = audioSource.clip;
                        audioSource.loop = false;
                    }
                }
                else
                {
                    // ie if we're choosing a new clip each loop iteration
                    newClip = AudioUtility.RandomClipFromList(playlist);
                }

                float newClipPitch = pitch + Random.Range(-pitchRand, pitchRand);
                audioSource.pitch = newClipPitch;

                audioSource.clip = newClip;
                audioSource.Play();
                looperIsLooping = true;

                float newInterval = Mathf.Clamp(interval + Random.Range(-intervalRand, intervalRand), 0, interval + intervalRand);
                newInterval += (audioSource.clip.length / newClipPitch);
                yield return new WaitForSeconds(newInterval);

                looperIsLooping = false;

                yield return null;
            }
            else yield break;
        }
    }

    #endregion

    #region Fader

    public void FadeTo(float targetVol, float fadetime, float curveShape, bool stopAfterFade)
    {
        curveShape = Mathf.Clamp(curveShape, 0.0f, 1.0f);

        currentFadeTarget = targetVol;
        currentFadeTime = fadetime;

        // Uses an AnimationCurve
        // curveShape 0.0 = linear; curveShape 0.5 = s-curve; curveshape 1.0 (exponential).
        Keyframe[] keys = new Keyframe[2];
        keys[0] = new Keyframe(0, 0, 0, 1f - curveShape, 0, 1f - curveShape);
        keys[1] = new Keyframe(1, 1, 1f - curveShape, 0f, curveShape, 0);
        AnimationCurve animcur = new AnimationCurve(keys);

        if (isFading)
        {
            StopCoroutine(fadeCoroutine);
            isFading = false;
        }
        fadeCoroutine = StartFadeInDb(fadetime, targetVol, animcur, stopAfterFade);
        StartCoroutine(fadeCoroutine);
        isFading = true;
    }


    private IEnumerator StartFadeInDb(float fadetime, float targetVol, AnimationCurve animcur, bool stopAfterFade)
    {
        GetAudioSourceVolume();
        float currentTime = 0f;
        float startVol = fadeVolume;

        // Debug.Log(this + "on " + gameObject.name + " : Fading to " + targetVol + " from " + startVol + " over " + fadetime);

        while (currentTime < fadetime)
        {
            currentTime += Time.deltaTime;
            fadeVolume = Mathf.Lerp(startVol, targetVol, animcur.Evaluate(currentTime / fadetime));

            UpdateParams();
            yield return null;
        }

        isFading = false;

        if (stopAfterFade)
        {
            //yield return new WaitForSeconds(fadetime);
            audioSource.Stop();
        }

        yield break;
    }

    private float GetGain()
    {
        return inputGain + fadeVolume + outputGain;
    }

    private void UpdateParams()
    {
        float currentVol = GetGain();
        audioSource.volume = AudioUtility.ConvertDbtoA(currentVol);
    }


    #endregion
    private void OnDisable()
    {
        StopAllCoroutines();
    }

}
