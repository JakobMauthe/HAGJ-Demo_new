using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour {
	static UIManager current;
	
	public GameObject gameOverUI;    // Game over UI Element
	public GameObject winUI;		 // Win UI Element


	void Awake() {
		
		//If an UIManager exists and it is not this...
		if (current != null && current != this) {
			//...destroy this and exit. There can be only one UIManager
			Destroy(gameObject);
			return;
		}
		//This is the current UIManager and it should persist between scene loads
		current = this;
		HideAllTexts();

		DontDestroyOnLoad(gameObject);
	}

	public static void HideAllTexts() {
		HideGameOverText();
		HideWinText();
	}

	public static void ShowGameOverText() {
		if (current == null)
			return;

		current.gameOverUI.SetActive(true);
	}

	public static void HideGameOverText() {
		if (current == null)
			return;

		current.gameOverUI.SetActive(false);
	}
	public static void ShowWinText() {
		if (current == null)
			return;

		current.winUI.SetActive(true);
	}

	public static void HideWinText() {
		if (current == null)
			return;

		current.winUI.SetActive(false);
	}
}