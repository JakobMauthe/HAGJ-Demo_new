using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	//This class holds a static reference to itself to ensure that there will only be
	//one in existence. This is often referred to as a "singleton" design pattern. Other
	//scripts access this one through its public static methods
	private static GameManager _instance;

	public float deathSequenceDuration = 1.5f;  //How long player death takes before restarting
		
	private bool isGamePaused;                            //Is the game currently over?

	public static GameManager Instance { get { return _instance; } }


	void Awake() {

		//If a Game Manager exists and this isn't it...
		if (_instance != null && _instance != this) {
			//...destroy this and exit. There can only be one Game Manager
			Destroy(gameObject);
			return;			
		}

		_instance = this;
		DontDestroyOnLoad(gameObject);
	}
    

    public static bool IsGamePaused() {
		if (_instance == null)
			return false;

		return _instance.isGamePaused;
	}

	public static void PauseGame() {
		if (_instance == null)
			return;

		Time.timeScale = 0f;
		_instance.isGamePaused = true;
	}

	public static void UnPauseGame() {
		if (_instance == null)
			return;

		Time.timeScale = 1f;
		_instance.isGamePaused = false;
	}

	public static void PlayerDied() {
		//If there is no current Game Manager, exit
		if (_instance == null)
			return;

		//Invoke the RestartScene() method after a delay
		UIManager.ShowGameOverText();
		_instance.Invoke("RestartScene", _instance.deathSequenceDuration);
	}

	public static void PlayerWon() {
		//If there is no current Game Manager, exit
		if (_instance == null)
			return;

		UIManager.ShowWinText();
		_instance.Invoke("BackToMainMenu", _instance.deathSequenceDuration);

		//AudioManager.PlayWonAudio();
	}

	public static void Level1Finished() {
		//If there is no current Game Manager, exit
		if (_instance == null)
			return;		

		UIManager.ShowLevel1FinishText();
		_instance.Invoke("GoToCutScene_lvl1_to_lvl2", _instance.deathSequenceDuration);		
	}

	void RestartScene() {
		//Play the scene restart audio
		//AudioManager.PlaySceneRestartAudio();

		//Reload the current scene
		Loader.LoadCurrentScene();
		UIManager.HideAllTexts();
	}
	void BackToMainMenu() {
		UIManager.HideAllTexts();
		Loader.Load(Loader.Scene.MainMenu);
	}

	void GoToCutScene_lvl1_to_lvl2() {		
		UIManager.HideAllTexts();
		Loader.Load(Loader.Scene.Cutscene_lvl1_to_lvl2);
	}
}