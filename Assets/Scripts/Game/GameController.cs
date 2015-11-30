using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class GameController {
	public static GameObject[] UI;
	public static bool turnLock = false;

	// Load previous data
	// Set saved data or default
	// default is map position, unit settings
	public static void Init () {

		// Load player data
		Player.x = PlayerDefault.x;
		Player.y = PlayerDefault.y;
		Player.overview = PlayerDefault.overview;

		// Create world
		Terrain.Init();
		Map.Init ();
	}

	// Human action
	// Walk or Attack of the player
	// Wall or Attack of the player's army
	// Wall or Attack of the enemy's army

}
