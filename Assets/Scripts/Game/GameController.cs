using UnityEngine;
using UnityEngine.SceneManagement;
using GenericData;

public static class GameController {
	public static PlayerData playerData;

	public static void Init () {

		SaveLoad.Delete (SaveLoad.namePlayerProgress);
		SaveLoad.Delete (SaveLoad.nameGameSessionWaypoints);
		SaveLoad.Delete (SaveLoad.nameGameSessionTriggers);
		SaveLoad.Delete (SaveLoad.nameGameSessionUnits);

		Debug.Log ("Init: Loading Player data");
		playerData = (PlayerData)SaveLoad.Load(SaveLoad.namePlayerProgress);

		if (playerData == null) {
			Debug.Log ("Init: First start");
			playerData = new PlayerData ();
			SaveLoad.Save (playerData, SaveLoad.namePlayerProgress);
			LoadScene (1);
			return;
		}

		Debug.Log ("Init: Looking for Game session");

		foreach (var _level in playerData.levelState) {
			for (int i = 0; i < playerData.levelState.Length; i++) {
				if (playerData.levelState[i] == LevelState.GameSession) {
					Debug.Log ("Init: Load Game Session");
					LoadScene (i);
					return;
				}
			}
		}

		Debug.Log ("Init: Loading Menu screen");
	}

	public static void LoadScene(int _scene) {
		Debug.Log ("Loading scene: " + _scene);
		playerData.levelState [_scene] = LevelState.GameSession;
		SaveLoad.Save (playerData, SaveLoad.namePlayerProgress);

		SceneManager.LoadScene (_scene);
	}
}
