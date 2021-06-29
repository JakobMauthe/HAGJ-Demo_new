using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    public static bool gameIsPaused;

    public GameObject pauseMenuUI;

    private void Awake() {
        gameIsPaused = false;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (gameIsPaused) {
                Resume();
            }
            else {
                Pause();
            }
        }
    }
    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void Pause() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void LoadMenu() {
        Time.timeScale = 1f;
        EventManager.Instance.NotifyOfOnHealthNotLow(this);
        Loader.Load(Loader.Scene.MainMenu);
    }

    public void Quit() {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}