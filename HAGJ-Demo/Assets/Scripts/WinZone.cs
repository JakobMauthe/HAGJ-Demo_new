// This script is responsible for detecting collision with the player and letting the 
// Game Manager know

using UnityEngine;

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

		//Tell the Game Manager that the player won
		GameManager.PlayerWon();
	}
}