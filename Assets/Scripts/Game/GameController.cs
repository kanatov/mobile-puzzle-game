﻿using UnityEngine;
using UnityEngine.SceneManagement;
using GenericData;

public static class GameController {
	public static PlayerData playerData;

	public static void Init () {
		ClearSavedData ();
		Debug.Log ("___Init: Loading Player data");
		playerData = (PlayerData)SaveLoad.Load(SaveLoad.namePlayerProgress);

		if (playerData == null) {
			Debug.Log ("___Init: First start");
			playerData = new PlayerData ();
			SaveLoad.Save (playerData, SaveLoad.namePlayerProgress);
			LoadScene (1);
			return;
		}

		Debug.Log ("___Init: Looking for Game session");

		foreach (var _level in playerData.levelState) {
			for (int i = 0; i < playerData.levelState.Length; i++) {
				if (playerData.levelState[i] == LevelState.GameSession) {
					Debug.Log ("___Init: Load Game Session");
					LoadScene (i);
					return;
				}
			}
		}

		Debug.Log ("___Init: Loading Menu screen");
	}

	public static void LoadScene(int _scene) {
		Debug.Log ("___Loading scene: " + _scene);
		playerData.levelState [_scene] = LevelState.GameSession;

		if (_scene == 0) {
			playerData.levelState [SceneManager.GetActiveScene().buildIndex] = LevelState.Locked;
			ClearSavedData ();
		}
		SaveLoad.Save (playerData, SaveLoad.namePlayerProgress);


		SceneManager.LoadScene (_scene);
	}

	static void ClearSavedData() {
		SaveLoad.Delete (SaveLoad.namePlayerProgress);
		SaveLoad.Delete (SaveLoad.nameGameSessionWaypoints);
		SaveLoad.Delete (SaveLoad.nameGameSessionTriggers);
		SaveLoad.Delete (SaveLoad.nameGameSessionUnits);
	}

	public static void SaveGameSession () {
		SaveLoad.Save (MapController.waypoints, SaveLoad.nameGameSessionWaypoints);
		SaveLoad.Save (MapController.units, SaveLoad.nameGameSessionUnits);
		SaveLoad.Save (MapController.triggers, SaveLoad.nameGameSessionTriggers);
	}

	public static void LoadGameSession () {
		MapController.waypoints = (Waypoint[])SaveLoad.Load (SaveLoad.nameGameSessionWaypoints);
		MapController.units = (Unit[])SaveLoad.Load (SaveLoad.nameGameSessionUnits);
		MapController.triggers = (Trigger[])SaveLoad.Load (SaveLoad.nameGameSessionTriggers);
	}
}
