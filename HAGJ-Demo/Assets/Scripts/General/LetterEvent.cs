using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterEvent : MonoBehaviour {

    public GameObject letterUI, findLetterUI;
    private GameObject player;

    public void Awake() {
        player = GameObject.FindWithTag("Player"); 
    }

    private void Update() {
        if (player.transform.position.x > -171f && !GameManager.IsGamePaused()){
            Pause();
        }
    }

    public void Continue() {
        GameManager.UnPauseGame();
        letterUI.SetActive(false);        
        Destroy(gameObject);
    }

    public void Open() {        
        findLetterUI.SetActive(false);
        letterUI.SetActive(true);
    }

    void Pause() {
        GameManager.PauseGame();
        findLetterUI.SetActive(true);              
    }
}