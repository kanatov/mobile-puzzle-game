using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public static class GameController {
	public static GameObject[] ui;
	public static int turnLockQueue = 0;
	static InputField input;
	// Load previous data
	// Set saved data or default
	// default is map position, unit settings
	public static void Init () {

		input = ui[0].GetComponent<InputField>();
		input.onEndEdit.AddListener(delegate{ChangeOverview(input);});
		LoadPlayerData();

		// Create world
		Map.Init ();
	}
	static void LoadPlayerData() {
		Player.x = PlayerDefault.x;
		Player.y = PlayerDefault.y;
		Player.overview = PlayerDefault.overview;
	}

	public static bool turnLock {
		get {
			if (turnLockQueue == 0){
				return false;
			} else {
				Debug.Log ("no");
				return true;
			}

		}
	}

	static void ChangeOverview (InputField _input) {
		Player.overview = int.Parse(_input.text);
		Map.Init ();
	}

	// Human action
	// Walk or Attack of the player
	// Wall or Attack of the player's army
	// Wall or Attack of the enemy's army

}
