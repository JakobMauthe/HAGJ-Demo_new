// This script is responsible for detecting collision with the player and letting the 
// Game Manager know

using UnityEngine;
using UnityEngine.SceneManagement;

public class WinZone : MonoBehaviour {
	int playerLayer;    //The layer the player game object is on


	private void Awake() {
		//Get the integer representation of the "Player" layer
		playerLayer = LayerMask.NameToLayer("Player");
	}

	void OnTriggerEnter2D(Collider2D collision) {
		//If the collision wasn't with the player, exit
		if (collision.gameObject.layer != playerLayer)
			return;

		int currentScene = SceneManager.GetActiveScene().buildIndex;

		//Let GameManager know that player finished Level1 if Active Scene is Level1
		//Let GameManager know that player won  if Active Scene is Level2
		if (currentScene == (int) Loader.Scene.Level1) {
			GameManager.Level1Finished();
        }
        else if (currentScene == (int) Loader.Scene.Level2) {
			GameManager.PlayerWon();
		}
	}
}