using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    public GameObject pauseMenuUI;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GameManager.IsGamePaused()) {
                Resume();
            }
            else {
                Pause();
            }
        }
    }
    public void Resume() {
        pauseMenuUI.SetActive(false);
        GameManager.UnPauseGame();
    }

    void Pause() {
        pauseMenuUI.SetActive(true);
        GameManager.PauseGame();
    }

    public void LoadMenu() {
        GameManager.UnPauseGame();
        EventManager.Instance.NotifyOfOnHealthNotLow(this);
        Loader.Load(Loader.Scene.MainMenu);
    }

    public void Quit() {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}