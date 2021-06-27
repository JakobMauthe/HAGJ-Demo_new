using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterEvent : MonoBehaviour {


    public static bool gameIsPaused;

    public GameObject letterUI, findLetterUI;

    private void Awake() {
        gameIsPaused = false;
    }

    private void Update() {
        if (PlayerController.Instance.transform.position.x > -171f  && !gameIsPaused){
            Pause();
        }
    }

    public void Continue() {
        letterUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        PlayerController.Instance.dirtyIsPaused = false;
        Destroy(gameObject);
    }

    public void Open() {
        findLetterUI.SetActive(false);
        letterUI.SetActive(true);
    }

    void Pause() {
        gameIsPaused = true;
        PlayerController.Instance.dirtyIsPaused = true;
        findLetterUI.SetActive(true);
        Time.timeScale = 0f;        
    }

}