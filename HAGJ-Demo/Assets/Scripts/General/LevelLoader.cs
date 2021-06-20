using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    public Animator transitionCrossfade;

    public float transtionTime = 1f; // Time the animation takes to complete

    //loads next scene in build
    public void LoadNextLevel() {
        StartCoroutine(LoadLevelwithCrossFade(SceneManager.GetActiveScene().buildIndex + 1));
    }

    //loads specific level with Crossfade
    public void LoadSpecificLevel(int specificLevel) {
        StartCoroutine(LoadLevelwithCrossFade(specificLevel));
    }    

    //loads scene with levelindex and CrossFade
    IEnumerator LoadLevelwithCrossFade (int levelIndex) {

        transitionCrossfade.SetTrigger("Start");

        yield return new WaitForSeconds(transtionTime);

        SceneManager.LoadScene(levelIndex);

    }   
}